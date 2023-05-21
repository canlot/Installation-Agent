using Installation.Executors;
using Installation.Models.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Installation.Models
{
    public class ApplicationExecutable : Executable, IInstallable, IReinstallable, IUninstallable
    {
        private List<ExecutableUnit> installableUnits = new List<ExecutableUnit>();
        public IEnumerable<ExecutableUnit> InstallableUnits
        {
            get
            {
                int index = 1;
                while(index <= installableUnits.Count)
                {
                    foreach (var unit in installableUnits)
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
        private List<ExecutableUnit> uninstallableUnits = new List<ExecutableUnit>();
        public IEnumerable<ExecutableUnit> UninstallableUnits
        {
            get
            {
                int index = 1;
                while(index <= uninstallableUnits.Count)
                {
                    foreach (var unit in uninstallableUnits)
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
        private List<ExecutableUnit> reinstallableUnits = new List<ExecutableUnit>();
        public IEnumerable<ExecutableUnit> ReinstallableUnits
        {
            get
            { 
                int index = 1;
                while(index <= reinstallableUnits.Count)
                {
                    foreach (var unit in reinstallableUnits)
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



        public ApplicationExecutable()
        {
        }
        

        private bool installed;
        private bool reinstalled;
        private bool uninstalled;
        
        public bool Installed { get => installed; set { installed = value; setSuccessfulRolloutState(); } }
        public bool ReInstalled { get => reinstalled; set { reinstalled = value; setSuccessfulRolloutState();  } }
        public bool UnInstalled { get => uninstalled; set { uninstalled = value; setSuccessfulRolloutState();  } }


        

        //----------------------------------

        protected void setSuccessfulRolloutState()
        {
            if ((Installed || ReInstalled) && !UnInstalled)
                successfulRollout = true;
            else
                successfulRollout = false;
        }

    }
}
