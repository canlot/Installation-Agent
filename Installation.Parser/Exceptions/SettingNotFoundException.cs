using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Installation.Logger;

namespace Installation.Parser.Exceptions
{
    public class SettingNotFoundException : Exception
    {
        public SettingNotFoundException(string setting) : base()
        {
            string message = $"Could not find {setting} in settings";
            CommonLogger.LogEvent(message, LogType.Error);
        }
    }
}
