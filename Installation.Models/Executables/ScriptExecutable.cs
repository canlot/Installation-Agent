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
    public class ScriptExecutable : Executable, IRunnable
    {
        public string RunFilePath { get; set; }
        private bool runned;
        public bool Runned { get => runned; set { runned = value; setSuccessfulRolloutState(); OnPropertyChanged("Runned"); } }


        public List<ExecutableUnit> RunnableUnits { get; set; }

        protected override void setSuccessfulRolloutState()
        {
            if (runned)
                successfulRollout = true;
            else
                successfulRollout = false;

        }

                
    }
}
