using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    class AfsdbRecord : IRecordData
    {
        private short _subType;
        private string _domain;

        public AfsdbRecord(DataBuffer buffer)
        {
            _subType = buffer.ReadShortInt();
            _domain = buffer.ReadDomainName();
        }

        public short SubType
        {
            get { return _subType; }
        }

        public string Domain
        {
            get { return _domain; }
        }

        public override string ToString()
        {
            return String.Format("SubType:{0} Domain:{1}", _subType, _domain);
        }
    }
}
