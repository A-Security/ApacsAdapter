using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using WSO2;
using WSO2.Registry;

namespace ApacsAdapter
{
    public class AdpGRAdapter
    {
        private string artifactPath = @"/_system/governance";
        private string holdersPath = @"/ssoi/cardholders";
        private string holdersFullPath;
        private string holdersPhotoPath;
        private string holdersPhotoFullPath;
        private string holdersPhotoPermaLinkUrl;
        private string serviceUrl;
        private string permaLinkBaseUrl;
        private static RegistryClient registry;
        public AdpGRAdapter(string GRhost, string GRuser, string GRpassword)
        {

            serviceUrl = @"https://" + GRhost + @":9443/services/";
            permaLinkBaseUrl = @"http://" + GRhost + @":9763/registry/resource";
            holdersFullPath = artifactPath + holdersPath;
            holdersPhotoPath = holdersPath + "/photo";
            holdersPhotoFullPath = artifactPath + holdersPhotoPath;
            holdersPhotoPermaLinkUrl = permaLinkBaseUrl + holdersPhotoFullPath;
            registry = new RegistryClient(GRuser, GRpassword, serviceUrl);
        }
        public List<string> getListStringCHs()
        {
            List<string> result = new List<string>();
            StringBuilder singleResult = new StringBuilder();
            Collection cardHolderCollection = (Collection)registry.Get(holdersFullPath);
            Resource res;
            foreach (string str in cardHolderCollection.children)
            {
                singleResult.Clear();
                res = registry.Get(str);
                singleResult.AppendLine(String.Format("======================START {0} ======================", res.name));
                singleResult.AppendLine(String.Format("======================START {0} Property==============", res.name));
                foreach (WSProperty prop in res.properties)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (string strv in prop.values)
                    {
                        sb.AppendLine(strv);
                    }
                    singleResult.AppendLine(String.Format("{0} = {1}", prop.key, sb.ToString()));
                }
                singleResult.AppendLine(String.Format("========================END {0} Property==============", res.name));
                singleResult.AppendLine(String.Format("======================START {0} Content==============", res.name));
                singleResult.AppendLine(Encoding.UTF8.GetString(res.contentFile));
                singleResult.AppendLine(String.Format("========================END {0} Content==============", res.name));
                singleResult.AppendLine(String.Format("========================END {0} ======================", res.name));
                result.Add(singleResult.ToString());
            }
            return result;
        }
        public void clearCollection(string collPath)
        {
            Collection coll = (Collection)registry.Get(collPath);
            foreach (string res in coll.children)
            {
                if (registry.Get(res).collection)
                {
                    clearCollection(res);
                }
                registry.Delete(res);
            }
            
        }
        public bool copyCHfromApacs(ApacsServer apacsInstance)
        {
            try
            {
                clearCollection(holdersFullPath);
                ApcGetDate agd = new ApcGetDate();
                Resource resource;
                foreach (AdpCardHolder ch in agd.getCardHoldersFromApacs(apacsInstance))
                {
                    resource = registry.NewResource();
                    resource.mediaType = "image/jpeg";
                    resource.contentFile = ch.Photo;
                    resource.name = ch.ID + ".jpg";
                    string resPath = holdersPhotoFullPath + @"/" + resource.name;
                    registry.Put(resPath, resource);
                    resource = registry.NewResource();
                    resource.mediaType = "application/vnd.cardholders+xml";
                    resource.name = ch.ID + ".xml";
                    resource.contentFile = CHtoGRcontent(ch);
                    resPath = holdersFullPath + @"/" + resource.name;
                    //resCH.properties = new WSProperty[]
                    //    {
                    //        new WSProperty(){ key = "holder_id", values = new string[] { ch.ID } },
                    //        new WSProperty(){ key = "holder_name", values = new string[] { ch.Name } },
                    //        new WSProperty(){ key = "holder_shortName", values = new string[] { ch.ShortName } },
                    //        new WSProperty(){ key = "holder_cardNo", values = new string[] { ch.CardNo.ToString() } },
                    //        new WSProperty(){ key = "holder_vip", values = new string[] { bool.FalseString } }
                    //    };
                    registry.Put(resPath, resource);
                }
            }
            catch (Exception) 
            {
                return false;
            }
            return true;
        }
        public byte[] CHtoGRcontent(AdpCardHolder ch)
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
            idNode.InnerText = ch.ID;
            shortNameNode.InnerText = ch.ShortName;
            nameNode.InnerText = ch.Name;
            cardNoNode.InnerText = ch.CardNo;
            string photoResName = @"/" + ch.ID + ".jpg";
            photoNode.InnerText = holdersPhotoPath + photoResName;
            photoLinkNode.InnerText = holdersPhotoPermaLinkUrl + photoResName;
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
            string result = xdoc.OuterXml.Replace(" xmlns=\"\"", String.Empty);
            return Encoding.UTF8.GetBytes(result);
        }
    }
}
