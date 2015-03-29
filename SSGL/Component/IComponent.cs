using SSGL.Entity;
using SSGL.Entity.Actor;
using SSGL.Helper.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSGL.Component
{
    public interface IComponent
    {
        BaseActor Owner { get; set; }
        Guid Id { get; set; }
        string Name { get; set; }

        void Send(Message message);
        void Receive(Message message);
    }
}
