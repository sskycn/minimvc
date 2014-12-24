using System.Collections.Generic;
using System.Text;

namespace Cvv.WebUtility.Expression
{
    public class HtmlLexer : ILexer
    {
        private Stack<TokenId> _stack;
        private TokenCollection _tokens;
        private LexReader _src;
        private char _curChar;
        private StringBuilder _html = new StringBuilder();

        public TokenCollection Lex(LexReader src)
        {
            _stack = new Stack<TokenId>();
            _tokens = new TokenCollection();
            _src = src;
            _curChar = src.Read();

            Lex();

            return _tokens;
        }

        private void Lex()
        {
            _html.Length = 0;
            Location startLocation = new Location(_src.LineIndex, _src.CharacterIndex - 1);

            while (!_src.EndOfStream)
            {
                switch (_curChar)
                {
                    case '$':
                        {
                            _curChar = _src.Read();

                            if (_html.Length > 0)
                            {
                                Location endLocation = new Location(_src.LineIndex, _src.CharacterIndex - 2);
                                _tokens.Add(new Token(TokenId.Html, _html.ToString(), startLocation, endLocation));
                                _html.Length = 0;
                            }

                            if (_curChar == '{')
                            {
                                TokenId tokenId = TokenId.Block;
                                _stack.Push(tokenId);
                                _tokens.Add(new Token(tokenId, _src.LineIndex, _src.CharacterIndex - 2, 2));
                                _curChar = _src.Read();
                                LexCode(true, '\0');
                                startLocation = new Location(_src.LineIndex, _src.CharacterIndex - 1);
                            }
                            else if (WillBeKeyOrIdent(_curChar))
                            {
                                _tokens.Add(new Token(TokenId.Dollar, _src.LineIndex, _src.CharacterIndex - 2, 1));

                                Token last = LexKeyOrIdent();
                                _tokens.Add(last);

                                if (last.TokenId == TokenId.Ident || last.TokenId == TokenId.Echo)
                                {
                                    LexInvoke();
                                }
                                else
                                {
                                    LexCode(false, '\0');
                                }

                                startLocation = new Location(_src.LineIndex, _src.CharacterIndex - 1);
                            }
                            else
                            {
                                _html.Append('$');
                            }

                            break;
                        }
                    case '}':
                        {
                            if (_stack.Count > 0 && _stack.Peek() == TokenId.LCurly)
                            {
                                if (_html.Length > 0)
                                {
                                    Location endLocation = new Location(_src.LineIndex, _src.CharacterIndex - 1);
                                    string data = _html.ToString();
                                    _tokens.Add(new Token(TokenId.Html, data, startLocation, endLocation));
                                    _html.Length = 0;
                                }

                                LexCode(false, '\0');
                                _html.Length = 0;
                                startLocation = new Location(_src.LineIndex, _src.CharacterIndex - 1);
                            }
                            else
                            {
                                _html.Append(_curChar);
                                _curChar = _src.Read();
                            }
                            break;
                        }
                    case '\\':
                        {
                            _curChar = _src.Read();

                            if (_curChar == '$' || _curChar == '}')
                            {
                                _html.Append(_curChar);
                            }
                            else
                            {
                                _html.Append('\\');
                                _html.Append(_curChar);
                            }

                            _curChar = _src.Read();
                            break;
                        }
                    default:
                        {
                            _html.Append(_curChar);
                            _curChar = _src.Read();
                            break;
                        }
                }
            }

            if (_html.Length > 0)
            {
                Location endLocation = new Location(_src.LineIndex, _src.CharacterIndex - 1);
                _tokens.Add(new Token(TokenId.Html, _html.ToString(), startLocation, endLocation));
                _html.Length = 0;
            }
        }

