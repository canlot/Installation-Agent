using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Executors
{
    public interface IApplicationExecutor
    {
        Task<(bool, string)> InstallAsync();
        Task<(bool, string)> ReinstallAsync();
        Task<(bool, string)> UninstallAsync();
    }
}
