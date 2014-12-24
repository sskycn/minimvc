using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    class MRRecord : DomainNameOnly
    {
        public MRRecord(DataBuffer buffer)
            : base(buffer)
        {
        }

        public string ForwardingAddress
        {
            get { return this.Domain; }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
