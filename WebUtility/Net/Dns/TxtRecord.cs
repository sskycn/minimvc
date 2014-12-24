using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    class TxtRecord : TextOnly
    {
        public TxtRecord(DataBuffer buffer, int length)
            : base(buffer, length)
        {
        }

        public new string Text
        {
            get { return base.Text; }
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
