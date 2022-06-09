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

        public Task<(bool, string)> InstallAsync()
        {
            throw new NotImplementedException();
        }

        public Task<(bool, string)> ReinstallAsync()
        {
            throw new NotImplementedException();
        }

        public Task<(bool, string)> RunAsync()
        {
            throw new NotImplementedException();
        }

        public Task<(bool, string)> UninstallAsync()
        {
            throw new NotImplementedException();
        }
    }
}
