using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    public class DnsEntry
    {
        private string _domain;
        private RecordType _recType;
        private int _classType;
        private int _ttl;
        private IRecordData _data;

        public DnsEntry(DataBuffer buffer)
        {
            try
            {
                _domain = buffer.ReadDomainName();
                byte b = buffer.ReadByte();
                _recType = (RecordType)buffer.ReadShortInt();
                _classType = buffer.ReadShortInt();
                _ttl = buffer.ReadInt();

                int length = buffer.ReadByte();
                switch (_recType)
                {
                    case RecordType.A: _data = new ARecord(buffer); break;
                    case RecordType.NS: _data = new NSRecord(buffer); break;
                    case RecordType.CNAME: _data = new CNameRecord(buffer); break;
                    case RecordType.SOA: _data = new SoaRecord(buffer); break;
                    case RecordType.MB: _data = new MBRecord(buffer); break;
                    case RecordType.MG: _data = new MGRecord(buffer); break;
                    case RecordType.MR: _data = new MRRecord(buffer); break;
                    case RecordType.NULL: _data = new NullRecord(buffer, length); break;
                    case RecordType.WKS: _data = new WksRecord(buffer, length); break;
                    case RecordType.PTR: _data = new PtrRecord(buffer); break;
                    case RecordType.HINFO: _data = new HInfoRecord(buffer, length); break;
                    case RecordType.MINFO: _data = new MInfoRecord(buffer); break;
                    case RecordType.MX: _data = new MXRecord(buffer); break;
                    case RecordType.TXT: _data = new TxtRecord(buffer, length); break;
                    case RecordType.RP: _data = new RPRecord(buffer); break;
                    case RecordType.AFSDB: _data = new AfsdbRecord(buffer); break;
                    case RecordType.X25: _data = new X25Record(buffer); break;
                    case RecordType.ISDN: _data = new IsdnRecord(buffer); break;
                    case RecordType.RT: _data = new RTRecord(buffer); break;
                    case RecordType.NSAP: _data = new NsapRecord(buffer, length); break;
                    case RecordType.SIG: _data = new SigRecord(buffer, length); break;
                    case RecordType.KEY: _data = new KeyRecord(buffer, length); break;
                    case RecordType.PX: _data = new PXRecord(buffer); break;
                    case RecordType.AAAA: _data = new AAAARecord(buffer); break;
                    case RecordType.LOC: _data = new LocRecord(buffer); break;
                    case RecordType.SRV: _data = new SrvRecord(buffer); break;
                    case RecordType.NAPTR: _data = new NaptrRecord(buffer); break;
                    case RecordType.KX: _data = new KXRecord(buffer); break;
                    case RecordType.A6: _data = new A6Record(buffer); break;
                    case RecordType.DNAME: _data = new DNameRecord(buffer); break;
                    case RecordType.DS: _data = new DSRecord(buffer, length); break;
                    case RecordType.TKEY: _data = new TKeyRecord(buffer); break;
                    case RecordType.TSIG: _data = new TSigRecord(buffer); break;
                    default: throw new DnsQueryException("Invalid DNS Record Type in DNS Response", null);
                }
            }
            catch (Exception ex)
            {
                _data = new ExceptionRecord(ex.Message);
                throw ex;
            }
           
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
        
        public int Ttl
        {
            get { return _ttl; }
        }
        
        public IRecordData Data
        {
            get { return _data; }
        }
    }
}
