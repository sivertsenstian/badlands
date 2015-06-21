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
    public class Tile
    {
        public int Id { get; set; }
        public Vector3 Position { get; set; }
        public int Size { get; set; }
        public List<VertexPositionNormalTexture> Vertices { get; set; }

        public Tile() {
            Size = Default.Size;
        }

        public Tile(int id, Vector3 pos, int size)
        {
            Id = id;
            Position = pos;
            Size = size;
            Vertices = GenerateVertices(pos, size);
        }

        private List<VertexPositionNormalTexture> GenerateVertices(Vector3 pos, int size)
        {
            List<VertexPositionNormalTexture> verts = new List<VertexPositionNormalTexture>();

            var addVector = new Vector3(pos.X * size, pos.Y * size, pos.Z * size);
            //First Triangle
            //verts.Add(new VertexPositionNormalTexture((Default.Position[0] * size) + addVector, Vector3.Up, new Vector2(0, 0)));
            //verts.Add(new VertexPositionNormalTexture((Default.Position[1] * size) + addVector, Vector3.Up, new Vector2(1, 0)));
            //verts.Add(new VertexPositionNormalTexture((Default.Position[2] * size) + addVector, Vector3.Up, new Vector2(1, 1)));

            ////Second Triangle
            //verts.Add(new VertexPositionNormalTexture((Default.Position[2] * size) + addVector, Vector3.Up, new Vector2(1, 1)));
            //verts.Add(new VertexPositionNormalTexture((Default.Position[3] * size) + addVector, Vector3.Up, new Vector2(0, 1)));
            //verts.Add(new VertexPositionNormalTexture((Default.Position[0] * size) + addVector, Vector3.Up, new Vector2(0, 0)));

            //Console.WriteLine(" ----------------------- ");
            //for (int i = 0; i < verts.Count; i++)
            //{
            //    Console.WriteLine(verts[i]);
            //}
            //Console.WriteLine(" ----------------------- ");
            return verts;
        }
    }
}
