using Installation.Models.Commands;
using Installation.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Controller.ExecutableControllers
{
    public class ExecutableController 
    {
        private EventDispatcher eventDispatcher;
        public ExecutableController(EventDispatcher eventDispatcher)
        {
            this.eventDispatcher = eventDispatcher;
        }

    }
}
