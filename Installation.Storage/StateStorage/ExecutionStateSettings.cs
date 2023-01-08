using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Installation.Models;
using Installation.Storage;



namespace Installation.Storage.StateStorage
{
    public class ExecutionStateSettings
    {

        private string installedKey = "Installed";
        private string reinstalledKey = "Reinstalled";
        private string runnedKey = "Runned";
        private string uninstalledKey = "Uninstalled";

        IExecutableStateStorageProvider stateStorageProvider;
        public ExecutionStateSettings()
        {
            stateStorageProvider = new ExecutableStateRegistryStorageProvider();
        }

        public void LoadExecutableState(Executable executable)
        {
            if (executable is IInstallable)
                (executable as IInstallable).Installed = stateStorageProvider.GetStateValue(installedKey, executable.Id.ToString());
            if (executable is IReinstallable)
                (executable as IReinstallable).ReInstalled = stateStorageProvider.GetStateValue(reinstalledKey, executable.Id.ToString());
            if (executable is IRunnable)
                (executable as IRunnable).Runned = stateStorageProvider.GetStateValue(runnedKey, executable.Id.ToString());
            if (executable is IUninstallable)
                (executable as IUninstallable).UnInstalled = stateStorageProvider.GetStateValue(uninstalledKey, executable.Id.ToString());
        }

        public void SaveExecutableState(Executable executable)
        {
            if (executable is IInstallable)
                stateStorageProvider.SaveStateValue(installedKey, executable.Id.ToString(), (executable as IInstallable).Installed);
            if (executable is IReinstallable)
                stateStorageProvider.SaveStateValue(reinstalledKey, executable.Id.ToString(), (executable as IReinstallable).ReInstalled);
            if (executable is IRunnable)
                stateStorageProvider.SaveStateValue(runnedKey, executable.Id.ToString(), (executable as IRunnable).Runned);
            if (executable is IUninstallable)
                stateStorageProvider.SaveStateValue(uninstalledKey, executable.Id.ToString(), (executable as IUninstallable).UnInstalled);
            return;
        }


    }
}
