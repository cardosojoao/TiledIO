using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TiledIO.Models
{

    public class Project
    {
        [JsonPropertyName("properties")]
        public List<Property> Properties { get; set; }
        [JsonPropertyName("propertyTypes")]
        public List<PropertyType> PropertyTypes { get; set; }
    }
}
