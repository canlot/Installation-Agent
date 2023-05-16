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
    public class ApplicationExecutable : IExecutable, IInstallable, IReinstallable, IUninstallable
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

        private Version version;
        public Version Version { get => version; set => version = value; }

        private Dictionary<string, string> versionDescriptions;
        public Dictionary<string, string> VersionDescriptions { get => versionDescriptions; set => versionDescriptions = value; }

        private string currentDirectory;
        public string CurrentDirectory { get => currentDirectory; set => currentDirectory = value; }

        // state variables -------------

        private bool currentlyExecuting;
        public bool CurrentlyExecuting { get => currentlyExecuting; set => currentlyExecuting = value; }

        private StatusState statusState;
        public StatusState StatusState { get => statusState; set => statusState = value; }

        public string statusMessage;
        public string StatusMessage { get => statusMessage; set => statusMessage = value; }

        protected bool successfulRollout;
        public bool SuccessfulRollout { get => successfulRollout; set => successfulRollout = value; }

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
