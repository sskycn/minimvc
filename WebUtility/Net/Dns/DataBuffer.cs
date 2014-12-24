using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Cvv.WebUtility.Net.Dns
{
    public class DataBuffer
    {
        private Byte[] _data;
        private int _pos = 0;

        public DataBuffer(Byte[] data)
            : this(data, 0)
        {
        }

        public DataBuffer(Byte[] data, int pos)
        {
            this._data = data;
            this._pos = pos;
        }

        public byte Next
        {
            get { return _data[_pos]; }
        }

        public byte ReadByte()
        {
            return _data[_pos++];
        }

        public short ReadShortInt()
        {
            return (short)(ReadByte() | ReadByte() << 8);
        }

        public short ReadBEShortInt()
        {
            return (short)(ReadByte() << 8 | ReadByte());
        }

        public ushort ReadShortUInt()
        {
            return (ushort)(ReadByte() | ReadByte() << 8);
        }

        public ushort ReadBEShortUInt()
        {
            return (ushort)(ReadByte() << 8 | ReadByte());
        }

        public int ReadInt()
        {
            return (int)(ReadBEShortUInt() << 16 | ReadBEShortUInt());
        }

        public uint ReadUInt()
        {
            return (uint)(ReadBEShortUInt() << 16 | ReadBEShortUInt());
        }

        public long ReadLongInt()
        {
            return ReadInt() | ReadInt() << 32;
        }

        public string ReadDomainName()
        {
            return ReadDomainName(1);
        }

        public string ReadDomainName(int depth)
        {
            //if (depth > 3) return String.Empty;
            StringBuilder domain = new StringBuilder();
            int length = 0;
            //read in each labels length and chars iuntil there is no more
            length = ReadByte();
            while (length != 0)
            {
                //Is this name conpressed?
                if ((length & 0xc0) == 0xc0)
                {   //Yes it is
                    //calculate address of reference label
                    int posReference = ((length & 0x3f) << 8 | ReadByte());
                    int oldPosition = _pos;
                    _pos = posReference;
                    domain.Append( ReadDomainName(depth + 1));  
                    _pos = oldPosition;
                    //length = ReadByte();
                    return domain.ToString();
                }
                else
                {   //No it isn't read the label
                    for (int i = 0; i < length; i++)
                    {
                        domain.Append((char)ReadByte());
                    }
                }
                if (Next != 0) //Not the end of the domain name get the next segment
                    domain.Append('.');
                length = ReadByte();
            }
            return domain.ToString();
        }

        public IPAddress ReadIPAddress()
        {
            Byte[] address = new Byte[4];
            for (int i = 0; i < 4; i++)
                address[i] = ReadByte();
            return new IPAddress(address);
        }

        public IPAddress ReadIPv6Address()
        {
            Byte[] address = new Byte[16];
            for (int i = 0; i < 16; i++)
                address[i] = ReadByte();
            return new IPAddress(address);
        }

        public Byte[] ReadBytes(int length)
        {
            Byte[] res = new Byte[length];
            for (int i = 0; i < length; i++)
                res[i] = ReadByte();
            return res;
        }

        public string ReadCharString()
        {
            int length = ReadByte();
            StringBuilder txt = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                txt.Append((char)ReadByte());
            }
            return txt.ToString();
        }

        public int Position
        {
            get { return _pos; }
            set { _pos = value; }
        }
    }
}
