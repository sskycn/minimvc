using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    class TSigRecord : IRecordData
    {
        private string _algorithm;
        private long _timeSigned;
        private ushort _fudge;
        private ushort _macSize;
        private byte[] _mac;
        private ushort _originalId;
        private ushort _error;
        private ushort _otherLen;
        private byte[] _otherData;

        public TSigRecord(DataBuffer buffer)
        {
            _algorithm = buffer.ReadDomainName();
            _timeSigned = buffer.ReadLongInt();
            _fudge = buffer.ReadShortUInt();
            _macSize = buffer.ReadShortUInt();
            _mac = buffer.ReadBytes(_macSize);
            _originalId = buffer.ReadShortUInt();
            _error = buffer.ReadShortUInt();
            _otherLen = buffer.ReadShortUInt();
            _otherData = buffer.ReadBytes(_otherLen);
        }

        public string Algorithm
        {
            get { return _algorithm; }
        }

        public long TimeSigned
        {
            get { return _timeSigned; }
        }

        public ushort Fudge
        {
            get { return _fudge; }
        }

        public byte[] Mac
        {
            get { return _mac; }
        }

        public ushort OriginalId
        {
            get { return _originalId; }
        }

        public ushort Error
        {
            get { return _error; }
        }

        public byte[] OtherData
        {
            get { return _otherData; }
        }

        public override string ToString()
        {
            return String.Format("Algorithm:{0} Signed Time:{1} Fudge Factor:{2} Mac:{3} Original ID:{4} Error:{5}\nOther Data:{6}",
                _algorithm, _timeSigned, _fudge, _mac, _originalId, _error, _otherData);
        }
    }
}
