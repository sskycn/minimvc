using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    class NsapRecord : IRecordData
    {
        public NsapRecord(DataBuffer buffer, int length)
        {
            buffer.Position += length;
            throw new NotImplementedException("Experimental Record Type Unable to Implement");
        }
    }
}
