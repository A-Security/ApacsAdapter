using System.Text;

namespace ApacsHelper
{
    // APACS 3000 API helper class
    public partial class ApcData
    {   
        // Get APACS 3000 property object in text
        public string getPropHierarchy(ApcPropObj objSets)
        {
            StringBuilder sb = new StringBuilder();
            string apoName = objSets.isProperty(ApcObjProp.strName) ? objSets.getStringProperty(ApcObjProp.strName) : "Event";
            string apoAlias = objSets.isProperty(ApcObjProp.strAlias) ? objSets.getStringProperty(ApcObjProp.strAlias) : "Event";
            sb.AppendLine("================Property=" + apoName + "==Alias=" + apoAlias + "=======");
            foreach (string propName in objSets.getPropertyNames())
            {
                object propObject = objSets.getProperty(propName);
                sb.AppendLine(propName + ": " + propObject);
                if (objSets.getProperty(propName).GetType().IsCOMObject)
                {
                    ApcObj propCOMObject = objSets.getObjectProperty(propName);
                    sb.AppendLine("===============Object=" + propName + "================");
                    sb.AppendLine("Object Apacs Type: " + propCOMObject.getApacsType());
                    sb.AppendLine(getPropHierarchy(propCOMObject.getCurrentSettings()));
                    foreach (ApcObj childPropCOMObject in propCOMObject.getChildrenObjs())
                    {
                        sb.AppendLine(getPropHierarchy(childPropCOMObject.getCurrentSettings()));
                    }
                    sb.AppendLine("================END Object=" + propName + "===============");
                }
            }
            sb.AppendLine("================END Property=" + apoName + "============");
            return sb.ToString();
        }
        // Create event class instance from APACS 3000 property object for APACS 3000 event object
        public AdpApcEvtObj mapAdpApcEvtObj(ApcPropObj evtSets, bool withCardHolderPhoto = false)
        {
            string eventType = evtSets.getEventTypeProperty();
            ApcObj holder = evtSets.getSysAddrHolderProperty();
            AdpApcEvtObj aobj = new AdpApcEvtObj
            {
                EventTime = evtSets.getRealDateTime().ToString("yyyy-MM-dd HH:mm:ss.fff"),
                EventID = evtSets.getSampleEventUID(),
                EventType = eventType,
                EventTypeDesc = getTypeDesc(eventType),
                SourceID = evtSets.getSampleSourceUID(),
                SourceAlias = evtSets.getSourceAlias(),
                SourceName = evtSets.getSourceNameProperty(),
                Parameters = mapAdpCHObj(holder, withCardHolderPhoto)
            };
            return aobj;
        }
        // Create cardholder class instance from APACS 3000 TApcCardHolder object
        public AdpCHObj mapAdpCHObj(ApcObj cardHolderAO, bool withPhoto = true)
        {
            if (!ApcObjType.TApcCardHolder.Equals(cardHolderAO.getApacsType()))
            {
                return new AdpCHObj();
            }
            ApcPropObj holderSets = cardHolderAO.getCurrentSettings();
            AdpCHObj chObj = new AdpCHObj
            {
                HolderPhoto = withPhoto ? cardHolderAO.getMainPhoto() : new byte[] { },
                HolderID = cardHolderAO.getSampleUID(),
                HolderName = holderSets.getFullNameProperty(),
                HolderShortName = holderSets.getNameProperty(),
                HolderCompany = holderSets.getCompanyNameProperty(),
                HolderJobTitle = holderSets.getJobTitleNameProperty(),
                HolderCategory = holderSets.getbEmployeeProperty(),
                CardNo = cardHolderAO.getCardNumber()
            };
            return chObj;
        }
        // return all cardholders object from APACS 3000
        public AdpCHObj[] getAdpCHObjs(ApcServer apacsInstance)
        {
            ApcObj[] cardHolders = apacsInstance.getObjectsByType(ApcObjType.TApcCardHolder);
            AdpCHObj[] result = new AdpCHObj[cardHolders.Length];
            for (int i = 0; i < result.Length; i++ )
            {
                result[i] = mapAdpCHObj(cardHolders[i]);
            }
            return result;
        }
        // return cardholder object from APACS 3000 by sample APACS 3000 cardholder UID
        public AdpCHObj getCardHolderByUID(ApcServer apacsInstance, string SampleUID)
        {
            ApcObj ch = apacsInstance.getObjectBySampleUID(SampleUID);
            return mapAdpCHObj(ch);
        }
    }
}
