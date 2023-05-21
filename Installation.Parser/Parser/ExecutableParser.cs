using System;
using System.Collections;
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

		public ExecutableBase ParseObject() 
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
					string propertyName;
					if (att.ConfigName != null) // if custom Name is not specified in the executable, then it takes the variable name
						propertyName = att.ConfigName;
					else
						propertyName = property.Name;


					string executableAttributeName = getExecutableNameAttribute(property.DeclaringType); // get from which type this attribute come from

					string settingValue;
					if (string.IsNullOrEmpty(executableAttributeName)) // when its come from base executable type
                    {
						settingValue = fileParseHelper.GetSetting(propertyName);
                    }
					else // when its come frome inhereted types
                    {
						settingValue = fileParseHelper.GetSetting(propertyName, executableAttributeName);
                    }

					if(string.IsNullOrEmpty(settingValue)) // checks if value got from file is empty
                    {
						if (att.DefaultValue != null)
							settingValue = att.DefaultValue;
						else
						{
							if (att.Mandatory)
							{
								Log.Error("setting {setting} is mandatory", propertyName);
								throw new ExecutableBrokenException(settingsFilePath);
							}
						}
					}
					else
                    {
						setProperty(property, executable, settingValue);
					}

				}
			}
			Log.Debug("File: {file} successfully parsed", settingsFilePath);
			return executable;

		}

		private void setProperty(PropertyInfo property, ExecutableBase executable, string settingValue)
        {
			
			if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
			{
				Type t = property.PropertyType;
				Type[] typeParameters = t.GetGenericArguments();
				
				if(typeParameters.Length == 1)
                {
					setPropertyList(property, executable, settingValue, typeParameters[0]);
                }
				else
                {
					Log.Error("1 argument of List<T> expected, more or less given from property: {property}", property.Name);
					throw new ArgumentOutOfRangeException();
                }

			}
			else
            {
				setPropertySingle(property, executable, settingValue);
            }
			
		}
		private List<T> getList<T>()
        {
			List<T> list = new List<T>();
			return list;
        }
		private void setPropertyList(PropertyInfo property, ExecutableBase executable, string settingValue, Type type)
        {
			Type genericListType = typeof(List<>).MakeGenericType(type);
			var list = (IList)Activator.CreateInstance(genericListType);
			addValuesToList(list, settingValue, type);
			property.SetValue(executable, list);
			
		}
		private void addValuesToList(IList list, string settingValue, Type type)
        {
			var array = settingValue.Split(',');
			foreach(var item in array)
            {
				var value = Convert.ChangeType(item, type);
				list.Add(value);
			}
        }
		private void setPropertySingle(PropertyInfo property, ExecutableBase executable, string settingValue)
        {
			if (property.PropertyType.IsEnum)
			{
				property.SetValue(executable, (object)Enum.Parse(property.PropertyType, settingValue));
			}
			else if (property.PropertyType == typeof(Guid))
			{
				property.SetValue(executable, (object)Guid.Parse(settingValue));
			}
			else if (property.PropertyType == typeof(string))
			{
				property.SetValue(executable, System.Convert.ChangeType(settingValue, property.PropertyType));
			}
			else
			{
				property.SetValue(executable, System.Convert.ChangeType(settingValue, property.PropertyType));
			}
		}

		private ExecutableBase getExecutableObject(string name)
		{
			foreach (Type type in Assembly.GetAssembly(typeof(ExecutableBase)).GetTypes())
			{
				if (type.IsSubclassOf(typeof(ExecutableBase)))
				{
					ExecutableAttribute executableAttribute = (ExecutableAttribute)Attribute.GetCustomAttribute(type, typeof(ExecutableAttribute));
					if (executableAttribute != null)
					{
						if (executableAttribute.ExecutableName == name)
							return (ExecutableBase)Activator.CreateInstance(type);
						else
							Log.Verbose("ExecutableBase type: {type} does not match with type: {name} from setting file", executableAttribute.ExecutableName, name);
					}
				}
			}
			Log.Error("Could not create instance for type {type} extracted from setting: {file}", name, settingsFilePath);
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
