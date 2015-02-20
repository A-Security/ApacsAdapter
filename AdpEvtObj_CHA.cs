using System.Runtime.Serialization;

namespace ApacsAdapter
{
    [DataContract]
    public class AdpEvtObj_CHA : AdpEvtObj
    {
        [DataMember]
        public string HolderID { get; set; }
        [DataMember]
        public string HolderName { get; set; }
        [DataMember]
        public string HolderShortName { get; set; }
        [DataMember]
        public uint CardNo { get; set; }
    }
}
