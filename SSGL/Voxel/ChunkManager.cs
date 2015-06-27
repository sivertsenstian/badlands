﻿using Microsoft.Xna.Framework;
using SSGL.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSGL.Voxel
{
    public class ChunkManager
    {
        public List<Chunk> Chunks;
        public List<Chunk> ChunkLoadList;
        public List<Chunk> ChunkRenderList;
        public List<Chunk> ChunkRebuildList;
        public List<Chunk> ChunkUnLoadList;
        public List<Chunk> ChunkVisibilityList;
        public List<Chunk> ChunkSetupList;
        public List<Chunk> ChunkUpdateFlagsList;

        private Vector3 _cameraPosition;
        private Matrix _cameraView;
        private bool _forceVisibilityUpdate;

        private const int ASYNC_NUM_CHUNKS_PER_FRAME = 8;

        public ChunkManager(int x, int y, int z)
        {
            Chunks = new List<Chunk>();
            ChunkLoadList = new List<Chunk>();
            ChunkUnLoadList = new List<Chunk>();
            ChunkVisibilityList = new List<Chunk>();
            ChunkRenderList = new List<Chunk>();
            ChunkRebuildList = new List<Chunk>();
            ChunkSetupList = new List<Chunk>();
            ChunkUpdateFlagsList = new List<Chunk>();

            for(int i = 0; i < x; i++)
            {
                for(int j = 0; j < y; j++)
                {
                    for(int k = 0; k < z; k++)
                    {
                        this.Chunks.Add(new Chunk(new Vector3(i * Chunk.CHUNK_SIZE, j * Chunk.CHUNK_SIZE, k * Chunk.CHUNK_SIZE)));
                    }
                }
            }
        }

        public void UpdateAsyncChunker()
        {

        }

        public void UpdateLoadList()
        {
            int numOfChunksLoaded = 0;
            Chunk chunk;

            for(int i = 0; i < ChunkLoadList.Count && numOfChunksLoaded != ASYNC_NUM_CHUNKS_PER_FRAME; i++)
            {
                chunk = ChunkLoadList[i];

                if (!chunk.IsLoaded && numOfChunksLoaded != ASYNC_NUM_CHUNKS_PER_FRAME)
                {
                    chunk.Load();
                    numOfChunksLoaded++;
                    this._forceVisibilityUpdate = true;

                }
            }

            ChunkLoadList.Clear();
        }

        public void UpdateSetupList()
        {
            Chunk chunk;

            for (int i = 0; i < ChunkSetupList.Count; i++)
            {
                chunk = ChunkSetupList[i];

                if(chunk.IsLoaded && !chunk.IsSetup)
                {
                    chunk.Setup();
                    if(chunk.IsSetup)
                    {
                        // Only force the visibility update if we actually setup the chunk, some chunks wait in the pre-setup stage...
                        this._forceVisibilityUpdate = true;
                    }
                }
            }

            ChunkSetupList.Clear();
        }

        //// Rebuild any chunks that are in the rebuild chunk list
        public void UpdateRebuildList()
        {

            int numRebuiltChunkThisFrame = 0;
            Chunk chunk;

            for (int i = 0; i < ChunkRebuildList.Count && i != ASYNC_NUM_CHUNKS_PER_FRAME; i++)
            {
                chunk = ChunkSetupList[i];

                if(chunk.IsLoaded && chunk.IsSetup)
                {
                    if (numRebuiltChunkThisFrame != ASYNC_NUM_CHUNKS_PER_FRAME)
                    {
                        chunk.RebuildMesh();

                        this.ChunkUpdateFlagsList.Add(chunk);

                        // Also add our neighbours since they might now be surrounded too (If we have neighbours)
                        Chunk chunkXMinus = GetChunk(chunk.Position + new Vector3(-1, 0, 0));
                        Chunk chunkXPlus = GetChunk(chunk.Position + new Vector3(1, 0, 0));
                        Chunk chunkYMinus = GetChunk(chunk.Position + new Vector3(0, -1, 0));
                        Chunk chunkYPlus = GetChunk(chunk.Position + new Vector3(0, 1, 0));
                        Chunk chunkZMinus = GetChunk(chunk.Position + new Vector3(0, 0, -1));
                        Chunk chunkZPlus = GetChunk(chunk.Position + new Vector3(0, 0, 1));

                        if (chunkXMinus != null)
                        {
                            this.ChunkUpdateFlagsList.Add(chunkXMinus);
                        }

                        if (chunkXPlus != null)
                        {
                            this.ChunkUpdateFlagsList.Add(chunkXPlus);
                        }

                        if (chunkYMinus != null)
                        {
                            this.ChunkUpdateFlagsList.Add(chunkYMinus);
                        }

                        if (chunkYPlus != null)
                        {
                            this.ChunkUpdateFlagsList.Add(chunkYPlus);
                        }

                        if (chunkZMinus != null)
                        {
                            this.ChunkUpdateFlagsList.Add(chunkZMinus);
                        }

                        if (chunkZPlus != null)
                        {
                            this.ChunkUpdateFlagsList.Add(chunkZPlus);
                        }

                        // Only rebuild a certain number of chunks per frame
                        numRebuiltChunkThisFrame++;

                        _forceVisibilityUpdate = true;
                    }

                }
            }

            ChunkRebuildList.Clear();
        }

        public void UpdateFlagsList()
        {
            Chunk chunk;

            for (int i = 0; i < ChunkRebuildList.Count; i++)
            {
                chunk = ChunkUpdateFlagsList[i];
                //Do stuff
            }

            ChunkUpdateFlagsList.Clear();
        }

        public void UpdateUnloadList()
        {
            Chunk chunk;

            for (int i = 0; i < ChunkUnLoadList.Count; i++)
            {
                chunk = ChunkUnLoadList[i];
                if (chunk.IsLoaded)
                {
                    chunk.Unload();
                }
            }

            _forceVisibilityUpdate = true;

            ChunkUnLoadList.Clear();
        }

        public void UpdateVisibilityList(Vector3 cameraPosition)
        {
           
            ChunkVisibilityList.Clear();
            Chunk chunk;

            for (var i = 0; i < this.Chunks.Count; i++)
            {
                chunk = this.Chunks[i];

                if (chunk.IsVisible)
                {
                    if (!chunk.IsLoaded)
                    {
                        ChunkLoadList.Add(chunk);
                    }

                    if (!chunk.IsSetup)
                    {
                        ChunkSetupList.Add(chunk);
                    }

                    ChunkVisibilityList.Add(chunk);
                }
                else
                {
                    if (InView(chunk.Bounds))
                    {
                        chunk.IsVisible = true;
                    }
                    else
                    {
                        if (chunk.IsLoaded)
                        {
                            ChunkUnLoadList.Add(chunk);
                        }

                    }
                }
            }
           
        }

        public void UpdateRenderList()
        {

            // Clear the render list each frame BEFORE we do our tests to see what chunks should be rendered
            this.ChunkRenderList.Clear();
            Chunk chunk;

            for (int i = 0; i < ChunkVisibilityList.Count; i++)
            {
                chunk = ChunkVisibilityList[i];
                if(chunk != null)
                {
                    if(chunk.IsLoaded && chunk.IsSetup)
                    {
                        if (chunk.ShouldRender())  // Early flags check so we don't always have to do the frustum check...
                        {
                            if (InView(chunk.Bounds))
                            {
                                this.ChunkRenderList.Add(chunk);
                            }
                            else
                            {
                                chunk.IsVisible = false;
                            }
                        }
                        else
                        {
                            chunk.IsVisible = false;
                        }
                    }
                    else
                    {
                        chunk.IsVisible = false;
                    }
                }
            }
         
        }

        public void Update(GameTime gameTime)
        {
            UpdateAsyncChunker();

            UpdateLoadList();

            UpdateSetupList();

            UpdateRebuildList();

            UpdateFlagsList();

            UpdateUnloadList();

            UpdateVisibilityList(_cameraPosition);

            if (_cameraPosition != GameDirector.Camera.Position || _cameraView != GameDirector.Camera.View)
            {
                UpdateRenderList();
            }

            _cameraPosition = GameDirector.Camera.Position;
            _cameraView = GameDirector.Camera.View;
        }

        public Chunk GetChunk(Vector3 position)
        {
            Chunk chunk;

            for(var i = 0; i < this.Chunks.Count; i++)
            {
                chunk = this.Chunks[i];
                if( chunk.Position.Equals(position))
                {
                    return chunk;
                }
            }

            return null;
        }

        public void Render() {

            for(var i = 0; i < ChunkRenderList.Count; i++)
            {
                ChunkRenderList[i].Render();
            }
        }

        bool InView(BoundingSphere boundingSphere)
        {
            return new BoundingFrustum(GameDirector.Camera.View * GameDirector.Camera.Projection).Intersects(boundingSphere);
        }
    }
}
