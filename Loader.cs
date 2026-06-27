using TiledIO.Mapper;
using System.IO;
using System.Text.Json;
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
    }
}
