using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Installation.Controller.Settings;
using Installation.Models;
using Installation.Models.Interfaces;
using Installation.Models.Notify;

namespace Installation.Controller.Communication
{
    public class InternalCommunicationController : IObjectReceiver<Notify<Executable>, Task>
    {
        private EventDispatcher eventDispatcher;
        SettingsContainer settingsContainer;

        public InternalCommunicationController(EventDispatcher eventDispatcher, SettingsContainer settingsContainer)
        {
            this.eventDispatcher = eventDispatcher;
            this.settingsContainer = settingsContainer;
        }

        public async Task Receive(Notify<Executable> rObject)
        {
            throw new NotImplementedException();
        }
        public async Task RunAsync()
        {
            throw new NotImplementedException();
        }
    }
}
