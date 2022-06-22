using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Installation.Models
{
    public interface IInstalable
    {
        bool Installed { get; set; }
        string InstallFilePath { get; set; }
        Task InstallAsync(CancellationToken cancellationToken);
    }
}
