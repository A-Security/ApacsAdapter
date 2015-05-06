using System;

namespace ApacsAdapter
{
    public class AdpLog
    {
        public string Log { get; private set; }
        public static event EventHandler OnAddLog;
        public void AddLog(object log)
        {
            this.Log = log.ToString();
            if (OnAddLog != null)
            {
                OnAddLog(this, EventArgs.Empty);
            }
        }
    }
}
