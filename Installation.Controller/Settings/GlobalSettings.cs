using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Serilog;
using Installation.Parser;

namespace Installation.Controller.Settings
{
    public class GlobalSettings : ISettings
    {
        private string applicationSettingsFileName;
        public string ApplicationSettingsFileName { get => applicationSettingsFileName; private set => applicationSettingsFileName = value; }

        private string executablesPath;
        public string ExecutablesPath { get => executablesPath; }

        private string serverLogsFilePath;
        public string ServerLogsFilePath { get => serverLogsFilePath; private set => serverLogsFilePath = value; }

        

        private int pullIntervalTimeInSeconds = 600;
        
        public int PullIntervalTimeInSeconds { get => pullIntervalTimeInSeconds;  set => pullIntervalTimeInSeconds = value; }

        public string SectionName => throw new NotImplementedException();

        public bool IsValid => throw new NotImplementedException();

        public GlobalSettings()
        {

        }
        public GlobalSettings(string applicationSettingsFileName, string executablesPath, string serverLogsFilePath)
        {

            
        }        

        public T GetSettings<T>() where T : ISettings, new()
        {
            T settings = new T();
            settings.LoadSettings();
            return settings;
        }

        public void LoadSettings()
        {
            SettingsParser settingsParser = new SettingsParser(section: "");
            executablesPath = settingsParser.GetValue<string>("ExecutablesPath");
            applicationSettingsFileName = settingsParser.GetValue<string>("AppSettingsFileName", "Settings.ini");
            pullIntervalTimeInSeconds = settingsParser.GetValue<int>("PullIntervallTimeInSeconds", 300);
            serverLogsFilePath = getServerLogsFilePath();

#if DEBUG
            pullIntervalTimeInSeconds = 10;
#endif
        }

        private string getServerLogsFilePath()
        {
            var programPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var serverLogsFilePath = Path.Combine(programPath, @"Logs\Server\log.txt");
            return serverLogsFilePath;
        }
    }
}
