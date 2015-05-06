using ApcSrvSDK;
using System;
using System.Reflection;

namespace ApacsAdapter
{
    public class ApacsObject
    {
        private AdpLog log = new AdpLog();
        internal IApcObjectWrap objWrap = null;
        public ApacsObject(IApcObjectWrap aobjWrap)
        {
            this.objWrap = aobjWrap;
        }

        public string getUID()
        {
            if (objWrap == null)
            {
                return null;
            }
            string strUID = null;
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
            string res = getUID();
            return String.IsNullOrEmpty(res) ? null : res.Split('.')[1];
        }

        public string getApacsType()
        {
            if (objWrap == null)
            {
                return null;
            }
            string strApacsType = null;
            objWrap.getApacsType(out strApacsType);
            return strApacsType;
        }

        public void deleteObject()
        {
            if (objWrap == null)
            {
                return;
            }
            objWrap.deleteObject();
        }

        public ApacsObject getParentObject()
        {
            if (objWrap == null)
            {
                return null;
            }
            object parent = null;
            objWrap.getParentObject(out parent);
            return parent == null ? null : new ApacsObject(parent as IApcObjectWrap);

        }

        public ApacsPropertyObject getCurrentSettings()
        {
            if (objWrap == null)
            {
                return null;
            }
            object objSettings = null;
            objWrap.getCurrentSettings(out objSettings);
            return objSettings == null ? null : new ApacsPropertyObject(objSettings);
        }

        public void applySettings(ApacsPropertyObject aobjSettings)
        {
            if (objWrap == null || aobjSettings == null)
            {
                return;
            }
            object nativeObjSettings = aobjSettings.objSettings;
            objWrap.applySettings(nativeObjSettings);

        }

        public ApacsObject[] getChildrenObjs()
        {
            if (objWrap == null)
            {
                return null;
            }
            Array childrenObjs = new object[] { };
            objWrap.getChildrenObjs(out childrenObjs);
            if (childrenObjs.Length == 0)
            {
                return null;
            }

            ApacsObject[] result = new ApacsObject[childrenObjs.Length];
            for (int i = 0; i < childrenObjs.Length; i++)
            {
                object obj = childrenObjs.GetValue(i);
                result[i] = obj == null ? null : new ApacsObject(obj as IApcObjectWrap);
            }
            return result;
        }

        public ApacsObject addChildWithSettings(ApacsPropertyObject aobjSettings)
        {
            if (objWrap == null || aobjSettings == null)
            {
                return null;
            }
            object obj = null;
            objWrap.addChildWithSettings(aobjSettings.objSettings, out obj);
            if (obj == null)
            {
                return null;
            }
            return new ApacsObject(obj as IApcObjectWrap);
        }

        public ApacsObject[] getChildrenObjsByTypes(string[] astrTypes)
        {
            if (objWrap == null)
            {
                return null;
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
                return null;
            }

            object[] childrenObjs = args[1] as object[];
            ApacsObject[] result = new ApacsObject[childrenObjs.Length];
            for (int i = 0; i < childrenObjs.Length; i++)
            {
                object obj = childrenObjs.GetValue(i);
                result[i] = obj == null ? null : new ApacsObject(obj as IApcObjectWrap);
            }
            return result;
        }
        public byte[] getMainPhoto()
        {
            byte[] result = null;
            ApacsObject[] chMainPhotoAOs = getChildrenObjsByTypes(new string[] { ApcObjType.TApcCHMainPhoto });
            foreach (ApacsObject photoProp in chMainPhotoAOs)
            {
                result = photoProp.getCurrentSettings().getByteArrayProperty(ApcObjProp.binBufPhoto);
                break;
            }
            return result;
        }
        public string getCardNumber()
        {
            string result = null;
            ApacsObject[] acc2linkAOs = getChildrenObjsByTypes(new string[] { ApcObjType.TApcAccount2HolderLink });
            foreach (ApacsObject chldAO in acc2linkAOs)
            {
                ApacsPropertyObject childAOCurSet = chldAO.getCurrentSettings();
                if (childAOCurSet != null)
                {
                    ApacsObject sysAdAc = childAOCurSet.getObjectProperty(ApcObjProp.SysAddrAccount);
                    if (sysAdAc != null)
                    {
                        ApacsPropertyObject sysAdAcProp = sysAdAc.getCurrentSettings();
                        if (sysAdAcProp != null)
                        {
                            result = sysAdAcProp.getNameProperty();
                        }
                    }
                }

            }

            return result;
        }
        public ApacsPropertyObject getChildSettingsForAdd(string aObjType)
        {
            if (objWrap == null || String.IsNullOrEmpty(aObjType))
            {
                return null;
            }
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
            if (objWrap == null || String.IsNullOrEmpty(strCmd))
            {
                return int.MinValue;
            }
            return (int)objWrap.GetType().InvokeMember(strCmd, BindingFlags.InvokeMethod, null, objWrap, null);
        }
    }
}
