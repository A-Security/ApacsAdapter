using System;

namespace ApacsAdapter
{
    public class AdpLog
    {
        public string log { get; private set; }
        public static event EventHandler OnAddLog;
        public void AddLog(object Log)
        {
            this.log = Log.ToString();
            if (OnAddLog != null)
            {
                OnAddLog(this, EventArgs.Empty);
            }
        }
    }
}
