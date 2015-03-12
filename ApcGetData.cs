using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ApacsAdapter
{
    public partial class ApcGetData
    {
        private string getTypeDesc(string strType)
        {
            return typeDescDict.TryGetValue(strType, out strType) ? strType : null;
        }
        public string getPropHierarchy(ApacsPropertyObject objSets)
        {
            StringBuilder sb = new StringBuilder();
            string apoName = objSets.hasProperty(ApcObjProp.strName) ? objSets.getStringProperty(ApcObjProp.strName) : "Event";
            string apoAlias = objSets.hasProperty(ApcObjProp.strAlias) ? objSets.getStringProperty(ApcObjProp.strAlias) : "Event";
            sb.AppendLine("================Property=" + apoName + "==Alias=" + apoAlias + "=======");
            foreach (string propName in objSets.getPropertyNames())
            {
                object propObject = objSets.getProperty(propName);
                sb.AppendLine(propName + ": " + propObject);
                if (objSets.getProperty(propName).GetType().IsCOMObject)
                {
                    ApacsObject propCOMObject = objSets.getObjectProperty(propName);
                    sb.AppendLine("===============Object=" + propName + "================");
                    sb.AppendLine("Object Apacs Type: " + propCOMObject.getApacsType());
                    sb.AppendLine(getPropHierarchy(propCOMObject.getCurrentSettings()));
                    foreach (ApacsObject childPropCOMObject in propCOMObject.getChildrenObjs())
                    {
                        sb.AppendLine(getPropHierarchy(childPropCOMObject.getCurrentSettings()));
                    }
                    sb.AppendLine("================END Object=" + propName + "===============");
                }
            }
            sb.AppendLine("================END Property=" + apoName + "============");
            return sb.ToString();
        }
        public AdpEvtObj_CHA getCHAobjFromEvtSet(ApacsPropertyObject evtSets)
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
                EventID = evtSets.getStringProperty(ApcObjProp.SysAddrEventID).Split('.')[1],
                EventType = eventType,
                EventTypeDesc = getTypeDesc(eventType),
                SourceID = evtSets.getObjectProperty(ApcObjProp.SysAddrInitObj).getSampleUID(),
                SourceName = evtSets.getStringProperty(ApcObjProp.strInitObjName),
                HolderID = (isNotErrHolder) ? uid : null,
                HolderName = (isNotErrHolder) ? fullName : "НЕИЗВЕСТНЫЙ",
                HolderShortName = (isNotErrHolder) ? shortName : "НЕИЗВЕСТНЫЙ",
                CardNo = evtSets.getUIntProperty(ApcObjProp.dwCardNumber)

            };
            return aobj;
        }
        public AdpEvtObj getEvtObjFromEvtSet (ApacsPropertyObject evtSets)
        {
            string eventType = evtSets.getStringProperty(ApcObjProp.strEventTypeID);
            AdpEvtObj aeobj = new AdpEvtObj
            {
                Time = evtSets.getDateTimeProperty(ApcObjProp.dtRealDateTime),
                EventID = evtSets.getStringProperty(ApcObjProp.SysAddrEventID).Split('.')[1],
                EventType = eventType,
                EventTypeDesc = getTypeDesc(eventType),
                SourceID = evtSets.getObjectProperty(ApcObjProp.SysAddrInitObj).getSampleUID(),
                SourceName = evtSets.getStringProperty(ApcObjProp.strInitObjName)
            };
            return aeobj;
        }
        public AdpCardHolder getAdpCardHolderFromApacsObject(ApacsObject cardHolderAO)
        {
            if (!ApcObjType.TApcCardHolder.Equals(cardHolderAO.getApacsType()))
            {
                return null;
            }
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
