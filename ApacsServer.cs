using ApcSrvSDK;
using System;

namespace ApacsAdapter
{
    public class ApacsServer : IDisposable
    {
        private AdpLog log = new AdpLog();
        IApcServerWrap Apacs = null;
        TApcSession Session = null;
        /*
         * Event
         */
        public delegate void ApacsEventHandler(ApacsPropertyObject aEvtSettings);
        public event ApacsEventHandler ApacsEvent
        {
            add
            {
                if (ApacsEventImp == null)
                {
                    Apacs.set_onEvent(new EventClass(this));
                }
                ApacsEventImp += value;
            }

            remove
            {
                ApacsEventImp -= value;
                if (ApacsEventImp == null)
                {
                    Apacs.set_onEvent(null);
                }
            }
        }

        private ApacsEventHandler ApacsEventImp;
        public class EventClass
        {
            private ApacsServer Apacs = null;
            public EventClass(ApacsServer Apacs)
            {
                this.Apacs = Apacs;
            }

            [System.Runtime.InteropServices.DispId(0)]

            public void defaultDispatchHandle(object aEvtSettings)
            {
                Apacs.onEvent(new ApacsPropertyObject(aEvtSettings));
            }
        }

        internal void onEvent(ApacsPropertyObject aEvtSettings)
        {
            ApacsEventImp(aEvtSettings);

        }
        /*
         * NotifyAdd
         */
        public delegate void ApacsNotifyAddHandler(ApacsObject aAddedObj);
        public event ApacsNotifyAddHandler ApacsNotifyAdd
        {
            add
            {
                if (ApacsNotifyAddImp == null)
                {
                    Apacs.set_onNotifyAdd(new NotifyAddClass(this));
                }
                ApacsNotifyAddImp += value;
            }

            remove
            {
                ApacsNotifyAddImp -= value;
                if (ApacsNotifyAddImp == null)
                {
                    Apacs.set_onNotifyAdd(null);
                }
            }
        }

        private ApacsNotifyAddHandler ApacsNotifyAddImp;
        public class NotifyAddClass
        {
            private ApacsServer Apacs = null;
            public NotifyAddClass(ApacsServer Apacs)
            {
                this.Apacs = Apacs;
            }

            [System.Runtime.InteropServices.DispId(0)]

            public void defaultDispatchHandle(object aAddedObj)
            {
                Apacs.onNotifyAdd(new ApacsObject((IApcObjectWrap)aAddedObj));
            }
        }

        internal void onNotifyAdd(ApacsObject aAddedObj)
        {
            ApacsNotifyAddImp(aAddedObj);
        }
        /*
         * NotifyChange
         */
        public delegate void ApacsNotifyChangeHandler(ApacsObject aChandgedObj, ApacsPropertyObject aChangedSettings);
        public event ApacsNotifyChangeHandler ApacsNotifyChange
        {
            add
            {
                if (ApacsNotifyChangeImp == null)
                {
                    Apacs.set_onNotifyChange(new NotifyChangeClass(this));
                }
                ApacsNotifyChangeImp += value;
            }

            remove
            {
                ApacsNotifyChangeImp -= value;
                if (ApacsNotifyChangeImp == null)
                {
                    Apacs.set_onNotifyChange(null);
                }
            }
        }

        private ApacsNotifyChangeHandler ApacsNotifyChangeImp;
        public class NotifyChangeClass
        {
            private ApacsServer Apacs = null;
            public NotifyChangeClass(ApacsServer Apacs)
            {
                this.Apacs = Apacs;
            }

            [System.Runtime.InteropServices.DispId(0)]

            public void defaultDispatchHandle(object aChangedObj, object aChangedSettings)
            {
                Apacs.onNotifyChange(new ApacsObject((IApcObjectWrap)aChangedObj), new ApacsPropertyObject(aChangedSettings));
            }
        }

        internal void onNotifyChange(ApacsObject aChandgedObj, ApacsPropertyObject aChangedSettings)
        {
            ApacsNotifyChangeImp(aChandgedObj, aChangedSettings);
        }
        /*
         * NotifyDelete
         */
        public delegate void ApacsNotifyDeleteHandler(ApacsObject aDeletedObj);
        public event ApacsNotifyDeleteHandler ApacsNotifyDelete
        {
            add
            {
                if (ApacsNotifyDeleteImp == null)
                {
                    Apacs.set_onNotifyDelete(new NotifyDeleteClass(this));
                }
                ApacsNotifyDeleteImp += value;
            }

            remove
            {
                ApacsNotifyDeleteImp -= value;
                if (ApacsNotifyDeleteImp == null)
                {
                    Apacs.set_onNotifyDelete(null);
                }
            }
        }

        private ApacsNotifyDeleteHandler ApacsNotifyDeleteImp;
        public class NotifyDeleteClass
        {
            private ApacsServer Apacs = null;
            public NotifyDeleteClass(ApacsServer Apacs)
            {
                this.Apacs = Apacs;
            }

            [System.Runtime.InteropServices.DispId(0)]

            public void defaultDispatchHandle(object aDeletedObj)
            {
                Apacs.onNotifyDeleted(new ApacsObject((IApcObjectWrap)aDeletedObj));
            }
        }

