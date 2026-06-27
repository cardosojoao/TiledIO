using System.Text.Json;
using Entity = TiledIO.Entities;
using Model = TiledIO.Models;

namespace TiledIO.Mapper
{
    public static class TemplateMapper
    {
        public static Entity.Template Map(Model.XML.Template templateRaw)
        {
            Entity.Template template= new Entity.Template();

            template.Gid = templateRaw.Object.Gid;
            template.Type = templateRaw.Object.Type;
            template.Height = templateRaw.Object.Height;
            template.Width = templateRaw.Object.Width;
            if (templateRaw.Object.Properties != null)
            {
                foreach (Model.XML.Property propertyRaw in templateRaw.Object.Properties.Property)
                {
                    Entity.Property property = new Entity.Property();
                    property.Name = propertyRaw.Name;
                    property.Type = propertyRaw.Type ?? "";
                    property.Value = JsonDocument.Parse($"\"{propertyRaw.Value}\"").RootElement;
                    //property.Value = propertyRaw.Value;
                    template.Properties.Add(property);
                }
            }
            return template;
        }
    }
}
