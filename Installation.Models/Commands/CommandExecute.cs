using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models.Commands
{
    abstract public class CommandExecute : Command
    {
        protected Guid executableID;
        public Guid ExecutableID { get => executableID; set { executableID = value; } }

        protected Version version;
        public Version Version { get => version; set {  version = value; } }
    }
}
