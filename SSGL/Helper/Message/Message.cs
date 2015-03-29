using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSGL.Helper.Message
{
    public class Message
    {
        public Guid Sender { get; set; }
        public Guid Receiver { get; set; }
        public Guid Type { get; set; }
        public IMessageData Data { get; set; }
    }
}
