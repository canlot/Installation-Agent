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
using Installation.Models.Executables;
using Installation.Models.Interfaces;
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

        public ServiceController()
        {
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(Serilog.Events.LogEventLevel.Verbose)
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
                .CreateLogger();

            finder = new ExecutableFinder(globalSettings);
            
            
            cancellationTokenSource = new CancellationTokenSource();
            
        }
        
        public void Start()
        {
            Log.Verbose("Started App");
            try
            {
                globalSettings.LoadSettings();
                globalSettings.ExecutablesSettings.Add(globalSettings.GetSettings<ScriptSettings>());
                Executables = finder.FindExecutables();
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Could not load settings file or executables");
                return;
            }

            serverCommunicator = new ServerCommunicator(cancellationTokenSource.Token);
            serverCommunicator.OnJobReceived += newJob;

            jobsQueue = new ConcurrentQueue<Job>();

            executionController = new ExecutionController(Executables, jobsQueue);
            executionController.OnCompleted += executionCompleted;


            var communicatorTask = Task.Run(() => serverCommunicator.ListenAsync());
            var executionTask = Task.Run(() => executionController.RunController(cancellationTokenSource.Token));
            var fire = Task.Run(() => TestItWithFire());

            Log.Verbose("Waiting for all task to finish");

            Task.WaitAll(communicatorTask, executionTask, fire);
            Log.CloseAndFlush();
        }
        public async Task TestItWithFire()
        {
            while (true)
            {
                Console.WriteLine("Guid: ");
                var input = Console.ReadLine();
                if (input == "exit")
                {
                    Stop();
                    break;
                }
                Guid guid;
                if (Guid.TryParse(input, out guid))
                {
                    Job job = new Job()
                    {
                        Action = ExecuteAction.Run,
                        ExecutableID = guid,
                        ExecutionState = ExecutionState.Started
                    }.WithNewGuiD();
                    await newJob(job);
                }
            }
            
        }
        public void Stop()
        {
            cancellationTokenSource.Cancel();
            
        }
        private async Task newJob(Job job)
        {
            Log.Debug("New job received with job id: {jid}", job.JobID);
            jobsQueue.Enqueue(job);
        }
        private async Task executionCompleted(Job job)
        {
            Log.Debug("send job with job id: {jid}", job.JobID);
            await serverCommunicator.SendJob(job).ConfigureAwait(false);
        }

    }
}
