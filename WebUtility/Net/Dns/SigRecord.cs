using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    class SigRecord : IRecordData
    {
        private short _coveredType;
        private byte _algorithm;
        private byte _numLabels;
        private uint _expiration;
        private uint _inception;
        private short _keyTag;
        private string _signer;

        public SigRecord(DataBuffer buffer, int length)
        {
            int pos = buffer.Position;
            _coveredType = buffer.ReadShortInt();
            _algorithm = buffer.ReadByte();
            _numLabels = buffer.ReadByte();
            _expiration = buffer.ReadUInt();
            _inception = buffer.ReadUInt();
            _keyTag = buffer.ReadShortInt();
            _signer = buffer.ReadDomainName();
            buffer.Position = pos - length;
        }

        public override string ToString()
        {
            return String.Format("Covered Type:{0} Algorithm:{1} Number of Labels:{2} Expiration:{3} Inception:{4} Key Tag:{5} Signer:{6}",
                _coveredType, _algorithm, _numLabels, _expiration, _inception, _keyTag, _signer);
        }

        public short CoveredType
        {
            get { return _coveredType; }
        }

        public byte Algorithm
        {
            get { return _algorithm; }
        }

        public byte NumLabels
        {
            get { return _numLabels; }
        }

        public uint Expiration
        {
            get { return _expiration; }
        }

        public uint Inception
        {
            get { return _inception; }
        }

        public short KeyTag
        {
            get { return _keyTag; }
        }

        public string Signer
        {
            get { return _signer; }
        }
    }
}
