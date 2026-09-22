using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace DatabaseBackup_Service
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        ServiceProcessInstaller processInstaller;
        ServiceInstaller serviceInstaller;

        public ProjectInstaller()
        {
            InitializeComponent();

            processInstaller = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem
            };

            serviceInstaller = new ServiceInstaller
            {
                ServiceName = "DataBaseBackup",
                DisplayName = "DataBase Backup Service",
                Description ="A Windows Service that automatically backs up a SQL Server database at a configurable interval.",
                StartType = ServiceStartMode.Automatic,
                // Define dependencies
                ServicesDependedOn = new string[]
                {
                    "MSSQLSERVER", // SQL Server default instance (adjust for named instances)
                    "RpcSs",       // Remote Procedure Call
                    "EventLog"     // Windows Event Log
                }
            };

            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
