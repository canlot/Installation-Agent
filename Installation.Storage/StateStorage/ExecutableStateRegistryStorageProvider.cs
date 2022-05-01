using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Serilog;

namespace Installation.Storage.StateStorage
{
    class ExecutableStateRegistryStorageProvider : IExecutableStateStorageProvider
    {
        private string softwareName = "Installation Agent";
        private string softwarePath;
        private string rootPath = "SOFTWARE";
        public ExecutableStateRegistryStorageProvider()
        {
            this.softwarePath = "SOFTWARE" + @"\" + softwareName;
            createRegistryPathIfNotExists();
        }
        private void createRegistryPathIfNotExists()
        {
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(softwarePath, true))
                {
                    if(key != null)
                        return;
                }
            }
            catch (Exception ex)
            {
                Log.Information(ex, "Registry ky {key} doesn't exist, will be created", softwarePath);
            }
            try
            {
                using(var key = Registry.LocalMachine.OpenSubKey(rootPath, true))
                {
                    if(key.GetValue(softwareName) == null)
                    {
                        key.CreateSubKey(softwareName);
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        
        public bool IsExecutableExists(string executableID)
        {
            try
            {
                using(var key = Registry.LocalMachine.OpenSubKey(softwarePath, true))
                {
                    if(key.OpenSubKey(executableID) == null)
                    {
                        key.CreateSubKey(executableID);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }
        public bool GetStateValue(string keyName, string executableID)
        {
            if(!IsExecutableExists(executableID))
                return false;
            string registryPath = softwarePath + @"\" + executableID;
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(registryPath))
                {
                    var result = key.GetValue(keyName);
                    if(result == null)
                    {
                        Log.Debug("Could not get key {key} value for {executable} from registry path {path}", keyName, executableID, registryPath);
                        return false;

                    }
                    else
                    {
                        Log.Debug("Got key {name} with value {key}", keyName, result);
                        return Convert.ToBoolean(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Could not get state value from registry");
                throw ex;
            }
        }

        public void SaveStateValue(string keyName, string executableID, bool value) 
        {
            IsExecutableExists(executableID);
            string registryPath = softwarePath + @"\" + executableID;
            try
            {
                using(var key = Registry.LocalMachine.OpenSubKey(registryPath, true))
                {
                    Log.Debug("Write property {keyName} with value of {value} in registry path {path}", keyName, value, registryPath);
                    if(key != null)
                        key.SetValue(keyName, value, RegistryValueKind.DWord);
                    else
                    {
                        Log.Error("Key {registry} is null", registryPath);
                        throw new Exception();
                    }    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
