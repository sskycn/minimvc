using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    class UnaryExpression : ExpressionNode
    {
        private static SortedList<TokenId, string> _stringMap;
        private TokenId _op;
        protected ExpressionNode _child;

        public UnaryExpression(TokenId op, Token relatedToken)
            : base(relatedToken)
        {
            _op = op;
        }

        public UnaryExpression(TokenId op, ExpressionNode child, Token relatedToken)
            : base(relatedToken)
        {
            _op = op;
            _child = child;
        }


        public TokenId Op
        {
            get { return _op; }
        }


        public ExpressionNode Child
        {
            get { return _child; }
            set { _child = value; }
        }

        public override object Execute(IContext context, Stream writer)
        {
            object lValue = Child.Execute(context, writer);
            return Evaluate(_op, lValue);
        }

        private object Evaluate(TokenId op, object lValue)
        {
            object result = null;

            if (lValue == null)
            {
                result = false;
            }
            else if (Util.IsInteger(lValue))
            {
                result = EvalIntOperator(op, lValue);
            }
            else if (Util.IsNumber(lValue))
            {
                result = EvalDoubleOperator(op, lValue);
            }
            else if (lValue is bool)
            {
                result = EvalBooleanOperator(op, lValue);
            }
            else
            {
                throw new ParseException(string.Format("UnaryNotSupport '{0}' not exists at line: {1} column:{2}", RelatedToken.Data, RelatedToken.StartLocation.LineIndex + 1, RelatedToken.StartLocation.CharacterIndex + 1));
            }

            return result;
        }

        private object EvalBooleanOperator(TokenId op, object lValue)
        {
            bool v = Util.ToBool(lValue);

            switch (op)
            {
                case TokenId.Not: return !v;
            }

            throw new ParseException(string.Format("Unary not support, tokenId: '{0}', value: {1} at line: {2} column:{3}", RelatedToken.TokenId, lValue, RelatedToken.StartLocation.LineIndex + 1, RelatedToken.StartLocation.CharacterIndex + 1));
        }

        private object EvalDoubleOperator(TokenId op, object value)
        {
            if (value is decimal)
                return EvalDecimalOperator(op, (decimal)value);

            double v = Convert.ToDouble(value);

            switch (op)
            {
                case TokenId.Plus:
                    return v;
                case TokenId.Minus:
                    return -v;
                case TokenId.PlusPlus:
                    return ++v;
                case TokenId.MinusMinus:
                    return --v;
                default:
                    throw new ParseException(string.Format("Unary not support, tokenId: '{0}', value: {1} at line: {2} column:{3}", RelatedToken.TokenId, value, RelatedToken.StartLocation.LineIndex + 1, RelatedToken.StartLocation.CharacterIndex + 1));
            }
        }

        private object EvalDecimalOperator(TokenId op, decimal value)
        {
            switch (op)
            {
                case TokenId.Plus:
                    return value;
                case TokenId.Minus:
                    return -value;
                case TokenId.PlusPlus:
                    return ++value;
                case TokenId.MinusMinus:
                    return --value;
                default:
                    throw new ParseException(string.Format("Unary not support, tokenId: '{0}', value: {1} at line: {2} column:{3}", RelatedToken.TokenId, value, RelatedToken.StartLocation.LineIndex + 1, RelatedToken.StartLocation.CharacterIndex + 1));
            }
        }

        private object EvalIntOperator(TokenId op, object value)
        {
            long result = Convert.ToInt64(value);

            switch (op)
            {
                case TokenId.Plus:
                    break;
                case TokenId.Minus:
                    result = -result;
                    break;
                case TokenId.PlusPlus:
                    ++result;
                    break;
                case TokenId.MinusMinus:
                    --result;
                    break;
                default:
                    throw new ParseException(string.Format("Unary not support, tokenId: '{0}', value: {1} at line: {2} column:{3}", RelatedToken.TokenId, value, RelatedToken.StartLocation.LineIndex + 1, RelatedToken.StartLocation.CharacterIndex + 1));
            }

            if (result >= Int32.MinValue && result <= Int32.MaxValue)
                return (int)result;
            else
                return result;
        }

        static UnaryExpression()
        {
            _stringMap = new SortedList<TokenId, string>();
            _stringMap.Add(TokenId.Minus, @"-");
            _stringMap.Add(TokenId.Not, @"!");
            _stringMap.Add(TokenId.Plus, @"+");
            _stringMap.Add(TokenId.PlusPlus, @"++");
            _stringMap.Add(TokenId.MinusMinus, @"--");
        }
    }
}
