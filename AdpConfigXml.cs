﻿using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Diagnostics;


namespace ApacsAdapter
{
    public class AdpConfigXml
    {
        private string path = AppDomain.CurrentDomain.BaseDirectory + "ApacsAdapter.cfg";
        public readonly string MBhost = "192.168.0.74";
        public readonly string MBuser = "Apacs";
        public readonly string MBpassword = "Aa1234567";
        public readonly string MBqueue = "ApacsOUT";
        public readonly string MBport = "5672";
        public readonly string apcLogin = "inst";
        public readonly string apcPasswd = "Aa1234567";

        private XmlDocument xdoc = new XmlDocument();
        public AdpConfigXml()
        {
            if (!File.Exists(path))
            {
                createConfig(this);
            }
            xdoc.Load(path);
            XmlElement configNode = xdoc.DocumentElement;
            foreach (FieldInfo fi in this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                foreach (XmlNode xn in configNode.ChildNodes)
                {
                    if (xn.Name == fi.Name)
                    {
                        fi.SetValue(this, xn.InnerText);
                        break;
                    }
                }
            }
        }

        private void createConfig(AdpConfigXml cfg)
        {
            XmlDeclaration xdec = xdoc.CreateXmlDeclaration("1.0", String.Empty, String.Empty);
            xdoc.InsertBefore(xdec, xdoc.DocumentElement);
            XmlElement xel = xdoc.CreateElement(cfg.GetType().Name);
            XmlNode xn;
            foreach (FieldInfo fi in cfg.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                xn = xdoc.CreateElement(fi.Name);
                xn.InnerText = fi.GetValue(this).ToString();
                xel.AppendChild(xn);
            }
            xdoc.AppendChild(xel);
            xdoc.Save(path);
        }
    }
}
