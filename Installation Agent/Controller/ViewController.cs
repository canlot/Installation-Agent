using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Installation.Models;
using Installation.Communication;
using System.Threading;
using System.Windows.Data;
using System.Windows.Controls;
using Serilog;
using System.ComponentModel;

namespace Installation_Agent.Controller
{
    public class ViewController
    {
        public ObservableCollection<Executable> Executables = new ObservableCollection<Executable>();
        public CollectionViewSource ExecutablesViewSource { get; set; } = new CollectionViewSource();
        public ICollectionView ExecutableCollection { get; set; }
        

        ClientCommunicator clientCommunicator;
        CancellationTokenSource cancellationTokenSource;

        Task runningNamedPipe;
        private readonly object _lock = new object();
        public string SearchText = "";

        public ViewController()
        {

            BindingOperations.EnableCollectionSynchronization(Executables, _lock);

            ExecutablesViewSource.Source = Executables;
            ExecutableCollection = CollectionViewSource.GetDefaultView(ExecutablesViewSource.View);
            ExecutableCollection.Filter = ExecutableFilter;

            cancellationTokenSource = new CancellationTokenSource();
            clientCommunicator = new ClientCommunicator(cancellationTokenSource.Token);
            clientCommunicator.OnExecutableReceived += newExecutableReceivedAsync;
            clientCommunicator.OnJobReceived += newJobReceivedAsync;
            clientCommunicator.OnClientConnected += OnClientConnectedAsync;


            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
                .CreateLogger();
            
        }
        
        private async Task OnClientConnectedAsync()
        {
            await clientCommunicator.SendCommandAsync(Command.SendExecutables);
        }
        private bool ExecutableFilter(object item)
        {
            if(item == null)
                return false;
            Executable executable = item as Executable;
            return executable.Name.ToLower().Contains(SearchText.ToLower());
        }

        public async Task RunAsync()
        {
            runningNamedPipe = clientCommunicator.ConnectAsync();
        }
        public async Task StopAsync()
        {
            cancellationTokenSource.Cancel();
            await runningNamedPipe;
        }
        private async Task newJobReceivedAsync(Job job)
        {
            Log.Debug("New job recieved {@job}", job);
            try
            {
                foreach(var executable in Executables)
                {
                    if(executable.Id == job.ExecutableID)
                    {
                        executable.StatusState = job.StatusState;
                        executable.StatusMessage = job.StatusMessage;
                        executable.CurrentlyRunning = false;

                        switch(job.StatusState)
                        {
                            case StatusState.Success:
                            case StatusState.Warning:
                                setState(executable, job.Action, true);
                                break;
                            case StatusState.Error:
                                setState(executable, job.Action, false);
                                break;
                        }
                        

                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "No job");
            }
        }

        private void setState(Executable executable, ExecuteAction action, bool state)
        {
            if(executable is ApplicationExecutable)
            {
                var applicationExecutable = (ApplicationExecutable)executable;
                switch (action)
                {
                    case ExecuteAction.Install:
                        applicationExecutable.Installed = state;
                        break;
                    case ExecuteAction.Reinstall:
                        applicationExecutable.ReInstalled = state;
                        break;
                    case ExecuteAction.Uninstall:
                        applicationExecutable.UnInstalled = state;
                        break;

                }    
            }
            else if(executable is ScriptExecutable)
            {
                var scriptExecutable = (ScriptExecutable)executable;
                if(action == ExecuteAction.Run)
                    scriptExecutable.Runned = state;
            }
            
        }

        private async Task newExecutableReceivedAsync(Executable executable)
        {
            Log.Debug("Executable received {id}", executable.Id);
            if(executable != null)
            {
                Executables.Add(executable);
            }
        }
        public void SendJob(Job job)
        {
            
        }
        private void mapExecutable(Executable executable)
        {
            
        }
        public async Task RunJob(Executable executable)
        {
            var job = new Job()
            {
                Action = ExecuteAction.Run,
                ExecutableID = executable.Id,
                ExecutionState = ExecutionState.Started
            }.WithNewGuiD();
            await clientCommunicator.SendJobAsync(job);
        }
        public async Task InstallApplication(Executable executable)
        {
            var job = new Job()
            {
                Action = ExecuteAction.Install,
                ExecutableID = executable.Id,
                ExecutionState = ExecutionState.Started
            }.WithNewGuiD();
            await clientCommunicator.SendJobAsync(job);
        }
        public async Task ReinstallApplication(Executable executable)
        {
            var job = new Job()
            {
                Action = ExecuteAction.Reinstall,
                ExecutableID = executable.Id,
                ExecutionState = ExecutionState.Started
            }.WithNewGuiD();
            await clientCommunicator.SendJobAsync(job);
        }
        public async Task UninstallApplication(Executable executable)
        {
            var job = new Job()
            {
                Action = ExecuteAction.Uninstall,
                ExecutableID = executable.Id,
                ExecutionState = ExecutionState.Started
            }.WithNewGuiD();
            await clientCommunicator.SendJobAsync(job);
        }
    }
}
