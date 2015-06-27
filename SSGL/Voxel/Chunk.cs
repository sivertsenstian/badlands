using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SSGL.Core;
using SSGL.Helper;
using SSGL.Helper.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSGL.Voxel
{
    public class Chunk
    {
        private Block[,,] _blocks;
        private List<VertexPositionNormalTexture> _vertices;
        private List<int> _indices;
        private BasicEffect _chunkEffect;
        private Matrix _worldMatrix;

        public const int CHUNK_SIZE = 8;
        public bool IsVisible { get; set; }
        public bool IsLoaded { get; set; }
        public bool IsSetup { get; set; }
        public Vector3 Position { get; set; }
        public BoundingSphere Bounds { get; set; }

        public Chunk(Vector3 position)
        {
            this.Position = position;

            this.IsVisible = true;
            _worldMatrix = Matrix.CreateTranslation(this.Position);

            float c_offset = (Chunk.CHUNK_SIZE * Block.RENDER_SIZE) - Block.RENDER_SIZE;
            Vector3 chunkCenter = this.Position + new Vector3(c_offset, c_offset, c_offset);
            Bounds = new BoundingSphere(chunkCenter, Chunk.CHUNK_SIZE / 2);
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Unload()
        {
            _vertices = null;
            _indices = null;
            _chunkEffect = null;
            _blocks = null;

            this.IsLoaded = false;
            this.IsSetup = false;
        }
        //Loads and creates initial block-data for this chunk
        public void Load() {
            _blocks = new Block[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];
            //TODO: FIX THE TYPE ASSIGNMENT
            Terrain type;
            // Create the blocks
            for (int i = 0; i < CHUNK_SIZE; i++)
            {
                for (int j = 0; j < CHUNK_SIZE; j++)
                {
                    for (int k = 0; k < CHUNK_SIZE; k++)
                    {
                        var height = j + Position.Y;
                        type = height < 3 ? Terrain.WATER : height < 5 ? Terrain.SAND : height < 7 ? Terrain.DIRT : height < 10 ? Terrain.GRASS : height < 16 ? Terrain.ROCK : Terrain.SNOW;
                        _blocks[i, j, k] = new Block() { Type = type };
                    }
                }
            }

            this.IsLoaded = true;
            this.IsSetup = false;
        }

        //Creates the loaded blocks for this chunk
        //Creates vertices and indices for the chunk-buffer
        public void Setup() {
            _vertices = new List<VertexPositionNormalTexture>();
            _indices = new List<int>();
            _chunkEffect = new BasicEffect(GameDirector.Device);

            bool renderDefault = true;

            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    for (int z = 0; z < CHUNK_SIZE; z++)
                    {
                        if (!_blocks[x, y, z].IsActive)
                        {
                            // Don't create triangle data for inactive blocks
                            continue;
                        }

                        bool xNegative = renderDefault;
                        if (x > 0)
                            xNegative = !_blocks[x - 1, y, z].IsActive;
                        bool xPositive = renderDefault;
                        if (x < CHUNK_SIZE - 1)
                            xPositive = !_blocks[x + 1, y, z].IsActive;
                        bool yNegative = renderDefault;
                        if (y > 0)
                            yNegative = !_blocks[x, y - 1, z].IsActive;
                        bool yPositive = renderDefault;
                        if (y < CHUNK_SIZE - 1)
                            yPositive = !_blocks[x, y + 1, z].IsActive;

                        bool zNegative = renderDefault;
                        if (z > 0)
                            zNegative = !_blocks[x, y, z - 1].IsActive;
                        bool zPositive = renderDefault;
                        if (z < CHUNK_SIZE - 1)
                            zPositive = !_blocks[x, y, z + 1].IsActive;

                        this.createBlock(xNegative, xPositive, yNegative, yPositive, zNegative, zPositive, x, y, z, _blocks[x, y, z].Type);
                    }
                }
            }
            this.IsSetup = true;
        }

        //ReLoad ?
        public void RebuildMesh()
        {
            this.Unload();
            this.Load();
            this.Setup();
        }

        public bool ShouldRender()
        {
            return true;
        }

        public void Render()
        {
            if(this._vertices.Count == 0)
            {
                return;
            }

            DepthStencilState _depthState = new DepthStencilState();
            _depthState.DepthBufferEnable = true; /* Enable the depth buffer */
            _depthState.DepthBufferWriteEnable = true; /* When drawing to the screen, write to the depth buffer */

            // Set the World matrix which defines the position of the cube
            _chunkEffect.World = this._worldMatrix * Matrix.Identity * Matrix.Identity;

            // Set the View matrix which defines the camera and what it's looking at
            _chunkEffect.View = GameDirector.Camera.View;

            // Set the Projection matrix which defines how we see the scene (Field of view)
            _chunkEffect.Projection = GameDirector.Camera.Projection;

            // Enable textures on the Cube Effect. this is necessary to texture the model
            _chunkEffect.Texture = GameDirector.Assets.Textures[Terrain.TEXTURE];
            _chunkEffect.TextureEnabled = true;

            // Enable some pretty lights
            //_chunkEffect.EnableDefaultLighting();

            // apply the effect and render the cube
            //RasterizerState rasterizerState = new RasterizerState();
            //rasterizerState.FillMode = FillMode.WireFrame;
            //GameDirector.Device.RasterizerState = rasterizerState;
            foreach (EffectPass pass in _chunkEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                using (VertexBuffer VertexBuffer = new VertexBuffer(GameDirector.Device, typeof(VertexPositionNormalTexture), this._vertices.Count, BufferUsage.WriteOnly))
                {
                    using (IndexBuffer IndexBuffer = new IndexBuffer(GameDirector.Device, typeof(int), this._indices.Count, BufferUsage.WriteOnly))
                    {
                        //Load Vertices into buffer
                        VertexBuffer.SetData<VertexPositionNormalTexture>(this._vertices.ToArray());

                        //Load Indices into buffer
                        IndexBuffer.SetData(this._indices.ToArray());

                        //Send IndexBuffer to device
                        GameDirector.Device.Indices = IndexBuffer;
                        //Send Vertexbuffer to device
                        GameDirector.Device.SetVertexBuffer(VertexBuffer);
                        //Use depthbuffer when drawing a shape
                        GameDirector.Device.DepthStencilState = _depthState;

                        // Draw the primitives from the vertex buffer and index buffer to the device as triangles
                        GameDirector.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this._vertices.Count, 0, this._indices.Count / 3);
                    }
                }
            }
        }


        private void createBlock(bool xNegative, bool xPositive, bool yNegative, bool yPositive, bool zNegative, bool zPositive, int x, int y, int z, Terrain type)
        {
            //Build block
            VertexPositionNormalTexture[] _blockVertices = new VertexPositionNormalTexture[24];
            int[] _blockIndices = new int[36];

            //Vectors
            Vector3 TopLeftFront = new Vector3(x - Block.RENDER_SIZE, y + Block.RENDER_SIZE, z - Block.RENDER_SIZE);
            Vector3 TopLeftBack = new Vector3(x - Block.RENDER_SIZE, y + Block.RENDER_SIZE, z + Block.RENDER_SIZE);
            Vector3 TopRightFront = new Vector3(x + Block.RENDER_SIZE, y + Block.RENDER_SIZE, z - Block.RENDER_SIZE);
            Vector3 TopRightBack = new Vector3(x + Block.RENDER_SIZE, y + Block.RENDER_SIZE, z + Block.RENDER_SIZE);

            Vector3 BottomLeftFront = new Vector3(x - Block.RENDER_SIZE, y - Block.RENDER_SIZE, z - Block.RENDER_SIZE);
            Vector3 BottomLeftBack = new Vector3(x - Block.RENDER_SIZE, y - Block.RENDER_SIZE, z + Block.RENDER_SIZE);
            Vector3 BottomRightFront = new Vector3(x + Block.RENDER_SIZE, y - Block.RENDER_SIZE, z - Block.RENDER_SIZE);
            Vector3 BottomRightBack = new Vector3(x + Block.RENDER_SIZE, y - Block.RENDER_SIZE, z + Block.RENDER_SIZE);

            Vector2[] TextureUV = Util.TerrainTextureCoordinates(type);

            /// VERTICES ////
            if (zNegative)
            {
                // Add the vertices for the FRONT face.
                _blockVertices[0] = new VertexPositionNormalTexture(TopLeftFront, Default.FrontNormal, TextureUV[0]);
                _blockVertices[1] = new VertexPositionNormalTexture(BottomLeftFront, Default.FrontNormal, TextureUV[1]);
                _blockVertices[2] = new VertexPositionNormalTexture(TopRightFront, Default.FrontNormal, TextureUV[2]);
                _blockVertices[3] = new VertexPositionNormalTexture(BottomRightFront, Default.FrontNormal, TextureUV[3]);
            }

            if (zPositive)
            {
                // Add the vertices for the BACK face.
                _blockVertices[4] = new VertexPositionNormalTexture(TopLeftBack, Default.BackNormal, TextureUV[2]);
                _blockVertices[5] = new VertexPositionNormalTexture(TopRightBack, Default.BackNormal, TextureUV[0]);
                _blockVertices[6] = new VertexPositionNormalTexture(BottomLeftBack, Default.BackNormal, TextureUV[3]);
                _blockVertices[7] = new VertexPositionNormalTexture(BottomRightBack, Default.BackNormal, TextureUV[1]);
            }

            if (yPositive)
            {
                // Add the vertices for the TOP face.
                _blockVertices[8] = new VertexPositionNormalTexture(TopLeftFront, Default.TopNormal, TextureUV[1]);
                _blockVertices[9] = new VertexPositionNormalTexture(TopRightBack, Default.TopNormal, TextureUV[2]);
                _blockVertices[10] = new VertexPositionNormalTexture(TopLeftBack, Default.TopNormal, TextureUV[0]);
                _blockVertices[11] = new VertexPositionNormalTexture(TopRightFront, Default.TopNormal, TextureUV[3]);
            }

            if (yNegative)
            {
                // Add the vertices for the BOTTOM face. 
                _blockVertices[12] = new VertexPositionNormalTexture(BottomLeftFront, Default.BottomNormal, TextureUV[0]);
                _blockVertices[13] = new VertexPositionNormalTexture(BottomLeftBack, Default.BottomNormal, TextureUV[1]);
                _blockVertices[14] = new VertexPositionNormalTexture(BottomRightBack, Default.BottomNormal, TextureUV[3]);
                _blockVertices[15] = new VertexPositionNormalTexture(BottomRightFront, Default.BottomNormal, TextureUV[2]);
            }

            if (xNegative)
            {
                // Add the vertices for the LEFT face.
                _blockVertices[16] = new VertexPositionNormalTexture(TopLeftFront, Default.LeftNormal, TextureUV[2]);
                _blockVertices[17] = new VertexPositionNormalTexture(BottomLeftBack, Default.LeftNormal, TextureUV[1]);
                _blockVertices[18] = new VertexPositionNormalTexture(BottomLeftFront, Default.LeftNormal, TextureUV[3]);
                _blockVertices[19] = new VertexPositionNormalTexture(TopLeftBack, Default.LeftNormal, TextureUV[0]);
            }


            if (xPositive)
            {
                // Add the vertices for the RIGHT face. 
                _blockVertices[20] = new VertexPositionNormalTexture(TopRightFront, Default.RightNormal, TextureUV[0]);
                _blockVertices[21] = new VertexPositionNormalTexture(BottomRightFront, Default.RightNormal, TextureUV[1]);
                _blockVertices[22] = new VertexPositionNormalTexture(BottomRightBack, Default.RightNormal, TextureUV[3]);
                _blockVertices[23] = new VertexPositionNormalTexture(TopRightBack, Default.RightNormal, TextureUV[2]);
            }

            //// INDICES ///
            //Front
            _blockIndices[0] = this._vertices.Count + 0;
            _blockIndices[1] = this._vertices.Count + 1;
            _blockIndices[2] = this._vertices.Count + 2;
            _blockIndices[3] = this._vertices.Count + 1;
            _blockIndices[4] = this._vertices.Count + 3;
            _blockIndices[5] = this._vertices.Count + 2;

            //Back
            _blockIndices[6] = this._vertices.Count + 4;
            _blockIndices[7] = this._vertices.Count + 5;
            _blockIndices[8] = this._vertices.Count + 6;
            _blockIndices[9] = this._vertices.Count + 6;
            _blockIndices[10] = this._vertices.Count + 5;
            _blockIndices[11] = this._vertices.Count + 7;

            //Top
            _blockIndices[12] = this._vertices.Count + 8;
            _blockIndices[13] = this._vertices.Count + 9;
            _blockIndices[14] = this._vertices.Count + 10;
            _blockIndices[15] = this._vertices.Count + 8;
            _blockIndices[16] = this._vertices.Count + 11;
            _blockIndices[17] = this._vertices.Count + 9;

            //Bottom
            _blockIndices[18] = this._vertices.Count + 12;
            _blockIndices[19] = this._vertices.Count + 13;
            _blockIndices[20] = this._vertices.Count + 14;
            _blockIndices[21] = this._vertices.Count + 12;
            _blockIndices[22] = this._vertices.Count + 14;
            _blockIndices[23] = this._vertices.Count + 15;

            //Left
            _blockIndices[24] = this._vertices.Count + 16;
            _blockIndices[25] = this._vertices.Count + 17;
            _blockIndices[26] = this._vertices.Count + 18;
            _blockIndices[27] = this._vertices.Count + 19;
            _blockIndices[28] = this._vertices.Count + 17;
            _blockIndices[29] = this._vertices.Count + 16;

            //Right
            _blockIndices[30] = this._vertices.Count + 20;
            _blockIndices[31] = this._vertices.Count + 21;
            _blockIndices[32] = this._vertices.Count + 22;
            _blockIndices[33] = this._vertices.Count + 23;
            _blockIndices[34] = this._vertices.Count + 20;
            _blockIndices[35] = this._vertices.Count + 22;

            this._vertices.AddRange(_blockVertices);
            this._indices.AddRange(_blockIndices);
        }
    }
}
