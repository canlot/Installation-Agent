using Installation.Executors;
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
    public class ApplicationExecutable : Executable, IInstalable, IReinstallable, IUninstallable
    {
        private string installFilePath;
        private string installArguments;
        
        [ExecutableSetting]
        public string InstallFilePath { get => installFilePath; set => installFilePath = value; }

        [ExecutableSetting(Mandatory = false)]
        public string ReinstallFilePath { get ; set ; }
        
        [ExecutableSetting]
        public string UninstallFilePath { get ; set; }


        [ExecutableSetting(Mandatory = false)]
        public string InstallArguments { get => installArguments; set => installArguments = value; }
        [ExecutableSetting(Mandatory = false)]
        public string ReinstallArguments { get; set; }
        [ExecutableSetting(Mandatory = false)]
        public string UninstallArguments { get; set; }

        private bool installed;
        private bool reinstalled;
        private bool uninstalled;
        
        public bool Installed { get => installed; set { installed = value; setSuccessfulRolloutState(); OnPropertyChanged("Installed");  } }
        public bool ReInstalled { get => reinstalled; set { reinstalled = value; setSuccessfulRolloutState(); OnPropertyChanged("ReInstalled"); } }
        public bool UnInstalled { get => uninstalled; set { uninstalled = value; setSuccessfulRolloutState(); OnPropertyChanged("UnInstalled"); } }


        [ExecutableSetting(Mandatory = false)]
        public List<int> SuccessfullInstallReturnCodes { get; set; }
        [ExecutableSetting(Mandatory = false)]
        public List<int> SuccessfullReInstallReturnCodes { get; set; }
        [ExecutableSetting(Mandatory = false)]
        public List<int> SuccessfullUnInstallReturnCodes { get; set; }

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
            Log.Information("Installing application: {name}, file: {file}, dir {dir}", Name, InstallFilePath, ExecutableDirectory);

            var executor = Executor.GetExecutor(installFilePath, InstallArguments, ExecutableDirectory, cancellationToken);
            try
            {
                CurrentlyRunning = true;

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
                CurrentlyRunning = false;
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
                CurrentlyRunning = true;

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
                CurrentlyRunning = false;
            }
        }


        public async Task UninstallAsync(CancellationToken cancellationToken)
        {
            Log.Information("Uninstalling application: {name}, file: {file}, dir: {dir}", Name, UninstallFilePath, ExecutableDirectory);


            var executor = Executor.GetExecutor(UninstallFilePath, UninstallArguments, ExecutableDirectory, cancellationToken);
            try
            {
                CurrentlyRunning = true;

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
                CurrentlyRunning = false;
            }

        }
        private bool getStateFromResult()
        {
            setStateToFalse();
            switch (StatusState)
            {
                case StatusState.Success:
                    return true;
                case StatusState.Warning:
                    return true;
                case StatusState.Error:
                    return false;
                default:
                    return false;
            }
        }
        private void setStateToFalse()
        {
            installed = false;
            reinstalled = false;
            uninstalled = false;
        }
    }
}
