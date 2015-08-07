using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;


namespace ApacsAdapter
{
    [DataContract]
    public class AdpApcEvtObj
    {
        private AdpLog log = new AdpLog();
        private const string NAMESPACE = @"http://schemas.datacontract.org/2004/07/ApacsAdapter";
        public string TYPE { get { return @"AccessControlEventRq"; } }
        [DataMember(EmitDefaultValue = false)]
        public string SourceID { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string SourceName { get; set; }
        public string SourceAlias { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string EventID { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string EventType { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string EventTypeDesc { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string EventTime { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public AdpCHObj Parameters { get; set; }
        public string ToJsonString()
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(this.GetType());
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
                DataContractSerializer xmlSerializer = new DataContractSerializer(this.GetType(), TYPE, NAMESPACE);
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
