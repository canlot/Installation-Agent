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

		public T ParseObject<T>(string name, Dictionary<string, string> dict) where T : class
		{
			try
            {

				var executable = getExecutableObject<T>(name);

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

						string value;
						if (!dict.TryGetValue(keyName, out value))
						{
							if (att.DefaultValue != null)
								value = att.DefaultValue;
							else
							{
								if (att.Mandatory)
									throw new Exception();

							}

						}
						
						if (property.PropertyType.IsEnum)
						{
							property.SetValue(executable, (object)Enum.Parse(property.PropertyType, value));
						}
						else if (property.PropertyType == typeof(Guid))
                        {
							property.SetValue(executable, (object)Guid.Parse(value));
                        }
						else
						{
							property.SetValue(executable, System.Convert.ChangeType(value, property.PropertyType));
						}
						
					}
				}
				return (T)executable;
			}
			catch (Exception ex)
            {
				Log.Error(ex, "could not parse file");
				return default(T);
            }

			
		}

		private object getExecutableObject<T>(string name)
		{
			foreach (Type type in Assembly.GetAssembly(typeof(T)).GetTypes())
			{
				if (type.IsSubclassOf(typeof(Executable)))
				{
					ExecutableAttribute executableAttribute = (ExecutableAttribute)Attribute.GetCustomAttribute(type, typeof(ExecutableAttribute));
					if (executableAttribute != null)
					{
						if (executableAttribute.ExecutableName == name)
							return (T)Activator.CreateInstance(type);
					}
				}
			}
			return default(T);
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
