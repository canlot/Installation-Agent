using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Installation.Parser.Exceptions
{
    public class SettingException : Exception
    {
        public SettingException(string setting) : base()
        {
            Log.Error("Error in setting: {setting} ", setting);
        }
    }
}
