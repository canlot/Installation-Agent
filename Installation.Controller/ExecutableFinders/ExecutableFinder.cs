using Installation.Controller.Settings;
using Installation.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Installation.Storage.StateStorage;
using Serilog;
using System.Threading;

namespace Installation.Controller.ExecutableFinders
{
    class ExecutableFinder
    {
        public delegate Task ExecutableAddedOrModified(ExecutableBase executable);

        public event ExecutableAddedOrModified OnExecutableAddedOrModified;

        private SettingsContainer settingsContainer;
        private Dictionary<Guid, ExecutableFileInfo> executableFileInformation = new Dictionary<Guid, ExecutableFileInfo>();
        private Dictionary<Guid, ExecutableBase> executables;
        private CancellationToken cancellationToken;

        public int RefreshCycleCounter = 0; //for every Run the counter will be incremented 


        //private bool firstRun = true;

        public ExecutableFinder(SettingsContainer settings, Dictionary<Guid, ExecutableBase> executables, CancellationToken cancellationToken)
        {
            this.settingsContainer = settings;
            this.executables = executables;
            this.cancellationToken = cancellationToken;

        }
        public async Task RunAsync()
        {
            while(true)
            {
                RefreshCycleCounter++;

                Log.Verbose("Searching for Executables");

                Log.Debug("Following executable paths found {paths}", settingsContainer.GlobalSettings.ExecutablesPath);
                ExecutableStorageProvider executableStorage = new ExecutableStorageProvider(settingsContainer.GlobalSettings.ExecutablesPath, 
                    settingsContainer.GlobalSettings.ApplicationSettingsFileName);

                foreach (var executableBundle in executableStorage.GetExecutables())
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    if(executableFileInformation.ContainsKey(executableBundle.executable.Id))
                    {
                        var existingFileInformation = executableFileInformation[executableBundle.executable.Id];
                        var givenFileInformation = (FilePath: executableBundle.filePath, FileHash: executableBundle.fileHash);

                        await handleFileInformation(executableBundle.executable, existingFileInformation, givenFileInformation);
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
                        await this?.OnExecutableAddedOrModified(executableBundle.executable);
                    }

                }


                await Task.Delay(settingsContainer.GlobalSettings.PullIntervalTimeInSeconds * 1000, cancellationToken);
                //firstRun = false;
            }
        }

        private async Task handleFileInformation(ExecutableBase givenExecutable, ExecutableFileInfo existingFileInformation, (string filePath, string fileHash) givenFileInformation)
        {
            ExecutableBase currentExecutable = executables[givenExecutable.Id];
            if (existingFileInformation == null)
                return; // this case should not exist, but if it will than this information will be deleted at the end
            
            if(existingFileInformation.FilePath == givenFileInformation.filePath && existingFileInformation.FileHash == givenFileInformation.fileHash) // everyting is fine, only the refresh counter will be updated so this record will not be deletet at the end
            {
                existingFileInformation.CycleGeneration = RefreshCycleCounter;
            }
            else if (existingFileInformation.FilePath == givenFileInformation.filePath && existingFileInformation.FileHash != givenFileInformation.fileHash) // fileHash is different, so the configuration file has changed, it should be remapped 
            {
                mappingExecutable(currentExecutable, givenExecutable);
                await this?.OnExecutableAddedOrModified(currentExecutable);
            }
            else if(existingFileInformation.FileHash == givenFileInformation.fileHash && existingFileInformation.FilePath != givenFileInformation.filePath) // filePath is different, so the files propably were moved
            {
                changeDirectory(currentExecutable, givenExecutable);
                await this?.OnExecutableAddedOrModified(currentExecutable);
            }
            else // could be double, we cannot say for sure that it is unique, so it had to be deleted
            {

            }


            //map new values, like Sha1 Hash of the application config file and the path anyway, so we don't need aditional if statements
            existingFileInformation.FilePath = givenFileInformation.filePath;
            existingFileInformation.FileHash = givenFileInformation.fileHash;
        }

        private void changeDirectory(ExecutableBase existingExecutable, ExecutableBase newExecutable)
        {
            if (existingExecutable.Id == newExecutable.Id)
            {
                if (!existingExecutable.CurrentlyExecuting)
                {
                    existingExecutable.ExecutableDirectory = newExecutable.ExecutableDirectory;
                }
            }
        }

        private void mappingExecutable(ExecutableBase existingExecutable, ExecutableBase newExecutable)
        {
            if(existingExecutable.Id == newExecutable.Id)
            {
                if(!existingExecutable.CurrentlyExecuting)
                {
                    existingExecutable = newExecutable;
                }
            }
        }

        private void addExecutable(ExecutableBase executable)
        {
            if (executables.ContainsKey(executable.Id))
            {
                Log.Error("ExecutableBase {name} with the id {id} already exist, executable not added", executable.Name, executable.Id);
            }
            else
            {
                ExecutionStateSettings executionStateSettings = new ExecutionStateSettings();
                executionStateSettings.LoadExecutableState(executable);
                executables.Add(executable.Id, executable);
                Log.Debug("ExecutableBase added {@executable}", executable);
            }
        }

        public void FindExecutables(Dictionary<Guid, ExecutableBase> executables)
        {
            Log.Verbose("Searching for Executables");

            Log.Debug("Search in following executable path {path}", settingsContainer.GlobalSettings.ExecutablesPath);
            ExecutableStorageProvider executableStorage = new ExecutableStorageProvider(settingsContainer.GlobalSettings.ExecutablesPath,
                settingsContainer.GlobalSettings.ApplicationSettingsFileName);

            foreach (var executableBundle in executableStorage.GetExecutables())
            {
                if (executables.ContainsKey(executableBundle.executable.Id))
                {
                    Log.Error("ExecutableBase {name} with the id {id} already exist, executable not added", executableBundle.executable.Name, executableBundle.executable.Id);
                }
                else
                {
                    ExecutionStateSettings executionStateSettings = new ExecutionStateSettings();
                    executionStateSettings.LoadExecutableState(executableBundle.executable);
                    executables.Add(executableBundle.executable.Id, executableBundle.executable);
                    Log.Debug("ExecutableBase added {@executable}", executableBundle.executable);
                }
            }
        }
        

    }
}
