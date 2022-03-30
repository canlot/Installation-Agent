using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Installation.Logger;

namespace Installation.Parser.Exceptions
{
    public class ExecutableNotFoundException : ExecutableException
    {
        public ExecutableNotFoundException(string name, string version) : base()
        {
            string message = $"Could not find {name} with version {version}";
            CommonLogger.LogEvent(message, LogType.Warning);
        }
    }
}
