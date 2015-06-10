using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
namespace ApacsAdapter
{
    [DataContract]
    public class AdpCHObj
    {
        private AdpLog log = new AdpLog();
        private const string NAMESPACE = @"http://schemas.datacontract.org/2004/07/ApacsAdapter";
        private int _ModType = 0;
        public int ModType
        {
            set
            {
                if (value >= -1 & value <= 1)
                {
                    _ModType = value;
                }
            }
        }
        public string TYPE 
        { 
            get 
            {
                string mod = @"CardHolder";
                switch (_ModType)
                {
                    case -1:
                        {
                            mod += "DelRq";
                            break;
                        }
                    case 0:
                        {
                            mod += "ModRq";
                            break;
                        }
                    case 1:
                        {
                            mod += "AddRq";
                            break;
                        }
                }
                return mod; 
            }
        }
        [DataMember(EmitDefaultValue = false)]
        public string HolderID { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string HolderName { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string HolderShortName { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string CardNo { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public byte[] HolderPhoto { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string HolderJobTitle { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string HolderCategory { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public string HolderCompany { get; set; }
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
