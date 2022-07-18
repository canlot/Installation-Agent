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
using System.Threading;

namespace Installation.Controller.ExecutableFinders
{
    class ExecutableFinder
    {
        private GlobalSettings globalSettings;
        private Dictionary<Guid, ExecutableFileInfo> executableFileInformation = new Dictionary<Guid, ExecutableFileInfo>();
        private Dictionary<Guid, Executable> executables;
        private CancellationToken cancellationToken;

        public int RefreshCycleCounter = 0; //for every Run the counter will be incremented 

        List<string> executablePaths = new List<string>();

        private bool firstRun = true;

        public ExecutableFinder(GlobalSettings settings, Dictionary<Guid, Executable> executables, CancellationToken cancellationToken)
        {
            this.globalSettings = settings;
            this.executables = executables;
            this.cancellationToken = cancellationToken;

            executablePaths.Add(globalSettings.ExecutablesPath);
            executablePaths.AddRange(globalSettings.ExecutablesSettings.Select(n => n.ExecutablesPath));
            executablePaths = executablePaths.Distinct().Where(n => !string.IsNullOrEmpty(n)).ToList();
        }
        public async Task RunAsync()
        {
            while(true)
            {
                RefreshCycleCounter++;

                Log.Verbose("Searching for Executables");

                if (executablePaths.Count > 0)
                {
                    Log.Debug("Following executable paths found {paths}", executablePaths);
                    ExecutableStorageProvider executableStorage = new ExecutableStorageProvider(executablePaths, globalSettings.ApplicationSettingsFileName);

                    foreach (var executableBundle in executableStorage.GetExecutables())
                    {
                        if(executableFileInformation.ContainsKey(executableBundle.executable.Id))
                        {
                            var existingFileInformation = executableFileInformation[executableBundle.executable.Id];
                            var givenFileInformation = (FilePath: executableBundle.filePath, FileHash: executableBundle.fileHash);

                            handleFileInformation(executableBundle.executable, existingFileInformation, givenFileInformation);
                        }
                        else
                        {
                            executableFileInformation.Add(executableBundle.executable.Id, new ExecutableFileInfo
                            {
                                FilePath = executableBundle.filePath,
                                FileHash = executableBundle.fileHash,
                                CycleGeneration = RefreshCycleCounter
                            }) ;
                            addExecutable(executableBundle.executable);
                        }

                    }
                }
                else
                {
                    Log.Fatal("No executable paths found");
                    throw new Exception("No executable paths found");
                }

                await Task.Delay(globalSettings.PullIntervalTimeInSeconds, cancellationToken);
                firstRun = false;
            }
        }

        private void handleFileInformation(Executable givenExecutable, ExecutableFileInfo existingFileInformation, (string filePath, string fileHash) givenFileInformation)
        {
            if (existingFileInformation == null)
                return; // this case should not exist, but if it will than this information will be deleted at the end
            
            if(existingFileInformation.FilePath == givenFileInformation.filePath && existingFileInformation.FileHash == givenFileInformation.fileHash) // everyting is fine, only the refresh counter will be updated so this record will not be deletet at the end
            {
                existingFileInformation.CycleGeneration = RefreshCycleCounter;
            }
            else if (existingFileInformation.FilePath == givenFileInformation.filePath && existingFileInformation.FileHash != givenFileInformation.fileHash) // fileHash is different, so the configuration file has changed, it should be remapped 
            {
                mappingExecutable(executables[givenExecutable.Id], givenExecutable);
            }
            else if(existingFileInformation.FileHash == givenFileInformation.fileHash && existingFileInformation.FilePath != givenFileInformation.filePath) // filePath is different, so the files propably were moved
            {
                changeDirectory(executables[givenExecutable.Id], givenExecutable);
            }
            else // could be double, we cannot say for sure that it is unique
            {

            }


            //map new values, like Sha1 Hash of the application config file and the path anyway, so we don't need aditional if statements
                existingFileInformation.FilePath = givenFileInformation.filePath;
            existingFileInformation.FileHash = givenFileInformation.fileHash;
        }

        private void changeDirectory(Executable existingExecutable, Executable newExecutable)
        {
            if (existingExecutable.Id == newExecutable.Id)
            {
                if (!existingExecutable.CurrentlyRunning)
                {
                    existingExecutable.ExecutableDirectory = newExecutable.ExecutableDirectory;
                }
            }
        }

        private void mappingExecutable(Executable existingExecutable, Executable newExecutable)
        {
            if(existingExecutable.Id == newExecutable.Id)
            {
                if(!existingExecutable.CurrentlyRunning)
                {
                    existingExecutable = newExecutable;
                }
            }
        }

        private void addExecutable(Executable executable)
        {
            if (executables.ContainsKey(executable.Id))
            {
                Log.Error("Executable {name} with the id {id} already exist, executable not added", executable.Name, executable.Id);
            }
            else
            {
                ExecutionStateSettings executionStateSettings = new ExecutionStateSettings();
                executionStateSettings.LoadExecutableState(executable);
                executables.Add(executable.Id, executable);
                Log.Debug("Executable added {@executable}", executable);
            }
        }

        public void FindExecutables(Dictionary<Guid, Executable> executables)
        {
            Log.Verbose("Searching for Executables");

            if(executablePaths.Count > 0)
            {
                Log.Debug("Following executable paths found {paths}", executablePaths);
                ExecutableStorageProvider executableStorage = new ExecutableStorageProvider(executablePaths, globalSettings.ApplicationSettingsFileName);

                foreach (var executableBundle in executableStorage.GetExecutables())
                {
                    if(executables.ContainsKey(executableBundle.executable.Id))
                    {
                        Log.Error("Executable {name} with the id {id} already exist, executable not added", executableBundle.executable.Name, executableBundle.executable.Id);
                    }
                    else
                    {
                        ExecutionStateSettings executionStateSettings = new ExecutionStateSettings();
                        executionStateSettings.LoadExecutableState(executableBundle.executable);
                        executables.Add(executableBundle.executable.Id, executableBundle.executable);
                        Log.Debug("Executable added {@executable}", executableBundle.executable);
                    }
                }
            }
            else
            {
                Log.Fatal("No executable paths found");
                throw new Exception("No executable paths found");
            }
        }
        

    }
}
