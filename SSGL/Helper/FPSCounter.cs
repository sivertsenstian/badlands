#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SSGL.Core;
using SSGL.Entity.UI;
using System;
#endregion

namespace SSGL.Helper
{
    public class FPSCounter : BaseUI
    {
        public SpriteFont Font { get; set; }

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }


        public override void Draw(GameTime gameTime)
        {
            frameCounter++;
            string fps = string.Format("fps: {0} mem : {1}", frameRate, GC.GetTotalMemory(false));
            GameDirector.SpriteBatch.DrawString(Font, fps, new Vector2(1, 1), Color.Black);
            GameDirector.SpriteBatch.DrawString(Font, fps, new Vector2(0, 0), Color.White);
        }
    }
}
