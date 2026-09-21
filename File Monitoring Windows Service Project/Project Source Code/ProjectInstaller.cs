using System;
using System.ComponentModel;
using System.ServiceProcess;

namespace File_Monitoring_Windows_Service
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
                ServiceName = "FileMonitoringService",
                DisplayName = "FileMonitoringService",
                Description = @"A Windows Service that Watch and Move Files From specific
                                Folder to another Folder With Rename File to GUID Name",
                StartType = ServiceStartMode.Automatic
            };

            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
