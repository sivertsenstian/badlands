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

        ChunkManager()
        {
            Chunks = new List<Chunk>();
            ChunkLoadList = new List<Chunk>();
            ChunkUnLoadList = new List<Chunk>();
            ChunkVisibilityList = new List<Chunk>();
            ChunkRenderList = new List<Chunk>();
            ChunkSetupList = new List<Chunk>();
        }
    }
}
