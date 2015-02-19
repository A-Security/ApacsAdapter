using System;
using ApcSrvSDK;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace ApacsAdapter
{
    public class ApacsException : Exception
    {
        private int _nResult = 0;
        public ApacsException(int nResult)
        {
            _nResult = nResult;
        }
        override public string ToString()
        {
            return "ApacsException with code: " + _nResult;
        }
    }

    public struct ApcObjType
    {
        public static string TApcCHMainPhoto { get { return "TApcCHMainPhoto"; } }
        public static string TApcCardHolderAccess { get { return "TApcCardHolderAccess"; } }
        public static string TApcCardHolder { get { return "TApcCardHolder"; } }
    }

    public struct ApcObjProp
    {
        public static string strName { get { return "strName"; } }
        public static string strDesc { get { return "strDesc"; } }
        public static string strAlias { get { return "strAlias"; } }
        public static string dtCreateTime { get { return "dtCreateTime"; } }
        public static string dtLastModifyTime { get { return "dtLastModifyTime"; } }
    }

    public struct ApcEvtProp
    {
        public static string dwCardNumber { get { return "dwCardNumber"; } }
        public static string SysAddrCard { get { return "SysAddrCard"; } }
        public static string strHolderName { get { return "strHolderName"; } }
        public static string SysAddrHolder { get { return "SysAddrHolder"; } }
        public static string isOffLineEvent { get { return "isOffLineEvent"; } }
        public static string SysAddrEventID { get { return "SysAddrEventID"; } }
        public static string dtRealDateTime { get { return "dtRealDateTime"; } }
        public static string dtRegisterTime { get { return "dtRegisterTime"; } }
        public static string strEventTypeID { get { return "strEventTypeID"; } }
        public static string SysAddrInitObj { get { return "SysAddrInitObj"; } }
        public static string strInitObjName { get { return "strInitObjName"; } }
        public static string binBufPhoto { get { return "binBufPhoto"; } }
        public static string strLastName { get { return "strLastName"; } }
        public static string strMiddleName { get { return "strMiddleName"; } }
        public static string strFirstName { get { return "strFirstName"; } }
    }

    public class ApacsPropertyObject
    {
        public object objSettings = null;
        public ApacsPropertyObject(object aobjSettings)
        {
            if (aobjSettings == null)
            {
                return;
            }
            objSettings = aobjSettings;
        }
        
        public bool hasProperty(string strName)
        {
            try
            {
                getProperty(strName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        public object getProperty(string strName)
        {
            return objSettings.GetType().InvokeMember(strName, BindingFlags.GetProperty, null, objSettings, null);
        }
        
        public string getStringProperty(string strName)
        {
            return (string)getProperty(strName);
        }
        
        public byte[] getByteArrayProperty(string strName)
        {
            return (byte[])getProperty(strName);
        }
        
        public ApacsObject getObjectProperty(string strName)
        {
            return new ApacsObject((IApcObjectWrap)getProperty(strName));
        }
        public string[] getPropertyNames()
        {
            List<string> list = new List<string>();
            IEnumerator enumer = ((IEnumerable)objSettings).GetEnumerator();
            while (enumer.MoveNext())
            {
                list.Add(enumer.Current.ToString());
            }
            return list.ToArray();
        }
        
        public uint getUIntProperty(string strName)
        {
            return (uint)getProperty(strName);
        }
        
        public DateTime getDateTimeProperty(string strName)
        {
            return (DateTime)getProperty(strName);
        }
        
        public void setProperty(string strName, object objProp)
        {
            objSettings.GetType().InvokeMember(strName, BindingFlags.SetProperty, null, objSettings, new object[] { objProp });
        }

        public void setObjectProperty(string strName, ApacsObject obj)
        {
            setProperty(strName, (obj == null ? null : obj.objWrap));
        }
        
    }

    public class ApacsObject
    {
        internal IApcObjectWrap objWrap = null;
        public ApacsObject(IApcObjectWrap aobjWrap)
        {
            if (aobjWrap == null)
            {
                return;
            }
            objWrap = aobjWrap;
        }
        
        public string getUID()
        {
            string strUID = null;
            int nResult = objWrap.getUID(out strUID);
            if (nResult != 0 || strUID == null) 
            {
                return null;
            }
            return strUID;
        }
        
        public string getApacsType()
        {
            string strApacsType = null;
            int nResult = objWrap.getApacsType(out strApacsType);
            if (nResult != 0)
            {
                return null;
            }
            return strApacsType;
        }
        
        public void deleteObject()
        {
            int nResult = int.MinValue;
            nResult = objWrap.deleteObject();
            if (nResult != 0)
            {
                return;
            }
        }
        
        public ApacsObject getParentObject()
        {
            object parent = null;
            int nResult = objWrap.getParentObject(out parent);
            if (nResult != 0 || parent == null)
            {
                return null;
            }
            return new ApacsObject((IApcObjectWrap)parent);
            
        }
        
        public ApacsPropertyObject getCurrentSettings()
        {
            object objSettings = null;
            int nResult = objWrap.getCurrentSettings(out objSettings);
            if (nResult != 0 || objSettings == null)
            {
                return null;
            }
            return new ApacsPropertyObject(objSettings);
        }
        
        public void applySettings(ApacsPropertyObject aobjSettings)
        {
            object nativeObjSettings = aobjSettings.objSettings;
            int nResult = objWrap.applySettings(nativeObjSettings);
            if (nResult != 0)
            {
                return;
            }
        }
        
        public ApacsObject[] getChildrenObjs()
        {
            Array childrenObjs = new object[] { };
            int nResult = objWrap.getChildrenObjs(out childrenObjs);
            if (nResult != 0)
            {
                return null;
            }
            
            ApacsObject[] result = new ApacsObject[childrenObjs.Length];
            for (int i = 0; i < childrenObjs.Length; i++)
            {
                result[i] = new ApacsObject((IApcObjectWrap)childrenObjs.GetValue(i));
            }
            return result;
        }

        public ApacsObject addChildWithSettings(ApacsPropertyObject aobjSettings)
        {
            object obj = null;
            int nResult = objWrap.addChildWithSettings(aobjSettings.objSettings, out obj);
            if (nResult != 0 || obj == null)
            {
                return null;
            }
            return new ApacsObject((IApcObjectWrap)obj);
        }

        public ApacsObject[] getChildrenObjsByTypes(string[] astrTypes)
        {
            ParameterModifier p = new ParameterModifier(2);
            p[1] = true;

            ParameterModifier[] mods = { p };

            object[] objChld = new object[] { };
            object[] args = new object[2] { astrTypes, objChld };
            int nResult 
                = (int)objWrap
                    .GetType()
                        .InvokeMember("getChildrenObjsByTypes",
                                      BindingFlags.InvokeMethod,
                                      null,
                                      objWrap,
                                      args,
                                      mods,
                                      null,
                                      null);
            if (nResult != 0)
            {
                return null;
            }

            object[] childrenObjs = (object[])args[1];
            ApacsObject[] result = new ApacsObject[childrenObjs.Length];
            for (int i = 0; i < childrenObjs.Length; i++)
            {
                result[i] = new ApacsObject((IApcObjectWrap)childrenObjs.GetValue(i));
            }
            return result;
        }
        public ApacsPropertyObject getChildSettingsForAdd(string aObjType)
        {
            object objSettings = null;
            int nResult = objWrap.getChildSettingsForAdd(aObjType, out objSettings);
            if (nResult != 0 || objSettings == null)
            {
                return null;
            }
            return new ApacsPropertyObject(objSettings);
        }

        public int execCmd(string strCmd)
        {
            return (int)objWrap.GetType().InvokeMember(strCmd, 
                                                       BindingFlags.InvokeMethod,
                                                       null,
                                                       objWrap,
                                                       null);
        }
    }

    public class ApacsServer : IDisposable
    {
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
            }
            catch (Exception) { }
        }
        public void Dispose()
        {
            if (Session != null)
            {
                Session.close();
                Session = null;
            }
            Apacs = null;
        }
        
        public ApacsObject getObjectByUID(string astrUID)
        {
            object obj = null;
            int nResult = Apacs.getObjectByUID(astrUID, out obj);
            if (nResult != 0 || obj == null)
            {
                return null;
            }
            return new ApacsObject((IApcObjectWrap)obj);
        }
        
        public ApacsObject getObjectByAlias(string astrAlias)
        {
            object obj = null;
            int nResult = Apacs.getObjectByAlias(astrAlias, out obj);
            if (nResult != 0 || obj == null)
            {
                return null;
            }
            return new ApacsObject((IApcObjectWrap)obj);
        }
        public ApacsObject[] getObjectsByFilter(string astrObjType, string filterValue, string filterStrName)
        {
            Array res = new object[] { };
            TApcEQUALObjFilter apFilter = new TApcEQUALObjFilter();
            apFilter.strName = filterStrName;
            apFilter.Value = filterValue;
            int nResult = Apacs.getObjectsByFilter(astrObjType, apFilter, out res);
            if (nResult != 0)
            {
                return null;
            }
            ApacsObject [] result = new ApacsObject[res.Length];
            for (int i = 0; i < res.Length; i++)
            {
                result[i] = new ApacsObject((IApcObjectWrap)res.GetValue(i));
            }
            return result;
        }

        public ApacsObject[] getObjectsByType(string astrObjType)
        {
            Array res = new object[] { };
            int nResult = Apacs.getObjectsByFilter(astrObjType, null, out res);
            if (nResult != 0)
            {
                return null;
            }
            ApacsObject[] result = new ApacsObject[res.Length];
            for (int i = 0; i < res.Length; i++)
            {
                result[i] = new ApacsObject((IApcObjectWrap)res.GetValue(i));
            }
            return result;
        }
        public ApacsObject getRootObject()
        {
            object obj = null;
            int nResult = Apacs.getRootObject(out obj);
            if (nResult != 0 || obj == null)
            {
                return null;
            }
            return new ApacsObject((IApcObjectWrap)obj);
        }

        public ApacsPropertyObject[] getEvents(string[] astrTypes, DateTime adtFrom, DateTime adtTo)
        {
            Array eventsProps = new object[] { };
            int nResult = Apacs.getEvents(astrTypes, adtFrom, adtTo, out eventsProps);
            if (nResult != 0)
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