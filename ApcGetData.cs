using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ApacsAdapter
{
    public class ApcGetDate
    {
        private string getTypeDesc(string strType)
        {
            ApcTypesDesc TApc = new ApcTypesDesc();
            return TApc.typeDescDict.TryGetValue(strType, out strType) ? strType : null;
        }
        public string getEventPropertiesHierarchy(ApacsPropertyObject objSettings)
        {
            StringBuilder sb = new StringBuilder();
            string apoName = objSettings.hasProperty(ApcObjProp.strName) ? objSettings.getStringProperty(ApcObjProp.strName) : "Event";
            string apoAlias = objSettings.hasProperty(ApcObjProp.strAlias) ? objSettings.getStringProperty(ApcObjProp.strAlias) : "Event";
            sb.AppendLine("================Property=" + apoName + "==Alias=" + apoAlias + "=======");
            foreach (string propName in objSettings.getPropertyNames())
            {
                object propObject = objSettings.getProperty(propName);
                sb.AppendLine(propName + ": " + propObject);
                if (objSettings.getProperty(propName).GetType().IsCOMObject)
                {
                    ApacsObject propCOMObject = objSettings.getObjectProperty(propName);
                    sb.AppendLine("===============Object=" + propName + "================");
                    sb.AppendLine("Object Apacs Type: " + propCOMObject.getApacsType());
                    sb.AppendLine(getEventPropertiesHierarchy(propCOMObject.getCurrentSettings()));
                    if (propCOMObject.getChildrenObjs().Length > 0)
                    {
                        foreach (ApacsObject childPropCOMObject in propCOMObject.getChildrenObjs())
                        {
                            sb.AppendLine(getEventPropertiesHierarchy(childPropCOMObject.getCurrentSettings()));
                        }
                    }
                    sb.AppendLine("================END Object=" + propName + "===============");
                }
            }
            sb.AppendLine("================END Property=" + apoName + "============");
            return sb.ToString();
        }
        public AdpEvtObj_CHA getCardHolderEventObjectFromEventSets(ApacsPropertyObject evtSets)
        {
            string eventType = evtSets.getStringProperty(ApcEvtProp.strEventTypeID);
            if (eventType.Contains("Will"))
            {
                return null;
            }
            bool isNotErrHolder = !(eventType.EndsWith("_ErrHolder"));
            string _lastName = null, _middleName = null, _firstName = null;
            if (isNotErrHolder)
            {
                ApacsObject holder = evtSets.getObjectProperty(ApcEvtProp.SysAddrHolder);
                _lastName = holder.getCurrentSettings().getStringProperty(ApcEvtProp.strLastName);
                _middleName = holder.getCurrentSettings().getStringProperty(ApcEvtProp.strMiddleName);
                _firstName = holder.getCurrentSettings().getStringProperty(ApcEvtProp.strFirstName);
            }
            AdpEvtObj_CHA aobj = new AdpEvtObj_CHA
            {
                Time = evtSets.getDateTimeProperty(ApcEvtProp.dtRealDateTime),
                EventID = evtSets.getStringProperty(ApcEvtProp.SysAddrEventID),
                EventType = eventType,
                EventTypeDesc = getTypeDesc(eventType),
                SourceID = evtSets.getObjectProperty(ApcEvtProp.SysAddrInitObj).getUID(),
                SourceName = evtSets.getStringProperty(ApcEvtProp.strInitObjName),
                HolderID = (isNotErrHolder) ?
                    evtSets.getObjectProperty(ApcEvtProp.SysAddrHolder).getUID() : null,
                HolderName = (isNotErrHolder) ?
                    _lastName + " " + _firstName + " " + _middleName : "НЕИЗВЕСТНЫЙ",
                HolderShortName = (isNotErrHolder) ?
                    evtSets.getStringProperty(ApcEvtProp.strHolderName) : "НЕИЗВЕСТНЫЙ",
                CardNo = evtSets.getUIntProperty(ApcEvtProp.dwCardNumber)

            };
            return aobj;
        }
        public AdpEvtObj getShareEventObjectFromEventSets (ApacsPropertyObject evtSets)
        {
            string eventType = evtSets.getStringProperty(ApcEvtProp.strEventTypeID);
            AdpEvtObj aeobj = new AdpEvtObj
            {
                Time = evtSets.getDateTimeProperty(ApcEvtProp.dtRealDateTime),
                EventID = evtSets.getStringProperty(ApcEvtProp.SysAddrEventID),
                EventType = eventType,
                EventTypeDesc = getTypeDesc(eventType),
                SourceID = evtSets.getObjectProperty(ApcEvtProp.SysAddrInitObj).getUID(),
                SourceName = evtSets.getStringProperty(ApcEvtProp.strInitObjName)
            };
            return aeobj;
        }
        public AdpCardHolder getCardHolderObjectFromApacsObject(ApacsObject cardHolderAO)
        {
            ApacsPropertyObject holderSets = cardHolderAO.getCurrentSettings();
            string _lastName = holderSets.getStringProperty(ApcEvtProp.strLastName),
                   _middleName = holderSets.getStringProperty(ApcEvtProp.strMiddleName),
                   _firstName = holderSets.getStringProperty(ApcEvtProp.strFirstName);
            ApacsObject[] chldHldrObjPhoto = cardHolderAO.getChildrenObjsByTypes(new string[] { ApcObjType.TApcCHMainPhoto });
            Byte[] photoByteArr = (chldHldrObjPhoto.Length > 0) ?
                chldHldrObjPhoto[0].getCurrentSettings().getByteArrayProperty(ApcEvtProp.binBufPhoto) : null;
            
            AdpCardHolder chObj = new AdpCardHolder
            {
                Photo = (photoByteArr != null) ? Convert.ToBase64String(photoByteArr) : null,
                HolderID = cardHolderAO.getUID(),
                HolderName = _lastName + " " + _firstName + " " + _middleName,
                HolderShortName = holderSets.getStringProperty(ApcObjProp.strName),
                //CardNo = 

            };
            return chObj;
        }
        public List<AdpCardHolder> getCardHoldersFromApacs(ApacsServer apacs)
        {
            List<AdpCardHolder> result = new List<AdpCardHolder>();
            ApacsObject[] cardHolders = apacs.getObjectsByType(ApcObjType.TApcCardHolder);
            foreach (ApacsObject ao in cardHolders)
            {
                result.Add(getCardHolderObjectFromApacsObject(ao));
            }
            return result;
        }
        
    }
}
