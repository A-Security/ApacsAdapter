using ApacsAdapter;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ApacsAdapterService
{
    public class AdpAPCEvtsListener
    {
        private AdpLog log;
        private AdpSrvCfg cfg;
        private ApcData data;
        private ApcServer Apacs;
        private AdpMBAdapter producer;
        private AdpGRAdapter grAdp;
        private DateTime lastSentEventTime;
        
        public AdpAPCEvtsListener(ApcServer Apacs, AdpSrvCfg cfg) 
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
        public void start()
        {
            try
            {
                producer.connect();
                sendLatestEvents(lastSentEventTime, DateTime.Now);
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
            AdpApcEvtObj aeObj = data.mapAdpApcEvtObj(evtSet);
            byte[] msgBody = Encoding.UTF8.GetBytes(aeObj.ToXmlString());
            AdpMBMsgObj msg = new AdpMBMsgObj(Guid.NewGuid().ToString(), msgBody, aeObj.TYPE);
            if (!producer.PublishMessage(msg))
            {
                log.AddLog("Error send event to MB " + Encoding.UTF8.GetString(msg.body));
            }
            else
            {
                cfg.setLastSentEventTime(aeObj.EventTime);
            }
            
        }
        private void onAddObject(ApcObj newObject) 
        {
            if (newObject == null || !String.Equals(ApcObjType.TApcCardHolder, newObject.getApacsType()))
            {
                return;
            }
            chModHundler(newObject, AdpCHObj.ModType.AddRq);
        }
        private void onDelObject(ApcObj delObject)
        {
            if (delObject == null || !String.Equals(ApcObjType.TApcCardHolder, delObject.getApacsType()))
            {
                return;
            }
            chModHundler(delObject, AdpCHObj.ModType.DelRq);
        }
        private void onChangeObject(ApcObj changeObject, ApcPropObj evtSet)
        {
            if (changeObject == null || !String.Equals(ApcObjType.TApcCardHolder, changeObject.getApacsType()))
            {
                return;
            }
            chModHundler(changeObject, AdpCHObj.ModType.ModRq);
        }
        private void chModHundler(ApcObj ch, AdpCHObj.ModType _modType)
        {
            AdpCHObj aeCHObj = data.mapAdpCHObj(ch);
            aeCHObj.modType = _modType;
            byte[] msgBody = Encoding.UTF8.GetBytes(aeCHObj.ToXmlString());
            AdpMBMsgObj msg = new AdpMBMsgObj(Guid.NewGuid().ToString(), msgBody, aeCHObj.TYPE);
            if (!producer.PublishMessage(msg))
            {
                log.AddLog("Error send event to MB " + Encoding.UTF8.GetString(msg.body));
            }
            //grAdp.removeCardHolder(ch.getSampleUID());
            //if (_ModType <0)
            //{
            //    return;
            //}
            //grAdp.putCardHolder(data.mapAdpCHObj(ch));
        }
    }
}