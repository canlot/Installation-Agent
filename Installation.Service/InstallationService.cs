using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Installation.Controller;

namespace Installation.Service
{
    public partial class InstallationService : ServiceBase
    {
        Controller.ServiceController controller;
        public InstallationService()
        {
            InitializeComponent();
            controller = new Controller.ServiceController();
        }

        protected override void OnStart(string[] args)
        {
            
            controller.Start();
        }

        protected override void OnStop()
        {
            controller.Stop();
        }

        private void process1_Exited(object sender, EventArgs e)
        {

        }
    }
}
