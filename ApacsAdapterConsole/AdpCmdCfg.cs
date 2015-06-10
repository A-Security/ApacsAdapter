using System;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Configuration;

namespace ApacsAdapterConsole
{
    public class AdpCmdCfg
    {
        private Configuration cfg;
        public string MBhost { get ; private set; }
        public string MBuser { get; private set; }
        public string MBpassword { get; private set; }
        public string MBoutQueue { get; private set; }
        public string MBinQueue { get; private set; }
        public string MBport { get; private set; }
        public string GRhost { get; private set; }
        public string GRuser { get; private set; }
        public string GRpassword { get; private set; }
        public string apcLogin { get; private set; }
        public string apcPasswd { get; private set; }
        public string lastSentEventTime { get; private set; }

        public AdpCmdCfg()
        {
            cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            // Create config if not exists
            if (cfg.AppSettings.Settings.Count == 0)
            {
                createConfig();
            }
            readConfig();
        }

        // Read config file and set property value in config class uses reflection
        private void readConfig()
        {
            this.MBhost = ConfigurationManager.AppSettings["MBhost"];
            this.MBuser = ConfigurationManager.AppSettings["MBuser"];
            this.MBpassword = ConfigurationManager.AppSettings["MBpassword"];
            this.MBoutQueue = ConfigurationManager.AppSettings["MBoutQueue"];
            this.MBinQueue = ConfigurationManager.AppSettings["MBinQueue"];
            this.MBport = ConfigurationManager.AppSettings["MBport"];

            this.GRhost = ConfigurationManager.AppSettings["GRhost"];
            this.GRuser = ConfigurationManager.AppSettings["GRuser"];
            this.GRpassword = ConfigurationManager.AppSettings["GRpassword"];

            this.apcLogin = ConfigurationManager.AppSettings["apcLogin"];
            this.apcPasswd = ConfigurationManager.AppSettings["apcPasswd"];
            this.lastSentEventTime = ConfigurationManager.AppSettings["lastSentEventTime"];
        }

        // Create config file uses default property value 
        private void createConfig()
        {
            // WSO2 Message Broker default settings
            //"10.28.65.224"; // PROD SERVER
            cfg.AppSettings.Settings.Add("MBhost", "192.168.0.74");
            cfg.AppSettings.Settings.Add("MBuser", "Apacs");
            cfg.AppSettings.Settings.Add("MBpassword", "Aa1234567");
            cfg.AppSettings.Settings.Add("MBoutQueue", "ApacsOUT");
            cfg.AppSettings.Settings.Add("MBinQueue", "ApacsIN");
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
            cfg.AppSettings.Settings.Add("lastSentEventTime", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss.fff"));

            cfg.Save();
        }
        public void setLastSentEventTime(string eventTime)
        {
            cfg.AppSettings.Settings["lastSentEventTime"].Value = this.lastSentEventTime = eventTime;
            cfg.Save(ConfigurationSaveMode.Modified);
        }
    }
}
