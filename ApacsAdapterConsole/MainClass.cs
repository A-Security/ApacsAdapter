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
            ApcServer apacsInstance = new ApcServer(cfg.apcLogin, cfg.apcPasswd);
            ApcData data = new ApcData();
            ApcPropObj[] events = apacsInstance.getEvents(data.getTApcEvents(), DateTime.Now.AddMinutes(-5), DateTime.Now);
            AdpAPCEvtsListener lister = new AdpAPCEvtsListener(apacsInstance, cfg);
            lister.sendLatestEvents();
            //AdpMBMsgsListener lister = new AdpMBMsgsListener(apacsInstance, cfg);
            
            /*lister.start();
            bool isRun = true;
            while (isRun)
            {
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.Escape:
                        isRun = false;
                        lister.stop();
                        apacsInstance.Dispose();
                        break;
                    default:
                        break;
                }
            }
            */
        }
        private static void AdpLog_OnAddLog(object sender, EventArgs arg)
        {
            Console.WriteLine(((AdpLog)sender).Log);
        }

    }
}
