using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models.Responses
{
    abstract public class Response
    {
        protected Guid? endpointId;

        public Guid? EndpointId
        {
            get { return endpointId; }
            set { endpointId = value; }
        }
    }
}
