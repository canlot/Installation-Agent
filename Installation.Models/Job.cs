using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Installation.Models
{

    [Serializable]
    public class Job : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private ExecuteAction action;
        public ExecuteAction Action { get => action; set { action = value; OnPropertyChanged("Action"); } }

        private ExecutionState executionState;
        public ExecutionState ExecutionState { get => executionState; set { executionState = value; OnPropertyChanged("ExecutionState"); } }

        private StatusState statusState;
        public StatusState StatusState { get => statusState; set { statusState = value; OnPropertyChanged("StatusState"); } }


        private Guid executableID;
        public Guid ExecutableID { get => executableID; set { executableID = value; OnPropertyChanged("ExecutableID"); } }

        public Guid JobID { get; set; }
        public Job()
        {

        }
        public Job WithNewGuiD()
        {
            this.JobID = Guid.NewGuid();
            return this;
        }

        public static bool operator == (Job a, Job b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(b, null)) return false;
            if (ReferenceEquals(a, false)) return false;
            if (a.JobID == b.JobID) return true;
            else return false;
        }
        public static bool operator != (Job a, Job b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            if(obj is Job == false)
            {
                return false;
            }
            if (obj == null)
                return false;
            try
            {
                if (!this.GetType().Equals(obj.GetType()))
                    return false;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if ((Job)obj == this)
                return true;
            else
                return false;
        }
        public override int GetHashCode()
        {
            return this.JobID.GetHashCode();
        }
    }
}
