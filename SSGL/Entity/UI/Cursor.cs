using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SSGL.Core;
using SSGL.Entity.Tile;
using SSGL.Entity.UI;
using SSGL.Helper;
using SSGL.Helper.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSGL.Entity.UI
{
    public class Cursor : BaseUI
    {
        public Texture2D Texture { get; set; }
        public Color Color { get; set; }
        public Vector2 Position { get; set; }

        public Cursor()
        {
            Texture = GameDirector.Assets.Textures[Misc.DEBUG];
            Color = GameDirector.Assets.Colors[Misc.DEBUG];
            Position = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
        }

        public override void Update(GameTime gameTime)
        {
            Position = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GameDirector.SpriteBatch.Draw(Texture, Position, Color);
            base.Draw(gameTime);
        }
    }
}
