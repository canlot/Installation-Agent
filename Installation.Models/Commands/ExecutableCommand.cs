using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models
{
    public class ExecutableCommand : Command
    {
        private ExecuteAction action;
        public ExecuteAction Action { get => action; set { action = value;} }

        private ExecutionState executionState;
        public ExecutionState ExecutionState { get => executionState; set { executionState = value;  } }

        private StatusState statusState;
        public StatusState StatusState { get => statusState; set { statusState = value;  } }

        public string StatusMessage { get; set; }


        private Guid executableID;
        public Guid ExecutableID { get => executableID; set { executableID = value;  } }
    }
}
