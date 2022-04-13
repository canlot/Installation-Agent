using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Installation.Parser.Exceptions
{
    public class ExecutableNotFoundException : ExecutableException
    {
        public ExecutableNotFoundException(string name, string version) : base()
        {
            Log.Warning("Could not find {name} with version {version}", name, version);
        }
    }
}
