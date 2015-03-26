using System;
using System.Collections.Generic;

namespace ApacsAdapter
{
    public class AdpEventsLister
    {
        class ListWithOnAddEvent<T> : List<T>
        {
            public event EventHandler OnAdd;
            public new void Add(T item)
            {
                base.Add(item);
                if (OnAdd != null)
                {
                    OnAdd(this, EventArgs.Empty);
                }
            }
        }
        
        private ListWithOnAddEvent<MQMessage> dispatchQueue = new ListWithOnAddEvent<MQMessage>();
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
            ListWithOnAddEvent<MQMessage> ls = (ListWithOnAddEvent<MQMessage>)sender;
            if (ls == null || ls.Count == 0)
            {
                return;
            }
            for (int i = 0; i < ls.Count; i++)
            {
                if (!ls[i].IsBodyEmpty || !mbAdp.PublishMessage(cfg.MBoutQueue, ls[i]))
                {
                    if (!ls[i].IsBodyEmpty || i > 0) 
                    { 
                        i--; 
                    }
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
            MQMessage msg = null;
            switch (evtType)
            {
                case ApcObjType.TApcCardHolderAccess:
                    {
                        AdpEvtObj_CHA aeObj_CHA = data.getEvtObjFromEvtSet_CHA(evtSet);
                        if (aeObj_CHA != null)
                        {
                            msg = new MQMessage(aeObj_CHA.EventID, aeObj_CHA.ToXmlString());
                        }
                        break;
                    }
                default:
                    {
                        AdpEvtObj aeObj = data.getEvtObjFromEvtSet(evtSet);
                        if (aeObj != null)
                        {
                            msg = new MQMessage(aeObj.EventID, aeObj.ToXmlString());
                        }
                        break;
                    }
            }
            if (msg != null || !msg.IsBodyEmpty)
            {
                if (!mbAdp.PublishMessage(cfg.MBoutQueue, msg))
                {
                    dispatchQueue.Add(msg);
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