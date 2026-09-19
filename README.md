# TiledIO

C# library for reading Tiled JSON maps and worlds, working with map entities, and reading/writing XML tilesets. The package ID is `Bapa.TiledIO`; the public loader is `TiledIO.Tiled`.

The current source targets .NET 10 and has no external package dependencies. The mapped scene loader includes custom tileset and palette conventions, so check the input requirements below before using it with existing maps.

**Build and reference**

Install the .NET 10 SDK, then run from the repository root:

```sh
dotnet restore TiledIO.sln
dotnet build TiledIO.sln --configuration Release --no-restore
```

Reference the source project from a .NET 10 application (adjust the path):

```sh
dotnet add reference ../TiledIO/TiledIO.csproj
```

To produce a local NuGet package:

```sh
dotnet pack TiledIO.csproj --configuration Release --output ./artifacts
```

There is no committed automated test project in this repository.

**Read and edit a raw map**

`Models` contains the serialization types. Raw loading does not resolve external tilesets or templates and retains invisible layers.

```csharp
using System;
using System.IO;
using TiledIO;

var mapPath = Path.GetFullPath("maps/level.tmj");
var raw = Tiled.LoadSceneRaw(mapPath);
var ground = Tiled.GetLayer(raw, "Ground");

if (ground != null)
{
    ground.Name = "Terrain";
}

Tiled.SaveSceneRaw(raw, Path.Combine(Path.GetDirectoryName(mapPath)!, "level-copy.tmj"));
```

Saving overwrites the destination and serializes only fields represented by the models. Unknown input fields are discarded; this is not a lossless round trip for arbitrary Tiled files. Use a separate output file when preserving the original matters.

**Load a mapped scene**

`Entities` provides scene, layer, object, property, tileset, and area types. Mapped loading resolves external XML tilesets and XML object templates and excludes invisible layers, including their children.

```csharp
using System;
using System.IO;
using TiledIO;
using TiledIO.Extensions;

var scene = Tiled.LoadScene(Path.GetFullPath("maps/level.tmj"));
var ground = Tiled.GetLayer(scene, "Ground");

if (ground?.Data != null && ground.Width > 0 && ground.Height > 0)
{
    uint gid = ground.GetTileId(0, 0);
    Console.WriteLine($"Top-left tile GID: {gid}");
}

string sceneName = scene.Properties.GetProperty("SceneName", "Unnamed");
var collisionLayer = Tiled.GetLayerByProperty(scene, "Collision");
```

Layer name lookup is case-insensitive and searches nested groups. Missing layers return `null`. `GetLayerByProperty` searches for the presence of a property, regardless of its value. `GetTileId` returns the stored GID without removing transformation flags.

Mapped scene input requirements:

- Use JSON maps with numeric tile data arrays and external `.tsx` tilesets. XML `.tmx` maps, embedded tilesets, JSON tilesets, infinite-map chunks, and encoded/compressed tile data are not implemented by this loading path.
- Maps must have non-null `properties`, `layers`, and `tilesets` collections. An empty `properties` array is sufficient. Optional custom properties `WorldName` and `SceneName` form `scene.FileName` as `WorldName_SceneName`; `PaletteLayer2` supplies `scene.Layer2Palette`.
- Each external tileset must define tileset-level integer properties `TileSheetId` and `Type`. `Type = 2` skips the tileset. Other values are loaded.
- Loaded tilesets are expected to have a sheet-level image. Explicit tile entries must have property collections for the current mapper.
- Object templates are read as XML. Keep template and tileset paths consistent with the loader's map-directory resolution; nested template directories need special care.

**Read a world**

```csharp
using System;
using TiledIO;

var world = Tiled.LoadWorld("maps/game.world");
foreach (var map in world.Maps)
{
    Console.WriteLine($"{map.FileName}: ({map.X}, {map.Y}), {map.Width} x {map.Height}");
}
```

World loading returns map references and placement information. It does not load the referenced maps or resolve their filenames into absolute paths.

**Read and write XML tilesets**

```csharp
using System.IO;
using TiledIO.Mapper;

var tileset = SceneMapper.ReadTileSet("tilesheets/terrain.tsx");
tileset.name = "Terrain";
SceneMapper.WriteTileSet(tileset, Path.GetFullPath("output/terrain.tsx"));
```

The writer creates the output directory and overwrites the destination. XML fields not represented by the model are not preserved. `SceneMapper.ReadTemplate` also exposes raw XML template reading.

**Current limitations**

- `Tiled.LoadScene` returns and mutates `Entities.Scene.Instance`. Loading another map changes the same object, and the template cache persists between loads. Avoid concurrent loads and retaining mapped scenes as independent snapshots.
- Layer opacity is modeled as an integer, so fractional opacity such as `0.5` fails JSON deserialization, including raw loading.
- XML tile IDs are stored as bytes; explicit tile IDs above 255 fail deserialization.
- XML property conversion constructs JSON strings without escaping their contents. Quotes, backslashes, and multiline values can fail or change meaning. Tileset property types are also mapped to strings.
- The mapped entity model does not preserve every raw field, including layer opacity/offsets and object rotation. Polygon and polyline points share the entity's `Polygon` representation.
- File, parsing, and mapping errors propagate to the caller. Property helpers generally require non-null collections.

**Source layout**

| Path | Purpose |
| --- | --- |
| `Loader.cs` | Public `Tiled` loading, saving, and layer lookup methods |
| `Models/` | JSON and XML serialization models |
| `Entities/` | Mapped data and tile/area utility types |
| `Mapper/` | Conversion between raw models and entities; XML I/O |
| `Extensions/` | Property lookup, dictionary, and hexadecimal helpers |

Project models and `ProjectMapper` are available for callers that deserialize project JSON themselves; `Tiled` has no `LoadProject` method.

**License**

[MIT](LICENSE), copyright 2026 João Cardoso.
