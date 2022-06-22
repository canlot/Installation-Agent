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

        public override List<int> SuccessfullReturnCodes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public async Task RunAsync()
        {
            SuccessfullReturnCodes.Clear();
            SuccessfullReturnCodes.Add(0);

            arguments = $"-NoProfile -ExecutionPolicy bypass -File \"{executableFile}\"";
            executableFile = "powershell.exe";
            await ExecuteAsync();
        }
    }
}
