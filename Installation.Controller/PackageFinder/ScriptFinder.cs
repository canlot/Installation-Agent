using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Controller.PackageFinder
{
    public class ScriptFinder
    {
        private string ExecutablesPath;
        private string InstalledRegistryPath;
        private string InstallFileName;
        private string UninstallFileName;

        public ScriptFinder(string executablesPath, string installedRegistryPath, string installFileName, string uninstallFileName)
        {
            this.ExecutablesPath = executablesPath;
            this.InstalledRegistryPath = installedRegistryPath;
            this.InstallFileName = installFileName;
            this.UninstallFileName = uninstallFileName;
        }
        
        public void FindExecutables()
        {

        }
    }
}
