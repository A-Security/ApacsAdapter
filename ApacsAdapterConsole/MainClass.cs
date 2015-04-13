using ApacsAdapter;
using System;
using System.ComponentModel;
using System.Text;
using System.Threading;

namespace ApacsAdapterConsole
{
    class MainClass
    {
        static void Main(string[] args)
        {
            AdpLog.OnAddLog += new EventHandler(AdpLog_OnAddLog);
            AdpCfgXml cfg = new AdpCfgXml();
            ApacsServer apacsInstance = new ApacsServer(cfg.apcLogin, cfg.apcPasswd);
            //AdpGRAdapter gr = new AdpGRAdapter(cfg.GRhost, cfg.GRuser, cfg.GRpassword);
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

            //AdpMBAdapter mb = new AdpMBAdapter(cfg.MBhost, Convert.ToInt32(cfg.MBport), cfg.MBuser, cfg.MBpassword);
            
            AdpEventsLister lister = new AdpEventsLister(apacsInstance, cfg);
            //lister.startEventsLister();
            Thread[] thirds = new Thread[3];
            for (int i = 0; i < thirds.Length; i++)
            {
                thirds[i] = new Thread(lister.startEventsLister);
            }
            foreach (Thread th in thirds)
            {
                th.Start();
            }            
            
            //bool isRun = true;
            //while (isRun)
            //{
            //    switch (Console.ReadKey().Key)
            //    {
            //        case ConsoleKey.Escape:
            //            isRun = false;
            //            lister.stopEventsLister();
            //            apacsInstance.Dispose();
            //            break;
            //        default:
            //            break;
            //    }
            //}
            Console.ReadLine();
        }
        private static void AdpLog_OnAddLog(object sender, EventArgs arg)
        {
            Console.WriteLine(((AdpLog)sender).log);
        }

    }
}
