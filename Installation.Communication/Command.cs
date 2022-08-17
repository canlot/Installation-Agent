using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Installation.Communication
{
    
    public enum CommandVerb
    {
        get,
        update,
        add,
        remove,
    }
    public enum CommandStatement
    {
        Executable,

    }
    public class Command
    {
        public CommandVerb Verb;
        public CommandStatement Statement;
    }
}
