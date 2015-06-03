using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;


namespace ApacsAdapter
{
    [DataContract]
    public class AdpAPCEvtObj
    {
        private AdpLog log = new AdpLog();
        private const string NAMESPACE = @"http://schemas.datacontract.org/2004/07/ApacsAdapter";
        [DataMember]
        public DateTime Time { get; set; }
        [DataMember]
        public string SourceID { get; set; }
        [DataMember]
        public string SourceName { get; set; }
        [DataMember]
        public string EventID { get; set; }
        [DataMember]
        public string EventType { get; set; }
        [DataMember]
        public string EventTypeDesc { get; set; }
        [DataMember]
        public string HolderID { get; set; }
        [DataMember]
        public string HolderName { get; set; }
        [DataMember]
        public string HolderShortName { get; set; }
        [DataMember]
        public string CardNo { get; set; }
        [DataMember]
        public string HolderCompany { get; set; }
        [DataMember]
        public string HolderJobTitle { get; set; }
        [DataMember]
        public string HolderCategory { get; set; }
        public string ToJsonString()
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(this.GetType(), EventType);
                jsonSerializer.WriteObject(ms, this);
                ms.Position = 0;
                return new StreamReader(ms).ReadToEnd();
            }
            catch (Exception e) 
            {
                log.AddLog(e.ToString());
                return String.Empty; 
            }
        }
        public string ToXmlString()
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                DataContractSerializer xmlSerializer = new DataContractSerializer(this.GetType(), EventType, NAMESPACE);
                xmlSerializer.WriteObject(ms, this);
                ms.Position = 0;
                return new StreamReader(ms).ReadToEnd();
            }
            catch (Exception e)
            {
                log.AddLog(e.ToString());
                return String.Empty;
            }
        }
    }
}
