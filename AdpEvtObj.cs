using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;


namespace ApacsAdapter
{
    [DataContract]
    public class AdpEvtObj
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
                return null; 
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
                return null;
            }
        }
    }
}
