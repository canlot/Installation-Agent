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
        public ICollectionView ExecutableCollection;

        ClientCommunicator clientCommunicator;
        CancellationTokenSource cancellationTokenSource;

        Task runningNamedPipe;
        private readonly object _lock = new object();
        public string SearchText = "";
        public ViewController()
        {
            Executables.Add(new ApplicationExecutable()
            {
                Name = "Vlc",
                Version = "14.5",

            });

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
                        executable.CurrentlyRunning = false;

                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "No job");
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
        public void mapJob(Job source, Job target)
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
    }
}
