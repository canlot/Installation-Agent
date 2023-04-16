﻿using Installation.Executors;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Installation.Models
{
    [Executable("App")]
    public class ApplicationExecutable : Executable, IInstallable, IReinstallable, IUninstallable
    {
        private string installFilePath;
        private string installArguments;

        public List<ExecutableUnit> InstallableUnits { get; set; }
        public List<ExecutableUnit> UninstallableUnits { get; set; }
        public List<ExecutableUnit> ReinstallableUnits { get; set; }
        

        private bool installed;
        private bool reinstalled;
        private bool uninstalled;
        
        public bool Installed { get => installed; set { installed = value; setSuccessfulRolloutState(); OnPropertyChanged("Installed");  } }
        public bool ReInstalled { get => reinstalled; set { reinstalled = value; setSuccessfulRolloutState(); OnPropertyChanged("ReInstalled"); } }
        public bool UnInstalled { get => uninstalled; set { uninstalled = value; setSuccessfulRolloutState(); OnPropertyChanged("UnInstalled"); } }

        

        protected override void setSuccessfulRolloutState()
        {
            if ((Installed || ReInstalled) && !UnInstalled)
                successfulRollout = true;
            else
                successfulRollout = false;
        }
        private void checkExecutor(Executor executor)
        {
            if (executor == null)
                throw new NullReferenceException("No executor for this file type found");
            if (!(executor is IApplicationExecutor))
                throw new InvalidOperationException("This operation is not supported for this file type");
        }
        public async Task InstallAsync(CancellationToken cancellationToken)
        {
            Log.Information("Installing application: {name}", Name);

            foreach (var unit in InstallableUnits)
            {
                
                try
                {
                    CurrentlyExecuting = true;

                    checkExecutor(executor);
                    await (executor as IApplicationExecutor).InstallAsync();

                    setExecutionStateFromExecutor(executor, SuccessfullInstallReturnCodes);
                    Installed = getStateFromResult();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    executor?.Dispose();
                    CurrentlyExecuting = false;
                }
            }

        }
        

        public async Task ReinstallAsync(CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(ReinstallFilePath))
                throw new ArgumentNullException(nameof(ReinstallFilePath));

            Log.Information("Reinstalling application: {name}, file: {file}, dir {dir}", ReinstallFilePath, ExecutableDirectory);
            

            var executor = Executor.GetExecutor(ReinstallFilePath, ReinstallArguments, ExecutableDirectory, cancellationToken);
            try
            {
                CurrentlyExecuting = true;

                checkExecutor(executor);
                await (executor as IApplicationExecutor).ReinstallAsync();

                setExecutionStateFromExecutor(executor, SuccessfullReInstallReturnCodes);
                ReInstalled = getStateFromResult();
            }
            catch
            {
                throw;
            }
            finally
            {
                executor?.Dispose();
                CurrentlyExecuting = false;
            }
        }


        public async Task UninstallAsync(CancellationToken cancellationToken)
        {
            Log.Information("Uninstalling application: {name}, file: {file}, dir: {dir}", Name, UninstallFilePath, ExecutableDirectory);


            var executor = Executor.GetExecutor(UninstallFilePath, UninstallArguments, ExecutableDirectory, cancellationToken);
            try
            {
                CurrentlyExecuting = true;

                checkExecutor(executor);
                await (executor as IApplicationExecutor).UninstallAsync();

                setExecutionStateFromExecutor(executor, SuccessfullUnInstallReturnCodes);
                UnInstalled = getStateFromResult();
            }
            catch
            {
                throw;
            }
            finally
            {
                executor?.Dispose();
                CurrentlyExecuting = false;
            }

        }
        

        protected override void setStateToFalse()
        {
            installed = false;
            reinstalled = false;
            uninstalled = false;
        }
    }
}
