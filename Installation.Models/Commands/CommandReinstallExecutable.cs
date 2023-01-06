using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models.Commands
{
    public class CommandReinstallExecutable : CommandExecutableExecution
    {
        public override ExecuteAction ExecuteAction { get => ExecuteAction.Reinstall; }

    }
}
