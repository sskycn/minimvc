using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    class NaptrRecord : IRecordData
    {
        private ushort _order;
        private ushort _priority;
        private string _flags;
        private string _services;
        private string _regexp;
        private string _replacement;

        public NaptrRecord(DataBuffer buffer)
        {
            _order = buffer.ReadShortUInt();
            _priority = buffer.ReadShortUInt();
            _flags = buffer.ReadCharString();
            _services = buffer.ReadCharString();
            _regexp = buffer.ReadCharString();
            _replacement = buffer.ReadCharString();
        }

        public ushort Order
        {
            get { return _order; }
        }

        public ushort Priority
        {
            get { return _priority; }
        }

        public string Flags
        {
            get { return _flags; }
        }

        public string Services
        {
            get { return _services; }
        }

        public string Regexp
        {
            get { return _regexp; }
        }

        public string Replacement
        {
            get { return _replacement; }
        }

        public override string ToString()
        {
            return String.Format("Order:{0}, Priority:{1} Flags:{2} Services:{3} RegExp:{4} Replacement:{5}",
                _order, _priority, _flags, _services, _regexp, _replacement);
        }
    }
}
