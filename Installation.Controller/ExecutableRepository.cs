using Installation.Models;
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
        public ExecutableUnit GetExecutableUnit(Guid executableId, Guid unitId, string version = "")
        {
            foreach (var executable in executables)
            {
                foreach(var versioninidExecutable in executable.Executables)
                {

                }
            }
        }

    }
}
