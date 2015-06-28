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
        private Block[][][] _blocks;
        private List<VertexPositionNormalTexture> _vertices;
        private List<int> _indices;
        private BasicEffect _chunkEffect;
        private Matrix _worldMatrix;
        private bool _isEmpty;

        public const int CHUNK_SIZE = 8;
        public bool IsVisible { get; set; }
        public bool IsLoaded { get; set; }
        public bool IsSetup { get; set; }
        public Vector3 Position { get; set; }
        public BoundingSphere Bounds { get; set; }
        public ChunkManager Manager { get; set; }

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

        public void UpdateShouldRenderFlags()
        {
            this._isEmpty = this._vertices.Count == 0;
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
            _blocks = new Block[CHUNK_SIZE][][];
            //TODO: FIX THE TYPE ASSIGNMENT
            Terrain type;
            // Create the blocks
            for (int i = 0; i < CHUNK_SIZE; i++)
            {
                _blocks[i] = new Block[CHUNK_SIZE][];
                for (int j = 0; j < CHUNK_SIZE; j++)
                {
                    _blocks[i][j] = new Block[CHUNK_SIZE];

                    for (int k = 0; k < CHUNK_SIZE; k++)
                    {
                        var height = j + Position.Y;
                        type = height < 3 ? Terrain.WATER : height < 5 ? Terrain.SAND : height < 7 ? Terrain.DIRT : height < 10 ? Terrain.GRASS : height < 16 ? Terrain.ROCK : Terrain.SNOW;
                        _blocks[i][j][k] = new Block() { Type = type };
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
                        if (!_blocks[x][y][z].IsActive)
                        {
                            // Don't create triangle data for inactive blocks
                            continue;
                        }

                        bool xNegative = renderDefault;
                        if (x > 0)
                        {
                            xNegative = !_blocks[x - 1][y][z].IsActive;
                        }

                        bool xPositive = renderDefault;
                        if (x < CHUNK_SIZE - 1)
                        {
                            xPositive = !_blocks[x + 1][y][z].IsActive;
                        } 

                        bool yNegative = renderDefault;
                        if (y > 0)
                        {
                            yNegative = !_blocks[x][y - 1][z].IsActive;
                        }

                        bool yPositive = renderDefault;
                        if (y < CHUNK_SIZE - 1)
                        {
                            yPositive = !_blocks[x][y + 1][z].IsActive;
                        }


                        bool zNegative = renderDefault;
                        if (z > 0)
                        {
                            zNegative = !_blocks[x][y][z - 1].IsActive;
                        } 

                        bool zPositive = renderDefault;
                        if (z < CHUNK_SIZE - 1)
                        {
                            zPositive = !_blocks[x][y][z + 1].IsActive;
                        } 
                        
                        this.createBlock(xNegative, xPositive, yNegative, yPositive, zNegative, zPositive, x, y, z, _blocks[x][y][z].Type);
                        
                    }
                }
            }
            this.UpdateShouldRenderFlags();
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
            return !this._isEmpty;
        }

        public void Render()
        {
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

        public Block GetBlock(int x, int y, int z)
        {
            try {
                if(this._blocks != null)
                {
                    return this._blocks[x][y][z];
                }
            }
            catch (System.IndexOutOfRangeException)
            {
                //Not valid !
                return null;
            }
            return null;
        }


        private void createBlock(bool xNegative, bool xPositive, bool yNegative, bool yPositive, bool zNegative, bool zPositive, int x, int y, int z, Terrain type)
        {
            //Build block
            VertexPositionNormalTexture[] _blockVertices = new VertexPositionNormalTexture[4];
            int[] _blockIndices = new int[6];

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

            /// VERTICES //// INDICES ///
            if (zNegative)
            {
                // Add the vertices for the FRONT face.
                _blockVertices[0] = new VertexPositionNormalTexture(TopLeftFront, Default.FrontNormal, TextureUV[0]);
                _blockVertices[1] = new VertexPositionNormalTexture(BottomLeftFront, Default.FrontNormal, TextureUV[1]);
                _blockVertices[2] = new VertexPositionNormalTexture(TopRightFront, Default.FrontNormal, TextureUV[2]);
                _blockVertices[3] = new VertexPositionNormalTexture(BottomRightFront, Default.FrontNormal, TextureUV[3]);

                //Add the indices for the FRONT face.
                _blockIndices[0] = this._vertices.Count + 0;
                _blockIndices[1] = this._vertices.Count + 1;
                _blockIndices[2] = this._vertices.Count + 2;
                _blockIndices[3] = this._vertices.Count + 1;
                _blockIndices[4] = this._vertices.Count + 3;
                _blockIndices[5] = this._vertices.Count + 2;

                this._vertices.AddRange(_blockVertices);
                this._indices.AddRange(_blockIndices);
            }

            if (zPositive)
            {
                // Add the vertices for the BACK face.
                _blockVertices[0] = new VertexPositionNormalTexture(TopLeftBack, Default.BackNormal, TextureUV[2]);
                _blockVertices[1] = new VertexPositionNormalTexture(TopRightBack, Default.BackNormal, TextureUV[0]);
                _blockVertices[2] = new VertexPositionNormalTexture(BottomLeftBack, Default.BackNormal, TextureUV[3]);
                _blockVertices[3] = new VertexPositionNormalTexture(BottomRightBack, Default.BackNormal, TextureUV[1]);

                //Add the indices for the BACK face.
                _blockIndices[0] = this._vertices.Count + 0;
                _blockIndices[1] = this._vertices.Count + 1;
                _blockIndices[2] = this._vertices.Count + 2;
                _blockIndices[3] = this._vertices.Count + 2;
                _blockIndices[4] = this._vertices.Count + 1;
                _blockIndices[5] = this._vertices.Count + 3;

                this._vertices.AddRange(_blockVertices);
                this._indices.AddRange(_blockIndices);
            }

            if (yPositive)
            {
                // Add the vertices for the TOP face.
                _blockVertices[0] = new VertexPositionNormalTexture(TopLeftFront, Default.TopNormal, TextureUV[1]);
                _blockVertices[1] = new VertexPositionNormalTexture(TopRightBack, Default.TopNormal, TextureUV[2]);
                _blockVertices[2] = new VertexPositionNormalTexture(TopLeftBack, Default.TopNormal, TextureUV[0]);
                _blockVertices[3] = new VertexPositionNormalTexture(TopRightFront, Default.TopNormal, TextureUV[3]);

                // Add the indices for the TOP face.
                _blockIndices[0] = this._vertices.Count + 0;
                _blockIndices[1] = this._vertices.Count + 1;
                _blockIndices[2] = this._vertices.Count + 2;
                _blockIndices[3] = this._vertices.Count + 0;
                _blockIndices[4] = this._vertices.Count + 3;
                _blockIndices[5] = this._vertices.Count + 1;

                this._vertices.AddRange(_blockVertices);
                this._indices.AddRange(_blockIndices);
            }

            if (yNegative)
            {
                // Add the vertices for the BOTTOM face. 
                _blockVertices[0] = new VertexPositionNormalTexture(BottomLeftFront, Default.BottomNormal, TextureUV[0]);
                _blockVertices[1] = new VertexPositionNormalTexture(BottomLeftBack, Default.BottomNormal, TextureUV[1]);
                _blockVertices[2] = new VertexPositionNormalTexture(BottomRightBack, Default.BottomNormal, TextureUV[3]);
                _blockVertices[3] = new VertexPositionNormalTexture(BottomRightFront, Default.BottomNormal, TextureUV[2]);

                // Add the indices for the BOTTOM face. 
                _blockIndices[0] = this._vertices.Count + 0;
                _blockIndices[1] = this._vertices.Count + 1;
                _blockIndices[2] = this._vertices.Count + 2;
                _blockIndices[3] = this._vertices.Count + 0;
                _blockIndices[4] = this._vertices.Count + 2;
                _blockIndices[5] = this._vertices.Count + 3;

                this._vertices.AddRange(_blockVertices);
                this._indices.AddRange(_blockIndices);
            }

            if (xNegative)
            {
                // Add the vertices for the LEFT face.
                _blockVertices[0] = new VertexPositionNormalTexture(TopLeftFront, Default.LeftNormal, TextureUV[2]);
                _blockVertices[1] = new VertexPositionNormalTexture(BottomLeftBack, Default.LeftNormal, TextureUV[1]);
                _blockVertices[2] = new VertexPositionNormalTexture(BottomLeftFront, Default.LeftNormal, TextureUV[3]);
                _blockVertices[3] = new VertexPositionNormalTexture(TopLeftBack, Default.LeftNormal, TextureUV[0]);

                // Add the indices for the LEFT face.
                _blockIndices[0] = this._vertices.Count + 0;
                _blockIndices[1] = this._vertices.Count + 1;
                _blockIndices[2] = this._vertices.Count + 2;
                _blockIndices[3] = this._vertices.Count + 3;
                _blockIndices[4] = this._vertices.Count + 1;
                _blockIndices[5] = this._vertices.Count + 0;

                this._vertices.AddRange(_blockVertices);
                this._indices.AddRange(_blockIndices);
            }


            if (xPositive)
            {
                // Add the vertices for the RIGHT face. 
                _blockVertices[0] = new VertexPositionNormalTexture(TopRightFront, Default.RightNormal, TextureUV[0]);
                _blockVertices[1] = new VertexPositionNormalTexture(BottomRightFront, Default.RightNormal, TextureUV[1]);
                _blockVertices[2] = new VertexPositionNormalTexture(BottomRightBack, Default.RightNormal, TextureUV[3]);
                _blockVertices[3] = new VertexPositionNormalTexture(TopRightBack, Default.RightNormal, TextureUV[2]);

                // Add the indices for the RIGHT face. 
                _blockIndices[0] = this._vertices.Count + 0;
                _blockIndices[1] = this._vertices.Count + 1;
                _blockIndices[2] = this._vertices.Count + 2;
                _blockIndices[3] = this._vertices.Count + 3;
                _blockIndices[4] = this._vertices.Count + 0;
                _blockIndices[5] = this._vertices.Count + 2;

                this._vertices.AddRange(_blockVertices);
                this._indices.AddRange(_blockIndices);
            }
        }
    }
}
