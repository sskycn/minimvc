using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    class HInfoRecord : TextOnly
    {
        public HInfoRecord(DataBuffer buffer, int length)
            : base(buffer, length)
        {

        }

        public string Cpu      
        {            
            get 
            {
                if (this.Count > 0)
                    return this.Strings[0];
                else
                    return "Unknown";
            }    
        }

        public string Os 
        { 
            get 
            {
                if (this.Count > 1)
                    return this.Strings[1];
                else
                    return "Unknown";
            } 
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
