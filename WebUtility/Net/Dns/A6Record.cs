using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Cvv.WebUtility.Net.Dns
{
    class A6Record : IRecordData
    {
        private int _prefixLength = -1;
        private IPAddress _ipAddress;
        private string _domain;

        public A6Record(DataBuffer buffer)
        {
            _prefixLength = buffer.ReadByte();

            if (_prefixLength == 0) //Only Address Present
            {
                _ipAddress = buffer.ReadIPv6Address();
            }
            else if (_prefixLength == 128) //Only Domain Name Present
            {
                _domain = buffer.ReadDomainName();
            }
            else //Address and Domain Name Present
            {
                _ipAddress = buffer.ReadIPv6Address();
                _domain = buffer.ReadDomainName();
            }
        }

        public int PrefixLength
        {
            get { return _prefixLength; }
        }

        public IPAddress IpAddress
        {
            get { return _ipAddress; }
        }

        public string Domain
        {
            get { return _domain; }
        }

        public override string ToString()
        {
            return String.Format("Prefix Length:{0} IP Address:{1} Domain:{2}", _prefixLength, _ipAddress, _domain);
        }
    }
}
