using System;

namespace ApacsAdapter
{
    public class AdpEventsLister
    {
        private AdpCfgXml cfg;
        private ApcGetDate data;
        private ApacsServer Apacs;
        private AdpMBAdapter mbAdp;
            
        public AdpEventsLister(ApacsServer Apacs, AdpCfgXml cfg) 
        {
            this.Apacs = Apacs;
            this.cfg = cfg;
            this.data = new ApcGetDate();
            this.mbAdp = new AdpMBAdapter(cfg.MBhost, cfg.MBuser, cfg.MBpassword, Convert.ToInt32(cfg.MBport));
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
            }
            catch (Exception) { }
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
            string aeobjStrXml;
            
            switch (evtType)
            {
                case ApcObjType.TApcCardHolderAccess:
                    {
                        AdpEvtObj_CHA aeCHAobj = data.getCHAobjFromEvtSet(evtSet);
                        aeobjStrXml = aeCHAobj != null ? aeCHAobj.ToXmlString() : null;
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
                mbAdp.PublishMessage(aeobjStrXml, cfg.MBoutQueue);
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