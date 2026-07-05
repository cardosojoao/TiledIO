using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using TiledIO.Extensions;
using TiledIO.Mapper;

namespace TiledIO
{
    public static class Tiled
    {
        public static Models.Scene LoadSceneRaw(string inputFile)
        {
            string data = File.ReadAllText(inputFile);
            Models.Scene sceneRaw = JsonSerializer.Deserialize<Models.Scene>(data);
            return sceneRaw;
        }

        public static  void SaveSceneRaw(Models.Scene sceneRaw, string inputFile)
        {
            string data = JsonSerializer.Serialize<Models.Scene>(sceneRaw, new JsonSerializerOptions { WriteIndented = true,  });
            File.WriteAllText(inputFile, data);
        }

        public static Entities.Scene LoadScene(Models.Scene sceneRaw, string inputFile)
        {
            Entities.Scene scene = SceneMapper.Map(sceneRaw, Entities.Scene.Instance, inputFile);
            scene.Layers = LayerMapper.Map(sceneRaw.Layers);
            return scene;
        }

        public static Entities.Scene LoadScene(string inputFile)
        {
            Models.Scene sceneRaw = LoadSceneRaw(inputFile);
            Entities.Scene scene = SceneMapper.Map(sceneRaw, Entities.Scene.Instance, inputFile);
            scene.Layers = LayerMapper.Map(sceneRaw.Layers);
            return scene;
        }

        public static Entities.World LoadWorld(string inputFile)
        {
            string worldRaw = File.ReadAllText(inputFile);
            Models.World worldData = JsonSerializer.Deserialize<Models.World>(worldRaw);
            Entities.World world = WorldMapper.Map(worldData);
            return world;
        }


        public static Models.Layer GetLayer(Models.Scene scene, string layerName)
        {
            return GetLayerRecursiveByName(scene.Layers, layerName);
        }

        public static Entities.Layer GetLayer(Entities.Scene scene, string layerName)
        {
            return GetLayerRecursiveByName(scene.Layers, layerName);
        }

        public static Entities.Layer GetLayerByProperty(Entities.Scene scene, string propertyName)
        {
            return GetLayerRecursiveByProperty(scene.Layers, propertyName);
        }


        private static Entities.Layer GetLayerRecursiveByName(List<Entities.Layer> layers, string layerName)
        {
            if (layers == null || layers.Count == 0)
                return null;

            // Search in current level
            var layer = layers.Find(layer => layer.Name.Equals(layerName, System.StringComparison.InvariantCultureIgnoreCase));
            if (layer != null)
                return layer;

            // Search recursively in child layers
            foreach (var currentLayer in layers)
            {
                if (currentLayer.Layers != null && currentLayer.Layers.Count > 0)
                {
                    var foundLayer = GetLayerRecursiveByName(currentLayer.Layers, layerName);
                    if (foundLayer != null)
                        return foundLayer;
                }
            }
            return null;
        }

        private static Models.Layer GetLayerRecursiveByName(List<Models.Layer> layers, string layerName)
        {
            if (layers == null || layers.Count == 0)
                return null;

            // Search in current level
            var layer = layers.Find(layer => layer.Name.Equals(layerName, System.StringComparison.InvariantCultureIgnoreCase));
            if (layer != null)
                return layer;

            // Search recursively in child layers
            foreach (var currentLayer in layers)
            {
                if (currentLayer.Layers != null && currentLayer.Layers.Count > 0)
                {
                    var foundLayer = GetLayerRecursiveByName(currentLayer.Layers, layerName);
                    if (foundLayer != null)
                        return foundLayer;
                }
            }
            return null;
        }



        private static Entities.Layer GetLayerRecursiveByProperty(List<Entities.Layer> layers, string propertyName)
        {
            if (layers == null || layers.Count == 0)
                return null;
            // Search recursively in child layers
            foreach (var currentLayer in layers)
            {
                if (currentLayer.Properties != null)
                {
                    if( currentLayer.Properties.ExistProperty(propertyName))
                    {
                        return currentLayer;
                    }   
                }
                if (currentLayer.Layers != null && currentLayer.Layers.Count > 0)
                {
                    var foundLayer = GetLayerRecursiveByProperty(currentLayer.Layers, propertyName);
                    if (foundLayer != null)
                    {
                        return foundLayer;
                    }
                       
                }
            }
            return null;
        }
    }
}
