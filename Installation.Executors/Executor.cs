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
    public abstract class Executor : IDisposable
    {
        protected string executableFile;
        protected string arguments;
        protected string baseFolder;
        protected CancellationToken cancellationToken;
        private Process process;

        public int LastReturnCode { get; private set; }
        public string LastReturnMessage { get; private set; }

        private Executor()
        {

        }
        protected Executor(string executableFile, string arguments, string baseFolder, CancellationToken cancellationToken) : this()
        {
            this.executableFile = executableFile;
            this.arguments = arguments;
            this.baseFolder = baseFolder;
            this.cancellationToken = cancellationToken;
        }

        public static Executor GetExecutor(string executableFile, string arguments, string baseFolder, CancellationToken token)
        {
            var extension = Path.GetExtension(executableFile);
            extension = extension.Replace(".", "");
            switch(extension)
            {
                case "exe":
                    return new ExeExecutor(executableFile, arguments, baseFolder, token);
                case "msi":
                    return new MsiExecutor(executableFile, arguments, baseFolder, token);
                case "ps1":
                    return new PowershellExecutor(executableFile, arguments, baseFolder, token);
                default:
                    return null;

            }
        }
        
        public async Task ExecuteAsync()
        {
            Log.Debug("Execute following executable: {executable} with arguments: {arguments} in folder: {folder}", executableFile, arguments, baseFolder);

            createProcess();

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

            LastReturnCode = process.ExitCode;
            LastReturnMessage = process.StandardError.ReadToEnd();
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
            if (!string.IsNullOrEmpty(arguments))
                processInfo.Arguments = arguments;
            if (!string.IsNullOrEmpty(baseFolder))
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
