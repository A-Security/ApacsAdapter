using System.Runtime.Serialization;

namespace ApacsAdapter
{
    [DataContract]
    public class AdpAPCEvtObj_CHA : AdpAPCEvtObj
    {
        [DataMember]
        public string HolderID { get; set; }
        [DataMember]
        public string HolderName { get; set; }
        [DataMember]
        public string HolderShortName { get; set; }
        [DataMember]
        public uint CardNo { get; set; }
        [DataMember]
        public string HolderCompany { get; set; }
        [DataMember]
        public string HolderJobTitle { get; set; }
    }
}
