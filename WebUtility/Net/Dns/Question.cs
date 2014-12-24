using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    public class Question
    {
        private string _domain;
        private RecordType _recType;
        private int _classType;

        public Question(DataBuffer buffer)
        {
            _domain = buffer.ReadDomainName();
            _recType = (RecordType)buffer.ReadBEShortInt();
            _classType = buffer.ReadBEShortInt();
        }

        public string Domain
        {
            get { return _domain; }
        }
        
        public RecordType RecType
        {
            get { return _recType; }
        }
        
        public int ClassType
        {
            get { return _classType; }
        }
    }
}
