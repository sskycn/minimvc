using System;
using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Expression
{
    class Keys
    {
        private readonly static Dictionary<string, TokenId> _keys;

        static Keys()
        {
            _keys = new Dictionary<string, TokenId>();

            _keys.Add("var", TokenId.Var);
            _keys.Add("if", TokenId.If);
            _keys.Add("else", TokenId.Else);
            _keys.Add("for", TokenId.For);
            _keys.Add("in", TokenId.In);
            _keys.Add("continue", TokenId.Continue);
            _keys.Add("break", TokenId.Break);
            _keys.Add("block", TokenId.Block);
            _keys.Add("echo", TokenId.Echo);
            _keys.Add("true", TokenId.True);
            _keys.Add("false", TokenId.False);
            _keys.Add("null", TokenId.Null);

        }

        public static void Init()
        {

        }

        public static bool TryGetValue(string key, out TokenId tokenId)
        {
            return _keys.TryGetValue(key, out tokenId);
        }
    }
}
