using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    /// <summary>
    /// Implementation Reference RFC 1183
    /// </summary>
    class X25Record : TextOnly
    {
        public X25Record(DataBuffer buffer)
            : base(buffer)
        {

        }

        public string PsdnAddress
        {
            get { return Text; }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
