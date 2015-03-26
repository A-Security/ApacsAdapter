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
            //Console.WriteLine(gr.copyCHfromApacs(apacsInstance).ToString());
            
            //    foreach (string str in gr.getListStringCHs())
            //    {
            //        Console.WriteLine(str);
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("Error filling Governance Registry from APACS 3000 server");
            //}

            AdpMBAdapter mb = new AdpMBAdapter(cfg.MBhost, Convert.ToInt32(cfg.MBport), cfg.MBuser, cfg.MBpassword);
            AdpEventsLister lister = new AdpEventsLister(apacsInstance, cfg);
            bool isRun = true;
            while (isRun)
            {
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.Escape:
                        isRun = false;
                        //lister.stopEventsLister();
                        //apacsInstance.Dispose();
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
