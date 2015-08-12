using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;


namespace ApacsAdapter
{   // APACS 3000 generic event Class
    [DataContract]
    public class AdpApcEvtObj
    {
        //Log instance
        private AdpLog log = new AdpLog();
        // Namespace for xml serialization
        private const string NAMESPACE = @"http://schemas.datacontract.org/2004/07/ApacsAdapter";
        // Message type
        public string TYPE 
        { 
            get 
            {
                string _type = @"AccessControl";
                if (Parameters.IsEmpty)
                {
                    _type += "SystemEventRq";
                }
                else
                {
                    _type += "EventRq";
                }
                return _type; 
            }
        }
        // APACS 3000 reader short ID
        [DataMember(EmitDefaultValue = false)]
        public string SourceID { get; set; }
        // APACS 3000 reader Name
        [DataMember(EmitDefaultValue = false)]
        public string SourceName { get; set; }
        // APACS 3000 reader alias (for ApacsAdapterController)
        public string SourceAlias { get; set; }
        // APACS 3000 event short ID
        [DataMember(EmitDefaultValue = false)]
        public string EventID { get; set; }
        // APACS 3000 event type name
        [DataMember(EmitDefaultValue = false)]
        public string EventType { get; set; }
        // APACS 3000 event type description
        [DataMember(EmitDefaultValue = false)]
        public string EventTypeDesc { get; set; }
        // APACS 3000 event time
        [DataMember(EmitDefaultValue = false)]
        public string EventTime { get; set; }
        // Adapter cardholder object
        [DataMember(EmitDefaultValue = false)]
        public AdpCHObj Parameters { get; set; }
        // To JSON serializer
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
        // To XML serializer
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
