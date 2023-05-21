using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models.Interfaces
{
    public interface IExternal
    {
        bool IsPrivilegedCommand { get; }
        Guid EndpointId { get; }
    }
}
