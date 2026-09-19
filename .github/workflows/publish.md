# Publish workflow guide

This document explains how [`publish.yml`](./publish.yml) builds, packages, and publishes `Bapa.TiledIO`.

## Overview

The workflow has two jobs:

1. `build` restores, builds, and packages the project for every supported event.
2. `publish` uploads the package to GitHub Packages, but only for tag pushes and manual runs.

This separation means pull requests can verify that the NuGet package is buildable without being allowed to publish it.

## Workflow name

```yaml
name: Publish NuGet Package
```

This is the name displayed for the workflow in the repository's **Actions** tab.

## Triggers

```yaml
on:
  push:
    tags:
      - "v*"
  pull_request:
    types: [opened, synchronize, reopened]
  workflow_dispatch:
    inputs:
      version:
        description: NuGet package version
        required: true
        type: string
```

The `on` section defines when the workflow runs.

### Version tag push

```yaml
push:
  tags:
    - "v*"
```

The workflow runs when a tag whose name starts with `v` is pushed. For example, pushing `v2.0.5` starts the workflow and produces package version `2.0.5`.

Pushing an ordinary branch does not start this workflow.

### Pull request

```yaml
pull_request:
  types: [opened, synchronize, reopened]
```

The workflow runs when a pull request is:

- `opened`: first submitted.
- `synchronize`: updated with one or more commits.
- `reopened`: opened again after it was closed.

Pull-request runs execute the `build` job only. They do not execute the package-publishing job.

### Manual run

```yaml
workflow_dispatch:
  inputs:
    version:
      description: NuGet package version
      required: true
      type: string
```

`workflow_dispatch` adds the **Run workflow** button in the Actions interface. The person starting the workflow must enter the desired NuGet package version, such as `2.0.5`.

The entered value is passed to the workflow as `inputs.version`. It should be a valid NuGet version and should not include the tag's `v` prefix.

## Build job

```yaml
build:
  runs-on: ubuntu-latest
  permissions:
    contents: read
```

The `build` job runs on GitHub's current Ubuntu runner. It receives read-only access to repository contents, which is sufficient to check out and build the code. It cannot publish a package.

### Checkout

```yaml
- name: Checkout
  uses: actions/checkout@v4
```

This downloads the relevant repository revision onto the runner. For a pull request, GitHub checks out the pull request's test merge revision; for a tag or manual run, it checks out the selected ref.

### Install .NET

```yaml
- name: Setup .NET
  uses: actions/setup-dotnet@v4
  with:
    dotnet-version: '10.0.x'
```

This installs the latest available .NET 10 SDK patch release. The project targets `net10.0`, so the runner needs this SDK to restore, build, and pack it.

### Restore dependencies

```yaml
- name: Restore
  run: dotnet restore
```

This restores the NuGet dependencies and generates the assets needed by the build.

### Build the project

```yaml
- name: Build
  run: dotnet build --configuration Release --no-restore
```

This compiles the project using the `Release` configuration. `--no-restore` prevents a redundant restore because the preceding step has already completed it.

### Resolve the package version

```yaml
- name: Resolve package version
  id: package-version
  shell: bash
  env:
    MANUAL_VERSION: ${{ inputs.version }}
    PR_NUMBER: ${{ github.event.pull_request.number }}
  run: |
    if [[ "$GITHUB_EVENT_NAME" == "push" ]]; then
      version="${GITHUB_REF_NAME#v}"
    elif [[ "$GITHUB_EVENT_NAME" == "workflow_dispatch" ]]; then
      version="$MANUAL_VERSION"
    else
      version="0.0.0-pr.${PR_NUMBER}.${GITHUB_RUN_NUMBER}"
    fi

    echo "version=$version" >> "$GITHUB_OUTPUT"
```

This step selects a version according to the event that started the workflow:

| Event | Version source | Example |
| --- | --- | --- |
| Tag push | Tag name with the leading `v` removed | `v2.0.5` becomes `2.0.5` |
| Manual run | The required `version` input | `2.0.5` |
| Pull request | Generated prerelease version | `0.0.0-pr.42.157` |

The relevant GitHub-provided values are:

