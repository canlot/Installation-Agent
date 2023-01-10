using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Installation.Executors
{
    public class ExeExecutor : Executor, IApplicationExecutor, IScriptExecutor
    {
        public ExeExecutor(string executableFile, string arguments, string baseFolder, CancellationToken token) 
            : base(executableFile, arguments, baseFolder, token)
        {
            SuccessfullInstallationReturnCodes = new List<int>();
            SuccessfullInstallationReturnCodes.Add(0);

            SuccessfullReinstallationReturnCodes = new List<int>();
            SuccessfullReinstallationReturnCodes.Add(0);

            SuccessfullUninstallationReturnCodes = new List<int>();
            SuccessfullUninstallationReturnCodes.Add(0);

            SuccessfullRunReturnCodes = new List<int>();
            SuccessfullRunReturnCodes.Add(0);
        }

        public List<int> SuccessfullInstallationReturnCodes { get; }

        public List<int> SuccessfullReinstallationReturnCodes { get; }

        public List<int> SuccessfullUninstallationReturnCodes {get; }

        public List<int> SuccessfullRunReturnCodes {get; }

        public async Task InstallAsync()
        {
            await ExecuteAsync();
        }

        public async Task ReinstallAsync()
        {
            await ExecuteAsync();
        }

        public async Task RunAsync()
        {
            await ExecuteAsync();
        }

        public async Task UninstallAsync()
        {
            await ExecuteAsync();
        }
    }
}
