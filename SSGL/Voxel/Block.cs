using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SSGL.Helper;
using SSGL.Helper.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSGL.Voxel
{
    public struct Block
    {
        public const float RENDER_SIZE = 0.5f;
        public Terrain Type { get; set; }
        public bool IsActive { get; set; }
        
        public Block(Terrain type) {
            this.Type = type;
            IsActive = true;
        }
    }
}
