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

        public async Task<(bool, string)> RunAsync()
        {
            arguments = $"-NoProfile -ExecutionPolicy bypass -File \"{executableFile}\"";
            executableFile = "powershell.exe";
            return await ExecuteAsync();
        }
    }
}
