﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Executors
{
    public interface IApplicationExecutor
    {
        Task InstallAsync();
        Task ReinstallAsync();
        Task UninstallAsync();

        List<int> SuccessfullInstallationReturnCodes { get; }
        List<int> SuccessfullReinstallationReturnCodes { get; }
        List<int> SuccessfullUninstallationReturnCodes { get; }
    }
}
