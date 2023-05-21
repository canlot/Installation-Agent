using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models
{
    public class Executable
    {
        private ExecutableBase executableBase;
        public ExecutableBase ExecutableBase { get => executableBase; set => executableBase = value; }
        public Guid Id { get => executableBase.Id; }
        public string Name { get => executableBase.Name; }
        private Version version;
        public Version Version { get => version; set => version = value; }
        private Dictionary<string, string> versionDescriptions;
        public Dictionary<string, string> VersionDescriptions { get => versionDescriptions; set => versionDescriptions = value; }
        private StatusState statusState;
        public StatusState StatusState { get => statusState; set => statusState = value; }

        public string statusMessage;
        public string StatusMessage { get => statusMessage; set => statusMessage = value; }
        protected bool successfulRollout;
        public bool SuccessfulRollout { get => successfulRollout; set => successfulRollout = value; }

        private bool currentlyExecuting;
        public bool CurrentlyExecuting { get => currentlyExecuting; set => currentlyExecuting = value; }
        private string currentDirectory;
        public string CurrentDirectory { get => currentDirectory; set => currentDirectory = value; }
    }
}
