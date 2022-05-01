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
        public string InstallFilePath { get => installFilePath; set => installFilePath = value.Replace("\"", ""); }

        [ExecutableSetting(Mandatory = false)]
        public string ReinstallFilePath { get ; set ; }
        
        [ExecutableSetting]
        public string UninstallFilePath { get ; set; }


        [ExecutableSetting(Mandatory = false)]
        public string InstallArguments { get => installArguments; set => installArguments = value.Replace("\"", ""); }
        [ExecutableSetting(Mandatory = false)]
        public string ReinstallArguments { get; set; }
        [ExecutableSetting(Mandatory = false)]
        public string UninstallArguments { get; set; }


        public bool Installed { get; set; }
        public bool ReInstalled { get; set; }
        public bool UnInstalled { get; set; }

        public async Task<string> InstallAsync(CancellationToken cancellationToken)
        {
            Log.Information("Installing application: {name}, file: {file}, dir {dir}", Name, InstallFilePath, ExecutableDirectory);

            using (var executor = new ApplicationExecutor(InstallFilePath, InstallArguments, ExecutableDirectory, cancellationToken))
            {
                (bool success, string errorMessage) executionStatement = await executor.ExecuteAsync();

                if (executionStatement.success)
                {
                    Installed = true;
                    StatusState = StatusState.Success;
                    return executionStatement.errorMessage;
                }
                else
                {
                    Installed = false;
                    StatusState = StatusState.Error;
                    return executionStatement.errorMessage;
                }
            }

        }


        public async Task<string> ReinstallAsync(CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(ReinstallFilePath))
                throw new ArgumentNullException(nameof(ReinstallFilePath));

            Log.Information("Reinstalling application: {name}, file: {file}, dir {dir}", ReinstallFilePath, ExecutableDirectory);


            using (var executor = new ApplicationExecutor(ReinstallFilePath, ReinstallArguments, ExecutableDirectory, cancellationToken))
            {
                (bool success, string errorMessage) executionStatement = await executor.ExecuteAsync();

                if (executionStatement.success)
                {
                    ReInstalled = true;
                    StatusState = StatusState.Success;
                    return executionStatement.errorMessage;
                }
                else
                {
                    ReInstalled = false;
                    StatusState = StatusState.Error;
                    return executionStatement.errorMessage;
                }
            }
        }


        public async Task<string> UninstallAsync(CancellationToken cancellationToken)
        {
            Log.Information("Uninstalling application: {name}, file: {file}, dir: {dir}", Name, UninstallFilePath, ExecutableDirectory);

            using (var executor = new ApplicationExecutor(UninstallFilePath, UninstallArguments, ExecutableDirectory, cancellationToken))
            {
                (bool success, string errorMessage) executionStatement = await executor.ExecuteAsync();

                if (executionStatement.success)
                {
                    UnInstalled = true;
                    StatusState = StatusState.Success;
                    return executionStatement.errorMessage;
                }
                else
                {
                    UnInstalled = false;
                    StatusState = StatusState.Error;
                    return executionStatement.errorMessage;
                }
            }
        }
    }
}
