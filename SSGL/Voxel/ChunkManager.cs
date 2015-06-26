using Microsoft.Xna.Framework;
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
        public List<Chunk> ChunkUnLoadList;
        public List<Chunk> ChunkVisibilityList;
        public List<Chunk> ChunkSetupList;

        private Vector3 _cameraPosition;
        private Matrix _cameraView;
        private bool _forceVisibilityUpdate;

        public ChunkManager()
        {
            Chunks = new List<Chunk>();
            ChunkLoadList = new List<Chunk>();
            ChunkUnLoadList = new List<Chunk>();
            ChunkVisibilityList = new List<Chunk>();
            ChunkRenderList = new List<Chunk>();
            ChunkSetupList = new List<Chunk>();
        }

        public void BuildChunks(int x, int y){
            int CHUNK_SIZE = 8;
            Chunk chunk;
            for(int i = 0; i < x; i++){
                for(int j = 0; j < y; j++){
                    chunk = new Chunk(new Vector3(i * CHUNK_SIZE, 0, j * CHUNK_SIZE));
                    chunk.CreateMesh();
                    Chunks.Add(chunk);
                }
            }
        }

        public void UpdateAsyncChunker()
        {

        }

        public void UpdateLoadList()
        {
            int lNumOfChunksLoaded = 0;

            ChunkList::iterator iterator;
            for (iterator = m_vpChunkLoadList.begin(); iterator != m_vpChunkLoadList.end() &&
                          (lNumOfChunksLoaded != ASYNC_NUM_CHUNKS_PER_FRAME); ++iterator)
            {
                Chunk* pChunk = (*iterator);

                if (pChunk->IsLoaded() == false)
                {
                    if (lNumOfChunksLoaded != ASYNC_NUM_CHUNKS_PER_FRAME))
            {
                pChunk->Load();

                // Increase the chunks loaded count
                lNumOfChunksLoaded++;

                _forceVisibilityUpdate = true;
            }
        }
    }

    // Clear the load list (every frame)
    m_vpChunkLoadList.clear();
        }

        public void UpdateSetupList()
        {

        }

        public void UpdateRebuildList()
        {

        }

        public void UpdateFlagsList()
        {

        }

        public void UpdateUnloadList()
        {

        }

        public void UpdateVisibilityList(Vector3 cameraPosition)
        {

        }

        public void UpdateRenderList()
        {

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

        public void Render() {

            foreach (Chunk chunk in Chunks)
            {
                chunk.Render();
            }
        }
    }
}
