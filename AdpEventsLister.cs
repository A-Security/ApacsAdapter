using System;
using System.Collections.Generic;
using System.Threading;

namespace ApacsAdapter
{
    public class AdpEventsLister
    {
        private AdpLog log = new AdpLog();
        private AdpCfgXml cfg;
        private ApcGetData data;
        private ApacsServer Apacs;
        private AdpMBAdapter mbAdp;
        public AdpEventsLister(ApacsServer Apacs, AdpCfgXml cfg) 
        {
            this.Apacs = Apacs;
            this.cfg = cfg;
            this.data = new ApcGetData();
            this.mbAdp = new AdpMBAdapter(cfg.MBhost, Convert.ToInt32(cfg.MBport), cfg.MBuser, cfg.MBpassword);
        }
        public void start()
        {
            try
            {
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
            AdpMBMessage msg = null;
            AdpEvtObj aeObj = null;
            switch (evtType)
            {
                case ApcObjType.TApcCardHolderAccess:
                    {
                        aeObj = data.getEvtObjFromEvtSet_CHA(evtSet, fullEvtType);
                        break;
                    }
                default:
                    {
                        aeObj = data.getEvtObjFromEvtSet(evtSet, fullEvtType);
                        break;
                    }
            }
            if (aeObj != null)
            {
                msg = new AdpMBMessage(aeObj.EventID, aeObj.ToXmlString(), aeObj.EventType);
                byte b = 0;
                while (!msg.IsBodyEmpty && !mbAdp.PublishMessage(cfg.MBoutQueue, msg))
                {
                    
                    log.AddLog("Error send event to MB("+b+"): " + msg.body);
                    Thread.Sleep(500);
                    if (b > 3) break;
                    b++;
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