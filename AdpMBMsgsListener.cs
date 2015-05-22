using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacsAdapter
{
    public class AdpMBMsgsListener
    {
        private AdpLog log;
        private AdpCfgXml cfg;
        private ApcServer Apacs;
        private AdpMBAdapter consumer;
        
        public AdpMBMsgsListener(ApcServer Apacs, AdpCfgXml cfg) 
        {
            this.log = new AdpLog();
            this.Apacs = Apacs;
            this.cfg = cfg;
            this.consumer = new AdpMBAdapter(cfg.MBhost, Convert.ToInt32(cfg.MBport), cfg.MBuser, cfg.MBpassword, cfg.MBinQueue);
        }
        public void start()
        {
            try
            {
                consumer.connect();
                consumer.onMessageReceived += onMessageReceived;
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
            log.AddLog(Encoding.UTF8.GetString(msg.body));
        }
        
        public void stop()
        {
            try
            {
                Apacs.ApacsDisconnect -= new ApcServer.ApacsDisconnectHandler(onDisconnect);
                consumer.onMessageReceived -= onMessageReceived;
                consumer.disconnect();
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
            stop();
            Apacs.Dispose();
            Apacs = new ApcServer(cfg.apcLogin, cfg.apcPasswd);
            start();
        }
    }
}