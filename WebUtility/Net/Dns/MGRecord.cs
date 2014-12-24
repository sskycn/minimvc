using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    class MGRecord : DomainNameOnly
    {
        public MGRecord(DataBuffer buffer)
            : base(buffer)
        {
        }

        public string MailGroupDomain
        {
            get { return this.Domain; }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
