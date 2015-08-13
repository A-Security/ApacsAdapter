using ApacsHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApacsAdapterController
{
    // Main form class
    public partial class MainForm : Form
    {
        // true if MessageLabel resizing
        private bool resizeFlag = false;
        private AdpLog log;
        private ApcServer Apacs;
        private AdpCtrlCfg cfg;
        private ApcData apcData;
        public MainForm()
        {
            // App.config helper instance
            this.cfg = new AdpCtrlCfg();
            InitializeComponent();
            this.mainPhoto.SizeMode = cfg.ctrlMainPhotoSizeMode;
            this.msgLabel.Height = cfg.ctrlMsgLabelHeight;
            this.msgLabel.Font = new Font(this.msgLabel.Font.Name, cfg.ctrlMsgLabelFontSize);
            this.timeLabel.Height = cfg.ctrlTimeLabelHeight;
            this.timeLabel.Font = new Font(this.timeLabel.Font.Name, cfg.ctrlTimeLabelFontSize);
        }
        

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Subscribe on add new message to log event
            AdpLog.OnAddLogEventHandler += new EventHandler(AdpLog_OnAddLog);
            // Log instance
            this.log = new AdpLog();
            // APACS 3000 Server instance
            this.Apacs = new ApcServer(cfg.apcLogin, cfg.apcPasswd);
            // APACS 3000 API helper instance
            this.apcData = new ApcData();
            apcSubscribe();

        }
        // Subscribe on APACS 3000 events
        private void apcSubscribe()
        {
            try
            {
                Apacs.ApacsDisconnect += new ApcServer.ApacsDisconnectHandler(onDisconnect);
                Apacs.ApacsEvent += new ApcServer.ApacsEventHandler(onEvent);
            }
            catch (Exception e)
            {
                log.AddLog(e.ToString());
            }
        }
        // Unsubscribe on APACS 3000 events
        private void apcUnsubscribe()
        {
            try
            {
                Apacs.ApacsEvent -= new ApcServer.ApacsEventHandler(onEvent);
                Apacs.ApacsDisconnect -= new ApcServer.ApacsDisconnectHandler(onDisconnect);
            }
            catch (Exception e)
            {
                log.AddLog(e.ToString());
            }
        }
        // On APACS 3000 Disconnect hundler
        private void onDisconnect()
        {
            Invoke(new ApcServer.ApacsDisconnectHandler(onDisconnectSync), null);
        }
        // On APACS 3000 Disconnect hundler
        private void onDisconnectSync()
        {
            apcUnsubscribe();
            Apacs.Dispose();
            Apacs = new ApcServer(cfg.apcLogin, cfg.apcPasswd);
            apcSubscribe();
        }
        // On APACS 3000 access control event hundler
        private void onEvent(ApcPropObj aEvtSettings)
        {
            Invoke(new ApcServer.ApacsEventHandler(onEventSync), new object[] { aEvtSettings });
        }
        // On APACS 3000 access control event hundler
        private void onEventSync(ApcPropObj evtSets)
        {
            // Get APACS 3000 Event type
            string eventType = evtSets.getEventTypeProperty();
            // Exit if eventType not equals TApcCardHolderAccess* or equals TApcCardHolderAccess*Will*
            string rePattern = "^(" + ApcObjType.TApcCardHolderAccess + ")(?!Will).*$";
            Regex regExp = new Regex(rePattern);
            if (!regExp.IsMatch(eventType))
            {
                return;
            }
            // Get Event object
            AdpApcEvtObj evnt = apcData.mapAdpApcEvtObj(evtSets, true);
            // Exit if not specified Source alias or Source alias not in SourceAlias config list
            if (String.IsNullOrEmpty(evnt.SourceAlias) || String.IsNullOrEmpty(cfg.ctrlSourceAliases.Single(x => x == evnt.SourceAlias)))
            {
                return;
            }
            // Halving if resizeFlag == true
            if (resizeFlag)
            {
                msgLabel.Height /= 2;
                resizeFlag = false;
            }
            // Background color for MainForm and main PuctureBox
            BackColor = mainPhoto.BackColor = Color.Black;
            // Background color for Message label and Time label
            timeLabel.BackColor = msgLabel.BackColor = Color.White;
            // Foreground color for Message label and Time label
            timeLabel.ForeColor = msgLabel.ForeColor = Color.Black;
            
            // Set Time label text in HH:mm format
            timeLabel.Text = DateTime.Parse(evnt.EventTime).ToString("HH:mm");
            // Set Message label text at cardholder name or "НЕИЗВЕСТНЫЙ" if HolderID is null
            msgLabel.Text = String.IsNullOrEmpty(evnt.Parameters.HolderID) ? "НЕИЗВЕСТНЫЙ" : evnt.Parameters.HolderName;
            // Set main PictureBox Image at cardholder main photo or null if HolderID is null
            mainPhoto.Image = String.IsNullOrEmpty(evnt.Parameters.HolderID) ? null : Image.FromStream(new MemoryStream(evnt.Parameters.HolderPhoto));
            regExp = new Regex("ErrHolder|Denied");
            /* Double down height Message label, draw red border and trim first word in alarm message
             * if event type contains "ErrHolder" or "Denied"
             */
            if (resizeFlag = regExp.IsMatch(eventType))
            {
                msgLabel.Height *= 2;
                BackColor = mainPhoto.BackColor = timeLabel.BackColor = msgLabel.BackColor = Color.Red;
                timeLabel.ForeColor = msgLabel.ForeColor = Color.White;
                string[] tmpEvntDescArr = evnt.EventTypeDesc.Split(new char[]{','}, 2);
                string tmpEvntDesc = tmpEvntDescArr.Length > 1 ? tmpEvntDescArr[1].TrimStart() : tmpEvntDescArr[0];
                tmpEvntDesc = tmpEvntDesc.First().ToString().ToUpper() + tmpEvntDesc.Substring(1);
                msgLabel.Text += "\n" + tmpEvntDesc;
            }
            
        }
        // Print error message in Message label
        private void AdpLog_OnAddLog(object sender, EventArgs arg)
        {
            msgLabel.Text = (sender as AdpLog).Log;
        }
        // Exit by Escape key
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                apcUnsubscribe();
                Apacs.Dispose();
                this.Close();
            }
        }
        
    }
}
