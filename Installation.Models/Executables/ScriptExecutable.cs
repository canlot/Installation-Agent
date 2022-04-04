using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Installation.Models.Interfaces;
using Installation.Executors;
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
            Log.Debug("Running Script {file} from {dir}", RunFilePath, ExecutableDirectory);

            var executor = new ScriptExecutor();

            (bool success, string errorMessage) executionStatement = await executor.ExecuteAsync(RunFilePath, ExecutableDirectory, cancellationToken);
            
            if(executionStatement.success)
            {
                Runned = true;
                return (StatusState.Success, executionStatement.errorMessage);
            }
            else
            {
                Runned = false;
                return (StatusState.Error, executionStatement.errorMessage);
            }
        }
    }
}
