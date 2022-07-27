using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Installation.Communication;
using Installation.Models;
using Installation.Controller.ExecutableControllers;
using System.Threading;
using Installation.Controller.Settings;
using Installation.Controller.ExecutableFinders;
using System.Collections.Concurrent;
using Serilog;

namespace Installation.Controller
{
    public class ServiceController
    {
        private CancellationTokenSource cancellationTokenSource;

        private GlobalSettings globalSettings = new GlobalSettings();

        public Dictionary<Guid, Executable> Executables = new Dictionary<Guid, Executable>();



        private ExecutableFinder finder;


        private ServerCommunicator serverCommunicator;
        private ExecutionController executionController;

        private ConcurrentQueue<Job> jobsQueue;

        private Task communicatorTask;
        private Task executionTask;
        private Task finderTask;


        public ServiceController()
        {
            cancellationTokenSource = new CancellationTokenSource();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .WriteTo.File(globalSettings.ServerLogsFilePath, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
                .CreateLogger();


            
        }
        
        public void Start()
        {
            Log.Information("------PROGRAM STARTED------");
            try
            {
                globalSettings.LoadSettings();
                globalSettings.ExecutablesSettings.Add(globalSettings.GetSettings<ScriptSettings>());
                //finder.FindExecutables(Executables);
            }
            catch(Exception ex)
            {
                Log.Fatal(ex, "Could not load settings file or executables");
                return;
            }
            finder = new ExecutableFinder(globalSettings, Executables, cancellationTokenSource.Token);

            finder.OnExecutableAddedOrModified += newExecutableAsync;

            serverCommunicator = new ServerCommunicator(cancellationTokenSource.Token);
            serverCommunicator.OnJobReceived += newJob;
            serverCommunicator.OnCommandReceived += newCommand;

            jobsQueue = new ConcurrentQueue<Job>();

            executionController = new ExecutionController(Executables, jobsQueue);
            executionController.OnCompleted += executionCompleted;


            communicatorTask = Task.Run(() => serverCommunicator.ListenAsync());
            executionTask = Task.Run(() => executionController.RunControllerAsync(cancellationTokenSource.Token));
            finderTask = Task.Run(() => finder.RunAsync());

        }
        public async Task Stop()
        {
            cancellationTokenSource.Cancel();
            Log.Verbose("Waiting for all task to finish");

            try
            {
                //communicatorTask will not be aborted with cancellationToken because it is not working.
                await executionTask;
                await finderTask;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Exception occured at stopping tasks");
            }
            Log.Information("------PROGRAM ENDED------ \n\n");
            Log.CloseAndFlush();

        }
        private async Task newJob(Job job)
        {
            Log.Debug("New job received with job id: {jid}", job.JobID);
            jobsQueue.Enqueue(job);
        }
        private async Task executionCompleted(Job job)
        {
            Log.Debug("send job with job id: {jid}", job.JobID);
            await serverCommunicator.SendJobAsync(job).ConfigureAwait(false);
        }
        private async Task newCommand(Command command)
        {
            Log.Debug("Command received {command}", command);
            if(command == Command.SendExecutables)
            {
                foreach(var executable in Executables)
                {
                    await serverCommunicator.SendExecutableAsync(executable.Value);
                }
            }
        }
        private async Task newExecutableAsync(Executable executable)
        {
            if(serverCommunicator.ClientConnected)
                await serverCommunicator.SendExecutableAsync(executable);
        }

    }
}
