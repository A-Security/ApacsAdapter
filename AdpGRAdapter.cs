using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSO2;
using WSO2.Registry;

namespace ApacsAdapter
{
    public class AdpGRAdapter
    {
        private const string BASE_PATH = "/testDotNet";
        private const string SERVER_URL = "https://192.168.0.151:9443/services/";
        private const string USERNAME = "admin";
        private const string PASSWORD = "Flvbybcnhfnjh1";

        private static RegistryClient registry;
        public AdpGRAdapter()
        {
            registry = new RegistryClient(USERNAME, PASSWORD, SERVER_URL);
        }
        public List<string> getResourceFromRegistry()
        {

            List<string> result = new List<string>();
            Collection cardHolderCollection = registry.Get(@"/_system/governance/ssoi/cardholders", 0, int.MaxValue);
            result = cardHolderCollection.children.ToList<string>();
            clearCollection(cardHolderCollection);
            //Resource resource = registry.Get(@"/_system/governance/ssoi/cardholders/Донг Й. Х.");
            //string temp = Encoding.UTF8.GetString(resource.contentFile);
            //string tempWrk = Encoding.UTF8.GetString(registry.GetContent(@"/_system/governance/ssoi/cardholders/Донг Й. Х."));
            //foreach (WSProperty prop in resource.properties)
            //{
            //    StringBuilder sb = new StringBuilder();
            //    foreach (string str in prop.values)
            //    {
            //        sb.AppendLine(str);
            //    }
            //    result.Add(prop.key + ": " + sb.ToString());
            //}
            
            return result;

        }
        public void clearCollection(Collection collect)
        {
            foreach (string res in collect.children)
            {
                registry.Delete(res);
            }
        }
        public void addCollection(Collection collect)
        {

        }
    }
}
