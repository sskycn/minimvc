using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    class NullRecord : TextOnly
    {
        public NullRecord(DataBuffer buffer, int length)
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
