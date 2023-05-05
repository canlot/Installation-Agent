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
        [JsonIgnore] //TODO: create Unit test that make sure that the property is ignored
        public abstract bool IsPrivilegedCommand { get; }
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
