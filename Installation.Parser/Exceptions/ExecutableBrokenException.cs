using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Installation.Parser.Exceptions
{
    public class ExecutableBrokenException : ExecutableException
    {
        public ExecutableBrokenException(string settingsFilePath) : base()
        {
            Log.Error("ExecutableBase settings file is broken {settingsFilePath}",settingsFilePath);
        }
    }
}
