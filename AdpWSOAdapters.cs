using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using WSO2;
using WSO2.Registry;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;


namespace ApacsAdapter
{
    public class AdpMBAdapter
    {
        private const string ExchangeName = "amq.direct";
        private const string ContentType = "application/xml";
        private const string VirtualHost = "/carbon";
        private ConnectionFactory Factory = new ConnectionFactory();

        public AdpMBAdapter(string hostName, int port, string userName, string password)
        {
            Factory.VirtualHost = VirtualHost;
            Factory.UserName = userName;
            Factory.Password = password;
            Factory.HostName = hostName;
            Factory.Port = port;
            Factory.Protocol = Protocols.DefaultProtocol;
        }
        public bool PublishMessage(string queue, string msg)
        {
            bool result = false;
            if (String.IsNullOrEmpty(msg) || String.IsNullOrEmpty(queue))
            {
                return result;
            }
            using (IConnection Connect = Factory.CreateConnection())
            {
                using (IModel Model = Connect.CreateModel())
                {
                    Model.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
                    Model.QueueDeclare(queue, true, false, false, null);
                    Model.QueueBind(queue, ExchangeName, queue);
                    IBasicProperties props = Model.CreateBasicProperties();
                    props.ContentEncoding = Encoding.UTF8.HeaderName;
                    props.ContentType = ContentType;
                    props.DeliveryMode = 2;
                    Model.BasicPublish(ExchangeName, queue, props, Encoding.UTF8.GetBytes(msg));
                    Model.Close(200, String.Empty);
                    Connect.Close();
                    result = true;
                }
            }
            return result;
        }
        
        public string RetriveMessage(string Queue, out bool queueIsNotEmpty)
        {
            string message = null;
            queueIsNotEmpty = false;
            using (IConnection Connect = Factory.CreateConnection())
            {
                using (IModel Model = Connect.CreateModel())
                {
                    Model.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
                    Model.QueueDeclare(Queue, true, false, false, null);
                    Model.QueueBind(Queue, ExchangeName, Queue);
                    QueueingBasicConsumer consumer = new QueueingBasicConsumer(Model);
                    Model.BasicConsume(Queue, false, consumer);
                    try
                    {
                        BasicDeliverEventArgs eventQueue = null;
                        queueIsNotEmpty = consumer.Queue.Dequeue(2000, out eventQueue);
                        if (eventQueue != null)
                        {
                            message = Encoding.UTF8.GetString(eventQueue.Body);
                            Model.BasicAck(eventQueue.DeliveryTag, false);
                        }
                    }
                    catch (Exception) { }
                    Model.Close(200, String.Empty);
                    Connect.Close();
                }
            }
            return message;
        }
    }
    public class AdpGRAdapter
    {
        private const string artifactPath = @"/_system/governance";
        private const string holdersPath = @"/ssoi/cardholders";
        private const string holdersPhotoPath = holdersPath + @"/photo";
        private const string VIPsPath = @"/ssoi/personcontrol";
        private string holdersFullPath = artifactPath + holdersPath;
        private string VIPsFullPath = artifactPath + VIPsPath;
        private string holdersPhotoFullPath = artifactPath + holdersPhotoPath;
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
                        new WSProperty(){ key = "holder_vip", values = new string[] { IsVIP(ch.ID).ToString() } }
                    };
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
            string photoResName = String.Format(@"/{0}.jpg", ch.ID);
            XNamespace xn = @"http://www.wso2.org/governance/metadata";
            XElement xdoc =
                new XElement(xn + "metadata",
                    new XElement(xn + "holder",
                        new XElement(xn + "id", ch.ID),
                        new XElement(xn + "shortName", ch.ShortName),
                        new XElement(xn + "name", ch.Name),
                        new XElement(xn + "cardNo", ch.CardNo),
                        new XElement(xn + "photo", holdersPhotoPath + photoResName),
                        new XElement(xn + "photoLink", holdersPhotoPermaLinkUrl + photoResName),
                        new XElement(xn + "vip", IsVIP(ch.ID).ToString())
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