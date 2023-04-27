using Installation.Models.Commands;
using Installation.Models.Interfaces;
using Installation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Installation.Controller.ExecutableFinders;
using System.Threading;
using Installation.Controller.Settings;

namespace Installation.Controller.ExecutableControllers
{
    public class ExecutableController : IObjectReceiver<CommandGetExecutable, Executable>
    {
        private EventDispatcher eventDispatcher;
        private ExecutableFinder finder;
        private Task finderTask;
        private SettingsContainer settingsContainer;

        public Dictionary<Guid, Executable> Executables = new Dictionary<Guid, Executable>();
        public ExecutableController(EventDispatcher eventDispatcher, SettingsContainer settingsContainer)
        {
            this.eventDispatcher = eventDispatcher;
            this.settingsContainer = settingsContainer;
            eventDispatcher.RegisterReceiver<CommandGetExecutable, Executable>(this);

            
        }
        
        public Executable Receive(CommandGetExecutable command)
        {
            throw new NotImplementedException();
        }
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;
                finder = new ExecutableFinder(settingsContainer, Executables, cancellationToken);

                finder.OnExecutableAddedOrModified += newExecutableAsync;
                finderTask = Task.Run(() => finder.RunAsync());
                await finderTask;
            }
        }
        private async Task newExecutableAsync(Executable executable)
        {
            if (serverCommunicator.ClientConnected)
                await serverCommunicator.SendExecutableAsync(executable);
        }
    }
}
