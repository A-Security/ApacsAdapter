using System.Text;

namespace ApacsAdapter
{
    public partial class ApcData
    {       
        public string getPropHierarchy(ApcPropObj objSets)
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
        public AdpAPCEvtObj getEvtObj(ApcPropObj evtSets)
        {
            string eventType = evtSets.getEventTypeProperty();
            ApcObj holder = evtSets.getObjectProperty(ApcObjProp.SysAddrHolder);
            ApcPropObj holderSets = holder.getCurrentSettings();
            
            AdpAPCEvtObj aobj = new AdpAPCEvtObj
            {
                Time = evtSets.getRealDateTime().ToString("yyyy-MM-dd HH:mm:ss.fff"),
                EventID = evtSets.getSampleEventUID(),
                EventType = eventType,
                EventTypeDesc = getTypeDesc(eventType),
                SourceID = evtSets.getSampleSourceUID(),
                SourceName = evtSets.getSourceNameProperty(),
                HolderID = holder.getSampleUID(),
                HolderName = holderSets.getFullNameProperty(),
                HolderShortName = holderSets.getNameProperty(),
                HolderCompany = holderSets.getCompanyNameProperty(),
                HolderJobTitle = holderSets.getJobTitleNameProperty(),
                HolderCategory = holderSets.getbEmployeeProperty(),
                CardNo = evtSets.getDwCardNumber()

            };
            return aobj;
        }
        public AdpCHObj getCardHolder(ApcObj cardHolderAO)
        {
            if (!ApcObjType.TApcCardHolder.Equals(cardHolderAO.getApacsType()))
            {
                return null;
            }
            ApcPropObj holderSets = cardHolderAO.getCurrentSettings();
            AdpCHObj chObj = new AdpCHObj
            {
                Photo = cardHolderAO.getMainPhoto(),
                ID = cardHolderAO.getSampleUID(),
                Name = holderSets.getFullNameProperty(),
                ShortName = holderSets.getNameProperty(),
                CardNo = cardHolderAO.getCardNumber()
            };
            return chObj;
        }
        public AdpCHObj[] getCardHolders(ApcServer apacsInstance)
        {
            ApcObj[] cardHolders = apacsInstance.getObjectsByType(ApcObjType.TApcCardHolder);
            AdpCHObj[] result = new AdpCHObj[cardHolders.Length];
            for (int i = 0; i < result.Length; i++ )
            {
                result[i] = getCardHolder(cardHolders[i]);
            }
            return result;
        }
        public AdpCHObj getCardHolderByUID(ApcServer apacsInstance, string SampleUID)
        {
            ApcObj ch = apacsInstance.getObjectBySampleUID(SampleUID);
            return getCardHolder(ch);
        }
    }
}
