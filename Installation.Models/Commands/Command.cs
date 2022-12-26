using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Installation.Models
{
    
    public enum CommandAction
    {
        Get = 1,
        Update = 2,
        Abort = 4,
        Add = 8,
        Remove = 16,
        Install = 32,
        Reinstall = 64,
        Uninstall = 128,
        Run = 256,
    }

    abstract public class Command<T>
    {
        abstract public CommandAction CommandAction { get; }
        public Command()
        {
            
        }
        protected Guid? endpointId;

        public Guid? EndpointId
        {
            get { return endpointId; }
            set { endpointId = value; }
        }


        protected CommandAction commandAction;
    }
}
