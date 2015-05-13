using System;

namespace ApacsAdapter
{
    public class AdpMBMessage
    {
        private string _appid;
        private byte[] _body;
        private string _id;
        private long _unixTime;
        private string _type;
        private long unixTimeOffset = new DateTime(1970, 1, 1).Ticks;

        public AdpMBMessage(string id, byte[] body, string type)
        {
            this.id = id;
            this.type = type;
            this.body = body;
            this.unixTime = (DateTime.Now.Ticks - unixTimeOffset) / TimeSpan.TicksPerSecond;
            this.appid = AppDomain.CurrentDomain.FriendlyName;
        }
        public AdpMBMessage (string id, byte[] body, string type, long unixTime, string appid) : this(id, body, type)
        {
            this.unixTime = unixTime;
            this.appid = appid;
        }
        public string appid
        {
            get { return _appid; }
            private set { _appid = value; }
        }
        public string type
        {
            get { return _type; }
            private set { _type = value; }
        }
        public byte[] body
        {
            get { return _body; }
            private set
            {
                IsBodyEmpty = (value == null);
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
        public DateTime dotNetTime
        {
            get 
            {
                return new DateTime(_unixTime * TimeSpan.TicksPerSecond + unixTimeOffset);
            }
        }
        public bool IsBodyEmpty { get; private set; }
        public bool IsIdEmpty { get; private set; }

    }
}
