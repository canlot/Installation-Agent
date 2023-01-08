using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Installation.Models
{
    public interface IReinstallable
    {
        bool ReInstalled { get; set; }
        Task ReinstallAsync(CancellationToken cancellationToken);
    }
}
