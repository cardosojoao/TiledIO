using System.Collections.Generic;
using System.IO;
using TiledIO.Entities;
using Entity = TiledIO.Entities;
using Model = TiledIO.Models;

namespace TiledIO.Mapper
{
    public class LayerMapper
    {
        public static List<Entity.Layer> Map(List<Model.Layer> LayersRaw)
        {
            List<Entity.Layer> layers = new();
            foreach (var layerRaw in LayersRaw)
            {
                if (layerRaw.Visible)
                {
                    layers.Add(ParseLayer(layerRaw));
                }
            }
            return layers;
        }

        private static Entity.Layer ParseLayer(Model.Layer layerRaw)
        {
            Breadcrumbs.Push(layerRaw.Name);
            Entity.Layer layer = new()
            {
                Layers = new(),
                Name = layerRaw.Name,
                Type = layerRaw.Type,
                Width = layerRaw.Width,
                Height = layerRaw.Height,
                X = layerRaw.X,
                Y = layerRaw.Y,
                Id = layerRaw.Id,
                Visible = layerRaw.Visible,
                Data = layerRaw.Data,
                Colour = layerRaw.TintColour
            };

            if (layerRaw.Objects != null)
            {
                Breadcrumbs.Push("MapObjects");
                layer.Objects = ParseObjects(layerRaw.Objects);
                Breadcrumbs.Pop();
            }
            if (layerRaw.Properties != null)
            {
                Breadcrumbs.Push("MapProperties");
                layer.Properties = PropertyMapper.Map(layerRaw.Properties);
                Breadcrumbs.Pop();
            }

            if (layerRaw.Layers != null)
            {
                foreach (Model.Layer layerRawChild in layerRaw.Layers)
                {
                    if (layerRawChild.Visible)
                    {
                        layer.Layers.Add(ParseLayer(layerRawChild));
                    }
                }
            }
            Breadcrumbs.Pop();
            return layer;
        }

        private static List<Entity.Object> ParseObjects(List<Model.Object> objectsRaw)
        {
            List<Entity.Object> objects = new();
            foreach (Model.Object objectRaw in objectsRaw)
            {
                Breadcrumbs.Push("MapObject_" + objectRaw.Name);
                Entity.Object obj = new()
                {
                    Gid = objectRaw.Gid,
                    Name = objectRaw.Name,
                    Width = objectRaw.Width,
                    Height = objectRaw.Height,
                    Visible = objectRaw.Visible,
                    Id = objectRaw.Id,
                    X = objectRaw.X,
                    Y = objectRaw.Y,
                    Type = objectRaw.Type,
                    Template = objectRaw.Template
                };

                
                
                if (objectRaw.Properties != null)
                {
                    obj.Properties = PropertyMapper.Map(objectRaw.Properties);
                }

                if (obj.Template != null)
                {
                    Breadcrumbs.Push("MapTemplate_" + obj.Template);

                    obj.Properties ??= new List<Entities.Property>();
                    Entities.Template template;
                    if (!Entities.Scene.Instance.Templates.ContainsKey(obj.Template))
                    {
                        string templatePath = Path.Combine(Entities.Scene.Instance.RootFolder, obj.Template);
                        Models.XML.Template templateData = SceneMapper.ReadTemplate(templatePath);
                        template = TemplateMapper.Map(templateData);
                        Entities.Scene.Instance.Templates.Add(obj.Template, template);
                    }
                    template = Entities.Scene.Instance.Templates[obj.Template];
                    if (template.Gid > 0)
                    {
                        string tilesetPath = Path.GetFullPath(Path.Combine(Entities.Scene.Instance.RootFolder, template.Source));
                        Tileset t = Entities.Scene.Instance.Tilesets.Find(x => x.Source.Equals(tilesetPath, System.StringComparison.CurrentCultureIgnoreCase));
                        obj.Gid = (uint)(t.Firstgid + template.Gid - 1);
                    }

                    obj.Type = template.Type;

                    if (obj.Width == 0 && template.Width != 0)
                    {
                        obj.Width = template.Width;
                    }

                    if (obj.Height == 0 && template.Height != 0)
                    {
                        obj.Height = template.Height;
                    }
                    Breadcrumbs.Pop();

                    foreach (var property in template.Properties)
                    {
                        if (obj.Properties.Find(p => p.Name == property.Name) == null)
                        {
                            obj.Properties.Add(property);
                        }
                    }
                }

                if (objectRaw.Polygon != null)
                {
                    Breadcrumbs.Push("MapPolygon_"+objectRaw.Name);

                    obj.Polygon = new();
                    if (objectRaw.Properties != null)
                    {
                        obj.Polygon.Properties = PropertyMapper.Map(objectRaw.Properties);
                    }
                    foreach (Model.PolygonPoint point in objectRaw.Polygon)
                    {
                        obj.Polygon.Add(new Entity.PolygonPoint() { X = point.x, Y = point.y });
                    }
                    Breadcrumbs.Pop(); 
                }

                if (objectRaw.PolyLine != null)
                {
                    Breadcrumbs.Push("MapPolyLine_" + objectRaw.Name);
                    obj.Polygon = new();
                    if (objectRaw.Properties != null)
                    {
                        obj.Polygon.Properties = PropertyMapper.Map(objectRaw.Properties);
                    }
                    foreach (Model.PolygonPoint point in objectRaw.PolyLine)
                    {
                        obj.Polygon.Add(new Entity.PolygonPoint() { X = point.x, Y = point.y });
                    }
                    Breadcrumbs.Pop();
                }
                objects.Add(obj);
                Breadcrumbs.Pop();
            }
            return objects;
        }
    }
}
