using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Installation.Models.Interfaces
{
    public interface IReinstallable
    {
        bool ReInstalled { get; set; }
        string ReinstallFilePath { get; set; }
        (StatusState, string) Reinstall();
        Task<(StatusState, string)> ReinstallAsync(CancellationToken cancellationToken);
    }
}
