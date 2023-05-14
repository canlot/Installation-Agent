using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models.Interfaces
{
    public interface IObjectReceiver<in T>
    {
        void Receive(T rObject);
    }
    public interface IObjectReceiver<in T, out TReturn>
    {
        TReturn Receive(T rObject);
    }
}