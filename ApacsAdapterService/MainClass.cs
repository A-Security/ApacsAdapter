using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ApacsAdapterService
{
    static class MainClass
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            if (Environment.UserInteractive)
            {
                using (AdpService service = new AdpService())
                {
                    service.StartService();
                }
            }
            else
            {
                ServiceBase.Run(new AdpService());
            }
        }
    }
}
