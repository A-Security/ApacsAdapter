using System;
using ApacsHelper;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Collections.Generic;

namespace ApacsAdapterService
{
    public partial class AdpService : ServiceBase
    {
        private AdpLog log = new AdpLog();
        private ApcServer apacsInstance = null;
        private AdpApcEvtsListener eventLister = null;

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
            AdpLog adpLog = sender as AdpLog;
            if (Environment.UserInteractive)
            {
                Console.WriteLine(adpLog.Log);
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
                    adpServiceLog.WriteEntry(adpLog.Log);
                }
                catch (Exception e)
                {
                    log.AddLog(e.ToString());
                }
            }
        }
        internal void StartService()
        {
            AdpLog.OnAddLogEventHandler += new EventHandler(AdpLog_OnAddLog);
            AdpSrvCfg cfg = new AdpSrvCfg();
            apacsInstance = new ApcServer(cfg.ApcUser, cfg.ApcPasswd);
            eventLister = new AdpApcEvtsListener(apacsInstance, cfg);
            eventLister.Start();
        }
        internal void StopService()
        {
            if (eventLister != null)
            {
                eventLister.Stop();
                eventLister = null;
            }
            if (apacsInstance != null)
            {
                apacsInstance.Dispose();
                apacsInstance = null;
            }
            AdpLog.OnAddLogEventHandler -= new EventHandler(AdpLog_OnAddLog);
        }
    }
}
