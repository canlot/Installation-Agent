using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Executors
{
    public interface IRunnableExecutor
    {
        Task RunAsync();
        List<int> SuccessfullRunReturnCodes { get; }
    }
}
