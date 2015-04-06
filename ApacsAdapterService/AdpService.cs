using System;

using ApacsAdapter;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;

namespace ApacsAdapterService
{
    public partial class AdpService : ServiceBase
    {
        ApacsServer apacsInstance = null;
        AdpEventsLister eventLister = null;
        public AdpService()
        {
            InitializeComponent();
            RestartServiceSetTask(05, 00, 00);
        }

        protected override void OnStart(string[] args)
        {
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
        }

        protected override void OnStop()
        {
            if (eventLister != null)
                eventLister.stopEventsLister();
            if (apacsInstance != null)
                apacsInstance.Dispose();
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
        private void RestartServiceTimerEvent(object obj)
        {
            //ExitCode = int.MinValue;
            //Stop();
            Dispose(true);
            OnStop();
            OnStart(null);
            //ServiceController svc = new ServiceController(ServiceName);
            //if (svc.Status == ServiceControllerStatus.Running)
            //    svc.Stop();
            //svc.WaitForStatus(ServiceControllerStatus.Stopped, new System.TimeSpan(0, 0, 20));
            //svc.Start();
            //svc.WaitForStatus(ServiceControllerStatus.Running, new System.TimeSpan(0, 0, 20));
            //svc.Dispose();
        }

        private void RestartServiceSetTask(byte hh, byte mm, byte ss)
        {
            TimerCallback callback = new TimerCallback(RestartServiceTimerEvent);
            DateTime todayTaskTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hh, mm, ss);
            DateTime tomorrowTaskTime = todayTaskTime.AddDays(1);
            Timer timer;
            if (DateTime.Now < todayTaskTime)
            {
                timer = new Timer(callback, null, todayTaskTime - DateTime.Now, TimeSpan.FromDays(1));
            }
            else if (DateTime.Now < tomorrowTaskTime)
            {
                timer = new Timer(callback, null, tomorrowTaskTime - DateTime.Now, TimeSpan.FromDays(1));
            }
        }

    }
}
