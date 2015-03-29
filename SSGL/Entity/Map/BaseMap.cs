using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSGL.Helper.Enum;
using Microsoft.Xna.Framework.Graphics;

namespace SSGL.Entity.Map
{
    public class BaseMap : IMap
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

        public List<List<int>> Terrain { get; set; }
        public Dictionary<MapTerrain, Texture2D> TerrainTextures { get; set; }

        public void Generate()
        {

        }

    }
}
