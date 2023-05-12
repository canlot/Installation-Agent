using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;
using Installation.Parser.Exceptions;
using System.IO;
using Serilog;

namespace Installation.Parser
{

    public class SettingsParser
    {
        public string IniFilePath = "Settings.ini";

        public IniData IniData;

        private string programPath;
        private string section;

        public SettingsParser(string iniFilePath = "Settings.ini")
        {
            IniData = new IniData();
            this.IniFilePath = iniFilePath;
            
            programPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            var path = Path.Combine(programPath, IniFilePath);

            if (File.Exists(path))
            {
                IniData = new FileIniDataParser().ReadFile(path);
            }
            else
            {
                Log.Fatal("Settings file not found {path}", path);
                throw new FileNotFoundException();
            }
        }
        public SettingsParser(string section = "", string iniFilePath = "Settings.ini") : this(iniFilePath)
        {
            this.section = section;
        }

        public T GetValue<T>(string paramName)
        {
            try
            {
                var param = matchEntry(paramName);
                return convertValue<T>(param, paramName);
            }
            catch 
            {
                throw;
            }
            
        }

        public T GetValue<T>(string paramName, T defaultValue)
        {
            string param;
            try
            {
                param = matchEntry(paramName);
                return convertValue<T>(param, paramName);
            }
            catch(SettingException ex)
            {
                return defaultValue;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                Log.Debug("The value was misformed");
                throw new SettingException(paramName);
            }
            
        }

        private T convertValue<T>(string value, string paramName)
        {
            try
            {
                return  (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                Log.Debug("could't convert {param} to type {type}", value, typeof(T));
                throw new SettingException(paramName);
            }
        }

        private string matchEntry(string entry)
        {
            string value;
            if (section == "")
            {
                value = IniData.GetKey(entry);
            }
            else
            {
                value = IniData[section].GetKeyData(entry).Value;
            }
            if (value == "" || value == null)
            {
                Log.Error("Setting {section} for GlobalSettings not found.", entry);
                throw new SettingException(entry);
            }
            else
                return value;

        }
    }

}
