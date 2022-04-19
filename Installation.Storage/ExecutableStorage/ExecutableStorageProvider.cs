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
using Serilog;

namespace Installation.Storage.ExecutableStorage
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
        public IEnumerable<Executable> GetExecutables(ExecutableFileParser parser)
        {
            Executable executable = null;
            foreach (var path in executablePaths)
            {
                string[] directories = new string[0];
                try
                {
                    directories = Directory.GetDirectories(path);
                    Log.Debug("{dir} folders found", directories.Length);
                }
                catch (Exception ex)
                {
                    Log.Debug(ex, "Exception occured by searching in {folder}", path);
                }

                foreach (var directory in directories)
                    {   
                        
                        try
                        {
                            if(searchForSettingFile(directory))
                            {
                                executable = parser.ParseExecutableSettingsFile(directory + @"\" + executableSettingsFileName);
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
