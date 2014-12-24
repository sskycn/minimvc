using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    class HtmlParser : IParser
    {
        private static readonly int ASSOC_LEFT = 1;
        private static readonly int ASSOC_RIGHT = 0;
        private static readonly byte PRECEDENCE_MULTIPLICATIVE = 0x7f;
        private static readonly byte PRECEDENCE_UNARY = 0x80;
        private static readonly byte PRECEDENCE_PRIMARY = 0x90;

        private static readonly Token _eof = new Token(TokenId.Eof);
        private static byte[] _precedence;

        private TokenCollection _tokens;
        private int _index;
        private Token _curtok;

        public CompilationUnit Parse(string text)
        {
            LexReader src = new LexReader(text);
            HtmlLexer lexer = new HtmlLexer();
            TokenCollection tokens = lexer.Lex(src);

            return Parse(tokens);
        }

        public CompilationUnit Parse(TokenCollection tokens)
        {
            _tokens = tokens;
            _index = 0;

            CompilationUnit compilationUnit = new CompilationUnit();

            try
            {
                Advance();

                while (_curtok != _eof)
                {
                    compilationUnit.Statements.Add(ParseStatement());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return compilationUnit;
        }

        private StatementNode ParseStatement()
        {
            switch (_curtok.TokenId)
            {
                case TokenId.LCurly:
                case TokenId.Block:
                    BlockStatement block = new BlockStatement(_curtok);
                    ParseBlock(block);
                    return block;
                case TokenId.Semi:
                    Advance();
                    return new StatementNode(_curtok);
                case TokenId.If:
                    return ParseIf();
                case TokenId.For:
                    return ParseFor();
                case TokenId.Break:
                    return ParseBreak();
                case TokenId.Continue:
                    return ParseContinue();
                case TokenId.Dollar:
                    return ParseDollar();
                case TokenId.Echo:
                    return ParseEcho();
                case TokenId.Html:
                    return ParseHtml();
                case TokenId.String:
                case TokenId.Int:
                case TokenId.Double:
                case TokenId.True:
                case TokenId.False:
                case TokenId.Null:
                case TokenId.LParen:
                case TokenId.PlusPlus:
                case TokenId.MinusMinus:
                case TokenId.Ident:
                    {
                        ExpressionStatement node = new ExpressionStatement(ParseExpression());
                        AssertAndAdvance(TokenId.Semi);
                        return node;
                    }
                case TokenId.Var:
                    {
                        Advance();
                        ExpressionStatement node = new ExpressionStatement(ParseExpression());
                        AssertAndAdvance(TokenId.Semi);
                        return node;
                    }
                default:
                    {
                        throw new ParseException("Unknow token, " + _curtok);

                    }
            }
        }

        private void ParseBlock(BlockStatement block)
        {
            if (_curtok.TokenId == TokenId.LCurly)
            {
                Token beginToken = _curtok;

                Advance();

                while (_curtok != _eof && _curtok.TokenId != TokenId.RCurly)
                {
                    block.Statements.Add(ParseStatement());
                }

                Token endToken = _curtok;

                AssertAndAdvance(TokenId.RCurly);

                if (beginToken.StartLocation.LineIndex != endToken.StartLocation.LineIndex && _curtok.TokenId == TokenId.Html)
                {
                    string data = _curtok.Data;

                    if (data.StartsWith("\r\n"))
                    {
                        _curtok.Data = data.Substring(2);
                    }
                }

            }
            else
            {
                block.Statements.Add(ParseStatement());
            }
        }

        private IfStatement ParseIf()
        {
            IfStatement node = new IfStatement(_curtok);
            Advance();

            AssertAndAdvance(TokenId.LParen);
            node.Test = ParseExpression();
            AssertAndAdvance(TokenId.RParen);

            ParseBlock(node.Statements);

            if (_curtok.TokenId == TokenId.Else)
            {
                Advance();
                ParseBlock(node.ElseStatements);
            }

            if (_curtok.TokenId == TokenId.Semi)
                Advance();

            return node;
        }

        private StatementNode ParseFor()
        {
            if (Peek(_index + 2).TokenId == TokenId.In || Peek(_index + 3).TokenId == TokenId.In)
            {
                ForeachStatement statement = new ForeachStatement(_curtok);
                Advance();

                AssertAndAdvance(TokenId.LParen);

                if (_curtok.TokenId == TokenId.Var)
                    Advance();

                statement.VarName = _curtok.Data;
                AssertAndAdvance(TokenId.Ident);
                AssertAndAdvance(TokenId.In);
                statement.Collection = ParseExpression();
                AssertAndAdvance(TokenId.RParen);

                ParseBlock(statement.BlockStatement);

                if (_curtok.TokenId == TokenId.Semi)
                    Advance();

                return statement;
            }
            else
            {
                ForStatement statement = new ForStatement(_curtok);
                Advance();
                AssertAndAdvance(TokenId.LParen);

                if (_curtok.TokenId != TokenId.Semi)
                {
                    statement.Init.Add(ParseExpression());
                    while (_curtok.TokenId == TokenId.Comma)
                    {
                        AssertAndAdvance(TokenId.Comma);
                        statement.Init.Add(ParseExpression());
                    }
                }

                AssertAndAdvance(TokenId.Semi);
                statement.Test = ParseExpression();
                AssertAndAdvance(TokenId.Semi);
                if (_curtok.TokenId != TokenId.RParen)
                {
                    statement.Inc.Add(ParseExpression());
                    while (_curtok.TokenId == TokenId.Comma)
                    {
                        AssertAndAdvance(TokenId.Comma);
                        statement.Inc.Add(ParseExpression());
                    }
                }
                AssertAndAdvance(TokenId.RParen);

                ParseBlock(statement.BlockStatement);

                if (_curtok.TokenId == TokenId.Semi)
                {
                    Advance();
                }
                return statement;
            }
        }

        private BreakStatement ParseBreak()
        {
            BreakStatement node = new BreakStatement(_curtok);
            Advance();

            if (_curtok.TokenId == TokenId.Semi)
                Advance();

            return node;
        }

        private ContinueStatement ParseContinue()
        {
            ContinueStatement node = new ContinueStatement(_curtok);
            Advance();

            if (_curtok.TokenId == TokenId.Semi)
                Advance();

            return node;
        }

        private StatementNode ParseDollar()
        {
            Token last = _curtok;

            Advance();
            if (_curtok.TokenId == TokenId.Ident)
            {
                HtmlStatement node = new HtmlStatement(last);
                node.Expression = ParseExpression();
                return node;
            }
            else
            {
                return ParseStatement();
            }
        }

        private EchoStatement ParseEcho()
        {
            EchoStatement node = new EchoStatement(_curtok);

            Advance();

            if (_curtok.TokenId == TokenId.LParen)
            {
                Advance();
                node.Expressions = ParseExpressionList();
                AssertAndAdvance(TokenId.RParen);
            }
            else
            {
                node.Expressions = ParseExpressionList();
            }

            if (_curtok.TokenId == TokenId.Semi)
                Advance();

            return node;
        }

        private HtmlStatement ParseHtml()
        {
            HtmlStatement node = new HtmlStatement(_curtok);

            Token tok = Peek();
            string data = _curtok.Data;

            switch (tok.TokenId)
            {
                case TokenId.RCurly:
                    {
                        int pos = data.LastIndexOf("\r\n");

                        if (pos > 0)
                            _curtok.Data = data.Substring(0, pos + 2);

                        break;
                    }
                case TokenId.Dollar:
                    {
                        Token tok2 = Peek(_index + 1);

                        if (tok2.TokenId == TokenId.If || tok2.TokenId == TokenId.For)
                        {
                            Token tok3 = Search(TokenId.Html);

                            if (tok2.StartLocation.LineIndex != tok3.StartLocation.LineIndex)
                            {
                                data = _curtok.Data.TrimEnd(' ', '\t');

                                if (data.EndsWith("\r\n"))
                                    _curtok.Data = data;
                            }
                        }

                        break;
                    }
            }

            node.Expression = new StringPrimitive(_curtok);

            Advance();

            if (_curtok.TokenId == TokenId.Semi)
                Advance();

            return node;
        }

        private ExpressionNode ParseExpression()
        {
            return ParseSubexpression(1);
        }

        private ExpressionNode ParsePrimaryExpression()
        {
            ExpressionNode node = null;
            TokenId startToken = _curtok.TokenId;

            switch (_curtok.TokenId)
            {
                case TokenId.Null:
                    {
                        node = new NullPrimitive(_curtok);
                        Advance();
                        break;
                    }
                case TokenId.True:
                    {
                        node = new BooleanPrimitive(true, _curtok);
                        Advance();
                        break;
                    }
                case TokenId.False:
                    {
                        node = new BooleanPrimitive(false, _curtok);
                        Advance();
                        break;
                    }
                case TokenId.Int:
                    {
                        node = new IntegralPrimitive(_curtok);
                        Advance();
                        break;
                    }
                case TokenId.Double:
                    {
                        node = new DoublePrimitive(_curtok);
                        Advance();
                        break;
                    }
                case TokenId.String:
                    {
                        node = new StringPrimitive(_curtok);
                        Advance();
                        break;
                    }
                case TokenId.Quote:
                    {
                        node = new QuotePrimitive(_curtok);
                        Advance();
                        break;
                    }
                case TokenId.Ident:
                    {
                        node = new IdentifierExpression(_curtok);
                        Advance();
                        break;
                    }
                case TokenId.LParen:
                    {
                        Advance();
                        node = new ParenthesizedExpression(ParseExpression());
                        AssertAndAdvance(TokenId.RParen);
                        break;
                    }
                case TokenId.LBracket:
                    {
                        node = ParseArrayCreation();
                        AssertAndAdvance(TokenId.RBracket);
                        break;
                    }
                case TokenId.LCurly:
                    {
                        node = ParseDictionaryCreation();
                        AssertAndAdvance(TokenId.RCurly);
                        break;
                    }
                default:
                    {
                        throw new ParseException("Unhandled case in ParseExpression " + _curtok.TokenId);
                    }
            }

            return node;
        }

        private ExpressionNode ParseSubexpression(int precBound)
        {
            ExpressionNode expr;
            switch (_curtok.TokenId)
            {
                case TokenId.Plus:
                case TokenId.Minus:
                case TokenId.Not:
                case TokenId.PlusPlus:
                case TokenId.MinusMinus:
                case TokenId.Star:
                    {
                        UnaryExpression unExpr = new UnaryExpression(_curtok.TokenId, _curtok);
                        Advance();
                        unExpr.Child = ParseSubexpression(PRECEDENCE_UNARY);
                        expr = unExpr;
                        break;
                    }
                default:
                    expr = ParsePrimaryExpression();
                    break;
            }
            return ParseSubexpression(precBound, expr);
        }

        private ExpressionNode ParseSubexpression(int precBound, ExpressionNode left)
        {
            while (true)
            {
                int curPrec = _precedence[(int)_curtok.TokenId];
                if (curPrec < precBound) break;

                int associativity = ASSOC_LEFT;
                TokenId curOp = _curtok.TokenId;
                switch (curOp)
                {
                    case TokenId.Equal:
                    case TokenId.PlusEqual:
                    case TokenId.MinusEqual:
                    case TokenId.StarEqual:
                    case TokenId.SlashEqual:
                    case TokenId.PercentEqual:
                    case TokenId.QuestionQuestion:
                        {
                            associativity = ASSOC_RIGHT;
                            goto case TokenId.Percent;		// "FALL THROUGH"
                        }
                    case TokenId.Greater:
                    case TokenId.Or:
                    case TokenId.And:
                    case TokenId.EqualEqual:
                    case TokenId.NotEqual:
                    case TokenId.Less:
                    case TokenId.LessEqual:
                    case TokenId.GreaterEqual:
                    case TokenId.Plus:
                    case TokenId.Minus:
                    case TokenId.Star:
                    case TokenId.Slash:
                    case TokenId.Percent:
                        {
                            Advance();
                            left = new BinaryExpression(curOp, left, ParseSubexpression(curPrec + associativity));
                            break;
                        }
                    case TokenId.PlusPlus:								// postfix
                        {
                            Advance();
                            left = new PostIncrementExpression(left);
                            break;
                        }
                    case TokenId.MinusMinus:							// postfix
                        {
                            Advance();
                            left = new PostDecrementExpression(left);
                            break;
                        }
                    case TokenId.Question:
                        {
                            Advance();
                            ExpressionNode node = ParseExpression();
                            AssertAndAdvance(TokenId.Colon);
                            left = new ConditionalExpression(left, node, ParseExpression());
                            break;
                        }

                    case TokenId.LParen:								// invocation
                        Advance();
                        left = new InvocationExpression(left, ParseArgumentList());
                        AssertAndAdvance(TokenId.RParen);
                        break;

                    case TokenId.LBracket:								// element access
                        Advance();
                        left = new ElementAccessExpression(left, ParseExpressionList());
                        AssertAndAdvance(TokenId.RBracket);
                        break;

                    case TokenId.Dot:
                        left = ParseMemberAccess(left);
                        break;

                    default:
                        throw new ParseException("Unexpected token");
                }
            }

            return left;
        }

        private ArrayCreationExpression ParseArrayCreation()
        {
            ArrayCreationExpression arNode = new ArrayCreationExpression(_curtok);
            AssertAndAdvance(TokenId.LBracket);

            arNode.Expressions = ParseExpressionList();

            return arNode;
        }

        private DictionaryCreationExpression ParseDictionaryCreation()
        {
            DictionaryCreationExpression dicNode = new DictionaryCreationExpression(_curtok);
            AssertAndAdvance(TokenId.LCurly);

            NodeCollection<NameExpressionNode> nodes = new NodeCollection<NameExpressionNode>();

            dicNode.Expressions = nodes;

            while (_curtok.TokenId != TokenId.RCurly && _curtok != _eof)
            {
                NameExpressionNode node = new NameExpressionNode(_curtok);

                Advance();

                AssertAndAdvance(TokenId.Colon);
                node.Expression = ParseExpression();
                nodes.Add(node);

                if (_curtok.TokenId == TokenId.Comma)
                    Advance();
            }

            return dicNode;
        }

        private NodeCollection<ArgumentNode> ParseArgumentList()
        {
            NodeCollection<ArgumentNode> list = new NodeCollection<ArgumentNode>();
            if (_curtok.TokenId == TokenId.RParen) return list;

            while (true)
            {
                ArgumentNode arg = new ArgumentNode(_curtok);
                arg.Expression = ParseExpression();
                list.Add(arg);
                if (_curtok.TokenId != TokenId.Comma) break;
                Advance();
            }

            return list;
        }

        private ExpressionList ParseExpressionList()
        {
            ExpressionList list = new ExpressionList();
            TokenId last = TokenId.Eof;

            while (true)
            {
                switch (_curtok.TokenId)
                {
                    case TokenId.Comma:
                        {
                            list.Add(new NullPrimitive(_curtok));
                            break;
                        }
                    case TokenId.RBracket:
                        {
                            if (last == TokenId.Comma)
                            {
                                list.Add(new NullPrimitive(_curtok));
                            }
                            break;
                        }
                    default:
                        {
                            list.Add(ParseExpression());
                            break;
                        }
                }

                if (_curtok.TokenId != TokenId.Comma)
                    break;

                last = _curtok.TokenId;
                Advance();
            }

            return list;
        }

        private ExpressionNode ParseMember()
        {
            ExpressionNode result = new IdentifierExpression(_curtok);
            AssertAndAdvance(TokenId.Ident);

            return result;
        }

        private ExpressionNode ParseMemberAccess(ExpressionNode left)
        {
            TokenId qualifierKind = _curtok.TokenId;

            AssertAndAdvance(TokenId.Dot);

            if (_curtok.TokenId != TokenId.Ident)
            {
                throw new ParseException("Right side of member access must be identifier");
            }

            return new MemberAccessExpression(left, ParseMember(), qualifierKind, _curtok.TokenId == TokenId.LParen);
        }

        private void Advance()
        {
            if (_index < _tokens.Count)
            {
                _curtok = _tokens[_index++];
            }
            else
            {
                _curtok = _eof;
            }
        }

        private Token Peek()
        {
            return Peek(_index);
        }

        private Token Peek(int index)
        {
            Token curtok;

            if (index < _tokens.Count)
            {
                curtok = _tokens[index];
            }
            else
            {
                curtok = _eof;
            }

            return curtok;
        }

        private Token Search(TokenId tokenId)
        {
            Token curtok;

            int index = _index;

            do
            {
                curtok = Peek(index++);

            } while (curtok.TokenId != tokenId && curtok != _eof);

            return curtok;
        }

        private void AssertAndAdvance(TokenId tokenId)
        {
            if (_curtok.TokenId != tokenId)
                throw new ParseException(string.Format("Expect token: {0}, but got: {1} at line: {2} column:{3}", tokenId, _curtok.TokenId, _curtok.StartLocation.LineIndex + 1, _curtok.StartLocation.CharacterIndex + 1));

            Advance();
        }

        static HtmlParser()
        {
            _precedence = new byte[0xFF];

            _precedence[(int)TokenId.LBracket] = PRECEDENCE_PRIMARY;
            _precedence[(int)TokenId.Dot] = PRECEDENCE_PRIMARY;

            _precedence[(int)TokenId.LParen] = PRECEDENCE_UNARY;		// 0x80
            _precedence[(int)TokenId.Not] = PRECEDENCE_UNARY;
            _precedence[(int)TokenId.PlusPlus] = PRECEDENCE_UNARY;
            _precedence[(int)TokenId.MinusMinus] = PRECEDENCE_UNARY;
            _precedence[(int)TokenId.Star] = PRECEDENCE_MULTIPLICATIVE; // 0x7f
            _precedence[(int)TokenId.Slash] = PRECEDENCE_MULTIPLICATIVE;
            _precedence[(int)TokenId.Percent] = PRECEDENCE_MULTIPLICATIVE;
            _precedence[(int)TokenId.Plus] = 0x7E;
            _precedence[(int)TokenId.Minus] = 0x7E;
            _precedence[(int)TokenId.Less] = 0x7C;
            _precedence[(int)TokenId.Greater] = 0x7C;
            _precedence[(int)TokenId.LessEqual] = 0x7C;
            _precedence[(int)TokenId.GreaterEqual] = 0x7C;
            _precedence[(int)TokenId.EqualEqual] = 0x7B;
            _precedence[(int)TokenId.NotEqual] = 0x7B;
            _precedence[(int)TokenId.And] = 0x77;
            _precedence[(int)TokenId.Or] = 0x76;
            _precedence[(int)TokenId.QuestionQuestion] = 0x75;
            _precedence[(int)TokenId.Question] = 0x74;

            _precedence[(int)TokenId.Equal] = 0x73;
            _precedence[(int)TokenId.PlusEqual] = 0x73;
            _precedence[(int)TokenId.MinusEqual] = 0x73;
            _precedence[(int)TokenId.StarEqual] = 0x73;
            _precedence[(int)TokenId.SlashEqual] = 0x73;
            _precedence[(int)TokenId.PercentEqual] = 0x73;
        }
    }
}
