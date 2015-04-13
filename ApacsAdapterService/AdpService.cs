using System;

using ApacsAdapter;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;

namespace ApacsAdapterService
{
    public partial class AdpService : ServiceBase
    {
        AdpLog log = new AdpLog();
        ApacsServer apacsInstance = null;
        AdpEventsLister eventLister = null;
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
        private void TaskRestart(object obj)
        {
            Break();
            log.AddLog("Stopped service (before disposing)");
            Run();
            log.AddLog("Starting service (after disposing)");
        }

        private void SetTaskRestart(byte hh, byte mm, byte ss)
        {
            TimerCallback callback = new TimerCallback(TaskRestart);
            DateTime todayTaskTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hh, mm, ss);
            DateTime tomorrowTaskTime = todayTaskTime.AddDays(1);
            Timer timer = null;
            if (DateTime.Now <= todayTaskTime)
            {
                timer = new Timer(callback, null, todayTaskTime - DateTime.Now, TimeSpan.FromDays(1));
            }
            else if (DateTime.Now <= tomorrowTaskTime)
            {
                timer = new Timer(callback, null, tomorrowTaskTime - DateTime.Now, TimeSpan.FromDays(1));
            }
        }
        internal void Run()
        {
            SetTaskRestart(3, 0, 0);
            AdpLog.OnAddLog += new EventHandler(AdpLog_OnAddLog);
            AdpCfgXml cfg = new AdpCfgXml();
            apacsInstance = new ApacsServer(cfg.apcLogin, cfg.apcPasswd);
            eventLister = new AdpEventsLister(apacsInstance, cfg);
            Thread[] thirds = new Thread[2];
            for (int i = 0; i < thirds.Length; i++)
            {
                thirds[i] = new Thread(eventLister.startEventsLister);
            }
            foreach (Thread th in thirds)
            {
                th.Start();
            }           
            eventLister.startEventsLister();
        }
        internal void Break()
        {
            if (eventLister != null)
                eventLister.stopEventsLister();
            if (apacsInstance != null)
                apacsInstance.Dispose();
        }
    }
}
