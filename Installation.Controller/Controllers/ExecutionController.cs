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
using Installation.Models.Responses;
using System.Dynamic;

namespace Installation.Controller.ExecutableControllers
{
    public class ExecutionController :  IObjectReceiver<CommandExecute>
    {
        public delegate Task ExecutionCompleted(Job job);
        public event ExecutionCompleted OnCompleted;


        private ManualResetEventSlim executionWaiting = new ManualResetEventSlim(false);
        private EventDispatcher eventDispatcher;
        private ConcurrentQueue<CommandExecute> commandQueue;
        private ConcurrentQueue<ExecutableUnit> executableUnitQueue;

        public ExecutionController(EventDispatcher eventDispatcher)
        {
            this.eventDispatcher = eventDispatcher;
            this.eventDispatcher.RegisterReceiver<CommandExecute>(this);
        }

        public void Receive(CommandExecute command)
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
                    CommandExecute executionCommand;
                    if(commandQueue.TryDequeue(out executionCommand))
                    {
                        try
                        {
                            if (executionCommand is CommandExecute)
                                await ExecuteExecutableAsync(executionCommand as CommandExecuteExecutable, cancellationToken).ConfigureAwait(false);
                            else if (executionCommand is CommandExecuteUnit)
                                await executeExecutableUnitAsync(executionCommand as CommandExecuteUnit, cancellationToken).ConfigureAwait(false);
                                                    }
                        catch(Exception ex)
                        {
                            Log.Error(ex, "Could't execute the command with this ID {id}", executionCommand.ExecutableID);
                        }
                        
                    }
                }
                executionWaiting.Wait(cancellationToken);
                executionWaiting.Reset();
                //await Task.Delay(500).ConfigureAwait(false);
            }
        }
        private async Task ExecuteExecutableAsync(CommandExecuteExecutable executionCommand, CancellationToken cancellationToken)
        {
            var executable = eventDispatcher.Send<CommandGetExecutable, Executable>(new CommandGetExecutable
            { ExecutableID = executionCommand.ExecutableID });

            Log.Debug("Execute executable with executable id {id} and installation action {action}", executionCommand.ExecutableID, executionCommand.ExecuteAction );


            if (executable == null)
            {
                Log.Debug("No executable with id: {eid}", executionCommand.ExecutableID);
                return;
            }
            try
            {
                if(executionCommand is CommandInstallExecutable && executable is IInstallable)
                {
                    Log.Verbose("Install {id} with name {name}", executable.Id, executable.Name);
                    await (executable as IInstallable).InstallAsync(cancellationToken);
                    await sendExecutableState(executable);
                }
                else if(executionCommand is CommandReinstallExecutable && executable is IReinstallable)
                {
                    Log.Verbose("Reinstall {id} with name {name}", executable.Id, executable.Name);
                    await (executable as IReinstallable).ReinstallAsync(cancellationToken);
                    await sendExecutableState(executable);
                }
                else if(executionCommand is CommandUninstallExecutable && executable is IUninstallable)
                {
                    Log.Verbose("Uninstall {id} with name {name}", executable.Id, executable.Name);
                    await (executable as IUninstallable).UninstallAsync(cancellationToken);
                    await sendExecutableState(executable);
                }
                else if(executionCommand is CommandRunExecutable && executable is IRunnable)
                {
                    Log.Debug("RunAsync {id} with name {name}", executable.Id, executable.Name);
                    await (executable as IRunnable).RunAsync(cancellationToken);
                    await sendExecutableState(executable);
                }
                else
                {
                    Log.Error("Command {Command} and Executable {Executable} doesn't match", executionCommand, executable );
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

        async Task executeExecutableUnitAsync(CommandExecuteUnit command, CancellationToken cancellationToken)
        {

        }

        async Task sendExecutableState(Executable executable)
        {
            var responseExecution = new ResponseExecution();
            switch(executable.StatusState)
            {
                case StatusState.Success:
                    responseExecution.StatusState = StatusState.Success;
                    responseExecution.ExecutionState = ExecutionState.Done;
                    Log.Information("Executable with id {id} executed successfully", executable.Id);
                    break;
                case StatusState.Warning:
                    responseExecution.StatusState = StatusState.Warning;
                    responseExecution.ExecutionState = ExecutionState.Done;
                    responseExecution.Message = executable.StatusMessage;
                    Log.Warning("Executable with id {id} executed with warning: {warning}", executable.Id, executable.StatusMessage);
                    break;
                case StatusState.Error:
                    responseExecution.StatusState = StatusState.Error;
                    responseExecution.ExecutionState = ExecutionState.Stopped;
                    responseExecution.Message = executable.StatusMessage;
                    Log.Error("Executable with id {id} executed with error: {warning}", executable.Id, executable.StatusMessage);
                    break;
            }
            await eventDispatcher.Send<ResponseExecution, Task>(responseExecution);
        }


    }
}
