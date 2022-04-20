using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;
using Installation.Parser.Helpers;
using Installation.Parser.Exceptions;
using Installation.Models;
using Serilog;



namespace Installation.Parser
{
    public class ExecutableFileParser
    {
        private readonly string RunFilePathName = "RunFilePath";
        private readonly string InstallFilePathName = "InstallFilePath";
        private readonly string ReinstallFilePathName = "ReinstallFilePath";
        private readonly string UninstallFilePathName = "UninstallFilePath";

        private IniData settingsData;
        private ParseHelper helper;

        
    public Executable ParseExecutableSettingsFile(string settingsFilePath) // parse installation file for current executable
    {
            helper = new ParseHelper(settingsFilePath);

            Executable executable;
            try
            {
                executable = getNewExecutable(helper.GetSetting(nameof(Executable.Category)));
            }
            catch
            {
                throw new ExecutableBrokenException(settingsFilePath);
            }

            try
            {
                executable.Id = helper.GetSettingGuid(nameof(Executable.Id));
                executable.Name = helper.GetSetting(nameof(Executable.Name)).Replace("\"", "");
                executable.Version = helper.GetSetting(nameof(Executable.Version)).Replace("\"", "");


            }
            catch
            {
                throw new ExecutableBrokenException(settingsFilePath);
            }
            
            if(!parseSettingsForExecutable(executable))
            {
                Log.Error("Executable settings file broken {file}", settingsFilePath);
                throw new ExecutableBrokenException(settingsFilePath);
            }

            return executable;

        }

        private Executable getNewExecutable(string category)
        {
            switch (category)
            {
                case "Script":
                    return new ScriptExecutable();
                default: throw new SettingNotFoundException(category);
            }
        }

        private bool parseSettingsForExecutable(Executable executable)
        {
            if ((executable is IInstalable) || (executable is IReinstallable) || (executable is IUninstallable))
            {
                return parseSettingsIfRight(executable, InstallFilePathName, ReinstallFilePathName, UninstallFilePathName);
            }
            else if (executable is IRunnable)
            {
                return parseSettingsIfRight(executable, RunFilePathName);
            }
            return false;

        }
        private bool parseSettingsIfRight(Executable executable, params string[] selectedExecutableOptions)
        {
            string[] allExecutableOptions = { InstallFilePathName, ReinstallFilePathName, UninstallFilePathName, RunFilePathName };
            foreach (var someOption in settingsData.Global)
            {
                foreach (var executableOption in allExecutableOptions)
                {
                    if(someOption.KeyName == executableOption)
                    {
                        if (existsInArguments(executableOption, selectedExecutableOptions))
                            setExecutableSetting(executable, someOption.KeyName);
                        else
                            return false;
                    }
                }
            }
            return true;
        }

        private void setExecutableSetting(Executable executable, string setting)
        {
            if (settingsData.Global[setting] == "")
            {
                Log.Error("Value to setting {setting} not found", setting);
                return;
            }
            if(executable is IInstalable)
            {
                if((executable as IInstalable).InstallFilePath == setting)// völlig Banane
                    (executable as IInstalable).InstallFilePath = settingsData.Global[setting];  
            }
            if(executable is IReinstallable)
            {
                if((executable as IReinstallable).ReinstallFilePath == setting) // völlig Banane
                    (executable as IReinstallable).ReinstallFilePath = settingsData.Global[setting];
            }
            if(executable is IUninstallable)
            {
                if((executable as IUninstallable).UninstallFilePath == setting) // völlig Banane
                    (executable as IUninstallable).UninstallFilePath = settingsData.Global[setting];
            }
            if(executable is IRunnable)
            {
                //if((executable as IRunnable).RunFilePath == setting) // völlig Banane
                    (executable as IRunnable).RunFilePath = settingsData.Global[setting];
            }

        }
        

        private bool existsInArguments(string argument, params string[] otherArguments)
        {
            foreach(var otherArgument in otherArguments)
            {
                if(argument == otherArgument)
                    return true;
            }
            return false;
        }
    }
}
