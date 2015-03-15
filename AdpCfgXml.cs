using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Diagnostics;


namespace ApacsAdapter
{
    public class AdpCfgXml
    {
        private string path = AppDomain.CurrentDomain.BaseDirectory + "ApacsAdapter.cfg";
        public string MBhost { get ; private set; }
        public string MBuser { get; private set; }
        public string MBpassword { get; private set; }
        public string MBoutQueue { get; private set; }
        public string MBport { get; private set; }
        public string GRhost { get; private set; }
        public string GRuser { get; private set; }
        public string GRpassword { get; private set; }
        public string apcLogin { get; private set; }
        public string apcPasswd { get; private set; }

        private XmlDocument xdoc = new XmlDocument();
        public AdpCfgXml()
        {
            this.MBhost = "192.168.0.74";
            this.MBuser = "Apacs";
            this.MBpassword = "Aa1234567";
            this.MBoutQueue = "ApacsOUT";
            this.MBport = "5672";
            this.GRhost = "192.168.0.151";
            this.GRuser = "Apacs";
            this.GRpassword = "Aa1234567";
            this.apcLogin = "Inst";
            this.apcPasswd = "1945";
            if (!File.Exists(path))
            {
                createConfig(path);
            }
            readConfig(path);
        }
        private void readConfig(string path)
        {
            xdoc.Load(path);
            XmlElement configNode = xdoc.DocumentElement;
            foreach (PropertyInfo pi in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                foreach (XmlNode xn in configNode.ChildNodes)
                {
                    if (xn.Name == pi.Name)
                    {
                        pi.SetValue(this, xn.InnerText);
                        break;
                    }
                }
            }
        }
        private void createConfig(string path)
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
    }
}
