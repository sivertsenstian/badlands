using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSGL.Entity.Tile
{
    public class TileChunk<T>
    {
        public List<T> Tiles { get; set; }
        public BoundingBox BoundingBox { get; set; }

        public TileChunk()
        {
            Tiles = new List<T>();
        }
    }
}
