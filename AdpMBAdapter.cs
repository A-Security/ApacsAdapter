using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace ApacsAdapter
{

    class AdpMBAdapter
    {
        private string ExchangeName { get { return "amq.direct"; } }
        private string ContentType { get { return "application/xml"; } }
        private ConnectionFactory Factory = new ConnectionFactory();
        private IConnection Connect = null;
        private IModel Model = null;
        public AdpMBAdapter(string hostName, string userName, string password, int port)
        {
            Factory.VirtualHost = "/carbon";
            Factory.UserName = userName;
            Factory.Password = password;
            Factory.HostName = hostName;
            Factory.Port = port;
            Factory.Protocol = Protocols.AMQP_0_9_1;
        }
        public bool PublishMessage(string msg, string Queue)
        {
            if (String.IsNullOrEmpty(msg) || String.IsNullOrEmpty(Queue))
            {
                return false;
            }
            try
            {
                Connect = Factory.CreateConnection();
                try 
                {
                    Model = Connect.CreateModel();
                    Model.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
                    Model.QueueDeclare(Queue, true, false, false, null);
                    Model.QueueBind(Queue, ExchangeName, Queue);
                    IBasicProperties props = Model.CreateBasicProperties();
                    props.ContentEncoding = Encoding.UTF8.HeaderName;
                    props.ContentType = ContentType;
                    props.DeliveryMode = 2;
                    Model.BasicPublish(ExchangeName, Queue, props, Encoding.UTF8.GetBytes(msg));
                    Model.Close(200, String.Empty);
                    Connect.Close();
                    return true;
                }
                catch (Exception)
                {
                    if (Model != null)
                    { 
                        Model.Close(200, String.Empty); 
                    }
                    Connect.Close();
                    return false;
                }
            }
            catch (Exception) { return false; }
        }
        public string RetriveMessage(string Queue, out bool queueIsEmpty)
        {
            string message = null;
            queueIsEmpty = true;
            try
            {
                Connect = Factory.CreateConnection();
                try 
                {
                    Model = Connect.CreateModel();
                    Model.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
                    Model.QueueDeclare(Queue, true, false, false, null);
                    Model.QueueBind(Queue, ExchangeName, Queue);
                    QueueingBasicConsumer consumer = new QueueingBasicConsumer(Model);
                    Model.BasicConsume(Queue, false, consumer);
                    try
                    {
                        BasicDeliverEventArgs eventQueue = null;
                        queueIsEmpty = !consumer.Queue.Dequeue(3000, out eventQueue);
                        if (!queueIsEmpty)
                        {
                            message = Encoding.UTF8.GetString(eventQueue.Body);
                            Model.BasicAck(eventQueue.DeliveryTag, false);
                        }
                    }
                    catch (Exception) { }
                    Model.Close(200, String.Empty);
                    Connect.Close();
                }
                catch (Exception)
                {
                    if (Model != null)
                    {
                        Model.Close(200, String.Empty);
                    }
                    Connect.Close();
                }
                
            }
            catch (Exception) { }
            return message;
        }
    }
}
