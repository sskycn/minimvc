using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    class CNameRecord : DomainNameOnly
    {
        public CNameRecord(DataBuffer buffer)
            : base(buffer)
        {
        }

        public new string Domain
        {
            get { return base.Domain; }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
