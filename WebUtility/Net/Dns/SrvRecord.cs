using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    class SrvRecord : IRecordData
    {
        private int _priority;
        private ushort _weight;
        private ushort _port;
        private string _domain;

        public SrvRecord(DataBuffer buffer)
        {
            _priority = buffer.ReadShortInt();
            _weight = buffer.ReadShortUInt();
            _port = buffer.ReadShortUInt();
            _domain = buffer.ReadDomainName();
        }

        public int Priority
        {
            get { return _priority; }
        }

        public ushort Weight
        {
            get { return _weight; }
        }

        public ushort Port
        {
            get { return _port; }
        }

        public string Domain
        {
            get { return _domain; }
        }

        public override string ToString()
        {
            return String.Format("Priority:{0} Weight:{1}  Port:{2} Domain:{3}", _priority, _weight, _port, _domain);
        }
    }
}
