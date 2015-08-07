using ApcSrvSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
namespace ApacsAdapter
{
    public class ApcPropObj
    {
        private AdpLog log = new AdpLog();
        internal object objSettings = null;
        public ApcPropObj(object aobjSettings)
        {
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
            if (objSettings == null || String.IsNullOrEmpty(strName))
            {
                return null;
            }
            try
            {
                return objSettings.GetType().InvokeMember(strName, BindingFlags.GetProperty, null, objSettings, null);
            }
            catch (Exception)
            {
                return null;
            }

        }

        public string getStringProperty(string strName)
        {
            object res = getProperty(strName);
            return res == null ? String.Empty : res as string;
        }

        public byte[] getByteArrayProperty(string strName)
        {
            object res = getProperty(strName);
            return res == null ? new byte[] { } : res as byte[];
        }

        public ApcObj getObjectProperty(string strName)
        {
            object res = getProperty(strName);
            return res == null ? new ApcObj(null) : new ApcObj(res as IApcObjectWrap);
        }
        public ApcObj getSysAddrHolderProperty()
        {
            return getObjectProperty(ApcObjProp.SysAddrHolder);
        }
        public string[] getPropertyNames()
        {
            if (objSettings == null)
            {
                return new string[] { };
            }
            try
            {
                List<string> list = new List<string>();
                IEnumerator enumer = (objSettings as IEnumerable).GetEnumerator();
                while (enumer.MoveNext())
                {
                    list.Add(enumer.Current.ToString());
                }
                return list.ToArray();
            }
            catch (Exception e)
            {
                log.AddLog(e.ToString());
                return new string[] { };
            }
        }

        public uint getUIntProperty(string strName)
        {
            object res = getProperty(strName);
            return res == null ? 0 : (uint)res;
        }

        public string getFullNameProperty()
        {
            if (objSettings == null)
            {
                return String.Empty;
            }
            string _lastName = getStringProperty(ApcObjProp.strLastName),
                   _middleName = getStringProperty(ApcObjProp.strMiddleName),
                   _firstName = getStringProperty(ApcObjProp.strFirstName);
            if (String.IsNullOrEmpty(_lastName) && String.IsNullOrEmpty(_middleName) && String.IsNullOrEmpty(_firstName))
            {
                return String.Empty;
            }
            return String.Format("{0} {1} {2}", _lastName, _firstName, _middleName);
        }
        public DateTime getRealDateTime()
        {
            return getDateTimeProperty(ApcObjProp.dtRealDateTime);
        }
        public string getSampleEventUID()
        {
            string[] res = getStringProperty(ApcObjProp.SysAddrEventID).Split('.');
            return res.Length > 1 ? res[1] : res[0];
        }
        public string getSampleSourceUID()
        {
            ApcObj sourceObj = getObjectProperty(ApcObjProp.SysAddrInitObj);
            return sourceObj.getSampleUID();
        }
        public string getSourceAlias()
        {
            ApcObj SysAddrInitObj = getObjectProperty(ApcObjProp.SysAddrInitObj);
            ApcPropObj SysAddrInitObjProp = SysAddrInitObj.getCurrentSettings();
            return SysAddrInitObjProp.getAliasProperty();
        }
        public string getAliasProperty()
        {
            return getStringProperty(ApcObjProp.strAlias);
        }
        public string getNameProperty()
        {
            return getStringProperty(ApcObjProp.strName);
        }
        public string getSourceNameProperty()
        {
            return getStringProperty(ApcObjProp.strInitObjName);
        }
        public string getDwCardNumber()
        {
            uint res = getUIntProperty(ApcObjProp.dwCardNumber);
            return res == 0 ? String.Empty: res.ToString();
        }
        public DateTime getDateTimeProperty(string strName)
        {
            object res = getProperty(strName);
            return res == null ? new DateTime() : (DateTime)res;
        }
        public string getCompanyNameProperty()
        {
            ApcObj sysAddrCompany = getObjectProperty(ApcObjProp.SysAddrCompany);
            ApcPropObj sysAddrCompProp = sysAddrCompany.getCurrentSettings();
            return sysAddrCompProp.getNameProperty();
        }
        public string getJobTitleNameProperty()
        {
            ApcObj sysAddrJobTitle = getObjectProperty(ApcObjProp.SysAddrJobTitle);
            ApcPropObj sysAddrJobTitleProp  = sysAddrJobTitle.getCurrentSettings();
            return sysAddrJobTitleProp.getNameProperty();
        }
        public string getEventTypeProperty()
        {
            return getStringProperty(ApcObjProp.strEventTypeID);
        }
        public string getbEmployeeProperty()
        {
            object emp = getProperty(ApcObjProp.bEmployee);
            return (emp == null || (byte)emp == byte.MaxValue) ? String.Empty : Convert.ToBoolean(emp).ToString().ToLower();
        }

        public void setProperty(string strName, object objProp)
        {
            if (objSettings == null || objProp == null || String.IsNullOrEmpty(strName))
            {
                return;
            }
            try
            {
                objSettings.GetType().InvokeMember(strName, BindingFlags.SetProperty, null, objSettings, new object[] { objProp });
            }
            catch (Exception e)
            {
                log.AddLog(e.ToString());
            }
        }

        public void setObjectProperty(string strName, ApcObj obj)
        {
            setProperty(strName, (obj == null ? null : obj.objWrap));
        }
    }
}
