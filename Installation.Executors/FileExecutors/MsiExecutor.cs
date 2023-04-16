using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Installation.Executors
{
    public class MsiExecutor : Executor, IInstallableExecutor, IReinstallableExecutor, IUninstallableExecutor
    {
        public MsiExecutor(string executableFile, string arguments, string baseFolder, CancellationToken token)
            : base(executableFile, arguments, baseFolder, token)
        {
            SuccessfullInstallationReturnCodes = new List<int>();
            SuccessfullInstallationReturnCodes.Add(0);

            SuccessfullReinstallationReturnCodes = new List<int>();
            SuccessfullReinstallationReturnCodes.Add(0);

            SuccessfullUninstallationReturnCodes = new List<int>();
            SuccessfullUninstallationReturnCodes.Add(0);
        }

        public List<int> SuccessfullInstallationReturnCodes { get; }

        public List<int> SuccessfullReinstallationReturnCodes { get; }

        public List<int> SuccessfullUninstallationReturnCodes { get; }
        public async Task InstallAsync()
        {
            arguments = $"/i {executableFile} {arguments}";
            executableFile = "msiexec";
            await ExecuteAsync();
        }

        public async Task ReinstallAsync()
        {
            arguments = $"/f {executableFile} {arguments}";
            executableFile = "msiexec";
            await ExecuteAsync();    
        }

        public async Task UninstallAsync()
        {
            arguments = $"/x {executableFile} {arguments}";
            executableFile = "msiexec";
            await ExecuteAsync();
        }
    }
}
