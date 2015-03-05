using ApacsAdapter;
using System;

namespace ApacsAdapterConsole
{
    class MainClass
    {
        static void Main(string[] args)
        {
            AdpCfgXml cfg = new AdpCfgXml();
            ApacsServer apacsInstance = new ApacsServer(cfg.apcLogin, cfg.apcPasswd);
            AdpGRAdapter gr = new AdpGRAdapter(cfg.GRhost, cfg.GRuser, cfg.GRpassword);
            if (gr.copyCHfromApacs(apacsInstance))
            {
                foreach (string str in gr.getListStringCHs())
                {
                    Console.WriteLine(str);
                }
            }
            else
            {
                Console.WriteLine("Error filling Governance Registry from APACS 3000 server");
            }
            //Console.ReadLine();
            
            //AdpEventsLister eventLister = new AdpEventsLister(apacsInstance, cfg);
            //ApcGetDate agd = new ApcGetDate();

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
