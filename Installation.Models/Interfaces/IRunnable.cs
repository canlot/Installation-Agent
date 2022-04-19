﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Installation.Models;

namespace Installation.Models
{
    public interface IRunnable
    {
        bool Runned { get; set; }
        string RunFilePath { get; set; }

        Task<string> RunAsync(CancellationToken cancellationToken);
    }
}
