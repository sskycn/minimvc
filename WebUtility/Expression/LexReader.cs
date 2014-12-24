using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Expression
{
    public class LexReader
    {
        private int _index, _lineIndex, _characterIndex;
        private bool _endOfStream;
        private string _text;

        public LexReader(string text)
        {
            _index = 0;
            _lineIndex = 0;
            _characterIndex = 0;
            _endOfStream = false;
            _text = text;
        }

        public char Read()
        {
            char rval;

            if (_index < _text.Length)
            {
                rval = _text[_index++];

                _characterIndex++;

                if (rval == '\r')
                {
                    _lineIndex++;
                    _characterIndex = 0;
                }
                else if (rval == '\n')
                {
                    _characterIndex = 0;
                }
            }
            else
            {
                _endOfStream = true;
                _characterIndex++;
                rval = '\0';
            }

            return rval;
        }

        public char Peek()
        {
            char rval;

            if (_index < _text.Length)
            {
                rval = _text[_index];
            }
            else
            {
                rval = '\0';
            }

            return rval;
        }

        public int LineIndex
        {
            get { return _lineIndex; }
        }

        public int CharacterIndex
        {
            get { return _characterIndex; }
        }

        public bool EndOfStream
        {
            get { return _endOfStream; }
        }
    }
}
