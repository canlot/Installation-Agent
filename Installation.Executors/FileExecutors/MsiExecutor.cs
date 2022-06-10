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
        public async Task<(bool, string)> InstallAsync()
        {
            arguments = $"/i {executableFile} {arguments}";
            executableFile = "msiexec";
            return await ExecuteAsync();
        }

        public async Task<(bool, string)> ReinstallAsync()
        {
            arguments = $"/f {executableFile} {arguments}";
            executableFile = "msiexec";
            return await ExecuteAsync();    
        }

        public async Task<(bool, string)> UninstallAsync()
        {
            arguments = $"/x {executableFile} {arguments}";
            executableFile = "msiexec";
            return await ExecuteAsync();
        }
    }
}
