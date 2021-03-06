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
        public bool Runned { get => runned; set { runned = value; setSuccessfulRolloutState(); OnPropertyChanged("Runned"); } }


        [ExecutableSetting(Mandatory = false)]
        public List<int> SuccessfullRunReturnCodes { get; set; }

        protected override void setSuccessfulRolloutState()
        {
            if (runned)
                successfulRollout = true;
            else
                successfulRollout = false;

        }

        
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            Log.Information("Running Script {file} from {dir}", RunFilePath, ExecutableDirectory);

            using (var executor = Executor.GetExecutor(RunFilePath, "",  ExecutableDirectory, cancellationToken))
            {
                checkExecutor(executor);
                await (executor as IScriptExecutor).RunAsync();
                setExecutionStateFromExecutor(executor, SuccessfullRunReturnCodes);
            }

            switch(StatusState)
            {
                case StatusState.Success:
                    Runned = true;
                    break;
                case StatusState.Warning:
                    Runned = true;
                    break;
                case StatusState.Error:
                    Runned = false;
                    break;
                default:
                    Runned = false;
                    break;
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
