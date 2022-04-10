using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Installation.Models
{
    public enum StatusState
    {
        NotExecuted,
        Success,
        Warning,
        Error
    }
    public enum ExecutionState
    {
        Idle,
        Started,
        Running,
        Stopped,
        Done
    }
    public enum ExecuteAction
    {
        Install,
        Uninstall,
        Reinstall,
        Run
    }
    public enum InstallationState
    {
        NotInstalled,
        Installed
    }
}