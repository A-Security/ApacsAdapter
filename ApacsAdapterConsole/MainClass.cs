using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApacsAdapter;
using System.Diagnostics;
using System.IO;

namespace ApacsAdapterConsole
{
    class MainClass
    {
        static void Main(string[] args)
        {
            AdpGRAdapter gr = new AdpGRAdapter();
            foreach (string str in gr.getResourceFromRegistry())
            {
                Console.WriteLine(str);
            }
            Console.ReadLine();
       /*   AdpConfigXml cfg = new AdpConfigXml();
            ApacsServer apacsInstance = new ApacsServer(cfg.apcLogin, cfg.apcPasswd);
            AdpEventsLister eventLister = new AdpEventsLister(apacsInstance, cfg);
            ApcGetDate agd = new ApcGetDate();
            foreach (AdpCardHolder ach in agd.getCardHoldersFromApacs(apacsInstance))
            {
                Console.WriteLine(ach.ToString());
            }
            bool isRun = true;
            while (isRun)
            {
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.Escape:
                        isRun = false;
                        eventLister.stopEventsLister();
                        apacsInstance.Dispose();
                        break;
                    default:
                        break;
                }
            }*/
        }

    }
}
