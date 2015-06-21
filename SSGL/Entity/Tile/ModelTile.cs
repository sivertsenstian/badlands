using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SSGL.Core;
using SSGL.Entity.Actor;
using SSGL.Helper;
using SSGL.Helper.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSGL.Entity.Tile
{
    public class ModelTile
    {
        public int Id { get; set; }
        public Matrix World { get; set; }
        public int Size { get; set; }
        public Model Model { get; set; }

        public ModelTile()
        {
            Size = Default.Size;
        }

        public ModelTile(int id, Matrix world, int size)
        {
            Id = id;
            World = world;
            Size = size;
        }
    }
}
