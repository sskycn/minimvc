using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    class PtrRecord : DomainNameOnly
    {
        public PtrRecord(DataBuffer buffer)
            : base(buffer)
        {
        }

        public string PtrDomain
        {
            get { return this.Domain; }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
