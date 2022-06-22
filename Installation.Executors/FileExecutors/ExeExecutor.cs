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
        }


        public async Task InstallAsync()
        {
            SuccessfullReturnCodes.Clear();
            SuccessfullReturnCodes.Add(0);

            await ExecuteAsync();
        }

        public async Task ReinstallAsync()
        {
            SuccessfullReturnCodes.Clear();
            SuccessfullReturnCodes.Add(0);

            await ExecuteAsync();
        }

        public async Task RunAsync()
        {
            SuccessfullReturnCodes.Clear();
            SuccessfullReturnCodes.Add(0);

            await ExecuteAsync();
        }

        public async Task UninstallAsync()
        {
            SuccessfullReturnCodes.Clear();
            SuccessfullReturnCodes.Add(0);

            await ExecuteAsync();
        }
    }
}
