using ApcSrvSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
namespace ApacsAdapter
{
    public class ApacsPropertyObject
    {
        AdpLog log = new AdpLog();
        internal object objSettings = null;
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
            if (objSettings == null || String.IsNullOrEmpty(strName))
            {
                return null;
            }
            try
            {
                return objSettings.GetType().InvokeMember(strName, BindingFlags.GetProperty, null, objSettings, null);
            }
            catch (Exception e)
            {
                log.AddLog(e.ToString());
                return null;
            }

        }

        public string getStringProperty(string strName)
        {
            object res = getProperty(strName);
            return res == null ? null : res as string;
        }

        public byte[] getByteArrayProperty(string strName)
        {
            object res = getProperty(strName);
            return res == null ? null : res as byte[];
        }

        public ApacsObject getObjectProperty(string strName)
        {
            object res = getProperty(strName);
            return res == null ? null : new ApacsObject(res as IApcObjectWrap);
        }
        public string[] getPropertyNames()
        {
            if (objSettings == null)
            {
                return null;
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
                return null;
            }
        }

        public uint getUIntProperty(string strName)
        {
            object res = getProperty(strName);
            return res == null ? 0 : (uint)res;
        }

        public string getFullNameProperty()
        {
            string _lastName = getStringProperty(ApcObjProp.strLastName),
                   _middleName = getStringProperty(ApcObjProp.strMiddleName),
                   _firstName = getStringProperty(ApcObjProp.strFirstName);
            if (!String.IsNullOrEmpty(_lastName) | !String.IsNullOrEmpty(_middleName) | !String.IsNullOrEmpty(_firstName))
            {
                return String.Format("{0} {1} {2}", _lastName, _firstName, _middleName);
            }
            else
            {
                return null;
            }
        }
        public DateTime getRealDateTime()
        {
            return getDateTimeProperty(ApcObjProp.dtRealDateTime);
        }
        public string getSampleEventUID()
        {
            string res = getStringProperty(ApcObjProp.SysAddrEventID);
            return String.IsNullOrEmpty(res) ? null : res.Split('.')[1];
        }
        public string getSampleSourceUID()
        {
            ApacsObject sourceObj = getObjectProperty(ApcObjProp.SysAddrInitObj);
            return sourceObj == null ? null : sourceObj.getSampleUID();
        }
        public string getNameProperty()
        {
            return getStringProperty(ApcObjProp.strName);
        }
        public string getSourceNameProperty()
        {
            return getStringProperty(ApcObjProp.strInitObjName);
        }
        public uint getDwCardNumber()
        {
            return getUIntProperty(ApcObjProp.dwCardNumber);
        }
        public DateTime getDateTimeProperty(string strName)
        {
            object res = getProperty(strName);
            return res == null ? new DateTime() : (DateTime)res;
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

        public void setObjectProperty(string strName, ApacsObject obj)
        {
            setProperty(strName, (obj == null ? null : obj.objWrap));
        }

    }
}
