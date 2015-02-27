using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WSO2;
using WSO2.Registry;

namespace ApacsAdapter
{
    public class AdpGRAdapter
    {
        private readonly AdpConfigXml cfg;
        private static RegistryClient registry;
        public AdpGRAdapter(AdpConfigXml cfg)
        {
            this.cfg = cfg;
            registry = new RegistryClient(cfg.GRuser, cfg.GRpassword, cfg.GRserviceUrl);
        }
        public List<string> getResourceFromRegistry()
        {
            List<string> result = new List<string>();
            StringBuilder singleResult = new StringBuilder();
            Collection cardHolderCollection = (Collection)registry.Get(cfg.GRholdersFullPath);
            foreach (string str in cardHolderCollection.children)
            {
                singleResult.Clear();
                Resource res = registry.Get(str);
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
        public void fillGRfromApacs()
        {
            clearCollection(cfg.GRholdersFullPath);
            ApacsServer apacsInstance = new ApacsServer(cfg.apcLogin, cfg.apcPasswd);
            ApcGetDate agd = new ApcGetDate(apacsInstance);
            foreach (AdpCardHolder ch in agd.getCardHoldersFromApacs())
            {
                Resource resource = registry.NewResource();
                resource.mediaType = "image/jpeg";
                resource.contentFile = ch.Photo;
                resource.name = ch.ID + ".jpg";
                string resPath = cfg.GRholdersPhotoFullPath + @"/" + resource.name;
                registry.Put(resPath, resource);
                resource = registry.Get(resPath);
                
                resource = registry.NewResource();
                resource.mediaType = "application/vnd.cardholders+xml";
                resource.name = ch.ID + ".xml";
                
                XDocument xdoc = new XDocument();
                
                resource.contentFile = Encoding.UTF8.GetBytes(ch.ToGRxmlContent(cfg));
                resPath = cfg.GRholdersFullPath + @"/" + resource.name;
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
    }
}
