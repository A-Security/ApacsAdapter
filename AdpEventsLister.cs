﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace ApacsAdapter
{
    public class AdpEventsLister
    {
        private object locker = new object();
        private static string CUR_EVENTID { get; set; }
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
        public void startEventsLister()
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

        public void stopEventsLister()
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
            stopEventsLister();
            Apacs.Dispose();
            Apacs = new ApacsServer(cfg.apcLogin, cfg.apcPasswd);
            startEventsLister();
        }

        private void onEvent(ApacsPropertyObject evtSet)
        {
            string eventId = null;
            lock(locker)
            {
                if (evtSet == null || AdpEventsLister.CUR_EVENTID == (eventId = evtSet.getSampleEventUID()))
                {
                    return;
                }
                AdpEventsLister.CUR_EVENTID = eventId;
            }
            string fullEvtType = evtSet.getStringProperty(ApcObjProp.strEventTypeID);
            string evtType = fullEvtType.Split('_')[0];
            AdpMQMessage msg = null;
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
                msg = new AdpMQMessage(aeObj.EventID, aeObj.ToXmlString(), aeObj.EventType);
                if (!msg.IsBodyEmpty && !mbAdp.PublishMessage(cfg.MBoutQueue, msg))
                {
                    log.AddLog("Error send event to MB: " + msg.body);
                }
            }
        }
        private void onAddObject(ApacsObject newObject) 
        {
            newObject.getSampleUID();
        }
        private void onDelObject(ApacsObject delObject)
        {
            delObject.getSampleUID();
        }
        private void onChangeObject(ApacsObject changeObject, ApacsPropertyObject evtSet)
        {
            changeObject.getSampleUID();
        }
    }
}