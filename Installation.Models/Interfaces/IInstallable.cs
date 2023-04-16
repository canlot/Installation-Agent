using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Installation.Models
{
    public interface IInstallable
    {
        //bool Installed { get; set; }
        //Task InstallAsync(CancellationToken cancellationToken);

        List<ExecutableUnit> InstallableUnits { get; set; }
    }
}
