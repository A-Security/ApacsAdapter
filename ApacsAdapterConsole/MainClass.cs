using ApacsAdapter;
using System;
using System.Threading;

namespace ApacsAdapterConsole
{
    class MainClass
    {

        private void RestartServiceTimerEvent(object obj)
        {
            Console.WriteLine("Hi Its Time");
        }

        private void RestartServiceSetTask(byte hh, byte mm, byte ss)
        {
            TimerCallback callback = new TimerCallback(RestartServiceTimerEvent);

            //first occurrence at
            DateTime todayTimer = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hh, mm, ss);
            DateTime tomorrowTimer = todayTimer.AddDays(1);
            Timer timer;
            if (DateTime.Now < todayTimer)
            {
                timer = new Timer(callback, null, todayTimer - DateTime.Now, TimeSpan.FromDays(1));
            }
            else if (DateTime.Now < tomorrowTimer)
            {
                timer = new Timer(callback, null, tomorrowTimer - DateTime.Now, TimeSpan.FromDays(1));
            }
        }
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
