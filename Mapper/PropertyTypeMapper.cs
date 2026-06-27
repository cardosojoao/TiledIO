using System.Collections.Generic;
using Entity = TiledIO.Entities;
using Model = TiledIO.Models;

namespace TiledIO.Mapper
{
    public static class PropertyTypeMapper
    {
        public static List<Entity.PropertyType> Map(List<Model.PropertyType> propertiesRaw)
        {
            List<Entity.PropertyType> properties = new();
            foreach (Model.PropertyType propertyRaw in propertiesRaw)
            {
                Entity.PropertyType property = new()
                {
                    Id = propertyRaw.Id,
                    Name = propertyRaw.Name,
                    StorageType = propertyRaw.StorageType,
                    Type = propertyRaw.Type,
                    Values = new List<string>(propertyRaw.Values),
                    ValuesAsFlags = propertyRaw.ValuesAsFlags,
                };
                properties.Add(property);
            }
            return properties;
        }
    }
}
