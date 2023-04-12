using Installation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Controller.Settings
{
    public sealed class SettingsContainer
    {
        private GlobalSettings globalSettings;
        public GlobalSettings GlobalSettings { get => globalSettings; 
            set 
            {
                if (globalSettings == null)
                    globalSettings = value;
                else
                    throw new ArgumentException();
            }
        }

    }
}
