using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    class RPRecord : IRecordData
    {
        private string _responsibleMailbox;
        private string _textDomain;

        public RPRecord(DataBuffer buffer)
        {
            _responsibleMailbox = buffer.ReadDomainName();
            _textDomain = buffer.ReadDomainName();
        }

        public override string ToString()
        {
            return String.Format("Responsible Mailbox:{0} Text Domain:{1}", _responsibleMailbox, _textDomain);
        }

        public string ResponsibleMailbox
        {
            get { return _responsibleMailbox; }
        }

        public string TextDomain
        {
            get { return _textDomain; }
        }
    }
}
