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
        private DateTime lastSentEventTime;
        private delegate void SendLatestEventsDelegate(DateTime fromDT, DateTime toDT);
        
        public AdpAPCEvtsListener(ApcServer Apacs, AdpCfgXml cfg) 
        {
            this.log = new AdpLog();
            this.Apacs = Apacs;
            this.cfg = cfg;
            this.lastSentEventTime = DateTime.Parse(cfg.lastSentEventTime);
            this.data = new ApcData();
            this.producer = new AdpMBAdapter(cfg.MBhost, Convert.ToInt32(cfg.MBport), cfg.MBuser, cfg.MBpassword, cfg.MBoutQueue);
            this.grAdp = new AdpGRAdapter(cfg.GRhost, cfg.GRuser, cfg.GRpassword);
        }
        private void sendLatestEvents(DateTime fromDT, DateTime toDT)
        {
            ApcPropObj[] events = Apacs.getEvents(data.getTApcEvents(), fromDT, toDT);
            log.AddLog("Send latest " + events.Length + " events");
            foreach (ApcPropObj aop in events)
            {
                onEvent(aop);
            }
            log.AddLog("Send complete");
        }
        private void sendLatestEvents()
        {
            SendLatestEventsDelegate sender = new SendLatestEventsDelegate(sendLatestEvents);
            sender.BeginInvoke(lastSentEventTime, DateTime.Now, null, null);
        }
        public void start()
        {
            try
            {
                producer.connect();
                sendLatestEvents();
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
            AdpAPCEvtObj aeObj = data.getEvtObj(evtSet);
            byte[] msgBody = Encoding.UTF8.GetBytes(aeObj.ToXmlString());
            AdpMBMsgObj msg = new AdpMBMsgObj(aeObj.EventID, msgBody, aeObj.EventType);
            if (!producer.PublishMessage(msg))
            {
                log.AddLog("Error send event to MB " + Encoding.UTF8.GetString(msg.body));
            }
            else
            {
                cfg.setLastSentEventTime(aeObj.Time);
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