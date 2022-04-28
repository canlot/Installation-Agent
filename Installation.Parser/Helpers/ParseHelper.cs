using IniParser.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Installation.Parser.Exceptions;
using IniParser;

namespace Installation.Parser.Helpers
{
    class ParseHelper
    {
        private IniData settings;
        private string machineContext = "Machine";
        private string userContext = "User";

        private string RunFilePathName = "RunFilePath";
        private string InstallFilePathName = "InstallFilePath";
        private string UninstallFilePathName = "UninstallFilePath";
        private string settingsFilePath;
        public ParseHelper(string settingsFilePath)
        {
            this.settingsFilePath = settingsFilePath;
            settings = new FileIniDataParser().ReadFile(settingsFilePath);
        }
        public bool ContainsScript()
        {
            if (settings[machineContext].ContainsKey(RunFilePathName) || settings[userContext].ContainsKey(RunFilePathName))
                return true;
            return false;
        }
        public bool ContainsApp()
        {
            if (settings[machineContext].ContainsKey(InstallFilePathName) || settings[userContext].ContainsKey(InstallFilePathName))
                if (settings[machineContext].ContainsKey(UninstallFilePathName) || settings[userContext].ContainsKey(UninstallFilePathName))
                    return true;
            return false;
        }
        public Guid GetSettingGuid(string var, string section = "")
        {
            
            try
            {
                return Guid.Parse(GetSetting(var, section));
            }
            catch (FormatException ex)
            {
                Log.Debug(ex, "Guid Parse failed");
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetSetting(string var, string section ="", bool required = true)
        {
            if (section == "")
            {
                var data = settings.Global.GetKeyData(var);
                if (data == null || data.Value == "")
                    return null;
                else
                    return data.Value;
            }
            else
            {
                var data = settings[section].GetKeyData(var);
                if (required)
                {
                    if (data == null || data.Value == "")
                        return null;
                    else
                        return data.Value;
                }
                else
                {
                    return "";
                }
                
            }
            
        }
        public bool SectionExists(string section)
        {
            foreach (var s in settings.Sections)
            {
                if (s.SectionName == section)
                    return true;
            }
            return false;
        }
        public List<string> GetSections()
        {
            return settings.Sections.Select(s => s.SectionName).ToList();
        }
    }
}
