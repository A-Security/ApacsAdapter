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
                Resource resPhoto = registry.NewResource();
                resPhoto.mediaType = "image/jpeg";
                Resource resCH = registry.NewResource();
                resCH.mediaType = "application/vnd.cardholders+xml";
                resPhoto.contentFile = ch.Photo;
                resPhoto.name = ch.ID + ".jpg";
                string resPhotoPath = cfg.GRholdersPhotoFullPath + @"/" + resPhoto.name;
                registry.Put(resPhotoPath, resPhoto);
                resCH.name = ch.ID + ".xml";
                XDocument xdoc = new XDocument();
                resCH.contentFile = Encoding.UTF8.GetBytes(ch.ToGRxmlContent(cfg));
                string resCardholderPath = cfg.GRholdersFullPath + @"/" + resCH.name;
                resCH.properties = new WSProperty[]
                    {
                        new WSProperty(){ key = "id", values = new string[] { ch.ID } },
                        new WSProperty(){ key = "name", values = new string[] { ch.Name } },
                        new WSProperty(){ key = "shortName", values = new string[] { ch.ShortName } },
                        new WSProperty(){ key = "cardNo", values = new string[] { ch.CardNo.ToString() } },
                        new WSProperty(){ key = "vip", values = new string[] { bool.FalseString } }
                    };
                registry.Put(resCardholderPath, resCH);
            }
        }
    }
}
