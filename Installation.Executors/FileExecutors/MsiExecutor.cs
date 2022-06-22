using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Installation.Executors
{
    public class MsiExecutor : Executor, IApplicationExecutor
    {
        public MsiExecutor(string executableFile, string arguments, string baseFolder, CancellationToken token)
            : base(executableFile, arguments, baseFolder, token)
        {
        }


        public async Task InstallAsync()
        {
            SuccessfullReturnCodes.Clear();
            SuccessfullReturnCodes.Add(0);

            arguments = $"/i {executableFile} {arguments}";
            executableFile = "msiexec";
            await ExecuteAsync();
        }

        public async Task ReinstallAsync()
        {
            SuccessfullReturnCodes.Clear();
            SuccessfullReturnCodes.Add(0);

            arguments = $"/f {executableFile} {arguments}";
            executableFile = "msiexec";
            await ExecuteAsync();    
        }

        public async Task UninstallAsync()
        {
            SuccessfullReturnCodes.Clear();
            SuccessfullReturnCodes.Add(0);

            arguments = $"/x {executableFile} {arguments}";
            executableFile = "msiexec";
            await ExecuteAsync();
        }
    }
}
