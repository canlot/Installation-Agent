using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Installation.Controller.Settings;
using Installation.Models;
using Installation.Models.Interfaces;
using Installation.Models.Notify;
using Installation.Communication;
using System.Threading;
using Installation.Models.Commands;

namespace Installation.Controller.Communication
{
    public class InternalCommunicationController : IObjectReceiver<Notify<Executable>, Task>
    {
        private EventDispatcher eventDispatcher;
        private SettingsContainer settingsContainer;

        private ServerCommunicator privilegedCommunicator;
        private ServerCommunicator unprivilegedCommunicator;
        public InternalCommunicationController(EventDispatcher eventDispatcher, SettingsContainer settingsContainer)
        {
            this.eventDispatcher = eventDispatcher;
            this.settingsContainer = settingsContainer;
            
        }

        public async Task Receive(Notify<Executable> rObject)
        {
            throw new NotImplementedException();
        }
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            privilegedCommunicator = new ServerCommunicator(cancellationToken, "InstallationAgentPrivileged", true);
            privilegedCommunicator.OnObjectReceived += ReceiveObjectFromClient;
            unprivilegedCommunicator = new ServerCommunicator(cancellationToken, "InstallationAgent");
            unprivilegedCommunicator.OnObjectReceived += ReceiveObjectFromClient;
            await Task.WhenAll(privilegedCommunicator.ListenAsync(), unprivilegedCommunicator.ListenAsync());
        }
        public async Task ReceiveObjectFromClient(object obj)
        {
            if (obj == null)
                return;
            if(obj is CommandExecuteExternal)
            {
                eventDispatcher.Send(obj as CommandExecuteExternal);
            }
        }
    }
}
