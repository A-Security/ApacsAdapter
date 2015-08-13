using System;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Configuration;
using System.Windows.Forms;

namespace ApacsAdapterController
{
    // App.config helper class
    public class AdpCtrlCfg
    {
        // App.config instance
        private Configuration cfg;
        // APACS 3000 Login
        public string apcLogin { get; private set; }
        // APACS 3000 Password
        public string apcPasswd { get; private set; }
        // list of APACS 3000 alias source device
        public string[] ctrlSourceAliases { get; private set; }
        // Message Label Height on MainForm
        public int ctrlMsgLabelHeight { get; private set; }
        // Message Label Font Size on MainForm
        public float ctrlMsgLabelFontSize { get; private set; }
        // Time Label Height on MainForm
        public int ctrlTimeLabelHeight { get; private set; }
        // Time Label Font Size on MainForm
        public float ctrlTimeLabelFontSize { get; private set; }
        /* Main PictureBox size mode:
         * "Normal"         - PictureBoxSizeMode.Normal;
         * "StretchImage"   - PictureBoxSizeMode.StretchImage;
         * "AutoSize"       - PictureBoxSizeMode.AutoSize;
         * "CenterImage"    - PictureBoxSizeMode.CenterImage;
         * "Zoom"           - PictureBoxSizeMode.Zoom;
         * else == default  - PictureBoxSizeMode.StretchImage;
         */
        public PictureBoxSizeMode ctrlMainPhotoSizeMode { get; private set; }

        public AdpCtrlCfg()
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
            this.apcLogin = ConfigurationManager.AppSettings["apcLogin"];
            this.apcPasswd = ConfigurationManager.AppSettings["apcPasswd"];
            
            string tmpSrcAlss = ConfigurationManager.AppSettings["ctrlSourceAliases"];
            this.ctrlSourceAliases = String.IsNullOrEmpty(tmpSrcAlss) ? new string[] { String.Empty } : tmpSrcAlss.Split(',');

            string tmpMsgLabelFontSize = ConfigurationManager.AppSettings["ctrlMsgLabelFontSize"];
            this.ctrlMsgLabelFontSize = float.Parse(tmpMsgLabelFontSize);
            
            string tmpMsgLabelHeight = ConfigurationManager.AppSettings["ctrlMsgLabelHeight"];
            this.ctrlMsgLabelHeight = int.Parse(tmpMsgLabelHeight);
            
            string tmpTimeLabelFontSize = ConfigurationManager.AppSettings["ctrlTimeLabelFontSize"];
            this.ctrlTimeLabelFontSize = float.Parse(tmpTimeLabelFontSize);
            
            string tmpTimeLabelHeight = ConfigurationManager.AppSettings["ctrlTimeLabelHeight"];
            this.ctrlTimeLabelHeight = int.Parse(tmpTimeLabelHeight);
            
            string tmpSizeMode = ConfigurationManager.AppSettings["ctrlMainPhotoSizeMode"];
            this.ctrlMainPhotoSizeMode = getSizeModeFromString(tmpSizeMode);
        }

        // Create config file uses default property value 
        private void createConfig()
        {
            // Apacs user\pass settings
            cfg.AppSettings.Settings.Add("apcLogin", "Inst");
            cfg.AppSettings.Settings.Add("apcPasswd", "1945");

            // Default set APACS Demo SourceID
            cfg.AppSettings.Settings.Add("ctrlSourceAliases", "OfficeEnter,OfficeExit,SecurityEnter1,SecurityEnter2");

            // Default set font size, label height and photo size mode
            cfg.AppSettings.Settings.Add("ctrlMsgLabelFontSize", "56");
            cfg.AppSettings.Settings.Add("ctrlMsgLabelHeight", "160");
            cfg.AppSettings.Settings.Add("ctrlTimeLabelFontSize", "62");
            cfg.AppSettings.Settings.Add("ctrlTimeLabelHeight", "115");
            
            /* 
             * "Normal"         - PictureBoxSizeMode.Normal;
             * "StretchImage"   - PictureBoxSizeMode.StretchImage;
             * "AutoSize"       - PictureBoxSizeMode.AutoSize;
             * "CenterImage"    - PictureBoxSizeMode.CenterImage;
             * "Zoom"           - PictureBoxSizeMode.Zoom;
             * default          - PictureBoxSizeMode.StretchImage;
             */
            cfg.AppSettings.Settings.Add("ctrlMainPhotoSizeMode", "StretchImage");

            cfg.Save();
        }
        private PictureBoxSizeMode getSizeModeFromString(string SizeMode)
        {
            PictureBoxSizeMode result;
            switch (SizeMode)
            {
                case "Normal":
                    {
                        result = PictureBoxSizeMode.Normal;
                        break;
                    }
                case "StretchImage":
                    {
                        result = PictureBoxSizeMode.StretchImage;
                        break;
                    }
                case "AutoSize":
                    {
                        result = PictureBoxSizeMode.AutoSize;
                        break;
                    }
                case "CenterImage":
                    {
                        result = PictureBoxSizeMode.CenterImage;
                        break;
                    }
                case "Zoom":
                    {
                        result = PictureBoxSizeMode.Zoom;
                        break;
                    }
                default:
                    {
                        result = PictureBoxSizeMode.StretchImage;
                        break;
                    }
            }
            return result;
        }
    }
}