        internal void onNotifyDeleted(ApacsObject aAddedObj)
        {
            ApacsNotifyDeleteImp(aAddedObj);
        }
        /*
         * Disconnect
         */
        public delegate void ApacsDisconnectHandler();
        public event ApacsDisconnectHandler ApacsDisconnect;
        public class DisconnectClass
        {
            private ApacsServer Apacs = null;
            public DisconnectClass(ApacsServer Apacs)
            {
                this.Apacs = Apacs;
            }

            [System.Runtime.InteropServices.DispId(0)]

            public void defaultDispatchHandle()
            {
                Apacs.onDisconnect();
            }
        }

        internal void onDisconnect()
        {
            ApacsDisconnect();
        }

        public ApacsServer(string astrLogin, string astrPassword)
        {
            TApcConnection Connection = new TApcConnection();
            int nResult = int.MinValue;
            while (nResult != 0)
            {
                nResult = Connection.createSession(astrLogin, astrPassword, out Session);
            }
            while (Apacs == null)
            {
                Apacs = (IApcServerWrap)Session.getServer();
            }
            try
            {
                Session.set_onDisconnect(new DisconnectClass(this));
                log.AddLog("APACS server connected");
            }
            catch (Exception e)
            {
                log.AddLog(e.ToString());
            }
        }
        public void Dispose()
        {
            if (Session != null)
            {
                Session.close();
                Session = null;
            }
            Apacs = null;
            log.AddLog("APACS Server disposed");
        }

        public ApacsObject getObjectByUID(string astrUID)
        {
            if (Apacs == null || String.IsNullOrEmpty(astrUID))
            {
                return null;
            }
            object obj = null;
            int nResult = Apacs.getObjectByUID(astrUID, out obj);
            if (nResult != 0 || obj == null)
            {
                return null;
            }
            return new ApacsObject(obj as IApcObjectWrap);
        }

        public ApacsObject getObjectBySampleUID(string UID)
        {
            if (String.IsNullOrEmpty(UID))
            {
                return null;
            }
            UID = "SA 0000." + UID;
            return getObjectByUID(UID);
        }

        public ApacsObject getObjectByAlias(string astrAlias)
        {
            if (Apacs == null || String.IsNullOrEmpty(astrAlias))
            {
                return null;
            }
            object obj = null;
            int nResult = Apacs.getObjectByAlias(astrAlias, out obj);
            if (nResult != 0 || obj == null)
            {
                return null;
            }
            return new ApacsObject(obj as IApcObjectWrap);
        }
        public ApacsObject[] getObjectsByFilter(string astrObjType, string filterStrName, string filterValue)
        {
            if (Apacs == null || String.IsNullOrEmpty(astrObjType) || String.IsNullOrEmpty(filterStrName) || String.IsNullOrEmpty(filterValue))
            {
                return null;
            }
            Array res = new object[] { };
            TApcEQUALObjFilter apFilter = new TApcEQUALObjFilter();
            apFilter.strName = filterStrName;
            apFilter.Value = filterValue;
            int nResult = Apacs.getObjectsByFilter(astrObjType, apFilter, out res);
            if (nResult != 0 || res == null)
            {
                return null;
            }
            ApacsObject[] result = new ApacsObject[res.Length];
            for (int i = 0; i < res.Length; i++)
            {
                result[i] = new ApacsObject(res.GetValue(i) as IApcObjectWrap);
            }
            return result;
        }

        public ApacsObject[] getObjectsByType(string astrObjType)
        {
            if (Apacs == null || String.IsNullOrEmpty(astrObjType))
            {
                return null;
            }
            Array res = new object[] { };
            int nResult = Apacs.getObjectsByFilter(astrObjType, null, out res);
            if (nResult != 0 || res == null)
            {
                return null;
            }
            ApacsObject[] result = new ApacsObject[res.Length];
            for (int i = 0; i < res.Length; i++)
            {
                result[i] = new ApacsObject(res.GetValue(i) as IApcObjectWrap);
            }
            return result;
        }
        public ApacsObject getRootObject()
        {
            if (Apacs == null)
            {
                return null;
            }
            object obj = null;
            int nResult = Apacs.getRootObject(out obj);
            if (nResult != 0 || obj == null)
            {
                return null;
            }
            return new ApacsObject(obj as IApcObjectWrap);
        }

        public ApacsPropertyObject[] getEvents(string[] astrTypes, DateTime adtFrom, DateTime adtTo)
        {
            if (Apacs == null || astrTypes == null || adtFrom == null || adtTo == null)
            {
                return null;
            }
            Array eventsProps = new object[] { };
            int nResult = Apacs.getEvents(astrTypes, adtFrom, adtTo, out eventsProps);
            if (nResult != 0 || eventsProps == null)
            {
                return null;
            }
            ApacsPropertyObject[] result = new ApacsPropertyObject[eventsProps.Length];
            for (int i = 0; i < eventsProps.Length; i++)
            {
                result[i] = new ApacsPropertyObject(eventsProps.GetValue(i));
            }
            return result;
        }
    }
}
