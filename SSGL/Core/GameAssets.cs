using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using SSGL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSGL.Core
{
    public class GameAssets
    {
        public Dictionary<Enum, Texture2D> Textures { get; set; }

        public Dictionary<Enum, Color> Colors { get; set; }

        public Dictionary<Enum, SoundEffect> Sounds { get; set; }

        public Dictionary<Enum, SoundEffect> Music { get; set; }

        public Dictionary<Enum, SpriteFont> Fonts { get; set; }

        public List<Texture2D> Skybox { get; set; }

        public GameAssets()
        {
            Textures = new Dictionary<Enum, Texture2D>();
            Colors = new Dictionary<Enum, Color>();
            Sounds = new Dictionary<Enum, SoundEffect>();
            Music = new Dictionary<Enum, SoundEffect>();
            Fonts = new Dictionary<Enum, SpriteFont>();
            Skybox = new List<Texture2D>();
        }

    }
}
