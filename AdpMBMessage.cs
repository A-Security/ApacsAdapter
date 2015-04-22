using System;

namespace ApacsAdapter
{
    public class AdpMBMessage
    {
        private string _body;
        private string _id;
        private long _unixTime;
        private string _type;

        public AdpMBMessage(string id, string body, string type)
        {
            this.id = id;
            this.type = type;
            this.body = body;
            this.unixTime = (DateTime.Now.Ticks - (new DateTime(1970, 1, 1)).Ticks) / TimeSpan.TicksPerSecond;
        }
        public string type
        {
            get { return _type; }
            private set { _type = value; }
        }
        public string body
        {
            get { return _body; }
            private set
            {
                IsBodyEmpty = String.IsNullOrEmpty(value);
                _body = value;
            }
        }
        public string id
        {
            get { return _id; }
            private set
            {
                IsIdEmpty = String.IsNullOrEmpty(value);
                _id = value;
            }
        }
        public long unixTime
        {
            get { return _unixTime; }
            private set
            {
                _unixTime = value;
            }
        }
        public bool IsBodyEmpty { get; private set; }
        public bool IsIdEmpty { get; private set; }

    }
}
