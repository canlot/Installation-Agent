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
        public bool Runned { get => runned; set { runned = value; setSuccessfulRolloutState(); } }



        private List<ExecutableUnit> runnableUnits = new List<ExecutableUnit>();

        public IEnumerable<ExecutableUnit> RunnableUnits
        {
            get
            {
                int index = 1;
                while (index <= runnableUnits.Count)
                {
                    foreach (var unit in runnableUnits)
                    {
                        if (index == unit.Index)
                        {
                            index++;
                            yield return unit;
                        }
                    }
                }

            }
        }

        protected void setSuccessfulRolloutState()
        {
            if (runned)
                successfulRollout = true;
            else
                successfulRollout = false;

        }

                
    }
}
