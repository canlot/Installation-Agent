using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Installation.Models
{
    
    public enum CommandVerb
    {
        Get = 1,
        Update = 2,
        Abort = 4,
        Add = 8,
        Remove = 16,
        Execute = 32
    }

    public class Command
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


        protected CommandVerb commandVerb;
        public CommandVerb CommandVerb
        { 
            get { return commandVerb; } 
            set { commandVerb = value; } 
        }
    }
}
