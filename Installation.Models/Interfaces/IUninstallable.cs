using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Installation.Models
{
    public interface IUninstallable
    {
        bool UnInstalled { get; set; }
        List<ExecutableUnit> UninstallableUnits { get; set; }
    }
}
