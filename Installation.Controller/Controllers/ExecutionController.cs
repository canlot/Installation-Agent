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
            executableUnitQueue = new ConcurrentQueue<ExecutableUnit>();
            commandQueue = new ConcurrentQueue<CommandExecute>();
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
        private void enqueueList(ConcurrentQueue<ExecutableUnit> queue, IEnumerable<ExecutableUnit> list)
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
        private void addExecutableUnitToQueue(CommandExecuteUnit command)
        {
            var executable = eventDispatcher.Send<CommandGetExecutable, Executable>(new CommandGetExecutable
            { ExecutableID = command.ExecutableID });
            if(executable == null)
            {
                Log.Error("no executable with id {id} found", command.ExecutableID);
                return;
            }
            ExecutableUnit executableUnit = default(ExecutableUnit);

            if (executable is IInstallable)
            {
                Log.Verbose("Add installable units from executable {id} with name {name} to executable queue", executable.Id, executable.Name);
                executableUnit = (executable as IInstallable).InstallableUnits.First(x => x.Id == command.ExecutableUnitID);
            }
            else if (executable is IReinstallable)
            {
                Log.Verbose("Add reinstallable units from executable {id} with name {name} to executable queue", executable.Id, executable.Name);
                executableUnit = (executable as IReinstallable).ReinstallableUnits.First(x => x.Id == command.ExecutableUnitID);
            }
            else if (executable is IUninstallable)
            {
                Log.Verbose("Add uninstallable units from executable {id} with name {name} to executable queue", executable.Id, executable.Name);
                executableUnit = (executable as IUninstallable).UninstallableUnits.First(x => x.Id == command.ExecutableUnitID);
            }
            else if (executable is IRunnable)
            {
                Log.Verbose("Add runnable units from executable {id} with name {name} to executable queue", executable.Id, executable.Name);
                executableUnit = (executable as IRunnable).RunnableUnits.First(x => x.Id == command.ExecutableUnitID);
            }
            else
            {
                Log.Error("Executable {executable} not valid", executable);
            }
            if (executableUnit != default(ExecutableUnit))
            {
                executableUnitQueue.Enqueue(executableUnit);
            }
            else
            {
                Log.Error("No executable unit with id {id} found in executable with id {eid}", command.ExecutableUnitID, executable.Id);
            }

        }
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while(true)
            {
                if(cancellationToken.IsCancellationRequested)
                    return;
                if(executableUnitQueue.Count > 0)
                {
                    ExecutableUnit executableUnit;
                    if(executableUnitQueue.TryDequeue(out executableUnit))
                    {
                        try
                        {
                            await executeExecutableUnitAsync(executableUnit, cancellationToken);
                        }
                        catch(Exception ex)
                        {
                            Log.Error(ex, "Could't execute unit with this ID {id} with executableId {eid}", executableUnit.Id, executableUnit.ExecutableId);
                        }
                        
                    }
                }
                executionWaiting.Wait(cancellationToken);
                executionWaiting.Reset();
                //await Task.Delay(500).ConfigureAwait(false);
            }
        }
        

        async Task executeExecutableUnitAsync(ExecutableUnit executableUnit, CancellationToken cancellationToken)
        {
            await executableUnit.Execute(cancellationToken);
            
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
