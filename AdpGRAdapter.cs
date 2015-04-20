using System;
using System.Xml.Linq;
using System.Collections.Generic;
using WSO2;
using WSO2.Registry;
using System.Text;

namespace ApacsAdapter
{
    public class AdpGRAdapter
    {
        private AdpLog log = new AdpLog();
        private const string ARTIFACT_PATH = @"/_system/governance";
        private const string HOLDERS_PATH = @"/ssoi/cardholders";
        private const string HOLDERS_PHOTO_PATH = HOLDERS_PATH + @"/photo";
        private const string VIPS_PATH = @"/ssoi/personcontrol";
        private string holdersFullPath = ARTIFACT_PATH + HOLDERS_PATH;
        private string VIPsFullPath = ARTIFACT_PATH + VIPS_PATH;
        private string holdersPhotoFullPath = ARTIFACT_PATH + HOLDERS_PHOTO_PATH;
        private string holdersPhotoPermaLinkUrl;
        private string serviceUrl;
        private string permaLinkBaseUrl;
        private static RegistryClient registry;
        public AdpGRAdapter(string GRhost, string GRuser, string GRpassword)
        {
            serviceUrl = String.Format(@"https://{0}:9443/services/", GRhost);
            permaLinkBaseUrl = String.Format(@"http://{0}:9763/registry/resource", GRhost);
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
                ApcGetData agd = new ApcGetData();
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
                    resource.properties = new WSProperty[]
                    {
                        new WSProperty(){ key = "holder_id", values = new string[] { ch.ID } },
                        new WSProperty(){ key = "holder_name", values = new string[] { ch.Name } },
                        new WSProperty(){ key = "holder_shortName", values = new string[] { ch.ShortName } },
                        new WSProperty(){ key = "holder_cardNo", values = new string[] { ch.CardNo } },
                        new WSProperty(){ key = "holder_vip", values = new string[] { IsVIP(ch.ID).ToString().ToLower() } }
                    };
                    registry.Put(resPath, resource);
                }
            }
            catch (Exception e)
            {
                log.AddLog(e.ToString());
                return false;
            }
            return true;
        }
        public byte[] CHtoGRcontent(AdpCardHolder ch)
        {
            string photoResName = String.Format(@"/{0}.jpg", ch.ID);
            XNamespace xn = @"http://www.wso2.org/governance/metadata";
            XElement xdoc =
                new XElement(xn + "metadata",
                    new XElement(xn + "holder",
                        new XElement(xn + "id", ch.ID),
                        new XElement(xn + "shortName", ch.ShortName),
                        new XElement(xn + "name", ch.Name),
                        new XElement(xn + "cardNo", ch.CardNo),
                        new XElement(xn + "photo", HOLDERS_PHOTO_PATH + photoResName),
                        new XElement(xn + "photoLink", holdersPhotoPermaLinkUrl + photoResName),
                        new XElement(xn + "vip", IsVIP(ch.ID).ToString().ToLower())
                    )
                );
            return Encoding.UTF8.GetBytes(xdoc.ToString());
        }
        private bool IsVIP(string id)
        {
            string VIPpath = VIPsFullPath + @"/" + id;
            return registry.ResourceExists(VIPpath); ;
        }
    }
}
