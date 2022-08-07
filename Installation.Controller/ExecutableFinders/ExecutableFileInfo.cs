using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Controller.ExecutableFinders
{
    public class ExecutableFileInfo
    {
        public string FilePath { get; set; }
        public string FileHash { get; set; }
        public int CycleGeneration { get; set; }
    }
}
