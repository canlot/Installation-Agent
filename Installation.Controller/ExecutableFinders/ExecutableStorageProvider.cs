using Installation.Models;
using Installation.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using Installation.Parser.Exceptions;
using Installation.Storage;
using Serilog;

namespace Installation.Controller.ExecutableFinders
{
    public class ExecutableStorageProvider
    {
        private List<string> executablePaths;
        private readonly string executableSettingsFileName;

        public ExecutableStorageProvider(List<string> executablePaths, string applicationSettingsFileName)
        {
            this.executablePaths = executablePaths;
            this.executableSettingsFileName = applicationSettingsFileName;
        }
        public IEnumerable<Executable> GetExecutables()
        {
            Executable executable = null;
            foreach (var path in executablePaths)
            {
                foreach (var directory in getDirectoriesInExecutablePath(path))
                {
                    try
                    {
                        if(searchForSettingFile(directory))
                        {
                            ExecutableParser executableParser = new ExecutableParser(directory + @"\" + executableSettingsFileName);
                            executable = executableParser.ParseObject();
                        }
                        else
                        {
                            Log.Debug("Could not find any settings file in folder {folder}", directory);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Debug(ex, "Exception occured in folder {folder}", directory);
                    }
                    if (executable != null)
                    {
                        executable.ExecutableDirectory = directory;
                        yield return executable;
                    }    
                    else
                        Log.Debug("settings could not be parsed from {dir}", directory);
                }
                
            }
                
        }

        public string[] getDirectoriesInExecutablePath(string executablePath)
        {
            string[] directories = new string[0];
            try
            {
                directories = Directory.GetDirectories(executablePath);
                Log.Debug("{dir} folders found", directories.Length);
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "Exception occured by searching in {folder}", executablePath);
            }
            return directories;
        }

        private bool searchForSettingFile(string directory)
        {
            string[] files = Directory.GetFiles(directory, executableSettingsFileName);
            if (files.Length > 1)
                Log.Information("More then one {executableSettingsFileName} found in {directory}", executableSettingsFileName, directory);
            if (files.Length == 1)
            {
                return true;
            }
            return false;
        }
    }
}
