using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models.Commands
{
    public class CommandReinstallExecutable : CommandExecuteExecutable
    {
        public override ExecuteAction ExecuteAction { get => ExecuteAction.Reinstall; }

        public override bool IsPrivilegedCommand => false;

    }
}
