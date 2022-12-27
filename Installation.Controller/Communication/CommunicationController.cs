using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Installation.Models;
using Installation.Models.Interfaces;

namespace Installation.Controller.Communication
{
    public class CommunicationController : IObjectReceiver<>
    {
        private EventDispatcher eventDispatcher;

        public CommunicationController(EventDispatcher eventDispatcher)
        {
            this.eventDispatcher = eventDispatcher;
        }
        

    }
}
