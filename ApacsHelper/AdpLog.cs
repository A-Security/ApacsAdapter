using System;

namespace ApacsHelper
{
    //Adapter Log class
    public class AdpLog
    {
        // Last message string
        public string Log { get; private set; }
        // On add message event handler
        public static event EventHandler OnAddLogEventHandler;
        // Add message to log method
        public void AddLog(object log)
        {
            this.Log = log.ToString();
            //call event subscribers and recive message (in instance)
            if (OnAddLogEventHandler != null)
            {
                OnAddLogEventHandler(this, EventArgs.Empty);
            }
        }

    }
}
