﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Installation.Models.Interfaces
{
    public interface IUninstallable
    {
        bool UnInstalled { get; set; }
        string UninstallFilePath { get; set; }
        (StatusState, string) Uninstall();
        Task<(StatusState, string)> UninstallAsync(CancellationToken cancellationToken);
    }
}