using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Installation.Models;
using Installation.Parser.Exceptions;
using Installation.Parser.Helpers;
using Serilog;

namespace Installation.Parser
{
    public class ExecutableParser
    {
		private ParseHelper fileParseHelper;
		private string settingsFilePath;

		public ExecutableParser(string settingsFilePath)
        {
			this.settingsFilePath = settingsFilePath;
			this.fileParseHelper = new ParseHelper(settingsFilePath);
        }

		public Executable ParseObject() 
		{
			Log.Debug("Start to parse following executable file: {file}", settingsFilePath);

			string executableTypeName = getExecutableTypeName();
			if (executableTypeName == null)
				throw new ExecutableBrokenException(settingsFilePath);
			var executable = getExecutableObject(executableTypeName);
			if (executable == null)
				throw new ExecutableBrokenException(settingsFilePath);

			foreach (var property in executable.GetType().GetProperties())
			{
				ExecutableSettingAttribute att = (ExecutableSettingAttribute)Attribute.GetCustomAttribute(property, typeof(ExecutableSettingAttribute));
				//Log.Verbose(property.Name);
				if (att != null)
				{
					string keyName;
					if (att.ConfigName != null) // if custom Name is not specified in the executable, then it take the variable name
						keyName = att.ConfigName;
					else
						keyName = property.Name;


					string executableAttributeName = getExecutableNameAttribute(property.DeclaringType); // get from which type this attribute come from

					string settingValue;
					if (executableAttributeName == null || executableAttributeName == "") // when its come from base executable type
                    {
						settingValue = fileParseHelper.GetSetting(att.ConfigName);
                    }
					else // when its come frome inhereted types
                    {
						settingValue = fileParseHelper.GetSetting(att.ConfigName, executableAttributeName);
                    }

					if(settingValue == null || settingValue == "") // checks if value got from file is empty
                    {
						if (att.DefaultValue != null)
							settingValue = att.DefaultValue;
						else
						{
							if (att.Mandatory)
							{
								Log.Error("setting {setting} is mandatory", att.ConfigName);
								throw new ExecutableBrokenException(settingsFilePath);
							}
						}
					}
					

						
					if (property.PropertyType.IsEnum)
					{
						property.SetValue(executable, (object)Enum.Parse(property.PropertyType, settingValue));
					}
					else if (property.PropertyType == typeof(Guid))
                    {
						property.SetValue(executable, (object)Guid.Parse(settingValue));
                    }
					else
					{
						property.SetValue(executable, System.Convert.ChangeType(settingValue, property.PropertyType));
					}
						
				}
			}
			return executable;

		}

		private Executable getExecutableObject(string name)
		{
			foreach (Type type in Assembly.GetAssembly(typeof(Executable)).GetTypes())
			{
				if (type.IsSubclassOf(typeof(Executable)))
				{
					ExecutableAttribute executableAttribute = (ExecutableAttribute)Attribute.GetCustomAttribute(type, typeof(ExecutableAttribute));
					if (executableAttribute != null)
					{
						if (executableAttribute.ExecutableName == name)
							return (Executable)Activator.CreateInstance(type);
						else
							Log.Error("No executable type tag on attribute");
					}
				}
			}
			return null;
		}

		private string getExecutableNameAttribute(Type type)
        {
			ExecutableAttribute executableAttribute = (ExecutableAttribute)Attribute.GetCustomAttribute(type, typeof(ExecutableAttribute));
			return executableAttribute != null ? executableAttribute.ExecutableName : null;
        }

		private string getExecutableTypeName()
        {
			var sections = fileParseHelper.GetSections();
			if(sections == null || sections.Count < 1)
            {
				Log.Error("No section exists");
				return null;
            }
			else if(sections.Count > 1)
            {
				Log.Error("More than one section exists");
			}
			return sections[0];
        }
	}
}
