using ApacsAdapter;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Impl;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ApacsAdapterService
{
    public class AdpMBAdapter : IDisposable
    {
        private delegate void ConsumeDelegate();
        private delegate void SendDeferredMsgDelegate();
        public delegate void ReceiverMessageEventHandler(AdpMBMsgObj msg);
        public event ReceiverMessageEventHandler OnMessageReceived;
        private const string EXCHANGE_NAME = "amq.direct";
        private const string CONTENT_TYPE = "text/xml";
        private const string VIRTUAL_HOST = "/carbon";
        private const byte DELIVERY_MODE = 2;
        private string Queue;
        private AdpLog log;
        private static Dictionary<string, AdpMBMsgObj> deferredMsgs = new Dictionary<string, AdpMBMsgObj>();
        private ConnectionFactory Factory;
        private IConnection Conn;
        private IModel Model;
        private Timer timer;
        private TimerCallback timerCallback;
        private bool isConsuming;

        private void sendDeferredMsg()
        {
            while (deferredMsgs.Count != 0)
            {
                foreach (AdpMBMsgObj msg in deferredMsgs.Values)
                {
                    if (PublishMessage(msg))
                    {
                        deferredMsgs.Remove(msg.id);
                    }
                }
            }
            log.AddLog("WSO2 MB deferred message send successful");
        }
        public AdpMBAdapter(string hostName, int port, string userName, string password, string queue)
        {
            this.log = new AdpLog();
            this.Factory = new ConnectionFactory();
            this.Factory.AutomaticRecoveryEnabled = true;
            this.Factory.VirtualHost = VIRTUAL_HOST;
            this.Factory.HostName = hostName;
            this.Factory.Port = port;
            this.Factory.UserName = userName;
            this.Factory.Password = password;
            this.Factory.Protocol = Protocols.AMQP_0_9_1;
            this.Queue = queue;
            this.isConsuming = false;
        }
        public void Connect()
        {
            if (Conn != null && Conn.IsOpen && Model != null && Model.IsOpen)
            {
                log.AddLog("WSO2 MB already connected");
                return;
            }
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
        public void Disconnect()
        {
            this.isConsuming = false;
            this.Conn.ConnectionShutdown -= ConnectionShutdownHundler;
            try
            {
                if (Conn != null)
                {
                    Conn.Close();
                }
                if (Model != null)
                {
                    Model.Abort();
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
            log.AddLog("WSO2 MB disconnected");
        }
        private void recovery(object sender)
        {
            if (Conn != null && Conn.IsOpen && Model != null && Model.IsOpen)
            {
                log.AddLog("WSO2 MB connection recovery!");
                log.AddLog("WSO2 MB deferred message to send: " + deferredMsgs.Count);
                if (!isConsuming)
                {
                    SendDeferredMsgDelegate sendDM = new SendDeferredMsgDelegate(sendDeferredMsg);
                    sendDM.BeginInvoke(null, null);
                }
                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }
                timerCallback = null;
            }
        }

        public bool PublishMessage(AdpMBMsgObj msg)
        {
            bool SendOk = false;
            if (msg == null || msg.IsBodyEmpty || String.IsNullOrEmpty(Queue))
            {
                return SendOk;
            }
            if (!(Model == null || Model.IsClosed || Conn == null || !Conn.IsOpen))
            {
                try
                {
                    IBasicProperties props = Model.CreateBasicProperties();
                    props.AppId = AppDomain.CurrentDomain.FriendlyName;
                    props.MessageId = msg.id;
                    props.Timestamp = new AmqpTimestamp(msg.UnixTime);
                    props.ContentEncoding = Encoding.UTF8.HeaderName;
                    props.ContentType = CONTENT_TYPE;
                    props.Type = msg.Type;
                    props.DeliveryMode = DELIVERY_MODE;
                    Model.BasicPublish(String.Empty, Queue, props, msg.Body);
                    SendOk = true;
                }
                catch (Exception e)
                {
                    log.AddLog(e.ToString());
                }
            }
            if (!SendOk && (deferredMsgs.Count == 0 || !deferredMsgs.ContainsKey(msg.id)))
            {
                deferredMsgs.Add(msg.id, msg);
            }
            return SendOk;
        }

        public void RetrieveMessage()
        {
            isConsuming = true;
            ConsumeDelegate consume = new ConsumeDelegate(Consume);
            consume.BeginInvoke(null, null);
        }
        private void Consume()
        {
            QueueingBasicConsumer consumer = new QueueingBasicConsumer(Model);
            Model.BasicConsume(Queue, false, consumer);
            while (isConsuming)
            {
                try
                {
                    BasicDeliverEventArgs eventQueue = consumer.Queue.Dequeue();
                    if (OnMessageReceived != null && eventQueue != null)
                    {
                        IBasicProperties props = eventQueue.BasicProperties;
                        AdpMBMsgObj msg = new AdpMBMsgObj(props.MessageId, eventQueue.Body, props.Type, props.Timestamp.UnixTime, props.AppId);
                        OnMessageReceived(msg);
                        Model.BasicAck(eventQueue.DeliveryTag, false);
                    }
                }
                catch (Exception e)
                {
                    log.AddLog(e.ToString());
                    break;
                }
            }
        }

        public void Dispose()
        {
            Disconnect();
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
            timerCallback = null;
            GC.SuppressFinalize(this);
        }
    }

}