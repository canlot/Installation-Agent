using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace Installation.Executors
{
    public class ScriptExecutor
    {
        public async Task<(bool success, string message)> ExecuteAsync(string scriptFile, string baseFolder, CancellationToken cancellationToken)
        {
            

            var processinfo = new ProcessStartInfo()
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy bypass -File {scriptFile}",
                WorkingDirectory = baseFolder,
                UseShellExecute = false
            };
            processinfo.RedirectStandardOutput = true;
            processinfo.RedirectStandardError = true;

            Process process = new Process();
            process.StartInfo = processinfo;

            await Task.Run(() =>
            {
                process.Start();
                cancellationToken.Register(() => process.Kill());
                process.WaitForExit();
            });

            Log.Debug("Return code: {code}", process.ExitCode);

            if (process.ExitCode == 0)
                return (true, "Success");
            else
                return (false, process.StandardError.ReadToEnd());
        }
    }

}
