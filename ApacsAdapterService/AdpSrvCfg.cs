using System;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Configuration;

namespace ApacsAdapterService
{
    public class AdpSrvCfg
    {
        private Configuration cfg;
        public string MbHost { get ; private set; }
        public string MbUser { get; private set; }
        public string MbPass { get; private set; }
        public string MbOutQueue { get; private set; }
        public string MbInQueue { get; private set; }
        public string MbPort { get; private set; }
        public string GrHost { get; private set; }
        public string GrUser { get; private set; }
        public string GrPass { get; private set; }
        public string ApcUser { get; private set; }
        public string ApcPasswd { get; private set; }
        public string LastSentEventTime { get; private set; }

        public AdpSrvCfg()
        {
            cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            // Create config if not exists
            if (cfg.AppSettings.Settings.Count == 0)
            {
                createConfig();
            }
            readConfig();
        }

        // Read app.config and set property value in config class 
        private void readConfig()
        {
            this.MbHost = ConfigurationManager.AppSettings["MBhost"];
            this.MbUser = ConfigurationManager.AppSettings["MBuser"];
            this.MbPass = ConfigurationManager.AppSettings["MBpassword"];
            this.MbOutQueue = ConfigurationManager.AppSettings["MBoutQueue"];
            this.MbInQueue = ConfigurationManager.AppSettings["MBinQueue"];
            this.MbPort = ConfigurationManager.AppSettings["MBport"];

            this.GrHost = ConfigurationManager.AppSettings["GRhost"];
            this.GrUser = ConfigurationManager.AppSettings["GRuser"];
            this.GrPass = ConfigurationManager.AppSettings["GRpassword"];

            this.ApcUser = ConfigurationManager.AppSettings["apcLogin"];
            this.ApcPasswd = ConfigurationManager.AppSettings["apcPasswd"];
            this.LastSentEventTime = ConfigurationManager.AppSettings["lastSentEventTime"];
        }

        // Create config file uses default property value 
        private void createConfig()
        {
            // WSO2 Message Broker default settings
            //"10.28.65.224"; // PROD SERVER
            cfg.AppSettings.Settings.Add("MBhost", "192.168.0.74");
            cfg.AppSettings.Settings.Add("MBuser", "Apacs");
            cfg.AppSettings.Settings.Add("MBpassword", "Aa1234567");
            cfg.AppSettings.Settings.Add("MBoutQueue", "AcsOUT");
            cfg.AppSettings.Settings.Add("MBinQueue", "AcsIN");
            cfg.AppSettings.Settings.Add("MBport", "5672");
            
            // WSO2 Governancy Registry default settings
            //"10.28.65.228"; // PROD SERVER
            cfg.AppSettings.Settings.Add("GRhost", "192.168.0.151");
            cfg.AppSettings.Settings.Add("GRuser", "Apacs");
            cfg.AppSettings.Settings.Add("GRpassword", "Aa1234567");
            
            // Apacs user\pass settings
            cfg.AppSettings.Settings.Add("apcLogin", "Inst");
            cfg.AppSettings.Settings.Add("apcPasswd", "1945");
            
            // Default last send event time - yesterday
            cfg.AppSettings.Settings.Add("lastSentEventTime", DateTime.Now.AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:ss.fff"));

            cfg.Save();
        }
        public void SetLastSentEventTime(string eventTime)
        {
            cfg.AppSettings.Settings["lastSentEventTime"].Value = this.LastSentEventTime = eventTime;
            cfg.Save(ConfigurationSaveMode.Modified);
        }
    }
}
