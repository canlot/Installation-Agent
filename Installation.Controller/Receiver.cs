using Installation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Controller
{
    public class Receiver
    {
        public object ReceiverObject { get; set; }
        public Type ReceivingType { get; set; }
        public CommandAction? CommandVerb { get; set; }
    }
}
