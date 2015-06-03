using ApcSrvSDK;
using System;
using System.Reflection;

namespace ApacsAdapter
{
    public class ApcObj
    {
        private AdpLog log = new AdpLog();
        internal IApcObjectWrap objWrap = null;
        public ApcObj(IApcObjectWrap aobjWrap)
        {
            this.objWrap = aobjWrap;
        }

        public string getUID()
        {
            string strUID = String.Empty;
            if (objWrap == null)
            {
                return strUID;
            }
            try
            {
                objWrap.getUID(out strUID);
            }
            catch (Exception e)
            {
                log.AddLog(e.ToString());
            }
            return strUID;
        }
        public string getSampleUID()
        {
            string[] res = getUID().Split('.');
            return res.Length > 1 ? res[1] : res[0];
        }

        public string getApacsType()
        {
            string strApacsType = String.Empty;
            if (objWrap == null)
            {
                return strApacsType;
            }
            try 
            {
                objWrap.getApacsType(out strApacsType);
            }
            catch (Exception e)
            {
                log.AddLog(e.ToString());
            }
            return strApacsType;
        }

        public void deleteObject()
        {
            if (objWrap == null)
            {
                return;
            }
            try
            {
                objWrap.deleteObject();
            }
            catch (Exception e)
            {
                log.AddLog(e.ToString());
            }
        }

        public ApcObj getParentObject()
        {
            if (objWrap == null)
            {
                return new ApcObj(null);
            }
            object parent = null;
            try
            {
                objWrap.getParentObject(out parent);
            }
            catch (Exception e)
            {
                log.AddLog(e.ToString());
            }
            return parent == null ? new ApcObj(null) : new ApcObj(parent as IApcObjectWrap);

        }

        public ApcPropObj getCurrentSettings()
        {
            object objSettings = null;
            if (objWrap == null)
            {
                return new ApcPropObj(objSettings);
            }
            try
            {
                objWrap.getCurrentSettings(out objSettings);
            }
            catch (Exception e)
            {
                log.AddLog(e.ToString());
            }
            return new ApcPropObj(objSettings);
        }

        public void applySettings(ApcPropObj aobjSettings)
        {
            if (objWrap == null || aobjSettings == null)
            {
                return;
            }
            object nativeObjSettings = aobjSettings.objSettings;
            objWrap.applySettings(nativeObjSettings);

        }

        public ApcObj[] getChildrenObjs()
        {
            if (objWrap == null)
            {
                return new ApcObj[] { };
            }
            Array childrenObjs = new object[] { };
            objWrap.getChildrenObjs(out childrenObjs);
            if (childrenObjs.Length == 0)
            {
                return new ApcObj[] { };
            }

            ApcObj[] result = new ApcObj[childrenObjs.Length];
            for (int i = 0; i < childrenObjs.Length; i++)
            {
                object obj = childrenObjs.GetValue(i);
                result[i] = obj == null ? new ApcObj(null) : new ApcObj(obj as IApcObjectWrap);
            }
            return result;
        }

        public ApcObj addChildWithSettings(ApcPropObj aobjSettings)
        {
            if (objWrap == null || aobjSettings == null)
            {
                return new ApcObj(null);
            }
            object obj = null;
            objWrap.addChildWithSettings(aobjSettings.objSettings, out obj);
            return obj == null ? new ApcObj(null) : new ApcObj(obj as IApcObjectWrap);
        }

        public ApcObj[] getChildrenObjsByTypes(string[] astrTypes)
        {
            if (objWrap == null)
            {
                return new ApcObj[] { };
            }
            ParameterModifier p = new ParameterModifier(2);
            p[1] = true;

            ParameterModifier[] mods = { p };

            object[] objChld = new object[] { };
            object[] args = new object[2] { astrTypes, objChld };
            int nResult = (int)objWrap.GetType().InvokeMember("getChildrenObjsByTypes",
                                                              BindingFlags.InvokeMethod,
                                                              null,
                                                              objWrap,
                                                              args,
                                                              mods,
                                                              null,
                                                              null);
            if (nResult != 0)
            {
                return new ApcObj[] { };
            }

            object[] childrenObjs = args[1] as object[];
            ApcObj[] result = new ApcObj[childrenObjs.Length];
            for (int i = 0; i < childrenObjs.Length; i++)
            {
                object obj = childrenObjs.GetValue(i);
                result[i] = obj == null ?  new ApcObj(null) : new ApcObj(obj as IApcObjectWrap);
            }
            return result;
        }
        public byte[] getMainPhoto()
        {
            byte[] result = new byte[] { };
            ApcObj[] chMainPhotoAOs = getChildrenObjsByTypes(new string[] { ApcObjType.TApcCHMainPhoto });
            foreach (ApcObj photoProp in chMainPhotoAOs)
            {
                result = photoProp.getCurrentSettings().getByteArrayProperty(ApcObjProp.binBufPhoto);
                break;
            }
            return result;
        }
        public string getCardNumber()
        {
            string result = String.Empty;
            ApcObj[] acc2linkAOs = getChildrenObjsByTypes(new string[] { ApcObjType.TApcAccount2HolderLink });
            foreach (ApcObj chldAO in acc2linkAOs)
            {
                ApcPropObj childAOCurSet = chldAO.getCurrentSettings();
                ApcObj sysAdAc = childAOCurSet.getObjectProperty(ApcObjProp.SysAddrAccount);
                ApcPropObj sysAdAcProp = sysAdAc.getCurrentSettings();
                result = sysAdAcProp.getNameProperty();
            }

            return result;
        }
        public ApcPropObj getChildSettingsForAdd(string aObjType)
        {
            object objSettings = null;
            if (objWrap == null || String.IsNullOrEmpty(aObjType))
            {
                return new ApcPropObj(objSettings);
            }
            objWrap.getChildSettingsForAdd(aObjType, out objSettings);
            return new ApcPropObj(objSettings);
        }

        public int execCmd(string strCmd)
        {
            if (objWrap == null || String.IsNullOrEmpty(strCmd))
            {
                return int.MinValue;
            }
            return (int)objWrap.GetType().InvokeMember(strCmd, BindingFlags.InvokeMethod, null, objWrap, null);
        }
    }
}
