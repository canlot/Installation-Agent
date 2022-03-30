using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Storage
{
    interface IExecutableStateStorageProvider
    {
        bool GetStateValue(string keyName, string executableID);
        void SaveStateValue(string keyName, string executableID, bool value);
    }
}
