using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TiledIO.Models
{


    [JsonConverter(typeof(PropertyTypeConverter))]
    public abstract class PropertyTypeDefinition
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
    }


    public sealed class ClassPropertyType : PropertyTypeDefinition
    {
        [JsonPropertyName("type")]
        public string Type => "class";

        [JsonPropertyName("color")]
        public string? Color { get; set; }

        [JsonPropertyName("drawFill")]
        public bool DrawFill { get; set; }

        [JsonPropertyName("members")]
        public List<PropertyMember> Members { get; set; } = [];

        [JsonPropertyName("useAs")]
        public List<string> UseAs { get; set; } = [];
    }

    public sealed class EnumPropertyType : PropertyTypeDefinition
    {
        [JsonPropertyName("type")]
        public string Type => "enum";

        [JsonPropertyName("storageType")]
        public string StorageType { get; set; } = "";

        [JsonPropertyName("values")]
        public List<string> Values { get; set; } = [];

        [JsonPropertyName("valuesAsFlags")]
        public bool ValuesAsFlags { get; set; }
    }

    public class PropertyMember
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("type")]
        public string Type { get; set; } = "";

        [JsonPropertyName("propertyType")]
        public string? PropertyType { get; set; }

        [JsonPropertyName("value")]
        public JsonElement Value { get; set; }

        public T? GetValue<T>()
        {
            return Value.Deserialize<T>();
        }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EnumStorageType
    {
        [JsonStringEnumMemberName("int")]
        Int,

        [JsonStringEnumMemberName("string")]
        String
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PropertyValueType
    {
        [JsonStringEnumMemberName("bool")]
        Bool,

        [JsonStringEnumMemberName("color")]
        Color,

        [JsonStringEnumMemberName("file")]
        File,

        [JsonStringEnumMemberName("float")]
        Float,

        [JsonStringEnumMemberName("int")]
        Int,

        [JsonStringEnumMemberName("object")]
        Object,

        [JsonStringEnumMemberName("string")]
        String,

        [JsonStringEnumMemberName("class")]
        Class
    }


    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PropertyUseTarget
    {
        [JsonStringEnumMemberName("property")]
        Property,

        [JsonStringEnumMemberName("map")]
        Map,

        [JsonStringEnumMemberName("layer")]
        Layer,

        [JsonStringEnumMemberName("object")]
        Object,

        [JsonStringEnumMemberName("tile")]
        Tile,

        [JsonStringEnumMemberName("tileset")]
        Tileset,

        [JsonStringEnumMemberName("wangcolor")]
        WangColor,

        [JsonStringEnumMemberName("wangset")]
        WangSet,

        [JsonStringEnumMemberName("project")]
        Project
    }


    public class PropertyTypeConverter : JsonConverter<PropertyTypeDefinition>
    {
        public override PropertyTypeDefinition Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            using JsonDocument document = JsonDocument.ParseValue(ref reader);

            JsonElement root = document.RootElement;

            if (!root.TryGetProperty("type", out JsonElement typeProperty))
                throw new JsonException("PropertyTypeDefinition is missing 'type'.");

            string? type = typeProperty.GetString();

            return type switch
            {
                "class" => JsonSerializer.Deserialize<ClassPropertyType>(
                                root.GetRawText(),
                                options)!,

                "enum" => JsonSerializer.Deserialize<EnumPropertyType>(
                                root.GetRawText(),
                                options)!,

                _ => throw new JsonException($"Unknown property type '{type}'.")
            };
        }

        public override void Write(
            Utf8JsonWriter writer,
            PropertyTypeDefinition value,
            JsonSerializerOptions options)
        {
            switch (value)
            {
                case ClassPropertyType c:
                    JsonSerializer.Serialize(writer, c, options);
                    break;

                case EnumPropertyType e:
                    JsonSerializer.Serialize(writer, e, options);
                    break;

                default:
                    throw new JsonException($"Unsupported type {value.GetType().Name}");
            }
        }
    }

}