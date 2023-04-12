using Installation.Controller.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models.Settings
{
    public class SettingsBuilder
    {
        public T GetSettings<T>() where T : ISettings, new()
        {
            T settings = new T();
            settings.LoadSettings();
            return new T();
        }
    }
}
