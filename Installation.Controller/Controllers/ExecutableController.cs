﻿using Installation.Models.Commands;
using Installation.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Controller.ExecutableControllers
{
    public class ExecutableController : IObjectReceiver<CommandInstallExecutable>,
                                        IObjectReceiver<CommandReinstallExecutable>,
                                        IObjectReceiver<CommandUninstallExecutable>,
                                        IObjectReceiver<CommandRunExecutable>
    {
        private EventDispatcher eventDispatcher;
        public ExecutableController(EventDispatcher eventDispatcher)
        {
            this.eventDispatcher = eventDispatcher;
        }

        public void Receive(CommandInstallExecutable command)
        {
            throw new NotImplementedException();
        }

        public void Receive(CommandReinstallExecutable rObject)
        {
            throw new NotImplementedException();
        }

        public void Receive(CommandUninstallExecutable rObject)
        {
            throw new NotImplementedException();
        }

        public void Receive(CommandRunExecutable rObject)
        {
            throw new NotImplementedException();
        }
    }
}
