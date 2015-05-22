using System;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Diagnostics;

namespace ApacsAdapter
{
    public class AdpCfgXml
    {
        private AdpLog log = new AdpLog();
        private string path = AppDomain.CurrentDomain.BaseDirectory + "ApacsAdapter.cfg";
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

        private XmlDocument xdoc = new XmlDocument();
        public AdpCfgXml()
        {
            // WSO2 Message Broker default settings
            this.MBhost = "10.28.65.224"; // PROD SERVER
            //this.MBhost = "192.168.0.74"; // TEST SERVER
            this.MBuser = "Apacs";
            this.MBpassword = "Aa1234567";
            this.MBoutQueue = "ApacsOUT";
            this.MBinQueue = "ApacsIN";
            this.MBport = "5672";
            
            // WSO2 Governancy Registry default settings
            this.GRhost = "10.28.65.228"; // PROD SERVER
            //this.GRhost = "192.168.0.151"; // TEST SERVER
            this.GRuser = "Apacs";
            this.GRpassword = "Aa1234567";

            // Apacs user\pass settings
            this.apcLogin = "Inst";
            this.apcPasswd = "1945";

            // Create config if file not exists
            if (!File.Exists(path))
            {
                createConfig(path);
            }

            readConfig(path);
        }

        // Read config file and set property value in config class uses reflection
        private void readConfig(string path)
        {
            try
            {
                xdoc.Load(path);
                XmlElement configNode = xdoc.DocumentElement;
                foreach (PropertyInfo pi in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    XmlNode xn = configNode.GetElementsByTagName(pi.Name)[0];
                    pi.SetValue(this, xn.InnerText);
                }
            }
            catch (Exception e)
            {
                log.AddLog(e.ToString());
            }
        }

        // Create config file uses default property value 
        private void createConfig(string path)
        {
            try
            {
                XmlDeclaration xdec = xdoc.CreateXmlDeclaration("1.0", String.Empty, String.Empty);
                xdoc.InsertBefore(xdec, xdoc.DocumentElement);
                XmlElement xel = xdoc.CreateElement(this.GetType().Name);
                XmlNode xn;
                foreach (PropertyInfo pi in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    xn = xdoc.CreateElement(pi.Name);
                    xn.InnerText = pi.GetValue(this).ToString();
                    xel.AppendChild(xn);
                }
                xdoc.AppendChild(xel);
                xdoc.Save(path);
            }
            catch (Exception e)
            {
                log.AddLog(e.ToString());
            }
        }
    }
}
