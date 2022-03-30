using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Installation.Models.Interfaces
{
    public interface IInstalable
    {
        bool Installed { get; set; }
        string InstallFilePath { get; set; }
        (StatusState, string) Install();
        Task<(StatusState, string)> InstallAsync(CancellationToken cancellationToken);
    }
}
