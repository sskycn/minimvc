using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Expression
{
    public class Token
    {
        private TokenId _tokenId;
        private string _data;
        private Location _startLocation;
        private Location _endLocation;

        public Token(TokenId tokenId)
        {
            _tokenId = tokenId;
            _startLocation = new Location(-1, -1);
            _endLocation = new Location(-1, -1);
        }

        public Token(TokenId tokenId, int lineIndex, int characterIndex)
        {
            _tokenId = tokenId;
            _startLocation = new Location(lineIndex, characterIndex);
            _endLocation = new Location(lineIndex, characterIndex + 1);
        }

        public Token(TokenId tokenId, string data, int lineIndex, int characterIndex)
        {
            _tokenId = tokenId;
            _data = data;
            _startLocation = new Location(lineIndex, characterIndex);
            _endLocation = new Location(lineIndex, characterIndex + 1);
        }

        public Token(TokenId tokenId, int lineIndex, int characterIndex, int length)
        {
            _tokenId = tokenId;
            _startLocation = new Location(lineIndex, characterIndex);
            _endLocation = new Location(lineIndex, characterIndex + length);
        }

        public Token(TokenId tokenId, string data, int lineIndex, int characterIndex, int length)
        {
            _tokenId = tokenId;
            _data = data;
            _startLocation = new Location(lineIndex, characterIndex);
            _endLocation = new Location(lineIndex, characterIndex + length);
        }

        public Token(TokenId tokenId, Location startLocation, Location endLocation)
        {
            _tokenId = tokenId;
            _startLocation = startLocation;
            _endLocation = endLocation;
        }

        public Token(TokenId tokenId, string data, Location startLocation, Location endLocation)
        {
            _tokenId = tokenId;
            _data = data;
            _startLocation = startLocation;
            _endLocation = endLocation;
        }

        public TokenId TokenId
        {
            get { return _tokenId; }
        }

        public string Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public Location StartLocation
        {
            get { return _startLocation; }
        }

        public Location EndLocation
        {
            get { return _endLocation; }
        }

        public override string ToString()
        {
            return string.Concat(_tokenId, ' ', _startLocation, ' ', _endLocation, ' ', _data);
        }
    }
}
