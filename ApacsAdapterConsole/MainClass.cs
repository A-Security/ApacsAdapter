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
            //AdpGRAdapter gr = new AdpGRAdapter(cfg.GRhost, cfg.GRuser, cfg.GRpassword);
            //if (gr.copyCHfromApacs(apacsInstance))
            //{
            //    foreach (string str in gr.getListStringCHs())
            //    {
            //        Console.WriteLine(str);
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("Error filling Governance Registry from APACS 3000 server");
            //}

            apacsInstance.ApacsEvent += new ApacsServer.ApacsEventHandler(onEvent);
            /*AdpMBAdapter mb = new AdpMBAdapter(cfg.MBhost, cfg.MBuser, cfg.MBpassword, Convert.ToInt32(cfg.MBport));
            AdpEventsLister lister = new AdpEventsLister(apacsInstance, cfg);
            bool isRun = true;
            while (isRun)
            {
            //    Console.WriteLine(mb.RetriveMessage(cfg.MBoutQueue, out isRun));
            }*/
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.Escape:
                    //isRun = false;
                    //lister.stopEventsLister();
                    apacsInstance.Dispose();
                    break;
                default:
                    break;
            }
        }
        static void onEvent(ApacsPropertyObject apo)
        {
            ApcGetDate agd = new ApcGetDate();
            AdpEvtObj_CHA ch = agd.getCHAobjFromEvtSet(apo);
            Console.WriteLine(ch.ToXmlString());
        }
    }
}
