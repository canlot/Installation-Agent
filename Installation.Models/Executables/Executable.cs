using Installation.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models.Executables
{
    public class Executable
    {

        private string name;
        private string version;
        private Guid id;

        private StatusState statusState;
        private string category;

        public Guid Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Version { get => version; set => version = value; }
        //public InstallationState InstallationState { get => installationState; set => installationState = value; }
        public StatusState StatusState { get => statusState; set => statusState = value; }
        public string Category { get => category; set => category = value; }

        public Executable()
        {

        }

        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if(this.id.Equals(obj)) return true;
            else return false;
        }

        public static bool operator ==(Executable a, Executable b)
        {
            if(ReferenceEquals(a, b)) return true;
            if(ReferenceEquals(b, null)) return false;
            if(ReferenceEquals(a, false)) return false;
            if (a.Id == b.Id) return true;
            else return false;
        }
        public static bool operator !=(Executable a, Executable b)
        {
            return !(a == b);
        }
        

    }
}
