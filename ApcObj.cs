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

        public ApcObj getParentObject()
        {
            if (objWrap == null)
            {
                return null;
            }
            object parent = null;
            objWrap.getParentObject(out parent);
            return parent == null ? null : new ApcObj(parent as IApcObjectWrap);

        }

        public ApcPropObj getCurrentSettings()
        {
            if (objWrap == null)
            {
                return null;
            }
            object objSettings = null;
            objWrap.getCurrentSettings(out objSettings);
            return objSettings == null ? null : new ApcPropObj(objSettings);
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
                return null;
            }
            Array childrenObjs = new object[] { };
            objWrap.getChildrenObjs(out childrenObjs);
            if (childrenObjs.Length == 0)
            {
                return null;
            }

            ApcObj[] result = new ApcObj[childrenObjs.Length];
            for (int i = 0; i < childrenObjs.Length; i++)
            {
                object obj = childrenObjs.GetValue(i);
                result[i] = obj == null ? null : new ApcObj(obj as IApcObjectWrap);
            }
            return result;
        }

        public ApcObj addChildWithSettings(ApcPropObj aobjSettings)
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
            return new ApcObj(obj as IApcObjectWrap);
        }

        public ApcObj[] getChildrenObjsByTypes(string[] astrTypes)
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
            ApcObj[] result = new ApcObj[childrenObjs.Length];
            for (int i = 0; i < childrenObjs.Length; i++)
            {
                object obj = childrenObjs.GetValue(i);
                result[i] = obj == null ? null : new ApcObj(obj as IApcObjectWrap);
            }
            return result;
        }
        public byte[] getMainPhoto()
        {
            byte[] result = null;
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
            string result = null;
            ApcObj[] acc2linkAOs = getChildrenObjsByTypes(new string[] { ApcObjType.TApcAccount2HolderLink });
            foreach (ApcObj chldAO in acc2linkAOs)
            {
                ApcPropObj childAOCurSet = chldAO.getCurrentSettings();
                if (childAOCurSet != null)
                {
                    ApcObj sysAdAc = childAOCurSet.getObjectProperty(ApcObjProp.SysAddrAccount);
                    if (sysAdAc != null)
                    {
                        ApcPropObj sysAdAcProp = sysAdAc.getCurrentSettings();
                        if (sysAdAcProp != null)
                        {
                            result = sysAdAcProp.getNameProperty();
                        }
                    }
                }

            }

            return result;
        }
        public ApcPropObj getChildSettingsForAdd(string aObjType)
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
