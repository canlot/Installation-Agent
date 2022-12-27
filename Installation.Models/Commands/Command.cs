using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Installation.Models
{
    

    abstract public class Command
    {
        public Command()
        {
            
        }
        protected Guid? endpointId;

        public Guid? EndpointId
        {
            get { return endpointId; }
            set { endpointId = value; }
        }

    }
}
