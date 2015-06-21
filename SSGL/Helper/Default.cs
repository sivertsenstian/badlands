using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSGL.Helper
{
    public static class Default
    {
        public static int Size;
        public static Color ClearColor;

        //Vectors
        public static Vector3 TopLeftFront;
        public static Vector3 TopLeftBack;
        public static Vector3 TopRightFront;
        public static Vector3 TopRightBack;

        public static Vector3 BottomLeftFront;
        public static Vector3 BottomLeftBack;
        public static Vector3 BottomRightFront;
        public static Vector3 BottomRightBack;

        //Normals
        public static Vector3 FrontNormal;
        public static Vector3 BackNormal;
        public static Vector3 LeftNormal;
        public static Vector3 RightNormal;
        public static Vector3 TopNormal;
        public static Vector3 BottomNormal;

        //UV
        // UV texture coordinates
        public static Vector2 TextureTopLeft;
        public static Vector2 TextureTopRight;
        public static Vector2 TextureBottomLeft;
        public static Vector2 TextureBottomRight;

        static Default()
        {
            Size = 2;
            ClearColor = new Color(127, 204, 127);

            // Normal vectors for each face (needed for lighting / display)
            FrontNormal = Vector3.UnitZ;
            BackNormal = -Vector3.UnitZ;
            TopNormal = Vector3.UnitY;
            BottomNormal = -Vector3.UnitY;
            LeftNormal = -Vector3.UnitX;
            RightNormal = Vector3.UnitX;
        }
    }
}