        private void LexInvoke()
        {
            bool keep = true;

            while (keep && !_src.EndOfStream)
            {
                switch (_curChar)
                {
                    case '(':
                        {
                            TokenId tokenId = TokenId.LParen;
                            _stack.Push(tokenId);
                            _tokens.Add(new Token(tokenId, _src.LineIndex, _src.CharacterIndex - 1, 1));
                            _curChar = _src.Read();

                            LexCode(false, ')');
                            break;
                        }
                    case ')':
                        {
                            if (_stack.Count > 0 && _stack.Peek() == TokenId.LParen)
                            {
                                _stack.Pop();
                                TokenId tokenId = TokenId.RParen;
                                _tokens.Add(new Token(tokenId, _src.LineIndex, _src.CharacterIndex - 1, 1));
                                _curChar = _src.Read();
                            }

                            keep = false;
                            break;
                        }
                    case '[':
                        {
                            TokenId tokenId = TokenId.LBracket;
                            _stack.Push(tokenId);
                            _tokens.Add(new Token(tokenId, _src.LineIndex, _src.CharacterIndex - 1, 1));
                            _curChar = _src.Read();

                            if (_curChar == '"')
                            {
                                LexString();
                            }
                            break;
                        }
                    case ']':
                        {
                            if (_stack.Count > 0 && _stack.Peek() == TokenId.LBracket)
                            {
                                _stack.Pop();
                                TokenId tokenId = TokenId.RBracket;
                                _tokens.Add(new Token(tokenId, _src.LineIndex, _src.CharacterIndex - 1, 1));
                                _curChar = _src.Read();
                            }
                            else
                            {
                                keep = false;
                            }

                            break;
                        }
                    case '.':
                        {
                            if (WillBeKeyOrIdent(_src.Peek()))
                            {
                                _tokens.Add(new Token(TokenId.Dot, _src.LineIndex, _src.CharacterIndex - 1, 1));
                                _curChar = _src.Read();
                            }
                            else
                            {
                                keep = false;
                            }

                            break;
                        }
                    default:
                        {
                            if (WillBeKeyOrIdent(_curChar))
                            {
                                _tokens.Add(LexKeyOrIdent());
                            }
                            else
                            {
                                keep = false;
                            }
                            break;
                        }
                }
            }

            if (_curChar == ';')
                _curChar = _src.Read();
        }

