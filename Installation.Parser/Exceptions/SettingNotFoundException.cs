using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Installation.Parser.Exceptions
{
    public class SettingNotFoundException : Exception
    {
        public SettingNotFoundException(string setting) : base()
        {
            Log.Error("Could not find {setting} in settings", setting);
        }
    }
}
