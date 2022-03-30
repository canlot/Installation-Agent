using Installation.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Parser.Exceptions
{
    public class ExecutableBrokenException : ExecutableException
    {
        public ExecutableBrokenException(string settingsFilePath) : base()
        {
            string message = $"Executable settings file is broken {settingsFilePath}";
            CommonLogger.LogEvent(message, LogType.Error);
        }
    }
}
