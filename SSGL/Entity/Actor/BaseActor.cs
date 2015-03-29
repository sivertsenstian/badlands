using Microsoft.Xna.Framework;
using SSGL.Component;
using SSGL.Helper.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSGL.Entity.Actor
{
    public class BaseActor : IEntity, IActor
    {
        private Dictionary<string, BaseComponent> Components { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual void Init(){

        }

        public virtual void Load()
        {

        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(GameTime gameTime)
        {

        }

        public virtual void Dispose()
        {

        }

        public virtual void Broadcast(Message message)
        {
            foreach (KeyValuePair<string, BaseComponent> component in this.Components)
            {
                component.Value.Send(message);
            }
        }

        public BaseComponent GetComponent(string name)
        {
            throw new NotImplementedException();
        }
    }
}
