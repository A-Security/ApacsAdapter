using System;
using System.Collections.Generic;
using System.Threading;

namespace ApacsAdapter
{
    public class AdpEvtsListener
    {
        private AdpLog log;
        private AdpCfgXml cfg;
        private ApcData data;
        private ApacsServer Apacs;
        private AdpMBAdapter mbAdp;
        
        public AdpEvtsListener(ApacsServer Apacs, AdpCfgXml cfg) 
        {
            this.log = new AdpLog();
            this.Apacs = Apacs;
            this.cfg = cfg;
            this.data = new ApcData();
            this.mbAdp = new AdpMBAdapter(cfg.MBhost, Convert.ToInt32(cfg.MBport), cfg.MBuser, cfg.MBpassword, cfg.MBoutQueue);
        }
        public void start()
        {
            try
            {
                mbAdp.connect();
                Apacs.ApacsDisconnect += new ApacsServer.ApacsDisconnectHandler(onDisconnect);
                Apacs.ApacsNotifyAdd += new ApacsServer.ApacsNotifyAddHandler(onAddObject);
                Apacs.ApacsNotifyDelete += new ApacsServer.ApacsNotifyDeleteHandler(onDelObject);
                Apacs.ApacsNotifyChange += new ApacsServer.ApacsNotifyChangeHandler(onChangeObject);
                Apacs.ApacsEvent += new ApacsServer.ApacsEventHandler(onEvent);
                log.AddLog("Events Lister Started");
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
                mbAdp.disconnect();
                log.AddLog("Events Lister Stopped");
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
                AdpMBMessage msg = new AdpMBMessage(aeObj.EventID, aeObj.ToXmlString(), aeObj.EventType);
                if (!mbAdp.PublishMessage(msg))
                {
                    log.AddLog("Error send event to MB " + msg.body);
                }
            }
        }
        private void onAddObject(ApacsObject newObject) 
        {
            //newObject.getSampleUID();
        }
        private void onDelObject(ApacsObject delObject)
        {
            //delObject.getSampleUID();
        }
        private void onChangeObject(ApacsObject changeObject, ApacsPropertyObject evtSet)
        {
            //changeObject.getSampleUID();
        }
    }
}