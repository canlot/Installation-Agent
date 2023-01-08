using Installation.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Installation.Storage.StateStorage;
using Installation.Models.Commands;
using Installation.Models.Interfaces;
using Installation.Models.Executables;

namespace Installation.Controller.ExecutableControllers
{
    public class ExecutionController :  IObjectReceiver<CommandInstallExecutable>,
                                        IObjectReceiver<CommandReinstallExecutable>,
                                        IObjectReceiver<CommandUninstallExecutable>,
                                        IObjectReceiver<CommandRunExecutable>
    {
        public delegate Task ExecutionCompleted(Job job);
        public event ExecutionCompleted OnCompleted;


        private ManualResetEventSlim executionWaiting = new ManualResetEventSlim(false);
        private EventDispatcher eventDispatcher;
        private ConcurrentQueue<CommandExecutableExecution> commandQueue;

        public ExecutionController(EventDispatcher eventDispatcher)
        {
            this.eventDispatcher = eventDispatcher;
        }

        public void Receive(CommandInstallExecutable command)
        {
            commandQueue.Enqueue(command);
            executionWaiting.Set();
        }

        public void Receive(CommandReinstallExecutable command)
        {
            commandQueue.Enqueue(command);
            executionWaiting.Set();
        }

        public void Receive(CommandUninstallExecutable command)
        {
            commandQueue.Enqueue(command);
            executionWaiting.Set();
        }

        public void Receive(CommandRunExecutable command)
        {
            commandQueue.Enqueue(command);
            executionWaiting.Set();
        }
        public async Task RunControllerAsync(CancellationToken cancellationToken)
        {
            while(true)
            {
                if(cancellationToken.IsCancellationRequested)
                    return;
                if(commandQueue.Count > 0)
                {
                    CommandExecutableExecution executionCommand;
                    if(commandQueue.TryDequeue(out executionCommand))
                    {
                        try
                        {
                            await runJobAsync(executionCommand, cancellationToken).ConfigureAwait(false);
                        }
                        catch(Exception ex)
                        {
                            Log.Error(ex, "Could't run the job {jid}", job.JobID);
                        }
                        
                    }
                }
                executionWaiting.Wait(cancellationToken);
                executionWaiting.Reset();
                //await Task.Delay(500).ConfigureAwait(false);
            }
        }
        private async Task runJobAsync(CommandExecutableExecution executionCommand, CancellationToken cancellationToken)
        {
            var executable = eventDispatcher.Send<CommandGetExecutable, Executable>(new CommandGetExecutable
            { ExecutableID = executionCommand.ExecutableID });

            Log.Debug("Execute executable with executable id {id} and installation action {action}", executionCommand.ExecutableID, executionCommand.ExecuteAction );


            if (executable == null)
            {
                Log.Debug("No executable with id: {eid}", executionCommand.ExecutableID);
                return;
            }

            if (job.ExecutionState == ExecutionState.Started)
            {
                try
                {
                    if (job.Action == ExecuteAction.Install && executable is IInstallable)
                    {
                        Log.Verbose("Install {id} with name {name}", executable.Id, executable.Name);
                        await (executable as IInstallable).InstallAsync(cancellationToken);
                        await executionCompletedAsync(job, executable.StatusState, executable.statusMessage);
                    }
                    else if (job.Action == ExecuteAction.Reinstall && executable is IReinstallable)
                    {
                        Log.Verbose("Reinstall {id} with name {name}", executable.Id, executable.Name);
                        await (executable as IReinstallable).ReinstallAsync(cancellationToken);
                        await executionCompletedAsync(job, executable.StatusState, executable.statusMessage);
                    }
                    else if (job.Action == ExecuteAction.Uninstall && executable is IUninstallable)
                    {
                        Log.Verbose("Uninstall {id} with name {name}", executable.Id, executable.Name);
                        await (executable as IUninstallable).UninstallAsync(cancellationToken);
                        await executionCompletedAsync(job, executable.StatusState, executable.statusMessage);
                    }
                    else if (job.Action == ExecuteAction.Run && executable is IRunnable)
                    {
                        Log.Debug("RunAsync {id} with name {name}", executable.Id, executable.Name);
                        await (executable as IRunnable).RunAsync(cancellationToken);
                        await executionCompletedAsync(job, executable.StatusState, executable.statusMessage);
                    }
                    else
                    {
                        Log.Error("Executable {name} has no {state} execution state", executable.Name, job.Action);
                    }
                    var executionStateSettings = new ExecutionStateSettings();
                    executionStateSettings.SaveExecutableState(executable);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Executable could't be executed");
                    await executionCompletedAsync(job, StatusState.Error, ex.Message);
                }
                
            }
            
        }


        async Task executionCompletedAsync(Job job, StatusState state, string message)
        {
            job.StatusState = state;
            if (state == StatusState.Success)
            {
                Log.Information("Job {jid} with excutable {eid} successfully executed", job.JobID, job.ExecutableID);
            }
            else if(state == StatusState.Warning)
            {
                Log.Warning(message);
            }
            else if(state == StatusState.Error)
            {
                Log.Error(message);
                job.ExecutionState = ExecutionState.Stopped;
            }
            job.StatusMessage = message;
            await OnCompleted(job);
            
        }
    }
}
