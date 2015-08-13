using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
namespace ApacsHelper
{
    // APACS 3000 cardholder Class
    [DataContract]
    public class AdpCHObj
    {
        public bool IsEmpty
        {
            get
            {
                return (String.IsNullOrEmpty(HolderID) 
                        && String.IsNullOrEmpty(HolderName) 
                        && String.IsNullOrEmpty(HolderShortName) 
                        && String.IsNullOrEmpty(CardNo)
                        && HolderPhoto == null 
                        && String.IsNullOrEmpty(HolderJobTitle) 
                        && String.IsNullOrEmpty(HolderCategory) 
                        && String.IsNullOrEmpty(HolderCompany));
            }
        }

        // enumeration of possible message types
        public enum ModType { AddRq, ModRq, DelRq }
        //Log instance
        private AdpLog log = new AdpLog();
        // Namespace for xml serialization
        private const string NAMESPACE = @"http://schemas.datacontract.org/2004/07/ApacsAdapter";
        // Instance message type
        private ModType _modType = ModType.ModRq;
        public ModType modType { set { _modType = value; } }

        /* Instance message type for XML serialization
         * Possible return value:
         *   CardHolderAddRq - add new cardholder in APACS 3000
         *   CardHolderModRq - modification cardholder in APACS 3000
         *   CardHolderDelRq - delete cardholder in APACS 3000
         */
        public string TYPE 
        { 
            get 
            {
                string mod = @"CardHolder";
                switch (_modType)
                {
                    case ModType.AddRq:
                        {
                            mod += "AddRq";
                            break;
                        }
                    case ModType.ModRq:
                        {
                            mod += "ModRq";
                            break;
                        }
                    case ModType.DelRq:
                        {
                            mod += "DelRq";
                            break;
                        }
                }
                return mod; 
            }
        }
        // APACS 3000 cardholder short ID
        [DataMember(EmitDefaultValue = false)]
        public string HolderID { get; set; }
        // APACS 3000 cardholder full name
        [DataMember(EmitDefaultValue = false)]
        public string HolderName { get; set; }
        // APACS 3000 cardholder short name
        [DataMember(EmitDefaultValue = false)]
        public string HolderShortName { get; set; }
        // APACS 3000 cardholder card number
        [DataMember(EmitDefaultValue = false)]
        public string CardNo { get; set; }
        // APACS 3000 cardholder photo 
        [DataMember(EmitDefaultValue = false)]
        public byte[] HolderPhoto { get; set; }
        // APACS 3000 cardholder job title
        [DataMember(EmitDefaultValue = false)]
        public string HolderJobTitle { get; set; }
        /* APACS 3000 cardholder category
         * Possible value:
         *   'false' - cardholder is employeer
         *   'true'  - cardholder is guest
         *   NULL or String.Empty - cardholder is undefined
         */
        [DataMember(EmitDefaultValue = false)]
        public string HolderCategory { get; set; }
        // APACS 3000 cardholder company title
        [DataMember(EmitDefaultValue = false)]
        public string HolderCompany { get; set; }
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
