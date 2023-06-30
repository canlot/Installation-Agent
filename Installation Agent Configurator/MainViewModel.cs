using Installation.Models;
using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Installation_Agent_Configurator
{
    public class MainViewModel
    {
        public List<ExecutableBase> Executables { get; set; }

        public MainViewModel() 
        {
            Executables = new List<ExecutableBase>();
            Executables.Add(new ExecutableBase()
            { Name = "Vlc Media Player"});
            Executables.Add(new ExecutableBase()
            {
                Name = "Linq Pad"
            });
        }


    }
}
