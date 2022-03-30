using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Installation.Models;

namespace Installation.Communication
{
    interface ITransport
    {
        //event EventHandler OnNewJob;

        void Listen(CancellationToken cancelationToken);
        void Send(Job job);
    }
}
