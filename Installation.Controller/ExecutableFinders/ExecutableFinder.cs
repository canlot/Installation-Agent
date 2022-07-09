using Installation.Controller.Settings;
using Installation.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Installation.Parser;
using Installation.Storage.StateStorage;
using Serilog;

namespace Installation.Controller.ExecutableFinders
{
    class ExecutableFinder
    {
        private GlobalSettings globalSettings;
        private Dictionary<Guid, Executable> executables = new Dictionary<Guid, Executable>();

        public ExecutableFinder(GlobalSettings settings)
        {
            this.globalSettings = settings;
        }
        public Dictionary<Guid, Executable> FindExecutables()
        {
            Log.Verbose("Searching for Executables");

            List<string> executablePaths = new List<string>();
            executablePaths.Add(globalSettings.ExecutablesPath);
            executablePaths.AddRange(globalSettings.ExecutablesSettings.Select(n => n.ExecutablesPath));
            executablePaths = executablePaths.Distinct().Where(n => !string.IsNullOrEmpty(n)).ToList();
            if(executablePaths.Count > 0)
            {
                Log.Debug("Following executable paths found {paths}", executablePaths);
                ExecutableStorageProvider executableStorage = new ExecutableStorageProvider(executablePaths, globalSettings.ApplicationSettingsFileName);

                foreach (var executable in executableStorage.GetExecutables())
                {
                    ExecutionStateSettings executionStateSettings = new ExecutionStateSettings();
                    executionStateSettings.LoadExecutableState(executable);
                    if(executables.ContainsKey(executable.Id))
                    {
                        Log.Error("Executable {name} with the id {id} alreade exist, executable not added",executable.Name, executable.Id);
                    }
                    else
                    {
                        executables.Add(executable.Id, executable);
                        Log.Debug("Executable added {@executable}", executable);
                    }
                }
            }
            else
            {
                Log.Fatal("No executable paths found");
                throw new Exception("No executable paths found");
            }
            return executables;
        }
        

    }
}
