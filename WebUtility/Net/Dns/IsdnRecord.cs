using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    class IsdnRecord : IRecordData
    {
        private string _isdnAddress;
        private string _subAddress;

        public IsdnRecord(DataBuffer buffer)
        {
            _isdnAddress = buffer.ReadCharString();
            _subAddress = buffer.ReadCharString();
        }

        public override string ToString()
        {
            return String.Format("ISDN Address:{0} Sub Address:{1}", _isdnAddress, _subAddress);
        }

        public string IsdnAddress
        {
            get { return _isdnAddress; }
        }

        public string SubAddress
        {
            get { return _subAddress; }
        }
    }
}
