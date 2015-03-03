﻿using System;
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
        public string getPropertiesHierarchy(ApacsPropertyObject objSettings)
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
                    sb.AppendLine(getPropertiesHierarchy(propCOMObject.getCurrentSettings()));
                    foreach (ApacsObject childPropCOMObject in propCOMObject.getChildrenObjs())
                    {
                        sb.AppendLine(getPropertiesHierarchy(childPropCOMObject.getCurrentSettings()));
                    }
                    sb.AppendLine("================END Object=" + propName + "===============");
                }
            }
            sb.AppendLine("================END Property=" + apoName + "============");
            return sb.ToString();
        }
        public AdpEvtObj_CHA getCardHolderEventObjectFromEventSets(ApacsPropertyObject evtSets)
        {
            string eventType = evtSets.getStringProperty(ApcObjProp.strEventTypeID);
            if (eventType.Contains("Will"))
            {
                return null;
            }
            bool isNotErrHolder = !(eventType.EndsWith("_ErrHolder"));
            string fullName = null,
                    shortName = null,
                    uid = null;
            if (isNotErrHolder)
            {
                ApacsObject holder = evtSets.getObjectProperty(ApcObjProp.SysAddrHolder);
                ApacsPropertyObject holderSets = holder.getCurrentSettings();
                fullName = holderSets.getFullNameProperty();
                shortName = holderSets.getNameProperty();
                uid = holder.getSampleUID();
            }
            AdpEvtObj_CHA aobj = new AdpEvtObj_CHA
            {
                Time = evtSets.getDateTimeProperty(ApcObjProp.dtRealDateTime),
                EventID = evtSets.getStringProperty(ApcObjProp.SysAddrEventID),
                EventType = eventType,
                EventTypeDesc = getTypeDesc(eventType),
                SourceID = evtSets.getObjectProperty(ApcObjProp.SysAddrInitObj).getUID(),
                SourceName = evtSets.getStringProperty(ApcObjProp.strInitObjName),
                HolderID = (isNotErrHolder) ? uid : null,
                HolderName = (isNotErrHolder) ? fullName : "НЕИЗВЕСТНЫЙ",
                HolderShortName = (isNotErrHolder) ? shortName : "НЕИЗВЕСТНЫЙ",
                CardNo = evtSets.getUIntProperty(ApcObjProp.dwCardNumber)

            };
            return aobj;
        }
        public AdpEvtObj getCommonEventObjectFromEventSets (ApacsPropertyObject evtSets)
        {
            string eventType = evtSets.getStringProperty(ApcObjProp.strEventTypeID);
            AdpEvtObj aeobj = new AdpEvtObj
            {
                Time = evtSets.getDateTimeProperty(ApcObjProp.dtRealDateTime),
                EventID = evtSets.getStringProperty(ApcObjProp.SysAddrEventID),
                EventType = eventType,
                EventTypeDesc = getTypeDesc(eventType),
                SourceID = evtSets.getObjectProperty(ApcObjProp.SysAddrInitObj).getUID(),
                SourceName = evtSets.getStringProperty(ApcObjProp.strInitObjName)
            };
            return aeobj;
        }
        public AdpCardHolder getAdpCardHolderFromApacsObject(ApacsObject cardHolderAO)
        {
            ApacsPropertyObject holderSets = cardHolderAO.getCurrentSettings();
            AdpCardHolder chObj = new AdpCardHolder
            {
                Photo = cardHolderAO.getMainPhoto(),
                ID = cardHolderAO.getSampleUID(),
                Name = holderSets.getFullNameProperty(),
                ShortName = holderSets.getNameProperty(),
                CardNo = cardHolderAO.getCardNumber()
            };
            return chObj;
        }
        public AdpCardHolder[] getCardHoldersFromApacs(ApacsServer apacsInstance)
        {
            ApacsObject[] cardHolders = apacsInstance.getObjectsByType(ApcObjType.TApcCardHolder);
            AdpCardHolder[] result = new AdpCardHolder[cardHolders.Length];
            for (int i = 0; i < result.Length; i++ )
            {
                result[i] = getAdpCardHolderFromApacsObject(cardHolders[i]);
            }
                
            return result;
        }
        
    }
}
