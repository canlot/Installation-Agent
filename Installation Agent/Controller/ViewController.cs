using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Installation.Models;
using Installation.Communication;
using System.Threading;
using System.Windows.Data;

namespace Installation_Agent.Controller
{
    public class ViewController
    {
        public ObservableCollection<Job> Apps = new ObservableCollection<Job>();
        public ObservableCollection<Job> Scripts = new ObservableCollection<Job>();
        FileTransport transport = new FileTransport();
        CancellationToken cancellationToken = new CancellationToken();
        private readonly object _lock = new object();
        public ViewController()
        {
            BindingOperations.EnableCollectionSynchronization(Apps, _lock);
            transport.OnNewJob += OnNewJob;
            transport.Listen(cancellationToken);
        }
        public void OnNewJob(Job job)
        {
           
        }
        public void SendJob(Job job)
        {
            transport.Send(job);
        }
        public void mapJob(Job source, Job target)
        {
            
        }
        public void RunJob(Job job)
        {
            
        }
    }
}
