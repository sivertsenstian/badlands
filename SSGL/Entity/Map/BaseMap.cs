using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSGL.Helper.Enum;
using Microsoft.Xna.Framework.Graphics;
using SSGL.Entity.Actor;

namespace SSGL.Entity.Map
{
    public abstract class BaseMap : BaseActor, IMap
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public List<Enum> MapTerrain { get; set; }

        public BaseMap(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public abstract void Generate();

    }
}
