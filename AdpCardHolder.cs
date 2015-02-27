using System;
using System.Xml;
namespace ApacsAdapter
{
    public class AdpCardHolder
    {
        public byte[] Photo { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }      
        public string ShortName { get; set; }
        public string CardNo { get; set; }
        public string ToGRxmlContent(AdpConfigXml cfg) 
        {

            XmlDocument xdoc = new XmlDocument();
            XmlElement metaNode = xdoc.CreateElement("metadata", @"http://www.wso2.org/governance/metadata");
            XmlElement holdNode = xdoc.CreateElement("holder");
            XmlElement idNode = xdoc.CreateElement("id");
            XmlElement shortNameNode = xdoc.CreateElement("shortName");
            XmlElement nameNode = xdoc.CreateElement("name");
            XmlElement cardNoNode = xdoc.CreateElement("cardNo");
            XmlElement photoNode = xdoc.CreateElement("photo");
            XmlElement photoLinkNode = xdoc.CreateElement("photoLink");
            XmlElement vipNode = xdoc.CreateElement("vip");
            idNode.InnerText = ID;
            shortNameNode.InnerText = ShortName;
            nameNode.InnerText = Name;
            cardNoNode.InnerText = CardNo;
            string photoResName = @"/" + ID + ".jpg";
            photoNode.InnerText = cfg.GRholdersPhotoPath + photoResName;
            photoLinkNode.InnerText = cfg.GRholdersPhotoPermaLinkUrl + photoResName;
            vipNode.InnerText = bool.FalseString;
            holdNode.AppendChild(idNode);
            holdNode.AppendChild(shortNameNode);
            holdNode.AppendChild(nameNode);
            holdNode.AppendChild(cardNoNode);
            holdNode.AppendChild(photoNode);
            holdNode.AppendChild(photoLinkNode);
            holdNode.AppendChild(vipNode);
            metaNode.AppendChild(holdNode);
            xdoc.AppendChild(metaNode);
            return xdoc.OuterXml.Replace(" xmlns=\"\"", String.Empty);
        }
    }
}
