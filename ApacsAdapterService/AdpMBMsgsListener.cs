using ApacsHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacsAdapterService
{
    public class AdpMBMsgsListener : IDisposable
    {
        private AdpLog log;
        private AdpSrvCfg cfg;
        private ApcServer Apacs;
        private AdpMBAdapter consumer;
        
        public AdpMBMsgsListener(ApcServer apacs, AdpSrvCfg cfg) 
        {
            this.log = new AdpLog();
            this.Apacs = apacs;
            this.cfg = cfg;
            this.consumer = new AdpMBAdapter(cfg.MbHost, Convert.ToInt32(cfg.MbPort), cfg.MbUser, cfg.MbPass, cfg.MbInQueue);
        }
        public void Start()
        {
            try
            {
                consumer.Connect();
                consumer.OnMessageReceived += onMessageReceived;
                Apacs.ApacsDisconnect += new ApcServer.ApacsDisconnectHandler(onDisconnect);
                consumer.RetrieveMessage();
                log.AddLog("WSO2 MB incoming messages listener started");
            }
            catch (Exception e) 
            {
                log.AddLog(e.ToString());
            }
        }

        private void onMessageReceived(AdpMBMsgObj msg)
        {
            log.AddLog(Encoding.UTF8.GetString(msg.Body));
        }
        
        public void Stop()
        {
            try
            {
                Apacs.ApacsDisconnect -= new ApcServer.ApacsDisconnectHandler(onDisconnect);
                consumer.OnMessageReceived -= onMessageReceived;
                consumer.Disconnect();
                log.AddLog("WSO2 MB incoming messages listener stopped");
            }
            catch (Exception e)
            {
                log.AddLog(e.ToString());
            }
        }
        private void onDisconnect()
        {
            log.AddLog("APACS SERVER DISCONNECTED!");
            Stop();
            Apacs.Dispose();
            Apacs = new ApcServer(cfg.ApcUser, cfg.ApcPasswd);
            Start();
        }

        public void Dispose()
        {
            Stop();
            if (Apacs != null)
            {
                Apacs.Dispose();
                Apacs = null;
            }
            if (consumer != null)
            {
                consumer.Dispose();
                consumer = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}