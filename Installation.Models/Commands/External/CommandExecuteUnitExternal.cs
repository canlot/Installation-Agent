using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models.Commands
{
    public class CommandExecuteUnitExternal : CommandExecuteExternal
    {

        private Guid executableUnitID;

        public Guid ExecutableUnitID
        {
            get { return executableUnitID; }
            set { executableUnitID = value; }
        }

        public override bool IsPrivilegedCommand => false;
    }
}
