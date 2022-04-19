using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Installation.Models;
using Serilog;

namespace Installation.Parser
{
    public class ExecutableParser
    {
		public T ParseObject<T>(string name, Dictionary<string, string> dict) where T : class
		{
			try
            {

				var executable = getRightExecutable<T>(name);

				foreach (var property in typeof(T).GetProperties())
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
								throw new Exception();
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
			}
			catch (Exception ex)
            {
				Log.Error(ex, "could not parse file");
            }

			
		}

		private object getRightExecutable<T>(string name)
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
	}
}
