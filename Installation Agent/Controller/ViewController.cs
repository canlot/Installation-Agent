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
    public class ViewController : INotifyPropertyChanged
    {
        public ObservableCollection<Executable> Executables = new ObservableCollection<Executable>();
        public CollectionViewSource ExecutablesViewSource { get; set; } = new CollectionViewSource();
        public ICollectionView ExecutableCollection { get; set; }
        

        ClientCommunicator clientCommunicator;
        CancellationTokenSource cancellationTokenSource;

        Task runningNamedPipe;
        private readonly object _lock = new object();
        public string SearchText = "";

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private bool serviceConnected = false;
        public bool ServiceConnected
        {
            get => serviceConnected;
            set
            {
                serviceConnected = value;
                OnPropertyChanged("ServiceConnected");
            }
        }
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
            clientCommunicator.OnClientDisconnected += OnClientDisconnectedAsync;

            var logFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Installation Agent\" + "log.txt";
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(logFile, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
                .CreateLogger();
            
        }
        
        private async Task OnClientConnectedAsync()
        {
            ServiceConnected = true;
            await clientCommunicator.SendCommandAsync(Command.SendExecutables);
        }
        private async Task OnClientDisconnectedAsync()
        {
            ServiceConnected = false;
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
                        applicationExecutable.SuccessfulRollout = state;
                        break;
                    case ExecuteAction.Reinstall:
                        applicationExecutable.SuccessfulRollout = state;
                        break;
                    case ExecuteAction.Uninstall:
                        applicationExecutable.SuccessfulRollout = !state;
                        break;

                }    
            }
            else if(executable is ScriptExecutable)
            {
                var scriptExecutable = (ScriptExecutable)executable;
                if(action == ExecuteAction.Run)
                    scriptExecutable.SuccessfulRollout = state;
            }
            
        }

        private async Task newExecutableReceivedAsync(Executable executable)
        {
            Log.Debug("Executable received {id}", executable.Id);
            if (executable != null)
            {
                if (!Executables.Contains(executable))
                    Executables.Add(executable);
                else
                    Executables[Executables.IndexOf(executable)].CurrentlyRunning = executable.CurrentlyRunning;
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
