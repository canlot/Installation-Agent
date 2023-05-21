using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models.Commands
{
    public abstract class CommandExecuteExecutableExternal : CommandExecuteExternal
    {
        abstract public ExecuteAction ExecuteAction { get; }
    }
}
