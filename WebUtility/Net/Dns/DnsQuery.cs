using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Cvv.WebUtility.Net.Dns
{
    public enum RecordType
    {     
        None = 0,      
        A = 1,
        NS = 2,
        CNAME = 5, 
        SOA = 6,
        MB = 7,
        MG = 8,
        MR = 9,
        NULL = 10,
        WKS = 11,
        PTR = 12,
        HINFO = 13,
        MINFO = 14,
        MX = 15,
        TXT = 16,
        RP = 17,
        AFSDB = 18,
        X25 = 19,
        ISDN = 20,
        RT = 21,
        NSAP = 22,
        SIG = 24,
        KEY = 25,
        PX = 26, 
        AAAA = 28,
        LOC = 29,
        SRV = 33,
        NAPTR = 35,
        KX = 36,
        A6 = 38,         
        DNAME = 39, 
        DS = 43,                  
        TKEY = 249, 
        TSIG = 250,
        All = 255 
    };

    public enum OpCode
    {
        StandardQuery   = 0,
        InverseQuery    = 1,
        StatusRequest   = 2        
    }

    public class DnsQueryException : Exception
    {
        private Exception[] exceptions;

        public DnsQueryException(string msg, Exception[] exs)
            : base(msg)
        {
            exceptions = exs;
        }
    }

    public class DnsQuery
    {
        const int DNS_PORT = 53;
        const int MAX_TRIES = 1;
        const byte IN_CLASS = 1;

        private byte[] _query;
        private string _domain;
        private IPEndPoint _dnsServer;
        private bool _recursiveQuery = true;
        private Socket _reqSocket;
        private int _numTries;
        private int _reqId;

        public DnsQuery() { }

        public DnsQuery(string serverUrl)
        {            
            IPHostEntry hostAddress = System.Net.Dns.GetHostEntry(serverUrl);
            if (hostAddress.AddressList.Length > 0)
                _dnsServer = new IPEndPoint(hostAddress.AddressList[0], DNS_PORT);
            else
                throw new DnsQueryException("Invalid DNS Server Name Specified", null);
        }

        public DnsQuery(IPAddress dnsAddress)
        {            
            _dnsServer = new IPEndPoint(dnsAddress, DNS_PORT);
        }

        public DnsAnswer QueryServer(RecordType recType, int timeout)
        {
            if (_dnsServer == null)
                throw new DnsQueryException("There is no Dns server set in Dns Query Component", null);
            if (!ValidRecordType(recType))
                throw new DnsQueryException("Invalid Record Type submitted to Dns Query Component", null);

            //Result object
            DnsAnswer res = null;

            //UDP being unreliable we may need to try multiple times 
            //therefore we count how many times we have tried
            _numTries = 0;

            //as per RFC 1035  there is a max size on the dns response
            byte[] dnsResponse = new byte[512];
            Exception[] exceptions = new Exception[MAX_TRIES];

            while (_numTries < MAX_TRIES)
            {
                try
                {
                    CreateDnsQuery(recType);
                    _reqSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                    _reqSocket.ReceiveTimeout = timeout;

                    _reqSocket.SendTo(_query, _query.Length, SocketFlags.None, _dnsServer);
                    _reqSocket.Receive(dnsResponse);

                    if (dnsResponse[0] == _query[0] && dnsResponse[1] == _query[1])
                        //this is our response so format and return the answer to the query
                        res = new DnsAnswer(dnsResponse);
                    _numTries++;
                    if (res.ReturnCode == ReturnCode.Success)
                        return res;
                }
                catch (SocketException ex)
                {
                    exceptions[_numTries] = ex;
                    _numTries++;
                    _reqId++;
                    if (_numTries > MAX_TRIES)
                        throw new DnsQueryException("Failure Querying DNS Server", exceptions);
                }
                finally
                {
                    _reqId++;
                    _reqSocket.Close();
                    Query = null; //Force a new message be built
                }

            }
            return res;
        }

        public DnsAnswer QueryServer(RecordType recType)
        {
            return this.QueryServer(recType, 5000);
        }

        private void CreateDnsQuery(RecordType recType)
        {
            List<Byte> queryBytes = new List<byte>();            
            queryBytes.Add((byte)(_reqId >> 8));
            queryBytes.Add((byte)(_reqId));
            
            //populate bit fields
            queryBytes.Add((byte)(((byte)OpCode.StandardQuery << 3) | ( RecursiveQuery ? 0x01 : 0x00)));
            queryBytes.Add(0x00);
            
            //set number of questions  we will always use to 1
            queryBytes.Add(0x00);
            queryBytes.Add(0x01);
            
            //no requests, no name servers, no additional records in standard request
            for (int i = 0; i < 6; i++)
                queryBytes.Add(0x00);

            InsertDomainName(queryBytes, _domain);
            //query Type
            queryBytes.Add(0x00);
            queryBytes.Add((byte)recType);
            //query class
            queryBytes.Add(0x00);
            queryBytes.Add(IN_CLASS);
            Query = queryBytes.ToArray();
        }

        private void InsertDomainName(List<Byte> data, string domain)
        {
            //Write each segment of the domain name to the array
            //Each segment is seperated by a '.' token and each segment is 
            //preceded by the couint of characters in the segment.
            int length = 0;
            int pos = 0;
            while (pos < domain.Length)
            {
                int prev_pos = pos;
                pos = domain.IndexOf('.', pos);
                length = pos - prev_pos;
                if (length < 0) //last segment
                    length = domain.Length - prev_pos;

                //Add segment data to array
                data.Add((byte)length);
                for(int i = 0; i < length; i++)
                    data.Add((byte)domain[prev_pos++]);

                //step past the '.'
                pos = prev_pos;
                pos++;
                //pos++;
            }
            //Terminate with a zero
            data.Add(0x00);
        }

        private bool ValidRecordType(RecordType t)
        {
            return (Enum.IsDefined(typeof(RecordType), t) || t == RecordType.All);           
        }

        public byte[] Query
        {
            get { return _query; }
            set { _query = value; }
        }

        public string Domain
        {
            get { return _domain; }
            set 
                {
                    if (value.Length == 0 || value.Length > 255 ||
                        !Regex.IsMatch(value, @"^[a-z|A-Z|0-9|\-|_]{1,63}(\.[a-z|A-Z|0-9|\-]{1,63})+$"))
                        throw new DnsQueryException("Invalid Domain Name", null);
                    _domain = value; 
                }
        }

        public IPEndPoint DnsServer
        {
            get { return _dnsServer; }
            set { _dnsServer = value; }
        }
        
        public bool RecursiveQuery
        {
            get { return _recursiveQuery; }
            set { _recursiveQuery = value; }
        }

        public static List<IPAddress> GetMachineDnsServers()
        {
            List<IPAddress> dnsServers = new List<IPAddress>();

            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface adapter in nics)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                
                    foreach (IPAddress ipAddress in properties.DnsAddresses)
                    {
                        dnsServers.Add(ipAddress);
                    }
                
            }

            return dnsServers;
        }
    }
}
