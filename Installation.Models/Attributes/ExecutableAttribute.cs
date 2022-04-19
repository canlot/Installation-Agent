using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExecutableAttribute : Attribute
    {
        public ExecutableAttribute(string name)
        {
            ExecutableName = name;
        }
        public string ExecutableName { get; set; }
    }
}
