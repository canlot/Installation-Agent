using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Installation.Models;
using Installation.Parser.Helpers;
using Serilog;

namespace Installation.Parser
{
    public class ExecutableParser
    {
		private ParseHelper fileParseHelper;

		public ExecutableParser(string settingsFilePath)
        {
			fileParseHelper = new ParseHelper(settingsFilePath);
        }

		public Executable ParseObject() 
		{
			try
            {
				string executableTypeName = getExecutableTypeName();
				if (executableTypeName == null)
					throw new Exception();
				var executable = getExecutableObject(executableTypeName);
				if (executable == null)
					throw new Exception();

				foreach (var property in executable.GetType().GetProperties())
				{
					ExecutableSettingAttribute att = (ExecutableSettingAttribute)Attribute.GetCustomAttribute(property, typeof(ExecutableSettingAttribute));
					Log.Verbose(property.Name);
					if (att != null)
					{
						string keyName;
						if (att.ConfigName != null)
							keyName = att.ConfigName;
						else
							keyName = property.Name;


						string executableAttributeName = getExecutableNameAttribute(property.DeclaringType);

						string settingValue;
						if (executableAttributeName == null || executableAttributeName == "")
                        {
							settingValue = fileParseHelper.GetSetting(att.ConfigName);
                        }
						else
                        {
							settingValue = fileParseHelper.GetSetting(att.ConfigName, executableAttributeName);
                        }


						if (att.DefaultValue != null)
							settingValue = att.DefaultValue;
						else
						{
							if (att.Mandatory)
								throw new Exception();

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
			catch (Exception ex)
            {
				Log.Error(ex, "could not parse file");
				return null;
            }

			
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
			if(sections == null || sections.Count != 2)
            {
				Log.Error("More or less than 2 sections exists");
				return null;
            }
			return sections.Where(name => name != "Global").Single();
        }
	}
}
