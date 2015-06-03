using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;

namespace ApacsAdapterService
{
    [RunInstaller(true)]
    public partial class AdpServiceInstaller : System.Configuration.Install.Installer
    {
        public AdpServiceInstaller()
        {
            InitializeComponent();
        }

        private void serviceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            int exitCode;
            using (Process proc = new Process())
            {
                ProcessStartInfo startInfo = proc.StartInfo;
                startInfo.FileName = "sc";
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                // tell Windows that the service should restart if it fails
                startInfo.Arguments = string.Format("failure {0} reset= 30 actions= restart/5000", serviceInstaller.ServiceName);
                proc.Start();
                proc.WaitForExit();
                exitCode = proc.ExitCode;
            }
        }
    }
}
