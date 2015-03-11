using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


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
}