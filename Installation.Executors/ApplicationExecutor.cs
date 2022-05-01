using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Installation.Executors
{
    public class ApplicationExecutor : IDisposable
    {
        private string executableFile;
        private string arguments;
        private string baseFolder;
        private CancellationToken cancellationToken;
        private Process process;
        public ApplicationExecutor(string executableFile, string arguments, string baseFolder, CancellationToken cancellationToken)
        {
            this.baseFolder = baseFolder;
            this.executableFile = executableFile;
            this.arguments = arguments;
            this.cancellationToken = cancellationToken;
            createProcess();
        }
        public async Task<(bool success, string message)> ExecuteAsync()
        {
            
            await Task.Run(() =>
            {
                Log.Verbose("Executable file: {file}", process.StartInfo.FileName);
                Directory.SetCurrentDirectory(baseFolder);
                process.Start();
                cancellationToken.Register(
                    () =>
                    {
                        try
                        {
                            process.Kill();
                        }
                        catch (Exception ex)
                        {
                            Log.Debug(ex, "Execution aborted from file {file} (error expected)", executableFile);
                            process.Dispose();
                        }
                    }
                    );
                process.WaitForExit();
            });
            
            Log.Debug("Return code: {code}", process.ExitCode);

            if (process.ExitCode == 0)
                return (true, "Success");
            else
                return (false, process.StandardError.ReadToEnd());
        }
        private void createProcess()
        {
            var processInfo = new ProcessStartInfo();
            checkArguments(processInfo);
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            Process process = new Process();
            process.StartInfo = processInfo;

            this.process = process;
        }
        private void checkArguments(ProcessStartInfo processInfo)
        {
            if(!string.IsNullOrEmpty(arguments))
                processInfo.Arguments = arguments;
            if(!string.IsNullOrEmpty(baseFolder))
                processInfo.WorkingDirectory = baseFolder;
            processInfo.FileName = Environment.ExpandEnvironmentVariables(executableFile);
            processInfo.UseShellExecute = false;
        }

        public void Dispose()
        {
            process?.Dispose();
        }
    }
}
