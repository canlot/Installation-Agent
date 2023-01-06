using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models.Commands
{
    abstract public class CommandExecutableExecution : Command
    {
        protected Guid executableID;
        public Guid ExecutableID { get => executableID; set { executableID = value; } }
        abstract public ExecuteAction ExecuteAction { get; }
    }
}
