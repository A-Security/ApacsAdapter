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
        private TimerCallback timerCallback = null;
        private Timer timer = null;

        public AdpService()
        {
            InitializeComponent();
            AdpLog.OnAddLog += new EventHandler(AdpLog_OnAddLog);
            setRestartServiceTask(3, 0, 0);
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
                        EventLog.CreateEventSource("ApacsAdapterService", "ApacsAdapterService");
                    }
                    adpServiceLog.Source = "ApacsAdapterService";
                    adpServiceLog.WriteEntry(((AdpLog)sender).Log);
                }
                catch { }
            }
        }
        private void RestartService(object obj)
        {
            log.AddLog("Run everyday restart task");
            StopService();
            StartService();
        }

        private void setRestartServiceTask(byte hh, byte mm, byte ss)
        {
            this.timerCallback = new TimerCallback(RestartService);
            DateTime todayTaskTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hh, mm, ss);
            DateTime nextTaskTime = DateTime.Now <= todayTaskTime ? todayTaskTime : todayTaskTime.AddDays(1);
            this.timer = new Timer(timerCallback, null, nextTaskTime - DateTime.Now, TimeSpan.FromDays(1));
        }
        internal void StartService()
        {
            
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
        }
    }
}
