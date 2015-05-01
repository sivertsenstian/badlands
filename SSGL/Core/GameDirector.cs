using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSGL.Entity;
using Microsoft.Xna.Framework.Graphics;
using SSGL.Helper.Enum;
using SSGL.Entity.Actor;
using SSGL.Entity.UI;
using Microsoft.Xna.Framework;

namespace SSGL.Core
{
    public static class GameDirector
    {
        public static Dictionary<Guid, BaseActor> Actors;
        public static List<BaseUI> UI;
        public static GameAssets Assets;
        public static Camera Camera;
        public static SpriteBatch SpriteBatch;
        public static GraphicsDevice Device;
        public static Game Game;

        static GameDirector()
        {
            Actors = new Dictionary<Guid, BaseActor>();
            UI = new List<BaseUI>();
            Assets = new GameAssets();
        }

        public static BaseActor GetActor(Guid id)
        {
            return Actors[id];
        }

    }
}
