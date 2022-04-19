using Installation.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models
{
    
    public class Executable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private string name;
        private string version;
        private Guid id;
        private string executableDirectory;

        private StatusState statusState;
        private bool currentlyRunning;
        private string category;

        public Guid Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Version { get => version; set => version = value; }
        public bool CurrentlyRunning { get => currentlyRunning; set { currentlyRunning = value; OnPropertyChanged("CurrentlyRunning"); } }
        public StatusState StatusState { get => statusState; set { statusState = value; OnPropertyChanged("StatusState"); } }
        public string ExecutableDirectory { get => executableDirectory; set => executableDirectory = value; }
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
