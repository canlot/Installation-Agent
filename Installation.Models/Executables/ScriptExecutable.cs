using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Installation.Executors;
using Serilog;

namespace Installation.Models
{
    [Executable("Script")]
    public class ScriptExecutable : Executable, IRunnable
    {
        [ExecutableSetting]
        public string RunFilePath { get; set; }
        private bool runned;
        public bool Runned { get => runned; set => runned = value; }

        public async Task<string> RunAsync(CancellationToken cancellationToken)
        {
            Log.Information("Running Script {file} from {dir}", RunFilePath, ExecutableDirectory);

            using (var executor = Executor.GetExecutor(RunFilePath, "",  ExecutableDirectory, cancellationToken))
            {
                checkExecutor(executor);

                (bool success, string errorMessage) executionStatement = await (executor as IScriptExecutor).RunAsync();


                if (executionStatement.success)
                {
                    Runned = true;
                    StatusState = StatusState.Success;
                    return executionStatement.errorMessage;
                }
                else
                {
                    Runned = false;
                    StatusState = StatusState.Error;
                    return executionStatement.errorMessage;
                }
            }
        }
        private void checkExecutor(Executor executor)
        {
            if (executor == null)
                throw new NullReferenceException("No executor for this file type found");
            if (!(executor is IScriptExecutor))
                throw new InvalidOperationException("This operation is not supported for this file type");
        }
    }
}
