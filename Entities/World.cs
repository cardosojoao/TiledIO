using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security;
using System.Text.Json.Serialization;
using System.Threading.Tasks.Dataflow;
using TiledIO.Models;

namespace TiledIO.Entities
{

    public class Map
    {
        public string FileName { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get { return X1 + Width; } }
        public int Y2 { get { return Y1 + Height; } }



        public NeighBours NeighBours { get; set; } = new NeighBours();
        public int Id { get; set; } = -1;
    }


    public class NeighBours
    {
        public NeighBour Left { get; set; } = new NeighBour();
        public NeighBour Right { get; set; } = new NeighBour();
        public NeighBour Top { get; set; } = new NeighBour();
        public NeighBour Bottom { get; set; } = new NeighBour();


    }


    public class NeighBour
    {
        public int Id { get; set; } = 0;
        public int Xoffset { get; set; } = 0;
        public int Yoffset { get; set; } = 0;
    }


    public class World
    {
        public string Name { get; set; }
        public List<Map> Maps { get; set; }
        public bool OnlyShowAdjacentMaps { get; set; }
        public string Type { get; set; }
    }
}
