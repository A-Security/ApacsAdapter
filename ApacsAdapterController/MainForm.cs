using ApacsAdapter;
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
    public partial class MainForm : Form
    {
        private bool resizeFlag = false;
        private AdpLog log;
        private ApcServer Apacs;
        private AdpCtrlCfg cfg;
        private ApcData apcData;
        public MainForm()
        {
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
            AdpLog.OnAddLog += new EventHandler(AdpLog_OnAddLog);
            this.log = new AdpLog();
            this.Apacs = new ApcServer(cfg.apcLogin, cfg.apcPasswd);
            this.apcData = new ApcData();
            apcSubscribe();

        }
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
        private void onDisconnect()
        {
            Invoke(new ApcServer.ApacsDisconnectHandler(onDisconnectSync), null);
        }
        private void onDisconnectSync()
        {
            apcUnsubscribe();
            Apacs.Dispose();
            Apacs = new ApcServer(cfg.apcLogin, cfg.apcPasswd);
            apcSubscribe();
        }
        private void onEvent(ApcPropObj aEvtSettings)
        {
            Invoke(new ApcServer.ApacsEventHandler(onEventSync), new object[] { aEvtSettings });
        }
        private void onEventSync(ApcPropObj evtSets)
        {
            string eventType = evtSets.getEventTypeProperty();
            string rePattern = "^(" + ApcObjType.TApcCardHolderAccess + ")(?!Will).*$";
            Regex regExp = new Regex(rePattern);
            if (!regExp.IsMatch(eventType))
            {
                return;
            }
            AdpAPCEvtObj evnt = apcData.mapAdpAPCEvtObj(evtSets);
            if (String.IsNullOrEmpty(cfg.ctrlSourceIDs.Single(x => x == evnt.SourceID)))
            {
                return;
            }
            if (resizeFlag)
            {
                msgLabel.Height /= 2;
                resizeFlag = false;
            }
            BackColor = mainPhoto.BackColor = Color.Black;
            timeLabel.BackColor = msgLabel.BackColor = Color.White;
            timeLabel.ForeColor = msgLabel.ForeColor = Color.Black;
            timeLabel.Text = DateTime.Parse(evnt.Time).ToString("HH:mm");
            msgLabel.Text = String.IsNullOrEmpty(evnt.HolderID) ? "НЕИЗВЕСТНЫЙ" : evnt.HolderName;
            ApcObj holder = evtSets.getSysAddrHolderProperty();
            mainPhoto.Image = String.IsNullOrEmpty(evnt.HolderID) ? null : Image.FromStream(new MemoryStream(holder.getMainPhoto()));
            regExp = new Regex("ErrHolder|Denied");
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

        private void AdpLog_OnAddLog(object sender, EventArgs arg)
        {
            msgLabel.Text = (sender as AdpLog).Log;
        }

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
