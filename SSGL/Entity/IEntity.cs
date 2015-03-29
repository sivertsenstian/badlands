using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSGL.Entity
{
    public interface IEntity
    {
        void Init();
        void Load();
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
        void Dispose();
    }
}
