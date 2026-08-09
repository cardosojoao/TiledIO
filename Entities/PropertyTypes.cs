using System;
using System.Collections.Generic;

namespace TiledIO.Entities
{
    /// <summary>
    /// project global properties, currently only allow enums
    /// </summary>
    //public class PropertyType
    //{
    //    public int Id { get; set; }

    //    public string Name { get; set; }

    //    public string StorageType { get; set; }

    //    public string Type { get; set; }

    //    public List<string> Values { get; set; }

    //    public bool ValuesAsFlags { get; set; }


    //}

    ///////////////////////////////////////////////////////////////////////////
    public abstract class PropertyTypeDefinition
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    public sealed class ClassPropertyType : PropertyTypeDefinition
    {
        public string Type => "class";

        public string? Color { get; set; }

        public bool DrawFill { get; set; }

        public List<PropertyMember> Members { get; set; } = new List<PropertyMember>();

        public List<string> UseAs { get; set; } = new List<string>();
    }

    public sealed class EnumPropertyType : PropertyTypeDefinition
    {
        public string Type => "enum";

        public string StorageType { get; set; } = "";

        public List<string> Values { get; set; } = new List<string>();

        public bool ValuesAsFlags { get; set; }
    }

    public class PropertyMember
    {
        public string Name { get; set; } = "";

        public string Type { get; set; } = "";

        public string? PropertyType { get; set; }

        // No JsonElement; use object to avoid JSON dependency
        public object? Value { get; set; }

        public T? GetValue<T>()
        {
            if (Value is null) return default;
            if (Value is T t) return t;
            // attempt cast/conversion where possible
            return (T)Convert.ChangeType(Value, typeof(T));
        }
    }

    public enum EnumStorageType
    {
        Int,
        String
    }

    public enum PropertyValueType
    {
        Bool,
        Color,
        File,
        Float,
        Int,
        Object,
        String,
        Class
    }

    public enum PropertyUseTarget
    {
        Property,
        Map,
        Layer,
        Object,
        Tile,
        Tileset,
        WangColor,
        WangSet,
        Project
    }



}

