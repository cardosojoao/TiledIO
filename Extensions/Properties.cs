using System;
using System.Collections.Generic;
using System.Linq;

namespace TiledIO.Extensions
{
    public static partial class PropertyExtensions
    {
        public static bool ExistProperty(this List<Models.Property> properties, string name)
        {
            if (properties == null) throw new ArgumentNullException(nameof(properties));
            Models.Property prop = properties.FirstOrDefault(p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            return prop != null;
        }

        public static bool ExistProperty(this List<Entities.Property> properties, string name)
        {
            if (properties == null) throw new ArgumentNullException(nameof(properties));

            int proptNameIndex = name.IndexOf('.');
            if (proptNameIndex != -1)
            {
                string subProp = name.Substring(proptNameIndex + 1);
                string propName = name.Substring(0, proptNameIndex);

                Entities.Property prop = properties.FirstOrDefault(p => p.Name.Equals(propName, StringComparison.InvariantCultureIgnoreCase));
                return prop != null;
            }
            else
            {
                Entities.Property prop = properties.FirstOrDefault(p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                return prop != null;
            }
        }

        public static string GetProperty(this List<Models.Property> properties, string name)
        {
            if (properties == null) throw new ArgumentNullException(nameof(properties));
            int proptNameIndex = name.IndexOf('.');
            if (proptNameIndex != -1)
            {
                string subProp = name.Substring(proptNameIndex + 1);
                string propName = name.Substring(0, proptNameIndex);
                Models.Property prop = properties.FirstOrDefault(p => p.Name.Equals(propName, StringComparison.InvariantCultureIgnoreCase));
                if (prop.Propertytype == "class")
                {
                    Dictionary<string, object> dict = (Dictionary<string, object>)prop.GetValue();
                    if (dict.ContainsKey(subProp))
                    {
                        return dict[subProp].ToString() ?? "";
                    }
                }
                return "";
            }
            else
            {
                Models.Property prop = properties.FirstOrDefault(p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                return prop?.GetValue().ToString() ?? "";
            }
        }

        public static string GetProperty(this List<Entities.Property> properties, string name, string defaultValue = "")
        {
            if (properties == null) throw new ArgumentNullException(nameof(properties));
            int proptNameIndex = name.IndexOf('.');
            if (proptNameIndex != -1)
            {
                string subProp = name.Substring(proptNameIndex + 1);
                string propName = name.Substring(0, proptNameIndex);
                Entities.Property prop = properties.FirstOrDefault(p => p.Name.Equals(propName, StringComparison.InvariantCultureIgnoreCase));
                if (prop.Propertytype == "class")
                {
                    Dictionary<string, object> dict = (Dictionary<string, object>)prop.GetValue();
                    if (dict.ContainsKey(subProp))
                    {
                        return dict[subProp].ToString() ?? defaultValue;
                    }
                }
                return defaultValue;
            }
            else
            {
                Entities.Property prop = properties.FirstOrDefault(p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                return prop?.GetValue().ToString() ?? defaultValue;
            }
        }

        /// <summary>
        /// get property value as int
        /// </summary>
        /// <param name="properties">properties</param>
        /// <param name="name">property name</param>
        /// <returns>value as int in case error 255</returns>
        public static int GetPropertyInt(this List<Entities.Property> properties, string name)
        {
            if (properties == null) throw new ArgumentNullException(nameof(properties));
            Entities.Property? prop = properties.Find(p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            return prop == null ? throw new KeyNotFoundException(name) : int.Parse(prop.Value.ToString());
        }

        public static int GetPropertyInt(this List<Entities.Property> properties, string name, int value = -65535)
        {
            if (properties == null) throw new ArgumentNullException(nameof(properties));
            int proptNameIndex = name.IndexOf('.');
            if (proptNameIndex != -1)
            {
                string subProp = name.Substring(proptNameIndex + 1);
                string propName = name.Substring(0, proptNameIndex);
                Entities.Property prop = properties.FirstOrDefault(p => p.Name.Equals(propName, StringComparison.InvariantCultureIgnoreCase));
                if (prop.Type == "class")
                {
                    Dictionary<string, object> dict = (Dictionary<string, object>)prop.GetValue();
                    if (dict.ContainsKey(subProp))
                    {
                        return int.Parse(dict[subProp].ToString());
                    }
                }
                return value;
            }
            else
            {
                Entities.Property? prop = properties.Find(p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                return prop == null ? value : int.Parse(prop.GetValue().ToString());
            }
        }


        public static bool GetPropertyBool(this List<Entities.Property> properties, string name, bool value = false)
        {
            if (properties == null) throw new ArgumentNullException(nameof(properties));
            int proptNameIndex = name.IndexOf('.');
            if (proptNameIndex != -1)
            {
                string subProp = name.Substring(proptNameIndex + 1);
                string propName = name.Substring(0, proptNameIndex);
                Entities.Property prop = properties.FirstOrDefault(p => p.Name.Equals(propName, StringComparison.InvariantCultureIgnoreCase));
                if (prop.Propertytype == "class")
                {
                    Dictionary<string, object> dict = (Dictionary<string, object>)prop.GetValue();
                    if (dict.ContainsKey(subProp))
                    {

                        
                        return dict[subProp] == null ? value : bool.Parse(dict[subProp].ToString());
                    }
                }
                return value;
            }
            else
            {
                Entities.Property prop = properties.FirstOrDefault(p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                return prop == null ? value : bool.Parse(prop.Value.ToString());
            }
        }

        public static void Merge(this List<Entities.Property> properties, List<Entities.Property> mergeProperties)
        {

            if (mergeProperties != null)
            {
                properties.AddRange(mergeProperties);
            }
        }
    }


    public static class DictionaryExtension
    {
        public static void Append<K, V>(this Dictionary<K, V> first, Dictionary<K, V> second) where V : class
        {
            List<KeyValuePair<K, V>> pairs = second.ToList();
            pairs.ForEach(pair => first.Add(pair.Key, pair.Value));
        }
    }
}
