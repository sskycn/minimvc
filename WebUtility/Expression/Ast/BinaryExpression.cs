using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    class BinaryExpression : ExpressionNode
    {
        private static SortedList<TokenId, string> _stringMap;
        private TokenId _op;
        private ExpressionNode _left;
        private ExpressionNode _right;

        public BinaryExpression(TokenId op, Token relatedToken)
            : base(relatedToken)
        {
            _op = op;
        }

        public BinaryExpression(TokenId op, ExpressionNode left)
            : base(left.RelatedToken)
        {
            _op = op;
            _left = left;
        }

        public BinaryExpression(TokenId op, ExpressionNode left, ExpressionNode right)
            : base(left.RelatedToken)
        {
            _op = op;
            _left = left;
            _right = right;
        }

        public TokenId Op
        {
            get { return _op; }
        }

        internal ExpressionNode Left
        {
            get { return _left; }
        }


        internal ExpressionNode Right
        {
            get { return _right; }
        }

        public override object Execute(IContext context, Stream writer)
        {
            object result = null;

            if (_op == TokenId.Equal)
            {
                result = Right.Execute(context, writer);

                if (Left is IdentifierExpression)
                {
                    IdentifierExpression ident = Left as IdentifierExpression;
                    context[ident.Name] = result;
                }
            }
            else
            {
                object lValue = Left.Execute(context, writer);
                object rValue = Right.Execute(context, writer);

                switch (_op)
                {
                    case TokenId.EqualEqual:
                        {
                            if (lValue == null)
                            {
                                if (rValue == null)
                                {
                                    result = true;
                                }
                                else
                                {
                                    result = false;
                                }
                            }
                            else
                            {
                                if (rValue == null)
                                {
                                    result = false;
                                }
                                else
                                {
                                    result = Evaluate(_op, lValue, rValue);
                                }
                            }

                            break;
                        }
                    case TokenId.NotEqual:
                        {
                            if (lValue == null)
                            {
                                if (rValue == null)
                                {
                                    result = false;
                                }
                                else
                                {
                                    result = true;
                                }
                            }
                            else
                            {
                                if (rValue == null)
                                {
                                    result = true;
                                }
                                else
                                {
                                    result = Evaluate(_op, lValue, rValue);
                                }
                            }

                            break;
                        }
                    case TokenId.Less:
                    case TokenId.LessEqual:
                    case TokenId.Greater:
                    case TokenId.GreaterEqual:
                        {
                            if (lValue == null)
                            {
                                result = false;
                            }
                            else
                            {
                                if (rValue == null)
                                {
                                    result = false;
                                }
                                else
                                {
                                    result = Evaluate(_op, lValue, rValue);
                                }
                            }

                            break;
                        }
                    default:
                        {
                            result = Evaluate(_op, lValue, rValue);
                            break;
                        }
                }
            }

            return result;
        }

        private object Evaluate(TokenId op, object lValue, object rValue)
        {
            object result = null;

            if (Util.IsInteger(lValue) && Util.IsInteger(rValue))
            {
                result = EvalIntOperator(op, lValue, rValue);
            }
            else if (Util.IsNumber(lValue) && Util.IsNumber(rValue))
            {
                result = EvalDoubleOperator(op, lValue, rValue);
            }
            else if (lValue is DateTime && rValue is DateTime)
            {
                result = EvalDateTimeOperator(op, (DateTime)lValue, (DateTime)rValue);
            }
            else if (lValue is bool && rValue is bool)
            {
                result = EvalBooleanOperator(op, lValue, rValue);
            }
            else if (Util.IsString(lValue) || Util.IsString(rValue))
            {
                result = EvalStringOperator(op, lValue, rValue);
            }
            else
            {
                throw new ParseException("BinaryNotSupport '{0}' between type of '{1}' and '{2}'", op.ToString(), lValue.GetType().Name, rValue.GetType().Name);
            }

            return result;
        }

        private object EvalIntOperator(TokenId op, object lValue, object rValue)
        {
            long result;

            switch (op)
            {
                case TokenId.Plus:
                case TokenId.PlusEqual:
                    {
                        result = (Convert.ToInt64(lValue) + Convert.ToInt64(rValue));
                        break;
                    }
                case TokenId.Minus:
                case TokenId.MinusEqual:
                    {
                        result = (Convert.ToInt64(lValue) - Convert.ToInt64(rValue));
                        break;
                    }
                case TokenId.Star:
                case TokenId.StarEqual:
                    {
                        result = (Convert.ToInt64(lValue) * Convert.ToInt64(rValue));
                        break;
                    }
                case TokenId.Slash:
                case TokenId.SlashEqual:
                    {
                        return (Convert.ToDouble(lValue) / Convert.ToDouble(rValue));
                    }
                case TokenId.Percent:
                case TokenId.PercentEqual:
                    {
                        result = ((Convert.ToInt64(lValue) % Convert.ToInt64(rValue)));
                        break;
                    }
                case TokenId.EqualEqual:
                    {
                        return Convert.ToInt64(lValue) == Convert.ToInt64(rValue);
                    }
                case TokenId.Less:
                    {
                        return Convert.ToInt64(lValue) < Convert.ToInt64(rValue);
                    }
                case TokenId.LessEqual:
                    {
                        return Convert.ToInt64(lValue) <= Convert.ToInt64(rValue);
                    }
                case TokenId.Greater:
                    {
                        return Convert.ToInt64(lValue) > Convert.ToInt64(rValue);
                    }
                case TokenId.GreaterEqual:
                    {
                        return Convert.ToInt64(lValue) >= Convert.ToInt64(rValue);
                    }
                case TokenId.NotEqual:
                    {
                        return Convert.ToInt64(lValue) != Convert.ToInt64(rValue);
                    }
                default:
                    {
                        throw new ParseException("BinaryNotSupport '{0}' between type of '{1}' and '{2}'", op.ToString(), lValue.GetType().Name, rValue.GetType().Name);
                    }
            }

            if (result >= Int32.MinValue && result <= Int32.MaxValue)
                return (int)result;
            else
                return result;
        }

        private object EvalDoubleOperator(TokenId op, object lValue, object rValue)
        {
            if (lValue is decimal && rValue is decimal)
                return EvalDecimalOperator(op, (decimal)lValue, (decimal)rValue);

            double v1 = Util.ToDouble(lValue);
            double v2 = Util.ToDouble(rValue);

            switch (op)
            {
                case TokenId.Plus:
                case TokenId.PlusEqual:
                    return (v1 + v2);
                case TokenId.Minus:
                case TokenId.MinusEqual:
                    return (v1 - v2);
                case TokenId.Star:
                case TokenId.StarEqual:
                    return (v1 * v2);
                case TokenId.Slash:
                case TokenId.SlashEqual:
                    return (v1 / v2);
                case TokenId.Percent:
                case TokenId.PercentEqual:
                    return (v1 % v2);
                case TokenId.EqualEqual:
                    return (v1 == v2);
                case TokenId.Greater:
                    return (v1 > v2);
                case TokenId.GreaterEqual:
                    return (v1 >= v2);
                case TokenId.Less:
                    return (v1 < v2);
                case TokenId.LessEqual:
                    return (v1 <= v2);
                case TokenId.NotEqual:
                    return (v1 != v2);
                default:
                    throw new ParseException("BinaryNotSupport '{0}' between type of '{1}' and '{2}'", op.ToString(), lValue.GetType().Name, rValue.GetType().Name);
            }
        }

        private bool EvalDateTimeOperator(TokenId op, DateTime lValue, DateTime rValue)
        {
            switch (op)
            {
                case TokenId.EqualEqual:
                    {
                        return lValue == rValue;
                    }
                case TokenId.Less:
                    {
                        return lValue < rValue;
                    }
                case TokenId.LessEqual:
                    {
                        return lValue <= rValue;
                    }
                case TokenId.Greater:
                    {
                        return lValue > rValue;
                    }
                case TokenId.GreaterEqual:
                    {
                        return lValue >= rValue;
                    }
                case TokenId.NotEqual:
                    {
                        return lValue != rValue;
                    }
                default:
                    {
                        throw new ParseException("BinaryNotSupport '{0}' between type of '{1}' and '{2}'", op.ToString(), lValue.GetType().Name, rValue.GetType().Name);
                    }
            }
        }

        private object EvalDecimalOperator(TokenId op, decimal v1, decimal v2)
        {
            switch (op)
            {
                case TokenId.Plus:
                case TokenId.PlusEqual:
                    return (v1 + v2);
                case TokenId.Minus:
                case TokenId.MinusEqual:
                    return (v1 - v2);
                case TokenId.Star:
                case TokenId.StarEqual:
                    return (v1 * v2);
                case TokenId.Slash:
                case TokenId.SlashEqual:
                    return (v1 / v2);
                case TokenId.Percent:
                case TokenId.PercentEqual:
                    return (v1 % v2);
                case TokenId.EqualEqual:
                    return (v1 == v2);
                case TokenId.Greater:
                    return (v1 > v2);
                case TokenId.GreaterEqual:
                    return (v1 >= v2);
                case TokenId.Less:
                    return (v1 < v2);
                case TokenId.LessEqual:
                    return (v1 <= v2);
                case TokenId.NotEqual:
                    return (v1 != v2);
                default:
                    throw new ParseException("BinaryNotSupport '{0}' between type of '{1}' and '{2}'", op.ToString(), v1.GetType().Name, v2.GetType().Name);
            }
        }

        private object EvalStringOperator(TokenId op, object lValue, object rValue)
        {

            switch (op)
            {
                case TokenId.Plus:
                    {
                        if (lValue == null)
                        {
                            return rValue;
                        }
                        else
                        {
                            return (lValue.ToString() + rValue);
                        }
                    }
                case TokenId.EqualEqual:
                    {
                        if (lValue == null)
                        {
                            if (rValue == null)
                                return true;
                            else
                                return false;
                        }
                        else
                        {
                            return lValue.ToString().Equals(rValue);
                        }
                    }
                case TokenId.NotEqual:
                    {
                        if (lValue == null)
                        {
                            if (rValue == null)
                                return false;
                            else
                                return true;
                        }
                        else
                        {
                            return !lValue.ToString().Equals(rValue);
                        }
                    }
                default:
                    throw new ParseException("BinaryNotSupport '{0}' between type of '{1}' and '{2}'", op.ToString(), lValue.GetType().Name, rValue.GetType().Name);
            }
        }

        private object EvalBooleanOperator(TokenId op, object lValue, object rValue)
        {
            bool v1 = Util.ToBool(lValue);
            bool v2 = Util.ToBool(rValue);

            switch (op)
            {
                case TokenId.Or: return v1 | v2;
                case TokenId.And: return v1 & v2;
                case TokenId.EqualEqual: return (v1 == v2);
                case TokenId.NotEqual: return (v1 != v2);
                default: throw new ParseException("BinaryNotSupport '{0}' between type of '{1}' and '{2}'", op.ToString(), lValue.GetType().Name, rValue.GetType().Name);
            }
        }

        static BinaryExpression()
        {
            _stringMap = new SortedList<TokenId, string>();
            _stringMap.Add(TokenId.Percent, @"%");
            _stringMap.Add(TokenId.Star, @"*");
            _stringMap.Add(TokenId.Plus, @"+");
            _stringMap.Add(TokenId.Minus, @"-");
            _stringMap.Add(TokenId.Slash, @"/");
            _stringMap.Add(TokenId.Less, @"<");
            _stringMap.Add(TokenId.Greater, @">");

            _stringMap.Add(TokenId.Equal, "=");
            _stringMap.Add(TokenId.PlusEqual, "+=");
            _stringMap.Add(TokenId.MinusEqual, "-=");
            _stringMap.Add(TokenId.StarEqual, "*=");
            _stringMap.Add(TokenId.SlashEqual, "/=");
            _stringMap.Add(TokenId.PercentEqual, "%=");

            _stringMap.Add(TokenId.And, @"&&");
            _stringMap.Add(TokenId.Or, @"||");
            _stringMap.Add(TokenId.EqualEqual, @"==");
            _stringMap.Add(TokenId.NotEqual, @"!=");
            _stringMap.Add(TokenId.LessEqual, @"<=");
            _stringMap.Add(TokenId.GreaterEqual, @">=");
        }
    }
}
