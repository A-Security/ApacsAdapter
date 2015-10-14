using ApacsHelper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ApacsAdapterService
{
    // APACS 3000 events listener
    public class AdpApcEvtsListener: IDisposable
    {
        private AdpLog log;
        private AdpSrvCfg cfg;
        private ApcData data;
        private ApcServer Apacs;
        private AdpMBAdapter producer;
        private AdpGRAdapter grAdp;
        private DateTime lastSentEventTime;
        
        // Default constructor
        public AdpApcEvtsListener(ApcServer Apacs, AdpSrvCfg cfg) 
        {
            // Log instance
            this.log = new AdpLog();
            // APACS 3000 Server instance
            this.Apacs = Apacs;
            // Config instance
            this.cfg = cfg;
            // Last sent event message to message broker time 
            this.lastSentEventTime = DateTime.Parse(cfg.LastSentEventTime);
            // APACS 3000 API helper instance
            this.data = new ApcData();
            // Message producer instance
            this.producer = new AdpMBAdapter(cfg.MbHost, Convert.ToInt32(cfg.MbPort), cfg.MbUser, cfg.MbPass, cfg.MbOutQueue);
            // Governance Registry Adapter instance (temporary)
            this.grAdp = new AdpGRAdapter(cfg.GrHost, cfg.GrUser, cfg.GrPass);
        }
        // Send messages from APACS 3000 by datetime range
        private void sendLatestEvents(DateTime fromDT, DateTime toDT)
        {
            ApcPropObj[] events = Apacs.getEvents(data.getTApcEvents(), fromDT, toDT);
            log.AddLog("Send latest " + events.Length + " events");
            foreach (ApcPropObj aop in events)
            {
                onEventHandler(aop);
            }
            log.AddLog("Send complete");
        }
        // Start APACS 3000 event listener
        public void Start()
        {
            try
            {
                // Connect to Message Broker
                //producer.Connect();
                
                // Send last unsent messages
                //sendLatestEvents(lastSentEventTime, DateTime.Now);
                
                // Subscribe on APACS 3000 events
                Apacs.ApacsDisconnect += new ApcServer.ApacsDisconnectHandler(onDisconnectHandler);
                Apacs.ApacsNotifyAdd += new ApcServer.ApacsNotifyAddHandler(onAddObjectHandler);
                Apacs.ApacsNotifyDelete += new ApcServer.ApacsNotifyDeleteHandler(onDelObjectHandler);
                Apacs.ApacsNotifyChange += new ApcServer.ApacsNotifyChangeHandler(onChangeObjectHandler);
                Apacs.ApacsEvent += new ApcServer.ApacsEventHandler(onEventHandler);
                // Write event to log
                log.AddLog("Apacs events listener started");
            }
            catch (Exception e) 
            {
                log.AddLog(e.ToString());
            }
        }
        // Stop APACS 3000 event listener
        public void Stop()
        {
            try
            {
                // Unsubscribe on APACS 3000 events
                Apacs.ApacsEvent -= new ApcServer.ApacsEventHandler(onEventHandler);
                Apacs.ApacsNotifyChange -= new ApcServer.ApacsNotifyChangeHandler(onChangeObjectHandler);
                Apacs.ApacsNotifyDelete -= new ApcServer.ApacsNotifyDeleteHandler(onDelObjectHandler);
                Apacs.ApacsNotifyAdd -= new ApcServer.ApacsNotifyAddHandler(onAddObjectHandler);
                Apacs.ApacsDisconnect -= new ApcServer.ApacsDisconnectHandler(onDisconnectHandler);
                // Disconnect from Message Broker
                producer.Disconnect();
                // Write event to log
                log.AddLog("Apacs events listener stopped");
            }
            catch (Exception e)
            {
                log.AddLog(e.ToString());
            }
        }
        // APACS 3000 disconnect event handler
        private void onDisconnectHandler()
        {
            // Write event to log
            log.AddLog("APACS SERVER DISCONNECTED!");
            // Stop listener
            Stop();
            // Dispose APACS 3000 instance
            Apacs.Dispose();
            // Create new APACS 3000 instance
            Apacs = new ApcServer(cfg.ApcUser, cfg.ApcPasswd);
            // Start listener
            Start();
        }
        // APACS 3000 access control event handler
        private void onEventHandler(ApcPropObj evtPropObj)
        {
            // Create Event object from APACS 3000 Property Object
            AdpApcEvtObj aeObj = data.mapAdpApcEvtObj(evtPropObj);
                        
            // Create XML message from Event object and cast to byte array
            byte[] msgBody = Encoding.UTF8.GetBytes(aeObj.ToXmlString());
            
            // Create AMQP message for send
            AdpMBMsgObj msg = new AdpMBMsgObj(Guid.NewGuid().ToString(), msgBody, aeObj.TYPE);
            
            // Send message to Message Broker and save sent time
            if (producer.PublishMessage(msg))
            {
                cfg.SetLastSentEventTime(aeObj.EventTime);
            }
            
            // Write to log if unsuccessful send
            else
            {
                log.AddLog("Error send event to MB " + Encoding.UTF8.GetString(msg.Body));
            }
            
        }
        // APACS 3000 add new object event handler
        private void onAddObjectHandler(ApcObj newObject) 
        {
            // Exit if object is not TApcCardHolder type
            if (newObject == null || !String.Equals(ApcObjType.TApcCardHolder, newObject.getApacsType()))
            {
                return;
            }
            // Call cardholder object actions handler
            chModHundler(newObject, AdpCHObj.ModType.AddRq);
        }
        // APACS 3000 add new object event handler
        private void onDelObjectHandler(ApcObj delObject)
        {
            // Exit if object is not TApcCardHolder type
            if (delObject == null || !String.Equals(ApcObjType.TApcCardHolder, delObject.getApacsType()))
            {
                return;
            }
            // Call cardholder object actions handler
            chModHundler(delObject, AdpCHObj.ModType.DelRq);
        }
        // APACS 3000 add new object event handler
        private void onChangeObjectHandler(ApcObj changeObject, ApcPropObj evtSet)
        {
            // Exit if object is not TApcCardHolder type
            if (changeObject == null || !String.Equals(ApcObjType.TApcCardHolder, changeObject.getApacsType()))
            {
                return;
            }
            // Call cardholder object actions handler
            chModHundler(changeObject, AdpCHObj.ModType.ModRq);
        }
        // Cardholder object actions handler
        private void chModHundler(ApcObj ch, AdpCHObj.ModType _modType)
        {
            // Create cardholder object
            AdpCHObj aeCHObj = data.mapAdpCHObj(ch);
            // Set action type
            aeCHObj.modType = _modType;
            // Create XML message from Cardholder object and cast to byte array
            byte[] msgBody = Encoding.UTF8.GetBytes(aeCHObj.ToXmlString());
            // Create AMQP message for send
            AdpMBMsgObj msg = new AdpMBMsgObj(Guid.NewGuid().ToString(), msgBody, aeCHObj.TYPE);
            // Send message to Message Broker, write to log if unsuccessful send
            if (!producer.PublishMessage(msg))
            {
                log.AddLog("Error send event to MB " + Encoding.UTF8.GetString(msg.Body));
            }
            //grAdp.removeCardHolder(ch.getSampleUID());
            //if (_ModType <0)
            //{
            //    return;
            //}
            //grAdp.putCardHolder(data.mapAdpCHObj(ch));
        }
        // Dispose method
        public void Dispose()
        {
            Stop();
            if (Apacs != null)
            {
                Apacs.Dispose();
                Apacs = null;
            }
            if (producer != null)
            {
                producer.Dispose();
                producer = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}