using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Installation.Controller.Executables
{
    public class ScriptExecutable : IExecutable
    {
        private string installFilePath;
        private string reinstallFilePath;
        private string uninstallFilePath;

        public string Name;
        public string Version;
        public ScriptExecutable(string installFilePath, string uninstallFilePath)
        {
            this.installFilePath = installFilePath;
            this.uninstallFilePath = uninstallFilePath;
        }
        public ScriptExecutable(string installFilePath, string reinstallFilePath, string uninstallFilePath) : this(installFilePath, uninstallFilePath)
        {
            this.reinstallFilePath = reinstallFilePath;
            
        }

        public async Task<bool> Install(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> Reinstall(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> Uninstall(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public static string GetUniqueName(string name, string version)
        {
            return name + "_" + version;
        }
    }
}
