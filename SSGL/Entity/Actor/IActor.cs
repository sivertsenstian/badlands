using SSGL.Component;
using SSGL.Helper.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSGL.Entity.Actor
{
    public interface IActor
    {
        Guid Id { get; set; }
        string Name { get; set; }

        void Broadcast(Message message);
        BaseComponent GetComponent(string name);
    }
}
