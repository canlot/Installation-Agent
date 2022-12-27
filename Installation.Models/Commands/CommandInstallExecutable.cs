using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models.Commands
{
    public class CommandInstallExecutable : Command
    {
        private Guid executableID;
        public Guid ExecutableID { get => executableID; set { executableID = value; } }
    }
}
