using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseBackup_Service
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new DatabaseBackup()
            };
            ServiceBase.Run(ServicesToRun);
        }

        //Test Mode
        //static void Main()
        //{
        //    if (Environment.UserInteractive)
        //    {
        //        Console.WriteLine("Running in Console Mode");
        //        DatabaseBackup databaseBackup = new DatabaseBackup();
        //        databaseBackup.TestOnConsole();
        //    }
        //    else
        //    {
        //        ServiceBase[] ServicesToRun;
        //        ServicesToRun = new ServiceBase[]
        //        {
        //             new DatabaseBackup()
        //        };
        //        ServiceBase.Run(ServicesToRun);
        //    }

        //}
    }
}
