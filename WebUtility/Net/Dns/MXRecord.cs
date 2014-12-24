using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    public class MXRecord : PrefAndDomain
    {
        public MXRecord(DataBuffer buffer)
            : base(buffer)
        {
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
