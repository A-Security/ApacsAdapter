using ApcSrvSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
namespace ApacsAdapter
{
    // APACS 3000 Property Object class
    public class ApcPropObj
    {
        // Log instance
        private AdpLog log = new AdpLog();
        // magic "AAM Systems" object (interface of APACS 3000 Property Object?)
        internal object objSettings = null;
        // Default constructor
        public ApcPropObj(object aobjSettings)
        {
            objSettings = aobjSettings;
        }
        // Return true if parametr name is APACS 3000 property
        public bool isProperty(string strName)
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
        // Get APACS 3000 Property object by parametr name for this object
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
        // Get string property from APACS 3000 Property object (if Property is string) for this object
        public string getStringProperty(string strName)
        {
            object res = getProperty(strName);
            return res == null ? String.Empty : res as string;
        }
        // Get byte array property from APACS 3000 Property object (if Property is byte array) for this object
        public byte[] getByteArrayProperty(string strName)
        {
            object res = getProperty(strName);
            return res == null ? new byte[] { } : res as byte[];
        }
        // Get Apacs Object property from APACS 3000 Property object (if Property is Apacs Object) for this object
        public ApcObj getObjectProperty(string strName)
        {
            object res = getProperty(strName);
            return res == null ? new ApcObj(null) : new ApcObj(res as IApcObjectWrap);
        }
        // Get SysAddrHolder property from APACS 3000 Property object (if Property is SysAddrHolder Object) for this object
        public ApcObj getSysAddrHolderProperty()
        {
            return getObjectProperty(ApcObjProp.SysAddrHolder);
        }
        // Get string array of APACS 3000 property name for this object
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
        // Get uint property from APACS 3000 Property object (if Property is uint) for this object
        public uint getUIntProperty(string strName)
        {
            object res = getProperty(strName);
            return res == null ? 0 : (uint)res;
        }
        // Get cardholder full name property from APACS 3000 Property object (if Property is string) for this object
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
        // Get real datetime property for this object
        public DateTime getRealDateTime()
        {
            return getDateTimeProperty(ApcObjProp.dtRealDateTime);
        }
        // Get event sample uid for this object
        public string getSampleEventUID()
        {
            string[] res = getStringProperty(ApcObjProp.SysAddrEventID).Split('.');
            return res.Length > 1 ? res[1] : res[0];
        }
        // Get event source sample uid for this object
        public string getSampleSourceUID()
        {
            ApcObj sourceObj = getObjectProperty(ApcObjProp.SysAddrInitObj);
            return sourceObj.getSampleUID();
        }
        // Get event source alias for this object
        public string getSourceAlias()
        {
            ApcObj SysAddrInitObj = getObjectProperty(ApcObjProp.SysAddrInitObj);
            ApcPropObj SysAddrInitObjProp = SysAddrInitObj.getCurrentSettings();
            return SysAddrInitObjProp.getAliasProperty();
        }
        // Get alias for this object
        public string getAliasProperty()
        {
            return getStringProperty(ApcObjProp.strAlias);
        }
        // Get name for this object
        public string getNameProperty()
        {
            return getStringProperty(ApcObjProp.strName);
        }
        // Get event source name for this object
        public string getSourceNameProperty()
        {
            return getStringProperty(ApcObjProp.strInitObjName);
        }
        // Get event card number for this object
        public string getDwCardNumber()
        {
            uint res = getUIntProperty(ApcObjProp.dwCardNumber);
            return res == 0 ? String.Empty: res.ToString();
        }
        // Get DateTime property from APACS 3000 Property object (if Property is DateTime)
        public DateTime getDateTimeProperty(string strName)
        {
            object res = getProperty(strName);
            return res == null ? new DateTime() : (DateTime)res;
        }
        // Get cardholder company name for this object (TApcCardHolder type)
        public string getCompanyNameProperty()
        {
            ApcObj sysAddrCompany = getObjectProperty(ApcObjProp.SysAddrCompany);
            ApcPropObj sysAddrCompProp = sysAddrCompany.getCurrentSettings();
            return sysAddrCompProp.getNameProperty();
        }
        //  Get cardholder job title for this object (TApcCardHolder type)
        public string getJobTitleNameProperty()
        {
            ApcObj sysAddrJobTitle = getObjectProperty(ApcObjProp.SysAddrJobTitle);
            ApcPropObj sysAddrJobTitleProp  = sysAddrJobTitle.getCurrentSettings();
            return sysAddrJobTitleProp.getNameProperty();
        }
        // Get event type for this object
        public string getEventTypeProperty()
        {
            return getStringProperty(ApcObjProp.strEventTypeID);
        }
        // Get cardholder category (true if guest, false if employee, String.Empty if undefied
        public string getbEmployeeProperty()
        {
            object emp = getProperty(ApcObjProp.bEmployee);
            return (emp == null || (byte)emp == byte.MaxValue) ? String.Empty : Convert.ToBoolean(emp).ToString().ToLower();
        }
        // Set property for this object by propertry name
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
        // Set Apacs Object property for this object by propertry name
        public void setObjectProperty(string strName, ApcObj obj)
        {
            setProperty(strName, (obj == null ? null : obj.objWrap));
        }
    }
}
