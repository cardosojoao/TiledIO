using System.Collections.Generic;
using TiledIO.Entities;

namespace TiledIO.Entities
{
    public class Template
    {
        public int Gid { get; set; }
        public string Type { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public List<Property> Properties { get; set; } = new List<Property>();
    }
}
