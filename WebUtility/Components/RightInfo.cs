using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility
{
    public class RightInfo
    {
        private string _name;
        private long _value;

        public RightInfo(string name, long value)
        {
            _name = name;
            _value = value;
        }

        public string Name
        {
            get { return _name; }
        }

        public long Value
        {
            get { return _value; }
        }
    }
}
