using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ApacsAdapter
{
    public class AdpEvtsListener
    {
        private AdpLog log;
        private AdpCfgXml cfg;
        private ApcData data;
        private ApacsServer Apacs;
        private AdpMBAdapter producer;
        private AdpGRAdapter grAdp;
        
        public AdpEvtsListener(ApacsServer Apacs, AdpCfgXml cfg) 
        {
            this.log = new AdpLog();
            this.Apacs = Apacs;
            this.cfg = cfg;
            this.data = new ApcData();
            this.producer = new AdpMBAdapter(cfg.MBhost, Convert.ToInt32(cfg.MBport), cfg.MBuser, cfg.MBpassword, cfg.MBoutQueue);
            this.grAdp = new AdpGRAdapter(cfg.GRhost, cfg.GRuser, cfg.GRpassword);
        }
        public void start()
        {
            try
            {
                producer.connect();
                Apacs.ApacsDisconnect += new ApacsServer.ApacsDisconnectHandler(onDisconnect);
                Apacs.ApacsNotifyAdd += new ApacsServer.ApacsNotifyAddHandler(onAddObject);
                Apacs.ApacsNotifyDelete += new ApacsServer.ApacsNotifyDeleteHandler(onDelObject);
                Apacs.ApacsNotifyChange += new ApacsServer.ApacsNotifyChangeHandler(onChangeObject);
                Apacs.ApacsEvent += new ApacsServer.ApacsEventHandler(onEvent);
                log.AddLog("Apacs events listener started");
            }
            catch (Exception e) 
            {
                log.AddLog(e.ToString());
            }
        }
        
        public void stop()
        {
            try
            {
                Apacs.ApacsEvent -= new ApacsServer.ApacsEventHandler(onEvent);
                Apacs.ApacsNotifyChange -= new ApacsServer.ApacsNotifyChangeHandler(onChangeObject);
                Apacs.ApacsNotifyDelete -= new ApacsServer.ApacsNotifyDeleteHandler(onDelObject);
                Apacs.ApacsNotifyAdd -= new ApacsServer.ApacsNotifyAddHandler(onAddObject);
                Apacs.ApacsDisconnect -= new ApacsServer.ApacsDisconnectHandler(onDisconnect);
                producer.disconnect();
                log.AddLog("Apacs events listener stopped");
            }
            catch (Exception e)
            {
                log.AddLog(e.ToString());
            }
        }
        private void onDisconnect()
        {
            log.AddLog("APACS SERVER DISCONNECTED!");
            stop();
            Apacs.Dispose();
            Apacs = new ApacsServer(cfg.apcLogin, cfg.apcPasswd);
            start();
        }

        private void onEvent(ApacsPropertyObject evtSet)
        {
            string fullEvtType = evtSet.getStringProperty(ApcObjProp.strEventTypeID);
            string evtType = fullEvtType.Split('_')[0];
            AdpEvtObj aeObj = null;
            switch (evtType)
            {
                case ApcObjType.TApcCardHolderAccess:
                    {
                        aeObj = data.getEvtObj_CHA(evtSet, fullEvtType);
                        break;
                    }
                default:
                    {
                        aeObj = data.getEvtObj(evtSet, fullEvtType);
                        break;
                    }
            }
            if (aeObj != null)
            {
                byte[] msgBody = Encoding.UTF8.GetBytes(aeObj.ToXmlString());
                AdpMBMsgObj msg = new AdpMBMsgObj(aeObj.EventID, msgBody, aeObj.EventType);
                if (!producer.PublishMessage(msg))
                {
                    log.AddLog("Error send event to MB " + msg.body);
                }
            }
        }
        private void onAddObject(ApacsObject newObject) 
        {
            grObjWorker(newObject, false);
        }
        private void onDelObject(ApacsObject delObject)
        {
            grObjWorker(delObject, true);
        }
        private void onChangeObject(ApacsObject changeObject, ApacsPropertyObject evtSet)
        {
            grObjWorker(changeObject, false);
        }
        private void grObjWorker(ApacsObject ch, bool IsDelete)
        {
            if (ch == null || !String.Equals(ApcObjType.TApcCardHolder, ch.getApacsType()))
            {
                return;
            }
            grAdp.removeCardHolder(ch.getSampleUID(), IsDelete);
            if (IsDelete)
            {
                return;
            }
            grAdp.putCardHolder(data.getCardHolder(ch));
        }
    }
}