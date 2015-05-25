using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ApacsAdapter
{
    public class AdpAPCEvtsListener
    {
        private AdpLog log;
        private AdpCfgXml cfg;
        private ApcData data;
        private ApcServer Apacs;
        private AdpMBAdapter producer;
        private AdpGRAdapter grAdp;
        
        public AdpAPCEvtsListener(ApcServer Apacs, AdpCfgXml cfg) 
        {
            this.log = new AdpLog();
            this.Apacs = Apacs;
            this.cfg = cfg;
            this.data = new ApcData();
            this.producer = new AdpMBAdapter(cfg.MBhost, Convert.ToInt32(cfg.MBport), cfg.MBuser, cfg.MBpassword, cfg.MBoutQueue);
            this.grAdp = new AdpGRAdapter(cfg.GRhost, cfg.GRuser, cfg.GRpassword);
        }
        public void sendLatestEvents()
        {
            producer.connect();
            if (String.IsNullOrEmpty(cfg.lastSentEventTime))
            {
                return;
            }
            ApcPropObj[] events = Apacs.getEvents(data.getTApcEvents(), DateTime.Parse(cfg.lastSentEventTime), DateTime.Now);
            log.AddLog("Send latest " + events.Length + " events");
            foreach(ApcPropObj aop in events)
            {
                onEvent(aop);
            }
            log.AddLog("Send complete");
            
        }
        public void start()
        {
            try
            {
                producer.connect();
                Apacs.ApacsDisconnect += new ApcServer.ApacsDisconnectHandler(onDisconnect);
                Apacs.ApacsNotifyAdd += new ApcServer.ApacsNotifyAddHandler(onAddObject);
                Apacs.ApacsNotifyDelete += new ApcServer.ApacsNotifyDeleteHandler(onDelObject);
                Apacs.ApacsNotifyChange += new ApcServer.ApacsNotifyChangeHandler(onChangeObject);
                Apacs.ApacsEvent += new ApcServer.ApacsEventHandler(onEvent);
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
                Apacs.ApacsEvent -= new ApcServer.ApacsEventHandler(onEvent);
                Apacs.ApacsNotifyChange -= new ApcServer.ApacsNotifyChangeHandler(onChangeObject);
                Apacs.ApacsNotifyDelete -= new ApcServer.ApacsNotifyDeleteHandler(onDelObject);
                Apacs.ApacsNotifyAdd -= new ApcServer.ApacsNotifyAddHandler(onAddObject);
                Apacs.ApacsDisconnect -= new ApcServer.ApacsDisconnectHandler(onDisconnect);
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
            Apacs = new ApcServer(cfg.apcLogin, cfg.apcPasswd);
            start();
        }

        private void onEvent(ApcPropObj evtSet)
        {
            string fullEvtType = evtSet.getStringProperty(ApcObjProp.strEventTypeID);
            string evtType = fullEvtType.Split('_')[0];
            AdpAPCEvtObj aeObj = null;
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
                    log.AddLog("Error send event to MB " + Encoding.UTF8.GetString(msg.body));
                }
                else
                {
                    cfg.saveLastSentEventTime(aeObj.Time.ToString());
                }
            }
        }
        private void onAddObject(ApcObj newObject) 
        {
            if (newObject == null || !String.Equals(ApcObjType.TApcCardHolder, newObject.getApacsType()))
            {
                return;
            }
            grObjWorker(newObject, false);
        }
        private void onDelObject(ApcObj delObject)
        {
            if (delObject == null || !String.Equals(ApcObjType.TApcCardHolder, delObject.getApacsType()))
            {
                return;
            }
            grObjWorker(delObject, true);
        }
        private void onChangeObject(ApcObj changeObject, ApcPropObj evtSet)
        {
            if (changeObject == null || !String.Equals(ApcObjType.TApcCardHolder, changeObject.getApacsType()))
            {
                return;
            }
            grObjWorker(changeObject, false);
        }
        private void grObjWorker(ApcObj ch, bool IsDelete)
        {
            grAdp.removeCardHolder(ch.getSampleUID());
            if (IsDelete)
            {
                return;
            }
            grAdp.putCardHolder(data.getCardHolder(ch));
        }
    }
}