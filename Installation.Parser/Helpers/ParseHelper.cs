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
        private string settingsFilePath;
        public ParseHelper(string settingsFilePath)
        {
            this.settingsFilePath = settingsFilePath;
            settings = new FileIniDataParser().ReadFile(settingsFilePath);
            settings.Configuration.CaseInsensitive = true;
            
        }

        public string GetSetting(string var, string section ="", bool required = true)
        {
            if (section == "")
            {
                var data = settings.Global.GetKeyData(var);
                if (data == null || data.Value == "")
                    return null;
                else
                    return data.Value.RemoveWrongCharacters();
            }
            else
            {
                var data = settings[section].GetKeyData(var);
                if (required)
                {
                    if (data == null || data.Value == "")
                        return null;
                    else
                        return data.Value.RemoveWrongCharacters();
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
