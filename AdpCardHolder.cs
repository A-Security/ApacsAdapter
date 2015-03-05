using System;
using System.Xml;
namespace ApacsAdapter
{
    public class AdpCardHolder
    {
        public string ID { get; set; }
        public string Name { get; set; }      
        public string ShortName { get; set; }
        public string CardNo { get; set; }
        public byte[] Photo { get; set; }
    }
}
