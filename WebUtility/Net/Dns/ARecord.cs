using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Cvv.WebUtility.Net.Dns
{
    class ARecord : IRecordData
    {
        private IPAddress _ipAddress;

        public ARecord(DataBuffer buffer)
        {
            Byte[] ipaddress = buffer.ReadBytes(4);
            _ipAddress = new IPAddress(ipaddress);
        }

        public IPAddress IpAddress
        {
            get { return _ipAddress; }
        }

        public override string ToString()
        {
            return  "IP Address: " + _ipAddress.ToString();
        }
    }
}
