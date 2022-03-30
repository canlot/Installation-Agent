using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Installation.Controller.Executables
{
    public interface IExecutable
    {
        Task<bool> Install(CancellationToken cancellationToken);
        Task<bool> Reinstall(CancellationToken cancellationToken);
        Task<bool> Uninstall(CancellationToken cancellationToken);
    }
}
