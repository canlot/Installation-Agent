using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models.Commands
{
    public class CommandExecuteUnit : CommandExecute
    {
        protected Guid executableID;
        public Guid ExecutableID { get => executableID; set { executableID = value; } }

        private Guid executableUnitID;

        public Guid ExecutableUnitID
        {
            get { return executableUnitID; }
            set { executableUnitID = value; }
        }

        public override ExecuteAction ExecuteAction => throw new NotImplementedException();
    }
}
