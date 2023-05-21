using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models.Commands
{
    public class CommandInstallExecutableExternal : CommandExecuteExecutableExternal
    {

        public override ExecuteAction ExecuteAction { get => ExecuteAction.Install; }

        public override bool IsPrivilegedCommand => false;
    }
}
