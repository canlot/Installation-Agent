using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Installation.Models.Executables;
using Installation.Models;
using Installation.Models.Interfaces;
using Installation.Communication;
using System.Threading;
using System.Windows.Data;
using System.Windows.Controls;
using Serilog;

namespace Installation_Agent.Controller
{
    public class ViewController
    {
        public ObservableCollection<Executable> Apps = new ObservableCollection<Executable>();
        public ObservableCollection<ScriptExecutable> Scripts = new ObservableCollection<ScriptExecutable>();
        ClientCommunicator clientCommunicator;
        CancellationTokenSource cancellationTokenSource;
        Task runningNamedPipe;
        private readonly object _lock = new object();
        public ViewController()
        {
            BindingOperations.EnableCollectionSynchronization(Scripts, _lock);
            cancellationTokenSource = new CancellationTokenSource();
            clientCommunicator = new ClientCommunicator(cancellationTokenSource.Token);
            clientCommunicator.OnExecutableReceived += newExecutableReceivedAsync;
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
                .CreateLogger();

        }

        public async Task RunAsync()
        {
            runningNamedPipe = clientCommunicator.ConnectAsync();
            await clientCommunicator.SendCommandAsync(Command.SendExecutables);
        }
        public async Task StopAsync()
        {
            cancellationTokenSource.Cancel();
            await runningNamedPipe;
        }
        private async Task newJob(Job job)
        {

        }
        private async Task newExecutableReceivedAsync(Executable executable)
        {
            Log.Debug("Executable received {id}", executable.Id);
            if(executable != null)
            {
                if (executable is IRunnable)
                {
                    Log.Verbose("Executable is Script");
                    Scripts.Add((ScriptExecutable)executable);
                    
                }
                else
                    Apps.Add(executable);
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
