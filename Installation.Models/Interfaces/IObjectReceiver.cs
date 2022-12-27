using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models.Interfaces
{
    public interface IObjectReceiver<T>
    {
        void Receive(T rObject);
    }
}