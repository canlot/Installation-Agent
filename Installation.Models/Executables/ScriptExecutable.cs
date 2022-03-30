using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Installation.Models.Interfaces;
using Serilog;

namespace Installation.Models.Executables
{
    public class ScriptExecutable : Executable, IRunnable
    {
        public string RunFilePath { get; set; }
        private bool runned;
        public bool Runned { get => runned; set => runned = value; }

        public (StatusState, string) Run()
        {
            throw new NotImplementedException();
        }
        public async Task<(StatusState, string)> RunAsync(CancellationToken cancellationToken)
        {
            Log.Debug("Running Script {file}", RunFilePath);
            var processinfo = new ProcessStartInfo()
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy bypass -File {RunFilePath}",
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
                return (StatusState.Success, "Success");
            else
                return (StatusState.Error, process.StandardError.ReadToEnd());
        }
    }
}
