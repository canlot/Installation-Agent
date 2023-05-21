using Installation.Executors;
using Installation.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Installation.Models
{

    public class ExecutableBase
    {
        public List<IExecutable> Executables { get; set; }

        private string name;
        
        private Guid id;
        
        private string iconPath;
        private Dictionary<string, string> descriptions;

        private string currentDirectory;
        public string CurrentDirectory { get => currentDirectory; set => currentDirectory = value; }

        private string category;

        
        public Guid Id { get => id; set => id = value; }

        public string Name { get => name; set => name = value; }

        

        public Dictionary<string, string> Descriptions { get => descriptions; set => descriptions = value; }

        public string Description { get => getDescription(); }

        private string getDescription()
        {
            return "kjd";
        }

        public string IconPath 
        { get
            {
                if(string.IsNullOrEmpty(iconPath))
                    return iconPath;
                if (!Path.IsPathRooted(iconPath))
                    return Path.Combine(CurrentDirectory, iconPath);
                else
                    return iconPath;  
            }
            set => iconPath = value;
        }
        
        

        public string Category { get => category; set => category = value; }

        


        public ExecutableBase()
        {

        }

        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if(obj == null)
                return false;
            if (obj is ExecutableBase)
            {
                if (this.id.Equals((obj as ExecutableBase).Id)) 
                    return true;
                else 
                    return false;
            }
            else
                return false;
            
        }

        public static bool operator ==(ExecutableBase a, ExecutableBase b)
        {
            if(ReferenceEquals(a, b)) return true;
            if(ReferenceEquals(b, null)) return false;
            if(ReferenceEquals(a, false)) return false;
            if (a.Id == b.Id) return true;
            else return false;
        }
        public static bool operator !=(ExecutableBase a, ExecutableBase b)
        {
            return !(a == b);
        }
        

    }
}
