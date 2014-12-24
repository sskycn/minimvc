using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    class DSRecord : IRecordData
    {
        private short _key;
        private byte _algorithm;
        private byte _digestType;
        private byte[] _digest;

        public DSRecord(DataBuffer buffer, int length)
        {
            _key = buffer.ReadShortInt();
            _algorithm = buffer.ReadByte();
            _digestType = buffer.ReadByte();
            _digest = buffer.ReadBytes(length - 4);
        }

        public short Key
        {
            get { return _key; }
        }

        public byte Algorithm
        {
            get { return _algorithm; }
        }

        public byte DigestType
        {
            get { return _digestType; }
        }

        public byte[] Digest
        {
            get { return _digest; }
        }

        public override string ToString()
        {
            return String.Format("Key:{0} Algorithm:{1} DigestType:{2} Digest:{3}", _key, _algorithm, _digestType, _digest);
        }
    }
}
