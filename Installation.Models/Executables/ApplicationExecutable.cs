using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Installation.Models.Interfaces;

namespace Installation.Models.Executables
{
    public class ApplicationExecutable : Executable, IInstalable, IReinstallable, IUninstallable
    {
        bool IInstalable.Installed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        string IInstalable.InstallFilePath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        bool IReinstallable.ReInstalled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        string IReinstallable.ReinstallFilePath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        bool IUninstallable.UnInstalled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        string IUninstallable.UninstallFilePath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        (StatusState, string) IInstalable.Install()
        {
            throw new NotImplementedException();
        }

        Task<(StatusState, string)> IInstalable.InstallAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        (StatusState, string) IReinstallable.Reinstall()
        {
            throw new NotImplementedException();
        }

        Task<(StatusState, string)> IReinstallable.ReinstallAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        (StatusState, string) IUninstallable.Uninstall()
        {
            throw new NotImplementedException();
        }

        Task<(StatusState, string)> IUninstallable.UninstallAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
