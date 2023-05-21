using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models.Commands.Internal
{
    public class CommandGetExecutableUnit : Command
    {
        private Guid executableUnitId;

        public Guid ExecutableUnitId
        {
            get { return executableUnitId; }
            set { executableUnitId = value; }
        }

        public Version ExecutableVersion { get; set; }

        public Guid ExecutableId { get; set; }
    }
}
