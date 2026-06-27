using System.Collections.Generic;
using Entity = TiledIO.Entities;
using Model = TiledIO.Models;


namespace TiledIO.Mapper
{
    public class WorldMapper
    {
        public static Entities.World Map(Model.World world)
        {
            Entity.World worldOutput = new();

            worldOutput.OnlyShowAdjacentMaps = world.OnlyShowAdjacentMaps;
            worldOutput.Type = world.Type;
            worldOutput.Maps = new List<Entity.Map>();
            foreach(Models.Map map in world.Maps)
            {
                Entity.Map mapOutput = new() { FileName=map.FileName, Height = map.Height, Width= map.Width, X=map.X, Y=map.Y };
                worldOutput.Maps.Add(mapOutput);
            }
            return worldOutput;
        }
    }
}
