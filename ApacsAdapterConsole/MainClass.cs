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
            AdpCmdCfg cfg = new AdpCmdCfg();


            AdpCHObj CardHolderMod = new AdpCHObj
            {
                CardNo = "CardNo",
                HolderCategory = "HolderCategory",
                HolderCompany = "HolderCompany",
                HolderID = "HolderID",
                HolderJobTitle = "HolderJobTitle",
                HolderName = "HolderName",
                HolderShortName = "HolderShortName",
            };
            CardHolderMod.modType = AdpCHObj.ModType.DelRq;
            Console.WriteLine(CardHolderMod.ToXmlString());
            //ApcServer apacsInstance = new ApcServer(cfg.apcLogin, cfg.apcPasswd);
            //ApcData data = new ApcData();
            //Console.WriteLine(data.getPropHierarchy(apacsInstance.getObjectBySampleUID("00000189").getCurrentSettings()));
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
            Console.ReadLine();
        }
        private static void AdpLog_OnAddLog(object sender, EventArgs arg)
        {
            Console.WriteLine(((AdpLog)sender).Log);
        }

    }
}
