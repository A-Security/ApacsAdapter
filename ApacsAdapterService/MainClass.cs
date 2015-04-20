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
            AdpService service = new AdpService();
            if (Environment.UserInteractive)
            {
                service.Run();
                Console.ReadLine();
            }
            else
            {
                ServiceBase.Run(service);
            }
        }
    }
}
