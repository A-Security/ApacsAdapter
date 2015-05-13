using System;
using ApacsAdapter;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Collections.Generic;

namespace ApacsAdapterService
{
    public partial class AdpService : ServiceBase
    {
        private AdpLog log = new AdpLog();
        private ApacsServer apacsInstance = null;
        private AdpEvtsListener eventLister = null;

        public AdpService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            StartService();
        }

        protected override void OnStop()
        {
            StopService();
        }
        protected void AdpLog_OnAddLog(object sender, EventArgs arg)
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine(((AdpLog)sender).Log);
            }
            else
            {
                try
                {
                    if (!EventLog.SourceExists("ApacsAdapterService"))
                    {
                        EventLog.CreateEventSource("ApacsAdapterService", "ApacsAdapter");
                    }
                    adpServiceLog.Source = "ApacsAdapterService";
                    adpServiceLog.WriteEntry(((AdpLog)sender).Log);
                }
                catch { }
            }
        }
        internal void StartService()
        {
            AdpLog.OnAddLog += new EventHandler(AdpLog_OnAddLog);
            AdpCfgXml cfg = new AdpCfgXml();
            apacsInstance = new ApacsServer(cfg.apcLogin, cfg.apcPasswd);
            eventLister = new AdpEvtsListener(apacsInstance, cfg);
            eventLister.start();
        }
        internal void StopService()
        {
            if (eventLister != null)
            {
                eventLister.stop();
                eventLister = null;
            }
            if (apacsInstance != null)
            {
                apacsInstance.Dispose();
                apacsInstance = null;
            }
            AdpLog.OnAddLog -= new EventHandler(AdpLog_OnAddLog);
        }
    }
}
