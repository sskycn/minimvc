using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    public class PrefAndDomain : IRecordData
    {
        private int _preference;
        private string _domain;

        public PrefAndDomain(DataBuffer buffer)
        {
            _preference = buffer.ReadBEShortInt();
            _domain = buffer.ReadDomainName();
        }

        public int Preference
        {
            get { return _preference; }
        }

        public string Domain
        {
            get { return _domain; }
        }

        public override string ToString()
        {
            return String.Format("Preference:{0} Domain:{1}", _preference, _domain);
        }
    }
}
