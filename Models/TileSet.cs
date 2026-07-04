using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TiledIO.Models
{
    public partial class TileSet
    {
        [JsonPropertyName("firstgid")]
        public int Firstgid { get; set; }

        /// <summary>
        /// first GID to be reported to parser
        /// </summary>
        [JsonIgnore]
        public int FirstgidMap { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonIgnore]
        public int Lastgid { get; set; }
        /// <summary>
        /// result of resolving the tileset names, different tileset could have the same image, just different tile size
        /// what should be the value substracted to the gid value
        /// </summary>
        [JsonIgnore]
        public int Parsedgid { get; set; }
        /// <summary>
        /// Sprite sheet id ( each sprite sheet is 8K and can have 32 sprites of 16x16 or 128 of 8x8)
        /// </summary>
        [JsonIgnore]
        public int TileSheetID { get; set; }
        [JsonIgnore]
        public int PaletteIndex { get; set; }
        [JsonIgnore]
        public int Order { get; set; }
        [JsonIgnore]
        public List<TilesetTile> Tiles { get; set; }
        [JsonIgnore]
        public List<TilesetTileProperty> Properties { get; set; }

    }
}
