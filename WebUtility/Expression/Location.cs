using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Expression
{
    public struct Location : IComparable<Location>, IEquatable<Location>
    {
        public static readonly Location Empty = new Location(0, 0);

        private int _lineIndex, _characterIndex;

        public Location(int lineIndex, int characterIndex)
        {
            _lineIndex = lineIndex;
            _characterIndex = characterIndex;
        }

        public int LineIndex
        {
            get { return _lineIndex; }
        }

        public int CharacterIndex
        {
            get { return _characterIndex; }
        }

        public bool IsEmpty
        {
            get { return _lineIndex <= 0 && _characterIndex <= 0; }
        }

        public override string ToString()
        {
            return string.Format("(Line {0}, Col {1})", _lineIndex, _characterIndex);
        }

        public override int GetHashCode()
        {
            return unchecked(87 * _lineIndex.GetHashCode() ^ _characterIndex.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Location)) return false;
            return (Location)obj == this;
        }

        public bool Equals(Location other)
        {
            return this == other;
        }

        public static bool operator ==(Location a, Location b)
        {
            return a.LineIndex == b.LineIndex && a.CharacterIndex == b.CharacterIndex;
        }

        public static bool operator !=(Location a, Location b)
        {
            return a.LineIndex != b.LineIndex || a.CharacterIndex != b.CharacterIndex;
        }

        public static bool operator <(Location a, Location b)
        {
            if (a.LineIndex < b.LineIndex)
                return true;
            else if (a.LineIndex == b.LineIndex)
                return a.CharacterIndex < b.CharacterIndex;
            else
                return false;
        }

        public static bool operator >(Location a, Location b)
        {
            if (a.LineIndex > b.LineIndex)
                return true;
            else if (a.LineIndex == b.LineIndex)
                return a.CharacterIndex > b.CharacterIndex;
            else
                return false;
        }

        public static bool operator <=(Location a, Location b)
        {
            return !(a > b);
        }

        public static bool operator >=(Location a, Location b)
        {
            return !(a < b);
        }

        public int CompareTo(Location other)
        {
            if (this == other)
                return 0;
            if (this < other)
                return -1;
            else
                return 1;
        }
    }
}
