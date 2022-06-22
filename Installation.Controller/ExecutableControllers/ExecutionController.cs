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

namespace Installation.Controller.ExecutableControllers
{
    public class ExecutionController
    {
        public delegate Task ExecutionCompleted(Job job);
        public event ExecutionCompleted OnCompleted;

        public Dictionary<Guid, Executable> executables;

        private ConcurrentQueue<Job> jobsQueue;

        public ExecutionController(Dictionary<Guid, Executable> executables, ConcurrentQueue<Job> jobsQueue)
        {
            this.executables = executables;
            this.jobsQueue = jobsQueue;
        }
        public async Task RunController(CancellationToken cancellationToken)
        {
            while(true)
            {
                if(cancellationToken.IsCancellationRequested)
                    return;
                if(jobsQueue.Count > 0)
                {
                    Job job;
                    if(jobsQueue.TryDequeue(out job))
                    {
                        try
                        {
                            await runJob(job, cancellationToken).ConfigureAwait(false);
                        }
                        catch(Exception ex)
                        {
                            Log.Error(ex, "Could't run the job {jid}", job.JobID);
                        }
                        
                    }
                }
                await Task.Delay(500).ConfigureAwait(false);
            }
        }
        private async Task runJob(Job job, CancellationToken cancellationToken)
        {
            Log.Debug("Execute job with job id {jid} and executable id {eid} and installation state {state}", job.JobID, job.ExecutableID, job.ExecutionState);
            if (job == null)
            {
                Log.Debug("Job is null");
                return;
            }

            if(executables.Count == 0)
            {
                Log.Error("Executables empty");
                return;
            }
            var executable = executables[job.ExecutableID];
            

            if (executable == null)
            {
                Log.Debug("No executable with id: {eid}", job.ExecutableID);
                return;
            }

            if (job.ExecutionState == ExecutionState.Started)
            {
                try
                {
                    if (job.Action == ExecuteAction.Install && executable is IInstalable)
                    {
                        Log.Verbose("Install {id} with name {name}", executable.Id, executable.Name);
                        await (executable as IInstalable).InstallAsync(cancellationToken);
                        await executionCompleted(job, executable.StatusState, executable.statusMessage);
                    }
                    else if (job.Action == ExecuteAction.Reinstall && executable is IReinstallable)
                    {
                        Log.Verbose("Reinstall {id} with name {name}", executable.Id, executable.Name);
                        await (executable as IReinstallable).ReinstallAsync(cancellationToken);
                        await executionCompleted(job, executable.StatusState, executable.statusMessage);
                    }
                    else if (job.Action == ExecuteAction.Uninstall && executable is IUninstallable)
                    {
                        Log.Verbose("Uninstall {id} with name {name}", executable.Id, executable.Name);
                        await (executable as IUninstallable).UninstallAsync(cancellationToken);
                        await executionCompleted(job, executable.StatusState, executable.statusMessage);
                    }
                    else if (job.Action == ExecuteAction.Run && executable is IRunnable)
                    {
                        Log.Debug("RunAsync {id} with name {name}", executable.Id, executable.Name);
                        await (executable as IRunnable).RunAsync(cancellationToken);
                        await executionCompleted(job, executable.StatusState, executable.statusMessage);
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
                    await executionCompleted(job, StatusState.Error, ex.Message);
                }
                
            }
            
        }
        async Task executionCompleted(Job job, StatusState state, string message)
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
