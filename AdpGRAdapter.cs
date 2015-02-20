using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSO2;

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
            Resource resource = registry.Get(@"/_system/governance/ssoi/cardholders/Донг Й. Х.");
            string temp = Encoding.UTF8.GetString(resource.contentFile);
            string tempWrk = Encoding.UTF8.GetString(registry.GetContent(@"/_system/governance/ssoi/cardholders/Донг Й. Х."));
            List<string> result = new List<string>();
            foreach (WSO2.Registry.WSProperty prop in resource.properties)
            {
                StringBuilder sb = new StringBuilder();
                foreach (string str in prop.values)
                {
                    sb.AppendLine(str);
                }
                result.Add(prop.key + ": " + sb.ToString());
            }
            
            return result;

        }
    }
}
