using Microsoft.Xna.Framework;
using SSGL.Entity.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSGL.Entity.Tile
{
    public class BaseTileMap : IEntity, ITileMap
    {
        public BaseTileMap(BaseMap map)
        {
            Console.WriteLine("Loaded map!");
        }

        public void Init()
        {
            throw new NotImplementedException();
        }

        public void Load()
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public void Draw(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
