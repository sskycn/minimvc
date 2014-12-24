using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    class LocRecord : IRecordData
    {
        private short _version;
        private short _size;
        private short _horzPrecision;
        private short _vertPrecision;
        private long _lattitude;
        private long _longitude;
        private long _altitude;

        public LocRecord(DataBuffer buffer)
        {
            _version = buffer.ReadShortInt();
            _size = buffer.ReadShortInt();
            _horzPrecision = buffer.ReadShortInt();
            _vertPrecision = buffer.ReadShortInt();
            _lattitude = buffer.ReadInt();
            _longitude = buffer.ReadInt();
            _altitude = buffer.ReadInt();
        }

        public short Version
        {
            get { return _version; }
        }
        
        public short Size
        {
            get { return _size; }
        }
        
        public short HorzPrecision
        {
            get { return _horzPrecision; }
        }

        public short VertPrecision
        {
            get { return _vertPrecision; }
        }
        
        public long Lattitude
        {
            get { return _lattitude; }
        }
        
        public long Longitude
        {
            get { return _longitude; }
        }
        
        public long Altitude
        {
            get { return _altitude; }
        }

        public override string ToString()
        {
            return String.Format("Version:{0} Size:{1} Horz Precision:{2} Veret Precision:{3} Lattitude:{4} Longitude:{5} Altitude:{6}",
                _version, _size, _horzPrecision, _vertPrecision, _lattitude, _longitude, _altitude);
        }
    }
}
