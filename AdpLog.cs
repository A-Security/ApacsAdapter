using System;

namespace ApacsAdapter
{
    public class AdpLog
    {
        public string log { get; private set; }
        public event EventHandler OnAddLog;
        public void AddLog(string Log)
        {
            this.log = Log;
            if (OnAddLog != null)
            {
                OnAddLog(this, EventArgs.Empty);
            }
        }
    }
}
