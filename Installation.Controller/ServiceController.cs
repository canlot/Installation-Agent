using Installation.Communication;
using Installation.Controller.ExecutableControllers;
using Installation.Controller.ExecutableFinders;
using Installation.Controller.Settings;
using Installation.Models;
using Installation.Models.Settings;
using Installation.Parser;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Installation.Controller
{
    public class ServiceController
    {
        private CancellationTokenSource cancellationTokenSource;

        private SettingsContainer SettingsContainer = new SettingsContainer();
        private ServerCommunicator serverCommunicator;

        //controllers
        private ExecutionController executionController;
        private ExecutableController executableController;


        private EventDispatcher eventDispatcher = new EventDispatcher();

        private Task communicatorTask;
        private Task executionTask;
        private Task executableTask;
        


        public ServiceController()
        {
            cancellationTokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            Log.Information("------PROGRAM STARTED------");
            try
            {
                SettingsBuilder settingsBuilder = new SettingsBuilder();
                SettingsContainer.GlobalSettings = settingsBuilder.GetSettings<GlobalSettings>();

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Could not load settings file or executables");
                return;
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .WriteTo.File(SettingsContainer.GlobalSettings.ServerLogsFilePath, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
                .CreateLogger();

            

            serverCommunicator = new ServerCommunicator(cancellationTokenSource.Token);

            executableController = new ExecutableController(eventDispatcher, SettingsContainer);

            executionController = new ExecutionController(eventDispatcher);


            communicatorTask = Task.Run(() => serverCommunicator.ListenAsync());
            executionTask = Task.Run(() => executionController.RunAsync(cancellationTokenSource.Token));
            executableTask = Task.Run(() => executableController.RunAsync(cancellationTokenSource.Token));
            
            

        }
        public async Task Stop()
        {
            cancellationTokenSource.Cancel();
            Log.Verbose("Waiting for all task to finish");

            try
            {
                //communicatorTask will not be aborted with cancellationToken because it is not working.
                await executionTask;
                await executableTask;
                
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Exception occured at stopping tasks");
            }
            Log.Information("------PROGRAM ENDED------ \n\n");
            Log.CloseAndFlush();

        }
        
    }
}
