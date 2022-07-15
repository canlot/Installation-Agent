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

        private string executablesPath;
        public string ExecutablesPath { get => executablesPath; }

        private string serverLogsFilePath;
        public string ServerLogsFilePath { get => serverLogsFilePath; private set => serverLogsFilePath = value; }

        private string programPath;
        public string ProgramPath { get => programPath; private set => programPath = value; }

        private int pullIntervalTimeInSeconds = 600;
        
        public int PullIntervalTimeInSeconds { get => pullIntervalTimeInSeconds; private set => pullIntervalTimeInSeconds = value; }

        public IniData IniData;

        public GlobalSettings()
        {
#if DEBUG
            pullIntervalTimeInSeconds = 10;
#endif
            programPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            serverLogsFilePath = Path.Combine(programPath, @"Logs\Server\log.txt");
        }
        public GlobalSettings(string iniFilePath)
        {
            this.IniFile = iniFilePath;
        }
        public void LoadSettings()
        {
            //string executablePath = AppDomain.CurrentDomain.BaseDirectory;
            
            var path = Path.Combine(programPath, IniFile);
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
            matchEntry("AppSettingsFileName", ref applicationSettingsFileName, "Global");
            matchEntry("ExecutablesPath", ref executablesPath, "Global");

            try
            {
                string time = "";
                matchEntry("PullIntervalTimeInSeconds", ref time, "Global");
                pullIntervalTimeInSeconds = (int)Convert.ChangeType(time, pullIntervalTimeInSeconds.GetType());
            }
            catch (Exception ex)
            {
                if (ex is SettingNotFoundException)
                    Log.Debug(ex, "{VarName} not found in config file", nameof(PullIntervalTimeInSeconds));
                else
                    Log.Debug(ex, "{VarName} cannot be converted", nameof(PullIntervalTimeInSeconds));
            }
        }

        private void addExecutableSetting(ISettings settings)
        {
            
        }

        private void matchEntry(string entry, ref string field, string section = "")
        {
            string value = "";
            if(section == "")
            {
                value = IniData.GetKey(entry);
            }
            else
            {
                value = IniData[section].GetKeyData(entry).Value;
            }
            if (value == ""  || value == null)
            {
                Log.Error("Setting {section} for GlobalSettings not found.", entry);
                throw new SettingNotFoundException(entry);
            }
            else
                field = value;

        }

        public T GetSettings<T>() where T : ISettings, new()
        {
            T settings = new T();
            settings.LoadSettings(IniData);
            return settings;
        }
        
    }
}
