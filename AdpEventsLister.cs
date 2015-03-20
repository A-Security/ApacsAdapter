using System;
using System.Collections.Generic;

namespace ApacsAdapter
{
    public class AdpEventsLister
    {
        class ListWithOnAddEvents<T> : List<T>
        {
            public event EventHandler OnAdd;
            public void Add(T item)
            {
                if (OnAdd != null)
                {
                    OnAdd(this, EventArgs.Empty);
                }
                base.Add(item);
            }
        }
        private ListWithOnAddEvents<string> dispatchQueue = new ListWithOnAddEvents<string>();
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
            startEventsLister();
        }
        private void startEventsLister()
        {
            try
            {
                Apacs.ApacsDisconnect += new ApacsServer.ApacsDisconnectHandler(onDisconnect);
                Apacs.ApacsNotifyAdd += new ApacsServer.ApacsNotifyAddHandler(onAddObject);
                Apacs.ApacsNotifyDelete += new ApacsServer.ApacsNotifyDeleteHandler(onDelObject);
                Apacs.ApacsNotifyChange += new ApacsServer.ApacsNotifyChangeHandler(onChangeObject);
                Apacs.ApacsEvent += new ApacsServer.ApacsEventHandler(onEvent);
                dispatchQueue.OnAdd += new EventHandler(dispatchQueue_OnAdd);
            }
            catch (Exception) { }
        }

        void dispatchQueue_OnAdd(object sender, EventArgs e)
        {
            if (dispatchQueue.Count == 0)
            {
                return;
            }
            for (int i = 0; i < dispatchQueue.Count; i++)
            {
                if (!mbAdp.PublishMessage(cfg.MBoutQueue, dispatchQueue[i]))
                {
                    if (i > 0) { i--; }
                }
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
            }
            catch (Exception) {}
        }
        private void onDisconnect()
        {
            stopEventsLister();
            Apacs.Dispose();
            Apacs = new ApacsServer(cfg.apcLogin, cfg.apcPasswd);
            startEventsLister();
        }

        private void onEvent(ApacsPropertyObject evtSet)
        {
            if (evtSet == null)
            {
                return;
            }
            string evtType = evtSet.getStringProperty(ApcObjProp.strEventTypeID).Split('_')[0];
            string aeobjStrXml = null;
            switch (evtType)
            {
                case ApcObjType.TApcCardHolderAccess:
                    {
                        AdpEvtObj_CHA aeObj_CHA = data.getEvtObjFromEvtSet_CHA(evtSet);
                        aeobjStrXml = aeObj_CHA != null ? aeObj_CHA.ToXmlString() : null;
                        break;
                    }
                default:
                    {
                        AdpEvtObj aeObj = data.getEvtObjFromEvtSet(evtSet);
                        aeobjStrXml = aeObj != null ? aeObj.ToXmlString() : null;
                        break;
                    }
            }
            if (aeobjStrXml != null)
            {
                if (!mbAdp.PublishMessage(cfg.MBoutQueue, aeobjStrXml))
                {
                    dispatchQueue.Add(aeobjStrXml);
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