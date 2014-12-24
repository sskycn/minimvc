using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Cvv.WebUtility.Net.Dns
{
    class WksRecord : IRecordData
    {
        private IPAddress _ipAddress;
        private byte _protocol;
        private byte[] _services;

        public WksRecord(DataBuffer buffer, int length)
        {
            _ipAddress = buffer.ReadIPAddress();
            _protocol = buffer.ReadByte();
            _services = new Byte[length - 5];

            for (int i = 0; i < (length - 5); i++)
                _services[i] = buffer.ReadByte();
        }

        public IPAddress IpAddress
        {
            get { return _ipAddress; }
        }

        public byte Protocol
        {
            get { return _protocol; }
        }

        public byte[] Services
        {
            get { return _services; }
        }

        public override string ToString()
        {
            return String.Format("IP Address:{0} Protocol:{1} Services:{2}", _ipAddress, _protocol, _services);
        }
    }
}
