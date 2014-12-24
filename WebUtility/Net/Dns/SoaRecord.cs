using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    class SoaRecord : IRecordData
    {
        private string _primaryNameServer;
        private string _responsibleMailAddress;
        private int _serial;
        private int _refresh;
        private int _retry;
        private int _expire;
        private int _defaultTtl;

        public SoaRecord(DataBuffer buffer)
        {
            _primaryNameServer = buffer.ReadDomainName();
            _responsibleMailAddress = buffer.ReadDomainName();
            _serial = buffer.ReadInt();
            _refresh = buffer.ReadInt();
            _retry = buffer.ReadInt();
            _expire = buffer.ReadInt();
            _defaultTtl = buffer.ReadInt();
        }

        public override string ToString()
        {
            return String.Format("Primary Name Server:{0} Responsible Name Address:{1} Serial:{2} Refresh:{3} Retry:{4} Expire:{5} Default TTL:{6}",
                _primaryNameServer, _responsibleMailAddress, _serial, _refresh, _retry, _expire, _defaultTtl);
        }

        public string PrimaryNameServer
        {
            get { return _primaryNameServer; }
        }
        
        public string ResponsibleMailAddress
        {
            get { return _responsibleMailAddress; }
        }
        
        public int Serial
        {
            get { return _serial; }
        }
        
        public int Refresh
        {
            get { return _refresh; }
        }
        
        public int Retry
        {
            get { return _retry; }
        }
        
        public int Expire
        {
            get { return _expire; }
        }
        
        public int DefaultTtl
        {
            get { return _defaultTtl; }
        }
    }
}
