using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using TiledIO.Entities;
using Entity = TiledIO.Entities;
using Model = TiledIO.Models;

namespace TiledIO.Mapper
{
    public static class PropertyTypeMapper
    {
        public static List<Entity.PropertyTypeDefinition> Map(List<Model.PropertyTypeDefinition> propertiesRaw)
        {
            List<Entity.PropertyTypeDefinition> properties = new();
            foreach (Model.PropertyTypeDefinition propertyRaw in propertiesRaw)
            {
                if (propertyRaw is Model.ClassPropertyType classType)
                {
                    Entity.ClassPropertyType property = new()
                    {
                        Id = classType.Id,
                        Name = classType.Name,
                        Color = classType.Color,
                        DrawFill = classType.DrawFill,
                        Members =  (List<Entity.PropertyMember>) classType.Members.Select(x => MapMember(x)).ToList(),
                        UseAs = classType.UseAs
                    };
                    properties.Add(property);

                }
                else if (propertyRaw is Model.EnumPropertyType enumType)
                {
                    Entity.EnumPropertyType property = new()
                    {
                        Id = enumType.Id,
                        Name = enumType.Name,
                        StorageType = enumType.StorageType,
                        Values = new List<string>(enumType.Values),
                        ValuesAsFlags = enumType.ValuesAsFlags,
                    };
                    properties.Add(property);
                }
            }
            return properties;
        }


        public static Entities.PropertyMember MapMember(Models.PropertyMember m)
        {
            return new Entities.PropertyMember
            {
                Name = m.Name,
                Type = m.Type,
                PropertyType = m.PropertyType,
                Value = ConvertJsonElement(m.Value)
            };
        }

        private static object? ConvertJsonElement(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    return null;

                case JsonValueKind.String:
                    return element.GetString();

                case JsonValueKind.Number:
                    if (element.TryGetInt64(out long l)) return l;
                    if (element.TryGetDecimal(out decimal d)) return d;
                    return element.GetDouble();

                case JsonValueKind.True:
                case JsonValueKind.False:
                    return element.GetBoolean();

                case JsonValueKind.Object:
                    var dict = new Dictionary<string, object?>();
                    foreach (var prop in element.EnumerateObject())
                    {
                        dict[prop.Name] = ConvertJsonElement(prop.Value);
                    }
                    return dict;

                case JsonValueKind.Array:
                    var list = new List<object?>();
                    foreach (var item in element.EnumerateArray())
                    {
                        list.Add(ConvertJsonElement(item));
                    }
                    return list;

                default:
                    // fallback to raw text
                    return element.GetRawText();
            }
        }
    }
}
