using System;
using System.Collections.Generic;
using System.Linq;
namespace TiledIO.Entities
{
    public partial class Layer
    {
        public List<Layer> Layers { get; set; }

        public List<uint> Data { get; set; }

        public int Height { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public bool Visible { get; set; }

        public int Width { get; set; }

        public int X { get; set; }

        public int Y { get; set; }
        public string Colour { get;set; }

        public List<Object> Objects { get; set; }

        public List<Property> Properties { get; set; }

        public bool Empty()
        {
            return Data.Count(c => c != 0) == 0;
        }

        /// <summary>
        /// Copies each nonempty pair of consecutive values into an empty pair at the same index.
        /// Both layers must have data with the same even number of values.
        /// </summary>
        public void MergeFrom(Layer source)
        {
            ArgumentNullException.ThrowIfNull(source);

            var destinationData = Data;
            var sourceData = source.Data;
            if (destinationData == null)
            {
                throw new InvalidOperationException("The destination layer has no data.");
            }

            if (sourceData == null || sourceData.Count != destinationData.Count || sourceData.Count % 2 != 0)
            {
                throw new ArgumentException("Both layers must have data with the same even number of values.", nameof(source));
            }

            for (int i = 0; i < destinationData.Count; i += 2)
            {
                uint first = sourceData[i];
                uint second = sourceData[i + 1];
                if ((first != 0 || second != 0) && destinationData[i] == 0 && destinationData[i + 1] == 0)
                {
                    destinationData[i] = first;
                    destinationData[i + 1] = second;
                }
            }
        }

        public uint GetTileId(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                throw new ArgumentOutOfRangeException("Coordinates are out of bounds.");
            }
            int index = y * Width + x;
            return Data[index];
        }
    }
}
