using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Impl;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ApacsAdapter
{
    public class AdpMBAdapter
    {
        
        private const string EXCHANGE_NAME = "amq.direct";
        private const string CONTENT_TYPE = "text/xml";
        private const string VIRTUAL_HOST = "/carbon";
        private const byte DELIVERY_MODE = 2;
        private string Queue;
        private AdpLog log;
        private Dictionary<string, AdpMBMessage> deferredMsgs;
        private ConnectionFactory Factory;
        private IConnection Conn;
        private IModel Model;
        private Timer timer;
        private TimerCallback timerCallback;
        
        public AdpMBAdapter(string hostName, int port, string userName, string password, string queue)
        {
            this.log = new AdpLog();
            this.deferredMsgs = new Dictionary<string, AdpMBMessage>();
            this.Factory = new ConnectionFactory();
            this.Factory.AutomaticRecoveryEnabled = true;
            this.Factory.VirtualHost = VIRTUAL_HOST;
            this.Factory.HostName = hostName;
            this.Factory.Port = port;
            this.Factory.UserName = userName;
            this.Factory.Password = password;
            this.Factory.Protocol = Protocols.AMQP_0_9_1;
            this.Queue = queue;
            this.Conn = Factory.CreateConnection();
            this.Model = Conn.CreateModel();
            this.Conn.ConnectionShutdown += ConnectionShutdownHundler;
            log.AddLog("WSO2 MB connected");
        }

        private void ConnectionShutdownHundler(object sender, ShutdownEventArgs e)
        {
            log.AddLog("WSO2 MB connection lost!");
            this.timerCallback = new TimerCallback(recovery);
            this.timer = new Timer(timerCallback, sender, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5));
        }

        private void recovery(object sender)
        {
            if (Conn.IsOpen && Model.IsOpen)
            {
                log.AddLog("WSO2 MB connection recovery!");
                log.AddLog("WSO2 MB deferred message to send: " + deferredMsgs.Count);
                if (deferredMsgs.Count != 0)
                {
                    foreach (AdpMBMessage msg in deferredMsgs.Values)
                    {
                        if (PublishMessage(msg))
                        {
                            deferredMsgs.Remove(msg.id);
                        }
                    }
                    log.AddLog("WSO2 MB deferred message send successful");
                }
                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }
                timerCallback = null;
            }
        }
        
        ~AdpMBAdapter()
        {
            this.Conn.ConnectionShutdown -= ConnectionShutdownHundler;
            try
            {
                if (Model != null)
                {
                    if (Model.IsOpen)
                    {
                        Model.Close();
                    }
                    Model.Dispose();
                }
                if (Conn != null)
                {
                    if (Conn.IsOpen)
                    {
                        Conn.Abort(5);
                    }
                    Conn.Dispose();
                }
            }
            catch (Exception e)
            {
                log.AddLog(e.ToString());
            }
            finally
            {
                Model = null;
                Conn = null;
            }

        }
        public bool PublishMessage(AdpMBMessage msg)
        {
            bool IsSendOk = true;
            if (Model == null || Model.IsClosed || Conn == null || !Conn.IsOpen || msg == null || msg.IsBodyEmpty || String.IsNullOrEmpty(Queue))
            {
                IsSendOk = false;
            }
            if (IsSendOk)
            {
                IsSendOk = false;
                try
                {
                    IBasicProperties props = Model.CreateBasicProperties();
                    props.AppId = AppDomain.CurrentDomain.FriendlyName;
                    props.MessageId = msg.id;
                    props.Timestamp = new AmqpTimestamp(msg.unixTime);
                    props.ContentEncoding = Encoding.UTF8.HeaderName;
                    props.ContentType = CONTENT_TYPE;
                    props.Type = msg.type;
                    props.DeliveryMode = DELIVERY_MODE;
                    Model.BasicPublish(String.Empty, Queue, props, Encoding.UTF8.GetBytes(msg.body));
                    IsSendOk = true;
                }
                catch (Exception e)
                {
                    log.AddLog(e.ToString());
                }
            }
            if (!IsSendOk && (deferredMsgs.Count == 0 || !deferredMsgs.ContainsKey(msg.id)))
            {
                deferredMsgs.Add(msg.id, msg);
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
                    Model.Close();
                    Connect.Abort();
                }
            }
            return message;
        }
    }
    
}