        private void LexCode(bool isBlock, char endChar)
        {
            bool keep = true;

            while (keep && !_src.EndOfStream && _curChar != endChar)
            {
                switch (_curChar)
                {
                    #region blank
                    case '\t':
                        {
                            do { _curChar = _src.Read(); } while (_curChar == '\t');
                            break;
                        }
                    case ' ':
                        {
                            do { _curChar = _src.Read(); } while (_curChar == ' ');
                            break;
                        }
                    case '\r':
                        {
                            _curChar = _src.Read();
                            if (_curChar == '\n')
                                _curChar = _src.Read();
                            break;
                        }
                    case '\n':
                        {
                            _curChar = _src.Read();
                            break;
                        }
                    #endregion
                    #region operator
                    case '{':
                        {
                            TokenId tokenId = TokenId.LCurly;
                            _stack.Push(tokenId);
                            _tokens.Add(new Token(tokenId, _src.LineIndex, _src.CharacterIndex - 1, 1));
                            _curChar = _src.Read();
                            if (!isBlock) keep = false;

                            while (_curChar == '\r' || _curChar == '\n')
                            {
                                _curChar = _src.Read();
                            }
                            break;
                        }
                    case '}':
                        {
                            if (_stack.Count > 0 && (_stack.Peek() == TokenId.LCurly || _stack.Peek() == TokenId.Block))
                            {
                                TokenId last = _stack.Pop();

                                _tokens.Add(new Token(TokenId.RCurly, _src.LineIndex, _src.CharacterIndex - 1, 1));
                                _curChar = _src.Read();

                                if (isBlock)
                                {
                                    if (last == TokenId.Block)
                                    {
                                        keep = false;
                                    }
                                }
                                else
                                {
                                    Location startLocation = new Location(_src.LineIndex, _src.CharacterIndex - 1);

                                    while (_curChar == '\t' || _curChar == ' ' || _curChar == '\r' || _curChar == '\n')
                                    {
                                        _html.Append(_curChar);
                                        _curChar = _src.Read();
                                    }

                                    if (WillBeKeyOrIdent(_curChar))
                                    {
                                        Token token = LexKeyOrIdent();

                                        if (token.TokenId != TokenId.Else)
                                        {
                                            _html.Append(token.Data);
                                            keep = false;

                                            if (_html.Length > 0)
                                            {
                                                Location endLocation = new Location(_src.LineIndex, _src.CharacterIndex - 1);
                                                _tokens.Add(new Token(TokenId.Html, _html.ToString(), startLocation, endLocation));
                                            }
                                        }
                                        else
                                        {
                                            _tokens.Add(token);
                                        }
                                    }
                                    else
                                    {
                                        keep = false;

                                        if (_html.Length > 0)
                                        {
                                            Location endLocation = new Location(_src.LineIndex, _src.CharacterIndex - 1);
                                            _tokens.Add(new Token(TokenId.Html, _html.ToString(), startLocation, endLocation));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                _tokens.Add(new Token(TokenId.Unknow, _curChar.ToString(), _src.LineIndex, _src.CharacterIndex - 1, 1));
                                _curChar = _src.Read();
                            }
                            break;
                        }
                    case '(':
                        {
                            TokenId tokenId = TokenId.LParen;
                            _stack.Push(tokenId);
                            _tokens.Add(new Token(tokenId, _src.LineIndex, _src.CharacterIndex - 1, 1));
                            _curChar = _src.Read();
                            break;
                        }
                    case ')':
                        {
                            if (_stack.Count > 0 && _stack.Peek() == TokenId.LParen)
                            {
                                _stack.Pop();
                                _tokens.Add(new Token(TokenId.RParen, _src.LineIndex, _src.CharacterIndex - 1, 1));
                                _curChar = _src.Read();
                            }
                            else
                            {
                                _tokens.Add(new Token(TokenId.Unknow, _curChar.ToString(), _src.LineIndex, _src.CharacterIndex - 1, 1));
                                _curChar = _src.Read();
                            }
                            break;
                        }
                    case '[':
                        {
                            TokenId tokenId = TokenId.LBracket;
                            _stack.Push(tokenId);
                            _tokens.Add(new Token(tokenId, _src.LineIndex, _src.CharacterIndex - 1, 1));
                            _curChar = _src.Read();
                            break;
                        }
                    case ']':
                        {
                            if (_stack.Count > 0 && _stack.Peek() == TokenId.LBracket)
                            {
                                _stack.Pop();
                                _tokens.Add(new Token(TokenId.RBracket, _src.LineIndex, _src.CharacterIndex - 1, 1));
                                _curChar = _src.Read();
                            }
                            else
                            {
                                _tokens.Add(new Token(TokenId.Unknow, _curChar.ToString(), _src.LineIndex, _src.CharacterIndex - 1, 1));
                                _curChar = _src.Read();
                            }
                            break;
                        }
                    case '"':
                        {
                            LexString();
                            break;
                        }
                    case '+':
                        {
                            _curChar = _src.Read();

                            if (_curChar == '=')
                            {
                                _tokens.Add(new Token(TokenId.PlusEqual, _src.LineIndex, _src.CharacterIndex - 2, 2));
                                _curChar = _src.Read();
                            }
                            else if (_curChar == '+')
                            {
                                _tokens.Add(new Token(TokenId.PlusPlus, _src.LineIndex, _src.CharacterIndex - 2, 2));
                                _curChar = _src.Read();
                            }
                            else
                            {
                                _tokens.Add(new Token(TokenId.Plus, _src.LineIndex, _src.CharacterIndex - 2, 1));
                            }

                            break;
                        }
                    case '-':
                        {
                            char c = _src.Peek();

                            if (c == '=')
                            {
                                _src.Read();
                                _tokens.Add(new Token(TokenId.MinusEqual, _src.LineIndex, _src.CharacterIndex - 2, 2));
                                _curChar = _src.Read();
                            }
                            else if (c == '-')
                            {
                                _src.Read();
                                _tokens.Add(new Token(TokenId.MinusMinus, _src.LineIndex, _src.CharacterIndex - 2, 2));
                                _curChar = _src.Read();
                            }
                            else
                            {
                                _tokens.Add(new Token(TokenId.Minus, _src.LineIndex, _src.CharacterIndex - 1, 1));
                                _curChar = _src.Read();
                            }

                            break;
                        }
                    case '*':
                        {
                            _curChar = _src.Read();

                            if (_curChar == '=')
                            {
                                _tokens.Add(new Token(TokenId.StarEqual, _src.LineIndex, _src.CharacterIndex - 2, 2));
                                _curChar = _src.Read();
                            }
                            else
                            {
                                _tokens.Add(new Token(TokenId.Star, _src.LineIndex, _src.CharacterIndex - 2, 1));
                            }

                            break;
                        }
                    case '/':
                        {
                            _curChar = _src.Read();

                            if (_curChar == '=')
                            {
                                _tokens.Add(new Token(TokenId.SlashEqual, _src.LineIndex, _src.CharacterIndex - 2, 2));
                                _curChar = _src.Read();
                            }
                            else if (_curChar == '/')
                            {
                                _tokens.Add(new Token(TokenId.Comment, _src.LineIndex, _src.CharacterIndex - 2, 2));
                                _curChar = _src.Read();
                            }
                            else
                            {
                                _tokens.Add(new Token(TokenId.Slash, _src.LineIndex, _src.CharacterIndex - 2, 1));
                            }

                            break;
                        }
                    case '%':
                        {
                            _curChar = _src.Read();

                            if (_curChar == '=')
                            {
                                _tokens.Add(new Token(TokenId.PercentEqual, _src.LineIndex, _src.CharacterIndex - 2, 2));
                                _curChar = _src.Read();
                            }
                            else
                            {
                                _tokens.Add(new Token(TokenId.Percent, _src.LineIndex, _src.CharacterIndex - 2, 1));
                            }

                            break;
                        }
                    case '&':
                        {
                            _curChar = _src.Read();

                            if (_curChar == '&')
                            {
                                _tokens.Add(new Token(TokenId.And, _src.LineIndex, _src.CharacterIndex - 2, 2));
                                _curChar = _src.Read();
                            }
                            else
                            {
                                _tokens.Add(new Token(TokenId.Unknow, _src.LineIndex, _src.CharacterIndex - 2, 1));
                            }

                            break;
                        }
                    case '|':
                        {
                            _curChar = _src.Read();

                            if (_curChar == '|')
                            {
                                _tokens.Add(new Token(TokenId.Or, _src.LineIndex, _src.CharacterIndex - 2, 2));
                                _curChar = _src.Read();
                            }
                            else
                            {
                                _tokens.Add(new Token(TokenId.Unknow, _src.LineIndex, _src.CharacterIndex - 2, 1));
                            }

                            break;
                        }
                    case '!':
                        {
                            _curChar = _src.Read();

                            if (_curChar == '=')
                            {
                                _tokens.Add(new Token(TokenId.NotEqual, _src.LineIndex, _src.CharacterIndex - 2, 2));
                                _curChar = _src.Read();
                            }
                            else
                            {
                                _tokens.Add(new Token(TokenId.Not, _src.LineIndex, _src.CharacterIndex - 2, 1));
                            }

                            break;
                        }
                    case '=':
                        {
                            _curChar = _src.Read();
                            if (_curChar == '=')
                            {
                                _tokens.Add(new Token(TokenId.EqualEqual, _src.LineIndex, _src.CharacterIndex - 2, 2));
                                _curChar = _src.Read();
                            }
                            else
                            {
                                _tokens.Add(new Token(TokenId.Equal, _src.LineIndex, _src.CharacterIndex - 2, 1));
                            }
                            break;
                        }
                    case '>':
                        {
                            _curChar = _src.Read();

                            if (_curChar == '=')
                            {
                                _tokens.Add(new Token(TokenId.GreaterEqual, _src.LineIndex, _src.CharacterIndex - 2, 2));
                                _curChar = _src.Read();
                            }
                            else
                            {
                                _tokens.Add(new Token(TokenId.Greater, _src.LineIndex, _src.CharacterIndex - 2, 1));
                            }

                            break;
                        }
                    case '<':
                        {
                            _curChar = _src.Read();

                            if (_curChar == '=')
                            {
                                _tokens.Add(new Token(TokenId.LessEqual, _src.LineIndex, _src.CharacterIndex - 2, 2));
                                _curChar = _src.Read();
                            }
                            else
                            {
                                _tokens.Add(new Token(TokenId.Less, _src.LineIndex, _src.CharacterIndex - 2, 1));
                            }

                            break;
                        }
                    case '?':
                        {
                            _curChar = _src.Read();

                            if (_curChar == '?')
                            {
                                _tokens.Add(new Token(TokenId.QuestionQuestion, _src.LineIndex, _src.CharacterIndex - 2, 2));
                                _curChar = _src.Read();
                            }
                            else
                            {
                                _tokens.Add(new Token(TokenId.Question, _src.LineIndex, _src.CharacterIndex - 2, 1));
                            }

                            break;
                        }
                    case ',':
                        {
                            _tokens.Add(new Token(TokenId.Comma, _src.LineIndex, _src.CharacterIndex - 1, 1));
                            _curChar = _src.Read();
                            break;
                        }
                    case '.':
                        {
                            if (WillBeNumber(_src.Peek()))
                            {
                                LexNumber();
                            }
                            else
                            {
                                _tokens.Add(new Token(TokenId.Dot, _src.LineIndex, _src.CharacterIndex - 1, 1));
                                _curChar = _src.Read();
                            }

                            break;
                        }
                    case ';':
                        {
                            _tokens.Add(new Token(TokenId.Semi, _src.LineIndex, _src.CharacterIndex - 1, 1));
                            _curChar = _src.Read();
                            break;
                        }
                    case ':':
                        {
                            _tokens.Add(new Token(TokenId.Colon, _src.LineIndex, _src.CharacterIndex - 1, 1));
                            _curChar = _src.Read();
                            break;
                        }
                    #endregion
                    default:
                        {
                            if (WillBeKeyOrIdent(_curChar))
                            {
                                _tokens.Add(LexKeyOrIdent());
                            }
                            else if (WillBeNumber(_curChar))
                            {
                                LexNumber();
                            }
                            else
                            {
                                keep = false;
                            }
                            break;
                        }
                }
            }
        }

        private void LexString()
        {
            Location startLocation = new Location(_src.LineIndex, _src.CharacterIndex - 1);
            _html.Length = 0;
            _curChar = _src.Read();
            bool keep = true;

            bool isQuote = false;

            if (_curChar == '~')
            {
                _html.Append(_curChar);
                _curChar = _src.Read();

                if (_curChar == '/')
                {
                    isQuote = true;
                }
            }

            while (keep && !_src.EndOfStream)
            {
                switch (_curChar)
                {
                    case '\\':
                        {
                            _curChar = _src.Read();

                            switch (_curChar)
                            {
                                case '0':
                                    {
                                        _html.Append('\0');
                                        _curChar = _src.Read();
                                        break;
                                    }
                                case 'a':
                                    {
                                        _html.Append('\a');
                                        _curChar = _src.Read();
                                        break;
                                    }
                                case 'b':
                                    {
                                        _html.Append('\b');
                                        _curChar = _src.Read();
                                        break;
                                    }
                                case 'f':
                                    {
                                        _html.Append('\f');
                                        _curChar = _src.Read();
                                        break;
                                    }
                                case 'n':
                                    {
                                        _html.Append('\n');
                                        _curChar = _src.Read();
                                        break;
                                    }
                                case 'r':
                                    {
                                        _html.Append('\r');
                                        _curChar = _src.Read();
                                        break;
                                    }
                                case 't':
                                    {
                                        _html.Append('\t');
                                        _curChar = _src.Read();
                                        break;
                                    }
                                case 'v':
                                    {
                                        _html.Append('\v');
                                        _curChar = _src.Read();
                                        break;
                                    }
                                case '\\':
                                    {
                                        _html.Append('\\');
                                        _curChar = _src.Read();
                                        break;
                                    }
                                case '\'':
                                    {
                                        _html.Append('\'');
                                        _curChar = _src.Read();
                                        break;
                                    }
                                case '"':
                                    {
                                        _html.Append('\"');
                                        _curChar = _src.Read();
                                        break;
                                    }
                                default:
                                    {
                                        _html.Append(_curChar);
                                        _curChar = _src.Read();
                                        break;
                                    }
                            }

                            break;
                        }
                    case '"':
                        {
                            keep = false;
                            _curChar = _src.Read();
                            break;
                        }
                    default:
                        {
                            _html.Append(_curChar);
                            _curChar = _src.Read();
                            break;
                        }
                }
            }

            string data = _html.ToString();
            _html.Length = 0;
            Location endLocation = new Location(_src.LineIndex, _src.CharacterIndex - 1);
            _tokens.Add(new Token(isQuote ? TokenId.Quote : TokenId.String, data, startLocation, endLocation));
        }

        private void LexNumber()
        {
            Location startLocation = new Location(_src.LineIndex, _src.CharacterIndex - 1);
            _html.Length = 0;

            TokenId tokenId = TokenId.Int;

            if (_curChar == '-')
            {
                _html.Append(_curChar);
                _curChar = _src.Read();
            }

            if (_curChar == '0')
            {
                _html.Append(_curChar);
                _curChar = _src.Read();

                if (_curChar == '.' && _src.Peek() >= 0 && _src.Peek() <= '9')
                {
                    tokenId = TokenId.Double;

                    do
                    {
                        _html.Append(_curChar);
                        _curChar = _src.Read();
                    } while (_curChar >= '0' && _curChar <= '9');
                }
            }
            else if (_curChar == '.' && _src.Peek() >= 0 && _src.Peek() <= '9')
            {
                _html.Append('0');
                tokenId = TokenId.Double;

                do
                {
                    _html.Append(_curChar);
                    _curChar = _src.Read();
                } while (_curChar >= '0' && _curChar <= '9');
            }
            else if (_curChar > '0' && _curChar <= '9')
            {
                do
                {
                    _html.Append(_curChar);
                    _curChar = _src.Read();
                } while (_curChar >= '0' && _curChar <= '9');

                if (_curChar == '.' && _src.Peek() >= 0 && _src.Peek() <= '9')
                {
                    tokenId = TokenId.Double;

                    do
                    {
                        _html.Append(_curChar);
                        _curChar = _src.Read();
                    } while (_curChar >= '0' && _curChar <= '9');
                }
            }

            string data = _html.ToString();
            _html.Length = 0;

            Location endLocation = new Location(_src.LineIndex, _src.CharacterIndex - 1);
            _tokens.Add(new Token(tokenId, data, startLocation, endLocation));
        }

        private Token LexKeyOrIdent()
        {
            Location startLocation = new Location(_src.LineIndex, _src.CharacterIndex - 1);
            bool exit = false;
            _html.Length = 0;
            _html.Append(_curChar);
            _curChar = _src.Read();

            while (!_src.EndOfStream && !exit)
            {
                switch (_curChar)
                {
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                    case 'g':
                    case 'h':
                    case 'i':
                    case 'j':
                    case 'k':
                    case 'l':
                    case 'm':
                    case 'n':
                    case 'o':
                    case 'p':
                    case 'q':
                    case 'r':
                    case 's':
                    case 't':
                    case 'u':
                    case 'v':
                    case 'w':
                    case 'x':
                    case 'y':
                    case 'z':
                    case 'A':
                    case 'B':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                    case 'G':
                    case 'H':
                    case 'I':
                    case 'J':
                    case 'K':
                    case 'L':
                    case 'M':
                    case 'N':
                    case 'O':
                    case 'P':
                    case 'Q':
                    case 'R':
                    case 'S':
                    case 'T':
                    case 'U':
                    case 'V':
                    case 'W':
                    case 'X':
                    case 'Y':
                    case 'Z':
                    case '_':
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        {
                            _html.Append(_curChar);
                            _curChar = _src.Read();
                            break;
                        }
                    default:
                        {
                            exit = true;
                            break;
                        }
                }
            }

            string ident = _html.ToString();
            _html.Length = 0;
            TokenId tokenId;

            Location endLocation = new Location(_src.LineIndex, _src.CharacterIndex - 1);

            if (!Keys.TryGetValue(ident, out tokenId))
            {
                tokenId = TokenId.Ident;
            }

            Token token = new Token(tokenId, ident, startLocation, endLocation);

            return token;
        }

        private static bool WillBeKeyOrIdent(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
        }

        private static bool WillBeNumber(char c)
        {
            return c >= '0' && c <= '9';
        }
    }
}
