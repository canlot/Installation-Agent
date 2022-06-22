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


        public bool Installed { get; set; }
        public bool ReInstalled { get; set; }
        public bool UnInstalled { get; set; }

        [ExecutableSetting(Mandatory = false)]
        public List<int> SuccessfullInstallReturnCodes { get; set; }
        [ExecutableSetting(Mandatory = false)]
        public List<int> SuccessfullReInstallReturnCodes { get; set; }
        [ExecutableSetting(Mandatory = false)]
        public List<int> SuccessfullUnInstallReturnCodes { get; set; }

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

            using (var executor = Executor.GetExecutor(installFilePath, InstallArguments, ExecutableDirectory, cancellationToken))
            {
                checkExecutor(executor);
                await (executor as IApplicationExecutor).InstallAsync();

                setExecutionStateFromExecutor(executor, SuccessfullInstallReturnCodes);
            }

        }
        

        public async Task ReinstallAsync(CancellationToken cancellationToken)
        {
            if(string.IsNullOrWhiteSpace(ReinstallFilePath))
                throw new ArgumentNullException(nameof(ReinstallFilePath));

            Log.Information("Reinstalling application: {name}, file: {file}, dir {dir}", ReinstallFilePath, ExecutableDirectory);


            using (var executor = Executor.GetExecutor(ReinstallFilePath, ReinstallArguments, ExecutableDirectory, cancellationToken))
            {
                checkExecutor(executor);
                await (executor as IApplicationExecutor).ReinstallAsync();

                setExecutionStateFromExecutor(executor, SuccessfullReInstallReturnCodes);
            }
        }


        public async Task UninstallAsync(CancellationToken cancellationToken)
        {
            Log.Information("Uninstalling application: {name}, file: {file}, dir: {dir}", Name, UninstallFilePath, ExecutableDirectory);

            using (var executor = Executor.GetExecutor(UninstallFilePath, UninstallArguments, ExecutableDirectory, cancellationToken))
            {
                checkExecutor(executor);
                await (executor as IApplicationExecutor).UninstallAsync();

                setExecutionStateFromExecutor(executor, SuccessfullUnInstallReturnCodes);
            }
        }
    }
}
