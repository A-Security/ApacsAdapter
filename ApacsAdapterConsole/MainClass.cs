using ApacsAdapter;
using System;

namespace ApacsAdapterConsole
{
    class MainClass
    {
        static void Main(string[] args)
        {
            AdpConfigXml cfg = new AdpConfigXml();
            AdpGRAdapter gr = new AdpGRAdapter(cfg);
            gr.fillGRfromApacs();
            foreach (string str in gr.getResourceFromRegistry())
            {
                Console.WriteLine(str);
            }
            Console.ReadLine();
            ApacsServer apacsInstance = new ApacsServer(cfg.apcLogin, cfg.apcPasswd);
            //AdpEventsLister eventLister = new AdpEventsLister(apacsInstance, cfg);
            //ApcGetDate agd = new ApcGetDate();
            //foreach (AdpCardHolder ach in agd.getCardHoldersFromApacs(apacsInstance))
            //{
            //    Console.WriteLine(ach.ToString());
            //}
            bool isRun = true;
            while (isRun)
            {
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.Escape:
                        isRun = false;
                        //eventLister.stopEventsLister();
                        //apacsInstance.Dispose();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
