using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models
{
    public interface ICommandAndObjectReceiver<T>
    {
        void Receive(T rObject, Command<T> command);
    }
}
