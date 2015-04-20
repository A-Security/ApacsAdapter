using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace ApacsAdapter
{
    public class AdpMBAdapter
    {
        private AdpLog log = new AdpLog();
        private const string EXCHANGE_NAME = "amq.direct";
        private const string CONTENT_TYPE = "application/xml";
        private const string VIRTUAL_HOST = "/carbon";
        private const byte DELIVERY_MODE = 2;
        private ConnectionFactory Factory;

        public AdpMBAdapter(string hostName, int port, string userName, string password)
        {
            Factory = new ConnectionFactory();
            Factory.RequestedHeartbeat = 30;
            Factory.VirtualHost = VIRTUAL_HOST;
            Factory.HostName = hostName;
            Factory.Port = port;
            Factory.UserName = userName;
            Factory.Password = password;
            Factory.Protocol = Protocols.AMQP_0_9_1;
        }
        public bool PublishMessage(string Queue, AdpMQMessage msg)
        {
            bool IsSendOk = false;
            if (msg == null || msg.IsBodyEmpty || String.IsNullOrEmpty(Queue))
            {
                return IsSendOk;
            }
            try
            {
                using (IConnection Connect = Factory.CreateConnection())
                {
                    using (IModel Model = Connect.CreateModel())
                    {
                        Model.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Direct, true);
                        Model.QueueDeclare(Queue, true, false, false, null);
                        Model.QueueBind(Queue, EXCHANGE_NAME, Queue);
                        IBasicProperties props = Model.CreateBasicProperties();
                        props.AppId = AppDomain.CurrentDomain.FriendlyName;
                        props.MessageId = msg.id;
                        props.Timestamp = new AmqpTimestamp(msg.unixTime);
                        props.ContentEncoding = Encoding.UTF8.HeaderName;
                        props.ContentType = CONTENT_TYPE;
                        props.DeliveryMode = DELIVERY_MODE;
                        props.Type = msg.type;
                        props.SetPersistent(true);
                        Model.BasicPublish(EXCHANGE_NAME, Queue, props, Encoding.UTF8.GetBytes(msg.body));
                        IsSendOk = true;
                        Model.Close();
                    }
                    Connect.Abort(100);
                }
            }
            catch (Exception e) 
            {
                log.AddLog(e.ToString());
            }
            return IsSendOk;
        }
        
        public string RetriveMessage(string Queue, out bool queueIsNotEmpty)
        {
            string message = null;
            queueIsNotEmpty = false;
            using (IConnection Connect = Factory.CreateConnection())
            {
                using (IModel Model = Connect.CreateModel())
                {
                    Model.ExchangeDeclare(EXCHANGE_NAME, ExchangeType.Direct);
                    Model.QueueDeclare(Queue, true, false, false, null);
                    Model.QueueBind(Queue, EXCHANGE_NAME, Queue);
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
                    catch (Exception e)
                    {
                        log.AddLog(e.ToString());
                    }
                    Model.Close(200, String.Empty);
                    Connect.Close();
                }
            }
            return message;
        }
    }
    
}