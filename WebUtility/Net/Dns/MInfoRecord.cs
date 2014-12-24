using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    class MInfoRecord : IRecordData
    {
        private string _responsibleMailbox;
        private string _errorMailbox;

        public MInfoRecord(DataBuffer buffer)
        {
            _responsibleMailbox = buffer.ReadDomainName();
            _errorMailbox = buffer.ReadDomainName();
        }

        public string ResponsibleMailbox
        {
            get { return _responsibleMailbox; }
        }

        public string ErrorMailbox
        {
            get { return _errorMailbox; }
        }

        public override string ToString()
        {
            return String.Format("Responsible Mailbox:{0} Error Mailbox:{1}", _responsibleMailbox, _errorMailbox);
        }
    }
}
