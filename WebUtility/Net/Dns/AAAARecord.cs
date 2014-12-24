using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Cvv.WebUtility.Net.Dns
{
    class AAAARecord : IRecordData
    {
        private IPAddress _ipAddress;

        public AAAARecord(DataBuffer buffer)
        {
            _ipAddress = buffer.ReadIPv6Address();
        }

        public IPAddress IpAddress
        {
            get { return _ipAddress; }
        }

        public override string ToString()
        { 
            return "IP Address: " + _ipAddress.ToString();
        }
     }
 }

