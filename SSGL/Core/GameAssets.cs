using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using SSGL.Entity;
using SSGL.Entity.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSGL.Core
{
    public class GameAssets
    {
        public Dictionary<Enum, Texture2D> Textures { get; set; }

        public Dictionary<Enum, BaseMap> Maps { get; set; }

        public Dictionary<Enum, SoundEffect> Sounds { get; set; }

        public Dictionary<Enum, SoundEffect> Music { get; set; }

        public GameAssets()
        {
            Textures = new Dictionary<Enum, Texture2D>();
            Maps = new Dictionary<Enum, BaseMap>();
            Sounds = new Dictionary<Enum, SoundEffect>();
            Music = new Dictionary<Enum, SoundEffect>();
        }

    }
}
