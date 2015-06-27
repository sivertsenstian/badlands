using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSGL.Voxel
{
    public class BlockAir : Block
    {
        public override bool IsSolid()
        {
            return false;
        }
    }
}
