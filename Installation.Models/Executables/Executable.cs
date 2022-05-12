using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
        private string iconPath;

        private StatusState statusState;
        private bool currentlyRunning;
        private string category;

        [ExecutableSetting]
        public Guid Id { get => id; set => id = value; }
        [ExecutableSetting]
        public string Name { get => name; set => name = value.Replace("\"", ""); }
        [ExecutableSetting]
        public string Version { get => version; set => version = value.Replace("\"", ""); }
        [ExecutableSetting(Mandatory = false)]
        public string IconPath 
        { get
            {
                if(string.IsNullOrEmpty(iconPath))
                    return iconPath;
                if (!Path.IsPathRooted(iconPath))
                    return Path.Combine(ExecutableDirectory, iconPath);
                else
                    return iconPath;  
            }
            set => iconPath = value;
        }
        public bool CurrentlyRunning { get => currentlyRunning; set { currentlyRunning = value; OnPropertyChanged("CurrentlyRunning"); } }
        public StatusState StatusState { get => statusState; set { statusState = value; OnPropertyChanged("StatusState"); } }
        public string ExecutableDirectory { get => executableDirectory; set => executableDirectory = value; }
        [ExecutableSetting(Mandatory = false)]
        public string Category { get => category; set => category = value; }

        public string statusMessage;
        public string StatusMessage { get => statusMessage; set { statusMessage = value; OnPropertyChanged("StatusMessage"); } }

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
