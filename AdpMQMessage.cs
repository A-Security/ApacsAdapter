using System;

namespace ApacsAdapter
{
    public class AdpMQMessage
    {
        private string _body;
        private string _id;
        private long _time;
        private string _type;

        public AdpMQMessage(string id, string body, string type)
        {
            this.id = id;
            this.type = type;
            this.body = body;
            this.time = (DateTime.Now.Ticks - (new DateTime(1970, 1, 1)).Ticks) / TimeSpan.TicksPerSecond;
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
        public long time
        {
            get { return _time; }
            private set
            {
                _time = value;
            }
        }
        public bool IsBodyEmpty { get; private set; }
        public bool IsIdEmpty { get; private set; }

    }
}
