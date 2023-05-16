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
        private ConcurrentBag<Executable> executables = new ConcurrentBag<Executable>();

        public void AddExecutable(Executable executable)
        {
            executables.Add(executable);
        }
        public Executable GetExecutable(Guid id)
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
        public IExecutable GetExecutable(string executableName, Version version = null)
        {

        }
        public IExecutable GetExecutable(Guid id) 
        {

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
        public ExecutableUnit GetNewestVersionExecutable(Guid executableId, Version version)
        {
            throw new NotImplementedException();
        }

    }
}
