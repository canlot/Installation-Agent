using IniParser.Model;
using Installation.Parser.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Installation.Controller.Settings
{
    public class AppSettings : ISettings
    {
        private string executablesPath;
        private string registryPath;
        public string ExecutablesPath { get => executablesPath; }
        public string RegistryPath { get => registryPath; }
        public string RunFileName;
        public string NameIdentifier;
        public string DependsOnExecutables;
        public string ExcludedExecutables;

        private string sectionName = "AppSettings";
        public string SectionName { get => sectionName; }
        public bool IsValid { get; private set; }

        private IniData iniData;
        public AppSettings()
        {

        }

        public void LoadSettings(IniData iniData)
        {
            if (!iniData.Sections.ContainsSection(sectionName))
            {
                IsValid = false;
                return;
            }
            try
            {
                matchEntry("ExecutablesPath", ref executablesPath);
            }
            catch
            {
                IsValid = false;
            }
            IsValid = true;
        }
        private void matchEntry(string entry, ref string field)
        {

            var temp = iniData[sectionName][entry];
            if (temp == null || temp == "")
            {
                Log.Error("Setting {section} for AppSetting not found.", entry);
                throw new SettingNotFoundException(entry);
            }
            else
                field = temp;
        }

    }
}
