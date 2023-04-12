using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Controller.Settings
{
    public interface ISettings
    {
        string SectionName { get; }
        string ExecutablesPath { get; }
        bool IsValid { get; }
        void LoadSettings();


    }
}
