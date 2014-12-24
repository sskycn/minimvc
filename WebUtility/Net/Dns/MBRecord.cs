using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    class MBRecord : DomainNameOnly
    {
        public MBRecord(DataBuffer buffer)
            : base(buffer)
        {
        }

        public string AdminMailboxDomain
        {
            get { return this.Domain; }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
