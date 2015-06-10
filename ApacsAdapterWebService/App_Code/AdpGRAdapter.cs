using System;
using System.Xml.Linq;
using System.Collections.Generic;
using WSO2;
using WSO2.Registry;
using System.Text;
using ApacsAdapter;

public class AdpGRAdapter
{
    private AdpLog log = new AdpLog();
    private const string ARTIFACT_PATH = @"/_system/governance/";
    private const string HOLDERS_PATH = @"ssoi/cardholders/";
    private const string HOLDERS_PHOTO_PATH = HOLDERS_PATH + @"photo/";
    private const string VIPS_PATH = @"ssoi/personcontrol/";
    private string holdersFullPath;
    private string VIPsFullPath;
    private string holdersPhotoFullPath;
    private string serviceUrl;
    private string permaLinkBaseUrl;
    private static RegistryClient registry;
    public AdpGRAdapter(string GRhost, string GRuser, string GRpassword)
    {
        holdersFullPath = ARTIFACT_PATH + HOLDERS_PATH;
        VIPsFullPath = ARTIFACT_PATH + VIPS_PATH;
        holdersPhotoFullPath = ARTIFACT_PATH + HOLDERS_PHOTO_PATH;
        serviceUrl = String.Format(@"https://{0}:9443/services/", GRhost);
        permaLinkBaseUrl = String.Format(@"http://{0}:9763/registry/resource", GRhost);
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
    public bool fillGRfromApacsCH(ApcServer apacsInstance)
    {
        try
        {
            clearCollection(holdersFullPath);
            ApcData agd = new ApcData();
            foreach (AdpCHObj ch in agd.getAdpCHObjs(apacsInstance))
            {
                putCardHolder(ch);
            }
        }
        catch (Exception e)
        {
            log.AddLog(e.ToString());
            return false;
        }
        return true;
    }
    public void putCardHolder(AdpCHObj ch)
    {
        string holderVipValue = IsVIP(ch.ID).ToString().ToLower();
        string holderResName = ch.ID + ".xml";
        string holderPhotoResName = ch.ID + ".jpg";
        string holderPhotoPath = @"/" + HOLDERS_PHOTO_PATH + holderPhotoResName;
        Resource holderPhotoRes = registry.NewResource();
        holderPhotoRes.mediaType = "image/jpeg";
        holderPhotoRes.contentFile = ch.Photo;
        holderPhotoRes.name = holderPhotoResName;
        string holderPhotoResPath = holdersPhotoFullPath + holderPhotoResName;
        holderPhotoRes.properties = new WSProperty[]
                    {
                        new WSProperty(){ key = "holder_id", values = new string[] { ch.ID } },
                        new WSProperty(){ key = "holder_shortName", values = new string[] { ch.ShortName } },
                        new WSProperty(){ key = "holder_name", values = new string[] { ch.Name } },
                        new WSProperty(){ key = "holder_cardNo", values = new string[] { ch.CardNo } },
                        new WSProperty(){ key = "holder_path", values = new string[] { @"/" + HOLDERS_PATH + holderResName } },
                        new WSProperty(){ key = "holder_vip", values = new string[] { holderVipValue } }
                    };
        registry.Put(holderPhotoResPath, holderPhotoRes);
        Resource holderRes = registry.NewResource();
        holderRes.mediaType = "application/vnd.cardholders+xml";
        holderRes.name = holderResName;
        string holderPhotoLinkValue = permaLinkBaseUrl + holderPhotoResPath;
        XNamespace xn = @"http://www.wso2.org/governance/metadata";
        XElement holderResContentXDoc =
            new XElement(xn + "metadata",
                new XElement(xn + "holder",
                    new XElement(xn + "id", ch.ID),
                    new XElement(xn + "shortName", ch.ShortName),
                    new XElement(xn + "name", ch.Name),
                    new XElement(xn + "cardNo", ch.CardNo),
                    new XElement(xn + "photo", holderPhotoPath),
                    new XElement(xn + "photoLink", holderPhotoLinkValue),
                    new XElement(xn + "vip", holderVipValue)
                )
            );
        holderRes.contentFile = Encoding.UTF8.GetBytes(holderResContentXDoc.ToString());
        holderRes.properties = new WSProperty[]
                    {
                        new WSProperty(){ key = "holder_id", values = new string[] { ch.ID } },
                        new WSProperty(){ key = "holder_shortName", values = new string[] { ch.ShortName } },
                        new WSProperty(){ key = "holder_name", values = new string[] { ch.Name } },
                        new WSProperty(){ key = "holder_cardNo", values = new string[] { ch.CardNo } },
                        new WSProperty(){ key = "holder_photo", values = new string[] { holderPhotoPath } },
                        new WSProperty(){ key = "holder_photoLink", values = new string[] { holderPhotoLinkValue } },
                        new WSProperty(){ key = "holder_vip", values = new string[] { holderVipValue } }
                    };
        registry.Put(holdersFullPath + holderResName, holderRes);
    }
    private bool IsVIP(string id)
    {
        string VIPpath = VIPsFullPath + id;
        return registry.ResourceExists(VIPpath); ;
    }
    public void removeCardHolder(string id)
    {
        string chPath = holdersFullPath + id + ".xml";
        string chPhotoPath = holdersPhotoFullPath + id + ".jpg";
        if (registry.ResourceExists(chPath))
        {
            registry.Delete(chPath);
        }
        if (registry.ResourceExists(chPhotoPath))
        {
            registry.Delete(chPhotoPath);
        }
    }
}
