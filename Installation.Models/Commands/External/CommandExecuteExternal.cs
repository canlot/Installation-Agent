using Installation.Models.Helpers;
using Installation.Models.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models.Commands
{
    abstract public class CommandExecuteExternal : Command, IExternal
    {
        [JsonIgnore] //TODO: create Unit test that make sure that the property is ignored
        public abstract bool IsPrivilegedCommand { get; }

        protected Guid endpointId = new Guid().Empty();

        public Guid EndpointId
        {
            get { return endpointId; }
            set { endpointId = value; }
        }

        protected Guid executableID;
        public Guid ExecutableID { get => executableID; set { executableID = value; } }

        protected Version version;
        public Version Version { get => version; set {  version = value; } }
    }
}
