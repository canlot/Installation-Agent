using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models.Responses
{
    public class ResponseExecutionUnit : Response
    {
        protected Guid executableID;
        public Guid ExecutableID { get => executableID; set { executableID = value; } }

        private Guid executableUnitID;

        public Guid ExecutableUnitID
        {
            get { return executableUnitID; }
            set { executableUnitID = value; }
        }

        private ExecutionState executionState;

        public ExecutionState ExecutionState
        {
            get { return executionState; }
            set { executionState = value; }
        }

        private StatusState statusState;

        public StatusState StatusState
        {
            get { return statusState; }
            set { statusState = value; }
        }



    }
}
