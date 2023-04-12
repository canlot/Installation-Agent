using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Installation.Models;
using Installation.Models.Interfaces;

namespace Installation.Controller.Communication
{
    public class InternalCommunicationController
    {
        private EventDispatcher eventDispatcher;

        public InternalCommunicationController(EventDispatcher eventDispatcher)
        {
            this.eventDispatcher = eventDispatcher;
        }
        

    }
}
