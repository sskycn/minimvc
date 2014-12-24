using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    class NSRecord : DomainNameOnly
    {
        public NSRecord(DataBuffer buffer)
            : base(buffer)
        {

        }

        public string NSDomain
        {
            get { return this.Domain; }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
