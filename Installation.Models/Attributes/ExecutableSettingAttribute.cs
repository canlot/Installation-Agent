using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models
{
    public enum ConfigContext
    {
        File,
        Registry
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ExecutableSettingAttribute : Attribute
    {
        public string DefaultValue { get; set; }
        public ConfigContext ConfigContext { get; set; } = ConfigContext.File;
        public string ConfigName { get; set; }
        public bool Mandatory { get; set; } = true;
    }
}
