using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models.Commands
{
    public class CommandUninstallExecutableExternal : CommandExecuteExecutableExternal
    {

        public override ExecuteAction ExecuteAction { get => ExecuteAction.Uninstall; }

        public override bool IsPrivilegedCommand => false;

    }
}
