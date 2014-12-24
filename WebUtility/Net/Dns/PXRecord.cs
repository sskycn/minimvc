using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    class PXRecord : PrefAndDomain
    {
        private string _x400Domain;

        public PXRecord(DataBuffer buffer)
            : base(buffer)
        {
            _x400Domain = buffer.ReadDomainName();
        }

        public string X400Domain
        {
            get { return _x400Domain; }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
