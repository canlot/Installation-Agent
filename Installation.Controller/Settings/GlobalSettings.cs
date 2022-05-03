using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;
using System.IO;
using Installation.Parser.Exceptions;
using Serilog;

namespace Installation.Controller.Settings
{
    public class GlobalSettings
    {
        public string IniFile = "Settings.ini";
        private string applicationSettingsFileName;
        public string ApplicationSettingsFileName { get => applicationSettingsFileName; private set => applicationSettingsFileName = value; }
        public List<ISettings> ExecutablesSettings = new List<ISettings>();

        public IniData IniData;

        public GlobalSettings()
        {
        }
        public GlobalSettings(string iniFilePath)
        {
            this.IniFile = iniFilePath;
        }
        public void LoadSettings()
        {
            //string executablePath = AppDomain.CurrentDomain.BaseDirectory;
            string executablePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Log.Debug("Exe file path: {path}", executablePath);
            var path = Path.Combine(executablePath, IniFile);
            //var path = Path.GetFullPath(IniFile);
            if(File.Exists(path))
            {
                IniData = new FileIniDataParser().ReadFile(path);
                parseSettings();
            }
            else
            {
                Log.Fatal("Settings file not found {path}", path);
                throw new FileNotFoundException();
            }

        }
        private void parseSettings()
        {
            matchEntry("AppSettingsFileName", ref applicationSettingsFileName);
        }

        private void addExecutableSetting(ISettings settings)
        {
            
        }

        private void matchEntry(string entry, ref string field)
        {
            var temp = IniData.GetKey(entry);
            if (temp == null || temp == "")
            {
                Log.Error("Setting {section} for GlobalSettings not found.", entry);
                throw new SettingNotFoundException(entry);
            }
            else
                field = temp;

        }

        public T GetSettings<T>() where T : ISettings, new()
        {
            T settings = new T();
            settings.LoadSettings(IniData);
            return settings;
        }
        
    }
}
