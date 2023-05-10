using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models.Helpers
{
    public static class GuidHelper
    {
        public static Guid Broadcast(this Guid guid)
        {
            return Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
        }
        public static Guid Empty(this Guid guid)
        {
            return Guid.Empty;
        }
        public static bool NullOrEmpty(this Guid guid)
        {
            if(guid == null)
                return true;
            if (guid == Guid.Empty)
                return true;
            return false;
        }
        public static bool IsBroadcast(this Guid guid)
        {
            if (guid == new Guid().Broadcast())
                return true;
            return false;
        }
    }
}
