using System;

using ApacsAdapter;
using System.Diagnostics;
using System.ServiceProcess;

namespace ApacsAdapterService
{
    public partial class AdpService : ServiceBase
    {
        ApacsServer apacsInstance = null;
        AdpEventsLister eventLister = null;
        public AdpService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            AdpLog.OnAddLog += new EventHandler(AdpLog_OnAddLog);
            AdpCfgXml cfg = new AdpCfgXml();
            apacsInstance = new ApacsServer(cfg.apcLogin, cfg.apcPasswd);
            eventLister = new AdpEventsLister(apacsInstance, cfg);
        }

        protected override void OnStop()
        {
            if (eventLister != null)
                eventLister.stopEventsLister();
            if (apacsInstance != null)
                apacsInstance.Dispose();
        }
        static void AdpLog_OnAddLog(object sender, EventArgs arg)
        {
            try
            {
                if (!EventLog.SourceExists("ApacsAdapterService"))
                {
                    EventLog.CreateEventSource("ApacsAdapterService", "ApacsAdapterService");
                }
                adpServiceLog.Source = "ApacsAdapterService";
                adpServiceLog.WriteEntry(sender.ToString());
            }
            catch { }
        }
    }
}
