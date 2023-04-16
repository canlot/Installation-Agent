using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Executors
{
    public interface IInstallableExecutor
    {
        Task InstallAsync();

        List<int> SuccessfullInstallationReturnCodes { get; }
    }
}
