using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models.Commands
{
    public class CommandRunExecutable : CommandExecutableExecution
    {
        public override ExecuteAction ExecuteAction { get => ExecuteAction.Run; }

    }
}
