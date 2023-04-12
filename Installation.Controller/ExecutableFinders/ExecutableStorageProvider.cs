using Installation.Models;
using Installation.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Installation.Controller.ExecutableFinders
{
    public class ExecutableStorageProvider
    {
        private string executablePath;
        private readonly string executableSettingsFileName;

        public ExecutableStorageProvider(string executablePath, string applicationSettingsFileName)
        {
            this.executablePath = executablePath;
            this.executableSettingsFileName = applicationSettingsFileName;
        }
        public IEnumerable<(Executable executable, string filePath, string fileHash)> GetExecutables()
        {
            foreach (var directory in getDirectoriesInExecutablePath(executablePath))
            {
                string fileHash = null;
                string filePath = directory + @"\" + executableSettingsFileName;
                Executable executable = executable = null; //sets the executable to null because last executable could get different directory
                try
                {
                    if(searchForSettingFile(directory))
                    {
                        ExecutableParser executableParser = new ExecutableParser(filePath);
                        executable = executableParser.ParseObject();

                        using(var sha1 = SHA1Managed.Create())
                        {
                            using (var reader = File.OpenRead(filePath))
                            {
                                fileHash = sha1.ComputeHash(reader).ToString();
                                Log.Debug("Hash for file {file} computed: {hash} ", filePath, fileHash);
                            }
                        }
                            
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
                if (executable != null && filePath != null && fileHash != null)
                {
                    executable.ExecutableDirectory = directory;
                    yield return (executable, filePath, fileHash);
                }    
                else
                    Log.Debug("settings could not be parsed from {dir}", directory);
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
