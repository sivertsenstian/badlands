using Microsoft.Xna.Framework;
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


        public void Render() {

            foreach (Chunk chunk in Chunks)
            {
                chunk.Render();
            }
        }
    }
}
