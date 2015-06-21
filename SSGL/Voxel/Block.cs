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
    public class Block
    {
        public const float RENDER_SIZE = 0.5f;
        public Terrain Type { get; set; }
        private bool _active;

        public Block() {
            _active = true;
        }

        public bool IsActive()
        {
            return this._active;
        }

        public void SetActive(bool active)
        {
            this._active = active;
        }
    }
}
