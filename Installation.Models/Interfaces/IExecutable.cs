using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models.Interfaces
{
    public interface IExecutable
    {
        ExecutableBase ExecutableBase { get; set; }
        Guid Id { get; }
        string Name { get; }
        Version Version { get; }
        Dictionary<string, string> VersionDescriptions { get; }
        StatusState StatusState { get; }

        string StatusMessage { get; }
        bool SuccessfulRollout { get; }

        bool CurrentlyExecuting { get; }
        string CurrentDirectory { get; }

    }
}
