using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SSGL.Core;
using SSGL.Entity.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSGL.Entity.Tile
{
    public class TileLayer : BaseActor
    {
        public List<TileChunk> Chunks { get; set; }
        public BoundingBox BoundingBox { get; set; }
        public Enum Texture { get; set; }

        public TileLayer(Enum texture)
        {
            Chunks = new List<TileChunk>();
            Texture = texture;

            BasicEffect effect = new BasicEffect(GameDirector.Device);
            effect.Texture = GameDirector.Assets.Textures[Texture];
            effect.TextureEnabled = true;
            this._effects.Add(effect);
        }
    }
}
