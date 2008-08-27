using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace Ilsrep.PollApplication.PollServer
{
    [RunInstaller( true )]
    public partial class PollServerInstaller : Installer
    {
        public PollServerInstaller()
        {
            InitializeComponent();

            ServiceProcessInstaller serviceProcessInstaller = 
                               new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            //# Service Account Information

            serviceProcessInstaller.Account = ServiceAccount.NetworkService;

            //# Service Information

            serviceInstaller.DisplayName = "ILS PollServer";
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            //# This must be identical to the WindowsService.ServiceBase name

            //# set in the constructor of WindowsService.cs

            serviceInstaller.ServiceName = "PollServer";

            this.Installers.Add( serviceProcessInstaller );
            this.Installers.Add( serviceInstaller );
        }
    }
}
