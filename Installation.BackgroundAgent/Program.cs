using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Installation.Controller;

namespace Installation.BackgroundAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceController controller = new ServiceController();
            Console.WriteLine("------PROGRAM STARTED------");
            controller.Start();
            Console.WriteLine("------PROGRAM ENDED------");
            Console.Read();
        }
    }
}
