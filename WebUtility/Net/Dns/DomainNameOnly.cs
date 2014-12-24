using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    class DomainNameOnly : IRecordData
    {
        private string _domain;

        public DomainNameOnly(DataBuffer buffer)
        {
            _domain = buffer.ReadDomainName();
        }

        protected string Domain
        {
            get { return _domain; }
        }

        public override string ToString()
        {
            return "Domain: " + Domain;
        }
    }
}
