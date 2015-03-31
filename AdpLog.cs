using System;

namespace ApacsAdapter
{
    public class AdpLog
    {
        public static event EventHandler OnAddLog;
        public static void AddLog(string Log)
        {
            if (OnAddLog != null)
            {
                OnAddLog(Log, EventArgs.Empty);
            }
        }
    }
}
