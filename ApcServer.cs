using ApcSrvSDK;
using System;

namespace ApacsAdapter
{
    public class ApcServer : IDisposable
    {
        private AdpLog log = new AdpLog();
        IApcServerWrap Apacs = null;
        TApcSession Session = null;
        /*
         * Event
         */
        public delegate void ApacsEventHandler(ApcPropObj aEvtSettings);
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
            private ApcServer Apacs = null;
            public EventClass(ApcServer Apacs)
            {
                this.Apacs = Apacs;
            }

            [System.Runtime.InteropServices.DispId(0)]

            public void defaultDispatchHandle(object aEvtSettings)
            {
                Apacs.onEvent(new ApcPropObj(aEvtSettings));
            }
        }

        internal void onEvent(ApcPropObj aEvtSettings)
        {
            ApacsEventImp(aEvtSettings);

        }
        /*
         * NotifyAdd
         */
        public delegate void ApacsNotifyAddHandler(ApcObj aAddedObj);
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
            private ApcServer Apacs = null;
            public NotifyAddClass(ApcServer Apacs)
            {
                this.Apacs = Apacs;
            }

            [System.Runtime.InteropServices.DispId(0)]

            public void defaultDispatchHandle(object aAddedObj)
            {
                Apacs.onNotifyAdd(new ApcObj((IApcObjectWrap)aAddedObj));
            }
        }

        internal void onNotifyAdd(ApcObj aAddedObj)
        {
            ApacsNotifyAddImp(aAddedObj);
        }
        /*
         * NotifyChange
         */
        public delegate void ApacsNotifyChangeHandler(ApcObj aChandgedObj, ApcPropObj aChangedSettings);
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
            private ApcServer Apacs = null;
            public NotifyChangeClass(ApcServer Apacs)
            {
                this.Apacs = Apacs;
            }

            [System.Runtime.InteropServices.DispId(0)]

            public void defaultDispatchHandle(object aChangedObj, object aChangedSettings)
            {
                Apacs.onNotifyChange(new ApcObj((IApcObjectWrap)aChangedObj), new ApcPropObj(aChangedSettings));
            }
        }

        internal void onNotifyChange(ApcObj aChandgedObj, ApcPropObj aChangedSettings)
        {
            ApacsNotifyChangeImp(aChandgedObj, aChangedSettings);
        }
        /*
         * NotifyDelete
         */
        public delegate void ApacsNotifyDeleteHandler(ApcObj aDeletedObj);
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
            private ApcServer Apacs = null;
            public NotifyDeleteClass(ApcServer Apacs)
            {
                this.Apacs = Apacs;
            }

            [System.Runtime.InteropServices.DispId(0)]

            public void defaultDispatchHandle(object aDeletedObj)
            {
                Apacs.onNotifyDeleted(new ApcObj((IApcObjectWrap)aDeletedObj));
            }
        }

        internal void onNotifyDeleted(ApcObj aAddedObj)
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
            private ApcServer Apacs = null;
            public DisconnectClass(ApcServer Apacs)
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

        public ApcServer(string astrLogin, string astrPassword)
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

        public ApcObj getObjectByUID(string astrUID)
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
            return new ApcObj(obj as IApcObjectWrap);
        }

        public ApcObj getObjectBySampleUID(string UID)
        {
            if (String.IsNullOrEmpty(UID))
            {
                return null;
            }
            UID = "SA 0000." + UID;
            return getObjectByUID(UID);
        }

        public ApcObj getObjectByAlias(string astrAlias)
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
            return new ApcObj(obj as IApcObjectWrap);
        }
        public ApcObj[] getObjectsByFilter(string astrObjType, string filterStrName, string filterValue)
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
            ApcObj[] result = new ApcObj[res.Length];
            for (int i = 0; i < res.Length; i++)
            {
                result[i] = new ApcObj(res.GetValue(i) as IApcObjectWrap);
            }
            return result;
        }

        public ApcObj[] getObjectsByType(string astrObjType)
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
            ApcObj[] result = new ApcObj[res.Length];
            for (int i = 0; i < res.Length; i++)
            {
                result[i] = new ApcObj(res.GetValue(i) as IApcObjectWrap);
            }
            return result;
        }
        public ApcObj getRootObject()
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
            return new ApcObj(obj as IApcObjectWrap);
        }

        public ApcPropObj[] getEvents(string[] astrTypes, DateTime adtFrom, DateTime adtTo)
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
            ApcPropObj[] result = new ApcPropObj[eventsProps.Length];
            for (int i = 0; i < eventsProps.Length; i++)
            {
                result[i] = new ApcPropObj(eventsProps.GetValue(i));
            }
            return result;
        }
    }
}
