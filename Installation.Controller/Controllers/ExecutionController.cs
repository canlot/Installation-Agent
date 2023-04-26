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
            if (command is CommandExecute)
                addExecutableToQueue(command as CommandExecuteExecutable);
            else if (command is CommandExecuteUnit)
                addExecutableUnitToQueue(command as CommandExecuteUnit);

            executionWaiting.Set();
        }
        private void enqueueList(ConcurrentQueue<ExecutableUnit> queue, List<ExecutableUnit> list)
        {
            foreach(var item in list)
                queue.Enqueue(item);
        }
        private void addExecutableToQueue(CommandExecuteExecutable command)
        {
            var executable = eventDispatcher.Send<CommandGetExecutable, Executable>(new CommandGetExecutable
            { ExecutableID = command.ExecutableID });
            if (executable == null)
            {
                Log.Error("no executable with id {id} found", command.ExecutableID);
                return;
            }

            if (command is CommandInstallExecutable && executable is IInstallable)
            {
                Log.Verbose("Add installable units from executable {id} with name {name} to executable queue", executable.Id, executable.Name);
                enqueueList(this.executableUnitQueue, (executable as IInstallable).InstallableUnits);
            }
            else if (command is CommandReinstallExecutable && executable is IReinstallable)
            {
                Log.Verbose("Add reinstallable units from executable {id} with name {name} to executable queue", executable.Id, executable.Name);
                enqueueList(this.executableUnitQueue, (executable as IReinstallable).ReinstallableUnits);
            }
            else if (command is CommandUninstallExecutable && executable is IUninstallable)
            {
                Log.Verbose("Add uninstallable units from executable {id} with name {name} to executable queue", executable.Id, executable.Name);
                enqueueList(this.executableUnitQueue, (executable as IUninstallable).UninstallableUnits);
            }
            else if (command is CommandRunExecutable && executable is IRunnable)
            {
                Log.Verbose("Add runnable units from executable {id} with name {name} to executable queue", executable.Id, executable.Name);
                enqueueList(this.executableUnitQueue, (executable as IRunnable).RunnableUnits);
            }
            else
            {
                Log.Error("Command {Command} and Executable {Executable} doesn't match", command, executable);
            }
        }
        private void findAndReturn(Guid unitId, ConcurrentQueue<ExecutableUnit> queue, List<ExecutableUnit> executableUnits)
        {

        }
        private void addExecutableUnitToQueue(CommandExecuteUnit command)
        {
            var executable = eventDispatcher.Send<CommandGetExecutable, Executable>(new CommandGetExecutable
            { ExecutableID = command.ExecutableID });
            if(executable == null)
            {
                Log.Error("no executable with id {id} found", command.ExecutableID);
                return;
            }

            if (executable is IInstallable)
            {
                Log.Verbose("Add installable units from executable {id} with name {name} to executable queue", executable.Id, executable.Name);
                executableUnitQueue.Enqueue((executable as IInstallable).InstallableUnits
                    .Find(x => x.Id == command.ExecutableUnitID));
            }
            else if (executable is IReinstallable)
            {
                Log.Verbose("Add reinstallable units from executable {id} with name {name} to executable queue", executable.Id, executable.Name);
                executableUnitQueue.Enqueue((executable as IReinstallable).ReinstallableUnits
                    .Find(x => x.Id == command.ExecutableUnitID));
            }
            else if (executable is IUninstallable)
            {
                Log.Verbose("Add uninstallable units from executable {id} with name {name} to executable queue", executable.Id, executable.Name);
                executableUnitQueue.Enqueue((executable as IUninstallable).UninstallableUnits
                    .Find(x => x.Id == command.ExecutableUnitID));
            }
            else if (executable is IRunnable)
            {
                Log.Verbose("Add runnable units from executable {id} with name {name} to executable queue", executable.Id, executable.Name);
                executableUnitQueue.Enqueue((executable as IRunnable).RunnableUnits
                    .Find(x => x.Id == command.ExecutableUnitID));
            }
            else
            {
                Log.Error("Command {Command} and Executable {Executable} doesn't match", command, executable);
            }


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