- `GITHUB_EVENT_NAME`: the event type, such as `push`, `workflow_dispatch`, or `pull_request`.
- `GITHUB_REF_NAME`: the short tag or branch name.
- `GITHUB_RUN_NUMBER`: the repository-specific sequence number for this workflow.
- `github.event.pull_request.number`: the pull request number.

`${GITHUB_REF_NAME#v}` is Bash syntax that removes a leading `v` from the tag name.

The final line writes the selected version to the step output named `version`. Later steps can access it through `steps.package-version.outputs.version` because this step has the ID `package-version`.

### Create the NuGet package

```yaml
- name: Pack
  env:
    PACKAGE_VERSION: ${{ steps.package-version.outputs.version }}
  run: |
    dotnet pack \
      --configuration Release \
      --no-build \
      -p:PackageVersion="$PACKAGE_VERSION" \
      --output ./artifacts
```

This creates the `.nupkg` file in the `artifacts` directory.

- `--configuration Release` packages the Release build.
- `--no-build` reuses the output from the previous build step.
- `-p:PackageVersion=...` assigns the resolved NuGet version.
- `--output ./artifacts` puts the resulting package in a predictable directory.

Passing the step output through the `PACKAGE_VERSION` environment variable keeps the value safely quoted in the shell command.

### Upload the workflow artifact

```yaml
- name: Upload package
  uses: actions/upload-artifact@v4
  with:
    name: nuget-package
    path: ./artifacts/*.nupkg
```

GitHub jobs run on separate machines and do not share a filesystem. This step stores the generated package as a workflow artifact named `nuget-package`, allowing the `publish` job to download it later.

For pull requests, this artifact also makes the package available for inspection from the completed workflow run even though it is not published.

## Publish job

```yaml
publish:
  if: github.event_name != 'pull_request'
  needs: build
  runs-on: ubuntu-latest
  permissions:
    contents: read
    packages: write
```

This job publishes the package.

- `if: github.event_name != 'pull_request'` skips the entire job for pull requests.
- `needs: build` waits for the `build` job and only proceeds when that job succeeds.
- `runs-on: ubuntu-latest` uses a fresh Ubuntu runner.
- `packages: write` allows this job's `GITHUB_TOKEN` to publish to GitHub Packages.

Package write access is deliberately assigned only to this job. The pull-request build job remains read-only.

### Download the package

```yaml
- name: Download package
  uses: actions/download-artifact@v4
  with:
    name: nuget-package
    path: ./artifacts
```

This downloads the artifact created by the `build` job into the new runner's `artifacts` directory.

### Install .NET for publishing

```yaml
- name: Setup .NET
  uses: actions/setup-dotnet@v4
  with:
    dotnet-version: '10.0.x'
```

Each job starts on a fresh runner, so .NET is configured again to ensure the `dotnet nuget` command is available in the publishing job.

### Publish to GitHub Packages

```yaml
- name: Publish to GitHub Packages
  run: |
    dotnet nuget push "./artifacts/*.nupkg" \
      --source "https://nuget.pkg.github.com/${{ github.repository_owner }}/index.json" \
      --api-key ${{ secrets.GITHUB_TOKEN }} \
      --skip-duplicate
```

This uploads every `.nupkg` file in the artifact directory to the repository owner's GitHub Packages NuGet feed.

- `github.repository_owner` resolves to the user or organization that owns the repository.
- `secrets.GITHUB_TOKEN` is the temporary token automatically created by GitHub for the workflow run.
- The job's `packages: write` permission authorizes that token to publish packages.
- `--skip-duplicate` allows a rerun to succeed when the same package version is already present in the feed.

## Event behavior summary

| Action | Build and pack | Publish to GitHub Packages |
| --- | --- | --- |
| Open, update, or reopen a pull request | Yes | No |
| Push a tag such as `v2.0.5` | Yes | Yes, as version `2.0.5` |
| Start a manual run with version `2.0.5` | Yes | Yes, as version `2.0.5` |
| Push an ordinary branch | No | No |

## Typical release methods

To publish by tag:

```text
git tag v2.0.5
git push origin v2.0.5
```

To publish manually, open **Actions**, select **Publish NuGet Package**, select **Run workflow**, enter a version such as `2.0.5`, and start the run.

NuGet package versions are immutable. Use a new version when publishing a new release; `--skip-duplicate` only prevents an existing version from making a rerun fail.
