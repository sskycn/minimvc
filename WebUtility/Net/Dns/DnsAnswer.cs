using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Net.Dns
{
    public enum ReturnCode
    {
        Success = 0,
        FormatError = 1,
        ServerFailure = 2,
        NameError = 3,
        NotImplemented = 4,
        Refused = 5,
        Other = 6
    }

    public class DnsAnswer
    {
        private ReturnCode _returnCode = ReturnCode.Other;
        private bool _authoritative;
        private bool _recursive;
        private bool _truncated;
        private List<Question> _questions;
        private List<Answer> _answers;
        private List<Server> _servers;
        private List<Record> _additional;
        private List<Exception> _exceptions;

        public DnsAnswer(byte[] response)
        {
            _questions = new List<Question>();
            _answers = new List<Answer>();
            _servers = new List<Server>();
            _additional = new List<Record>();
            _exceptions = new List<Exception>();
            DataBuffer buffer = new DataBuffer(response, 2);
            byte bits1 = buffer.ReadByte();
            byte bits2 = buffer.ReadByte();
            //Mask off return code
            int returnCode = bits2 & 15;
            if (returnCode > 6) returnCode = 6;
            this._returnCode = (ReturnCode)returnCode;
            //Get Additional Flags
            _authoritative = TestBit(bits1, 2);
            _recursive = TestBit(bits2, 8);
            _truncated = TestBit(bits1, 1);

            int nQuestions = buffer.ReadBEShortInt();
            int nAnswers = buffer.ReadBEShortInt();
            int nServers = buffer.ReadBEShortInt();
            int nAdditional = buffer.ReadBEShortInt();

            //read in questions
            for (int i = 0; i < nQuestions; i++)
            {
                try
                {
                    _questions.Add(new Question(buffer));
                }
                catch (Exception ex)
                {
                    _exceptions.Add(ex);
                }
            }
            //read in answers
            for (int i = 0; i < nAnswers; i++)
            {
                try
                {
                    _answers.Add(new Answer(buffer));
                }
                catch (Exception ex)
                {
                    _exceptions.Add(ex);
                }
            }
            //read in servers
            for (int i = 0; i < nServers; i++)
            {
                try
                {
                    _servers.Add(new Server(buffer));
                }
                catch (Exception ex)
                {
                    _exceptions.Add(ex);
                }
            }
            //read in additional records 
            for (int i = 0; i < nAdditional; i++)
            {
                try
                {
                    _additional.Add(new Record(buffer));
                }
                catch (Exception ex)
                {
                    _exceptions.Add(ex);
                }
            }
        }

        private bool TestBit(Byte b, byte pos)
        {
            byte mask = (byte)(0x01 << pos);
            return ((b & mask) != 0);
        }

        public ReturnCode ReturnCode
        {
            get { return _returnCode; }
        }

        public bool Authoritative
        {
            get { return _authoritative; }
        }

        public bool Recursive
        {
            get { return _recursive; }
        }

        public bool Truncated
        {
            get { return _truncated; }
        }

        public List<Question> Questions
        {
            get { return _questions; }
        }

        public List<Answer> Answers
        {
            get { return _answers; }
        }

        public List<Server> Servers
        {
            get { return _servers; }
        }

        public List<Record> Additional
        {
            get { return _additional; }
        }

        public List<Exception> Exceptions
        {
            get { return _exceptions; }
        }

        public List<DnsEntry> Entries
        {
            get
            {
                List<DnsEntry> res = new List<DnsEntry>();
                foreach (Answer ans in _answers)
                    res.Add(ans);

                foreach (Server svr in _servers)
                    res.Add(svr);

                foreach (Record adl in _additional)
                    res.Add(adl);

                return res;
            }
        }
    }
}
