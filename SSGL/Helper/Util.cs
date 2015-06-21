using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SSGL.Helper.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSGL.Helper
{
    public static class Util
    {

        public static Vector2[] TerrainTextureCoordinates(Terrain terrain){
            Vector2[] coords = new Vector2[4];
            Vector2 TextureTopLeft = Vector2.Zero;
            Vector2 TextureBottomLeft = Vector2.Zero;
            Vector2 TextureTopRight = Vector2.Zero;
            Vector2 TextureBottomRight = Vector2.Zero;
            
            //TODO: Possible to calculate this ?
            if (terrain == Terrain.WATER) //TOP LEFT
            {
                TextureTopLeft = new Vector2(0.5f, 0);
                TextureBottomLeft = new Vector2(0.5f, 0.5f);
                TextureTopRight = new Vector2(0, 0);
                TextureBottomRight = new Vector2(0, 0.5f);
            }
            else if (terrain == Terrain.SAND) //TOP RIGHT
            {
                TextureTopLeft = new Vector2(1.0f, 0);
                TextureBottomLeft = new Vector2(1.0f, 0.5f);
                TextureTopRight = new Vector2(0.5f, 0);
                TextureBottomRight = new Vector2(0.5f, 0.5f);
            }
            else if (terrain == Terrain.GRASS) //BOTTOM RIGHT
            {
                TextureTopLeft = new Vector2(1.0f, 0.5f);
                TextureBottomLeft = new Vector2(1.0f, 1.0f);
                TextureTopRight = new Vector2(0.5f, 0.5f);
                TextureBottomRight = new Vector2(0.5f, 1.0f);
            }
            else if (terrain == Terrain.DIRT) //BOTTOM LEFT
            {
                TextureTopLeft = new Vector2(0.5f, 0.5f);
                TextureBottomLeft = new Vector2(0.5f, 1.0f);
                TextureTopRight = new Vector2(0, 0.5f);
                TextureBottomRight = new Vector2(0, 1.0f);
            }

            coords[0] = TextureTopLeft;
            coords[1] = TextureBottomLeft;
            coords[2] = TextureTopRight;
            coords[3] = TextureBottomRight;

            return coords;
        }

        public static bool ListsAreEqual<T>(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var cnt = new Dictionary<T, int>();
            foreach (T s in list1)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]++;
                }
                else
                {
                    cnt.Add(s, 1);
                }
            }
            foreach (T s in list2)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]--;
                }
                else
                {
                    return false;
                }
            }
            return cnt.Values.All(c => c == 0);
        }

        // CalculateCursorRay Calculates a world space ray starting at the camera's
        // "eye" and pointing in the direction of the cursor. Viewport.Unproject is used
        // to accomplish this. see the accompanying documentation for more explanation
        // of the math behind this function.
        public static Ray CalculateCursorRay(GraphicsDevice device, Matrix projectionMatrix, Matrix viewMatrix)
        {
            int x = Mouse.GetState().X;
            int y = Mouse.GetState().Y;
            // create 2 positions in screenspace using the cursor position. 0 is as
            // close as possible to the camera, 1 is as far away as possible.
            Vector3 nearSource = new Vector3(x, y, 0f);
            Vector3 farSource = new Vector3(x, y, 1f);

            // use Viewport.Unproject to tell what those two screen space positions
            // would be in world space. we'll need the projection matrix and view
            // matrix, which we have saved as member variables. We also need a world
            // matrix, which can just be identity.
            Vector3 nearPoint = device.Viewport.Unproject(nearSource,
                projectionMatrix, viewMatrix, Matrix.Identity);

            Vector3 farPoint = device.Viewport.Unproject(farSource,
                projectionMatrix, viewMatrix, Matrix.Identity);

            // find the direction vector that goes from the nearPoint to the farPoint
            // and normalize it....
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            // and then create a new ray using nearPoint as the source.
            return new Ray(nearPoint, direction);
        }

        /// Checks whether a ray intersects a triangle. This uses the algorithm
        /// developed by Tomas Moller and Ben Trumbore, which was published in the
        /// Journal of Graphics Tools, volume 2, "Fast, Minimum Storage Ray-Triangle
        /// Intersection".
        /// 
        /// This method is implemented using the pass-by-reference versions of the
        /// XNA math functions. Using these overloads is generally not recommended,
        /// because they make the code less readable than the normal pass-by-value
        /// versions. This method can be called very frequently in a tight inner loop,
        /// however, so in this particular case the performance benefits from passing
        /// everything by reference outweigh the loss of readability.
        /// </summary>
        public static void RayIntersectsTriangle(ref Ray ray,
                                          ref Vector3 vertex1,
                                          ref Vector3 vertex2,
                                          ref Vector3 vertex3, out float? result)
        {
            // Compute vectors along two edges of the triangle.
            Vector3 edge1, edge2;

            Vector3.Subtract(ref vertex2, ref vertex1, out edge1);
            Vector3.Subtract(ref vertex3, ref vertex1, out edge2);

            // Compute the determinant.
            Vector3 directionCrossEdge2;
            Vector3.Cross(ref ray.Direction, ref edge2, out directionCrossEdge2);

            float determinant;
            Vector3.Dot(ref edge1, ref directionCrossEdge2, out determinant);

            // If the ray is parallel to the triangle plane, there is no collision.
            if (determinant > -float.Epsilon && determinant < float.Epsilon)
            {
                result = null;
                return;
            }

            float inverseDeterminant = 1.0f / determinant;

            // Calculate the U parameter of the intersection point.
            Vector3 distanceVector;
            Vector3.Subtract(ref ray.Position, ref vertex1, out distanceVector);

            float triangleU;
            Vector3.Dot(ref distanceVector, ref directionCrossEdge2, out triangleU);
            triangleU *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleU < 0 || triangleU > 1)
            {
                result = null;
                return;
            }

            // Calculate the V parameter of the intersection point.
            Vector3 distanceCrossEdge1;
            Vector3.Cross(ref distanceVector, ref edge1, out distanceCrossEdge1);

            float triangleV;
            Vector3.Dot(ref ray.Direction, ref distanceCrossEdge1, out triangleV);
            triangleV *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleV < 0 || triangleU + triangleV > 1)
            {
                result = null;
                return;
            }

            // Compute the distance along the ray to the triangle.
            float rayDistance;
            Vector3.Dot(ref edge2, ref distanceCrossEdge1, out rayDistance);
            rayDistance *= inverseDeterminant;

            // Is the triangle behind the ray origin?
            if (rayDistance < 0)
            {
                result = null;
                return;
            }

            result = rayDistance;
        }
    }
}
