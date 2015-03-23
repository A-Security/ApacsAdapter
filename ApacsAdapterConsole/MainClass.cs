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
                Console.WriteLine("true");
            }
            //    foreach (string str in gr.getListStringCHs())
            //    {
            //        Console.WriteLine(str);
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("Error filling Governance Registry from APACS 3000 server");
            //}

            //apacsInstance.ApacsEvent += new ApacsServer.ApacsEventHandler(onEvent);
            /*AdpMBAdapter mb = new AdpMBAdapter(cfg.MBhost, cfg.MBuser, cfg.MBpassword, Convert.ToInt32(cfg.MBport));
            AdpEventsLister lister = new AdpEventsLister(apacsInstance, cfg);*/
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
        static void onEvent(ApacsPropertyObject apo)
        {
            ApcGetData agd = new ApcGetData();
            AdpEvtObj_CHA ch = agd.getEvtObjFromEvtSet_CHA(apo);
            Console.WriteLine(ch.ToXmlString());
        }
    }
}
