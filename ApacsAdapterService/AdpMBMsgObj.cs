using System;

namespace ApacsAdapterService
{
    public class AdpMBMsgObj
    {
        private string _appid;
        private byte[] _body;
        private string _id;
        private long _unixTime;
        private string _type;
        private long unixTimeOffset = new DateTime(1970, 1, 1).Ticks;

        public AdpMBMsgObj(string id, byte[] body, string type)
        {
            this.id = id;
            this.Type = type;
            this.Body = body;
            this.UnixTime = (DateTime.Now.Ticks - unixTimeOffset) / TimeSpan.TicksPerSecond;
            this.AppId = AppDomain.CurrentDomain.FriendlyName;
        }
        public AdpMBMsgObj (string id, byte[] body, string type, long unixTime, string appid) : this(id, body, type)
        {
            this.UnixTime = unixTime;
            this.AppId = appid;
        }
        public string AppId
        {
            get { return _appid; }
            private set { _appid = value; }
        }
        public string Type
        {
            get { return _type; }
            private set { _type = value; }
        }
        public byte[] Body
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
        public long UnixTime
        {
            get { return _unixTime; }
            private set
            {
                _unixTime = value;
            }
        }
        public DateTime DotNetTime
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
