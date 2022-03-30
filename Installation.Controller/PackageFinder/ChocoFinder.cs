using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Installation.Models;
using Installation.Controller.Logger;

namespace Installation.Controller.PackageFinder
{
    public class ChocoFinder
    {
        public string Path;

        public string InstallFileName = "Install.ps1";
        public string UninstallFileName = "Uninstall.ps1";
        public ChocoFinder(string path)
        {
            this.Path = path;
        }
        public void FindPackages(List<IExecutable> executables)
        {
            string[] directories;
            try
            {
                directories = Directory.GetDirectories(Path);
            }
            catch (Exception ex)
            {
                string message = "Could not read directory. Error: " + ex.Message;
                CommonLogger.LogEvent(message, LogType.Error);
                return;
            }
            if (directories.Length == 0)
            {
                CommonLogger.LogEvent("No Software in directory", LogType.Warning);
            }

            getVersions(directories);

        }

        private (bool, ChocoApp) getVersions(string[] directories)
        {
            foreach (var directory in directories)
            {
                string[] versions;
                try
                {
                    versions = Directory.GetDirectories(directory);
                }
                catch(Exception ex)
                {
                    string message = "Could not read versions from directory " + ex.Message;
                    CommonLogger.LogEvent(message, LogType.Error);

                    return (false, null);
                }
                if(versions.Length == 0)
                {
                    CommonLogger.LogEvent($"No Versions exists in {directory}", LogType.Warning);
                    return (false, null);
                }

                ChocoApp app;
                foreach(var version in versions)
                {
                    if(checkForFiles())
                    {
                        if (app == null)
                        {
                            app = new ChocoApp
                            {
                                InstallFilePath = version + InstallFileName,
                                UninstallFilePath = version + UninstallFileName
                            };
                        }
                    }

                }
                

            }
        }
    }
}
