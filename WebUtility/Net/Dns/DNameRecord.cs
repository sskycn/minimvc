using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    class DNameRecord : DomainNameOnly
    {
        public DNameRecord(DataBuffer buffer)
            : base(buffer)
        {
        }

        public string DomainName
        {
            get { return this.Domain; }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
