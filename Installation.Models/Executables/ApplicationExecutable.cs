using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Installation.Models
{
    [Executable("App")]
    public class ApplicationExecutable : Executable, IInstalable, IReinstallable, IUninstallable
    {
        public bool Installed { get; set; }
        [ExecutableSetting(ConfigName = "InstallFilePath")]
        public string InstallFilePath { get; set; }
        public bool ReInstalled { get ; set; }
        [ExecutableSetting(ConfigName = "ReinstallFilePath", Mandatory = false)]
        public string ReinstallFilePath { get ; set ; }
        public bool UnInstalled { get ; set ; }
        [ExecutableSetting(ConfigName = "UninstallFilePath")]
        public string UninstallFilePath { get ; set; }


        public Task<string> InstallAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


        public Task<string> ReinstallAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


        public Task<string> UninstallAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
