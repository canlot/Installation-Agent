using Installation.Executors;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Installation.Models
{
    [Executable("App")]
    public class ApplicationExecutable : Executable, IInstallable, IReinstallable, IUninstallable
    {

        public List<ExecutableUnit> InstallableUnits { get; set; }
        public List<ExecutableUnit> UninstallableUnits { get; set; }
        public List<ExecutableUnit> ReinstallableUnits { get; set; }
        

        private bool installed;
        private bool reinstalled;
        private bool uninstalled;
        
        public bool Installed { get => installed; set { installed = value; setSuccessfulRolloutState(); OnPropertyChanged("Installed");  } }
        public bool ReInstalled { get => reinstalled; set { reinstalled = value; setSuccessfulRolloutState(); OnPropertyChanged("ReInstalled"); } }
        public bool UnInstalled { get => uninstalled; set { uninstalled = value; setSuccessfulRolloutState(); OnPropertyChanged("UnInstalled"); } }

        

        protected override void setSuccessfulRolloutState()
        {
            if ((Installed || ReInstalled) && !UnInstalled)
                successfulRollout = true;
            else
                successfulRollout = false;
        }

    }
}
