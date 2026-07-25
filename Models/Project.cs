using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TiledIO.Models
{

    public class Project
    {
        [JsonPropertyName("automappingRulesFile")]
        public string AutomappingRulesFile { get; set; } = string.Empty;

        [JsonPropertyName("commands")]
        public List<Command> Commands { get; set; } = new();

        [JsonPropertyName("extensionsPath")]
        public string ExtensionsPath { get; set; } = string.Empty;

        [JsonPropertyName("folders")]
        public List<string> Folders { get; set; } = new();

        [JsonPropertyName("properties")]
        public List<Property> Properties { get; set; }
        [JsonPropertyName("propertyTypes")]
        public List<PropertyType> PropertyTypes { get; set; }
    }


    public class Command
    {
        [JsonPropertyName("arguments")]
        public string Arguments { get; set; } = string.Empty;

        [JsonPropertyName("command")]
        public string Executable { get; set; } = string.Empty;

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("saveBeforeExecute")]
        public bool SaveBeforeExecute { get; set; }

        [JsonPropertyName("shortcut")]
        public string? Shortcut { get; set; }

        [JsonPropertyName("showOutput")]
        public bool ShowOutput { get; set; }

        [JsonPropertyName("workingDirectory")]
        public string WorkingDirectory { get; set; } = string.Empty;
    }
}
