using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSGL.Entity;
using Microsoft.Xna.Framework.Graphics;
using SSGL.Helper.Enum;
using SSGL.Entity.Actor;

namespace SSGL.Core
{
    public static class GameDirector
    {
        public static List<BaseActor> Actors = new List<BaseActor>();
        public static GameAssets Assets = new GameAssets();

    }
}
