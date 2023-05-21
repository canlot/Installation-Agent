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
using Installation.Models.Notify;

namespace Installation.Controller.ExecutableControllers
{
    public class ExecutableController : IObjectReceiver<CommandGetExecutable, ExecutableBase>
    {
        private EventDispatcher eventDispatcher;
        private ExecutableFinder finder;
        private Task finderTask;
        private SettingsContainer settingsContainer;

        public Dictionary<Guid, ExecutableBase> Executables = new Dictionary<Guid, ExecutableBase>();
        public ExecutableController(EventDispatcher eventDispatcher, SettingsContainer settingsContainer)
        {
            this.eventDispatcher = eventDispatcher;
            this.settingsContainer = settingsContainer;
            eventDispatcher.RegisterReceiver<CommandGetExecutable, ExecutableBase>(this);

            
        }
        
        public ExecutableBase Receive(CommandGetExecutable command)
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
        private async Task newExecutableAsync(ExecutableBase executable)
        {
            await eventDispatcher.Send<Notify<ExecutableBase>, Task>(new Notify<ExecutableBase>
            {
                Object = executable
            });
        }
    }
}
