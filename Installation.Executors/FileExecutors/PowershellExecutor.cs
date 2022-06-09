using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Installation.Executors
{
    public class PowershellExecutor : Executor, IScriptExecutor
    {
        public PowershellExecutor(string executableFile, string arguments, string baseFolder, CancellationToken token)
            : base(executableFile, arguments, baseFolder, token)
        {

        }

        public Task<(bool, string)> RunAsync()
        {
            throw new NotImplementedException();
        }
    }
}
