using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    class TKeyRecord : IRecordData
    {
        private string _algorithm;
        private uint _inception;
        private uint _expiration;
        private ushort _mode;
        private ushort _error;
        private ushort _keySize;
        private byte[] _keyData;
        private ushort _otherSize;
        private byte[] _otherData;

        public TKeyRecord(DataBuffer buffer)
        {
            _algorithm = buffer.ReadDomainName();
            _inception = buffer.ReadUInt();
            _expiration = buffer.ReadUInt();
            _mode = buffer.ReadShortUInt();
            _error = buffer.ReadShortUInt();
            _keySize = buffer.ReadShortUInt();
            _keyData = buffer.ReadBytes(_keySize);
            _otherSize = buffer.ReadShortUInt();
            _otherData = buffer.ReadBytes(_otherSize);
        }

        public string Algorithm
        {
            get { return _algorithm; }
        }

        public uint Inception
        {
            get { return _inception; }
        }

        public uint Expiration
        {
            get { return _expiration; }
        }

        public ushort Mode
        {
            get { return _mode; }
        }

        public ushort Error
        {
            get { return _error; }
        }

        public byte[] KeyData
        {
            get { return _keyData; }
        }

        public byte[] OtherData
        {
            get { return _otherData; }
        }

        public override string ToString()
        {
            return String.Format("Algorithm:{0} Inception:{1} Expiration:{2} Mode:{3} Error:{4} \nKey Data:{5} \nOther Data:{6} ",
                _algorithm, _inception, _expiration, _mode, _error, _keyData, _otherData);
        }
    }
}
