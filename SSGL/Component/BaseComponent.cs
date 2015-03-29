using Microsoft.Xna.Framework;
using SSGL.Entity;
using SSGL.Entity.Actor;
using SSGL.Helper.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSGL.Component
{
    public abstract class BaseComponent : IEntity, IComponent
    {
        public BaseActor Owner { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual void Init()
        {
            
        }

        public virtual void Load()
        {
            throw new NotImplementedException();
        }

        public virtual void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public virtual void Draw(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
       
        public void Send(Message message)
        {
            throw new NotImplementedException();
        }

        public void Receive(Message message)
        {
            throw new NotImplementedException();
        }
    }
}
