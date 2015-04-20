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
        private AdpEventsLister eventLister = null;
        private TimerCallback timerCallback = null;
        private Timer timer = null;

        public AdpService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Run();
        }

        protected override void OnStop()
        {
            Break();
        }
        protected void AdpLog_OnAddLog(object sender, EventArgs arg)
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine(((AdpLog)sender).log);
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
                    adpServiceLog.WriteEntry(((AdpLog)sender).log);
                }
                catch { }
            }
        }
        private void TaskRestart(object obj)
        {
            log.AddLog("Stopped service (everyday Restart Timer)");
            Break();
            log.AddLog("Starting service (everyday Restart Timer)");
            Run();
        }

        private void setTaskRestart(byte hh, byte mm, byte ss)
        {
            if (this.timer != null)
            {
                return;
            }
            this.timerCallback = new TimerCallback(TaskRestart);
            DateTime todayTaskTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hh, mm, ss);
            DateTime nextTaskTime = DateTime.Now <= todayTaskTime ? todayTaskTime : todayTaskTime.AddDays(1);
            this.timer = new Timer(timerCallback, null, nextTaskTime - DateTime.Now, TimeSpan.FromDays(1));
        }
        internal void Run()
        {
            setTaskRestart(3, 0, 0);
            AdpLog.OnAddLog += new EventHandler(AdpLog_OnAddLog);
            AdpCfgXml cfg = new AdpCfgXml();
            apacsInstance = new ApacsServer(cfg.apcLogin, cfg.apcPasswd);
            eventLister = new AdpEventsLister(apacsInstance, cfg);
            eventLister.start();
        }
        internal void Break()
        {
            if (eventLister != null)
                eventLister.stop();
            if (apacsInstance != null)
                apacsInstance.Dispose();
            AdpLog.OnAddLog -= new EventHandler(AdpLog_OnAddLog);
        }
    }
}
