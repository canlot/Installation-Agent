using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models.Responses
{
    public class ResponseExecution : Response
    {
        protected Guid executableID;
        public Guid ExecutableID { get => executableID; set { executableID = value; } }

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

        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }


    }
}
