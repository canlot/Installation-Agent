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

        public async Task<(bool, string)> InstallAsync()
        {
            return await ExecuteAsync();
        }

        public async Task<(bool, string)> ReinstallAsync()
        {
            return await ExecuteAsync();
        }

        public async Task<(bool, string)> RunAsync()
        {
            return await ExecuteAsync();
        }

        public async Task<(bool, string)> UninstallAsync()
        {
            return await ExecuteAsync();
        }
    }
}
