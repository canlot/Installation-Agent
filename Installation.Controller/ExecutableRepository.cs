using Installation.Models;
using Installation.Models.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Controller
{
    public class ExecutableRepository
    {
        private ConcurrentBag<ExecutableBase> executables = new ConcurrentBag<ExecutableBase>();

        public void AddExecutable(ExecutableBase executable)
        {
            var existingExecutable = executables.FirstOrDefault(x => x.Id == executable.Id);

            executables.Add(executable);
        }
        public void AddExecutableVersion(IExecutable executableVersion)
        {

        }
        public ExecutableBase GetExecutable(Guid id)
        {
            return executables.FirstOrDefault(x => x.Id == id);
        }
        public ExecutableUnit GetExecutableUnit(Guid unitId)
        {
            foreach (var executable in executables)
            {
                foreach(var versioninidExecutable in executable.Executables)
                {
                    if(versioninidExecutable is IInstallable)
                    {
                        var unit = getExecutableUnitInList((versioninidExecutable as IInstallable).InstallableUnits, unitId);
                        if(unit != null)
                            return unit;
                    }
                    else if (versioninidExecutable is IReinstallable)
                    {
                        var unit = getExecutableUnitInList((versioninidExecutable as IReinstallable).ReinstallableUnits, unitId);
                        if (unit != null)
                            return unit;
                    }
                    else if (versioninidExecutable is IUninstallable)
                    {
                        var unit = getExecutableUnitInList((versioninidExecutable as IUninstallable).UninstallableUnits, unitId);
                        if (unit != null)
                            return unit;
                    }
                    if (versioninidExecutable is IRunnable)
                    {
                        var unit = getExecutableUnitInList((versioninidExecutable as IRunnable).RunnableUnits, unitId);
                        if (unit != null)
                            return unit;
                    }

                }
            }
            return null;
        }
        public IExecutable GetExecutableVersion(string executableName, Version version = null)
        {
            foreach (var executable in executables)
            {
                if (executable.Name == executableName)
                {
                    if (version == null)
                        return GetNewestVersionExecutable(executable);
                    foreach (var executableVersion in executable.Executables)
                    {
                        if (executableVersion.Version == version)
                            return executableVersion;
                    }
                }
            }
            return null;
        }
        public IExecutable GetExecutableVersion(Guid id, Version version = null)
        {
            foreach(var executable in executables)
            {
                if(executable.Id == id)
                {
                    foreach (var executableVersion in executable.Executables)
                    {
                        if (executableVersion.Version == version)
                            return executableVersion;
                    }
                }
            }
            return null;
        }
        private ExecutableUnit getExecutableUnitInList(IEnumerable<ExecutableUnit> units, Guid UnitId)
        {
            foreach(var unit in units)
            {
                if(unit.Id == UnitId)
                    return unit;
            }
            return null;
        }
        public IExecutable GetNewestVersionExecutable(Guid executableId)
        {
            foreach( var executable in executables)
            {
                if(executable.Id == executableId)
                {
                    return GetNewestVersionExecutable(executable);
                }
            }
            return null;
        }
        public IExecutable GetNewestVersionExecutable(ExecutableBase executable)
        {
            IExecutable temp = null;

            if (executable.Executables.Count == 0)
                return null;
            temp = executable.Executables[0];
            foreach (var executableVersion in executable.Executables)
            {

                if (temp.Version < executableVersion.Version)
                    temp = executableVersion;
            }
            return temp;
        }

    }
}
