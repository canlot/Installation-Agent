using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models
{
    public class CommandGetExecutable : Command
    {
        public CommandGetExecutable()
        {
        }

        private Guid executableID;
        public Guid ExecutableID { get => executableID; set { executableID = value;  } }

        private Version version;

        public Version Version
        {
            get { return version; }
            set { version = value; }
        }


        public override bool IsPrivilegedCommand => false;
    }
}
