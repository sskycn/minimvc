using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;
using Cvv.WebUtility.Mvc;
using System.IO;

namespace Cvv.WebUtility.Expression
{
    class MemberAccessExpression : PrimaryExpression
    {
        private ExpressionNode _right;
        private ExpressionNode _left;
        private TokenId _qualifierKind;
        private bool _isMember;

        public MemberAccessExpression(ExpressionNode left, ExpressionNode right, TokenId qualifierKind, bool isMember)
            : base(left.RelatedToken)
        {
            _left = left;
            _right = right;
            _isMember = isMember;
        }

        public ExpressionNode Right
        {
            get { return _right; }
        }

        public ExpressionNode Left
        {
            get { return _left; }
        }

        public TokenId QualifierKind
        {
            get { return _qualifierKind; }
            set { _qualifierKind = value; }
        }

        public override object Execute(IContext context, Stream writer)
        {
            object lValue = _left.Execute(context, writer);
            object result = null;
            IdentifierExpression ident = _right as IdentifierExpression;

            if (lValue == null)
            {
                throw new ParseException(string.Format("variable '{0}' not exists at line: {1} column:{2}", RelatedToken.Data, RelatedToken.StartLocation.LineIndex + 1, RelatedToken.StartLocation.CharacterIndex + 1));
            }
            else if (lValue is IDictionary)
            {
                IDictionary dic = lValue as IDictionary;
                result = dic[ident.Name];
            }
            else
            {
                Type type = lValue.GetType();

                if (_isMember)
                {
                    result = new Function(ident.Name, lValue, type);
                }
                else
                {
                    PropertyInfo pInfo = type.GetProperty(ident.Name, BindingFlags.Public | BindingFlags.Instance);

                    if (pInfo == null)
                    {
                        throw new ParseException(string.Format("variable '{0}' do not have property '{1}' at line: {2} column:{3}", RelatedToken.Data, ident.Name, RelatedToken.StartLocation.LineIndex + 1, RelatedToken.StartLocation.CharacterIndex + 1));
                    }
                    else
                    {
                        result = pInfo.GetValue(lValue, null);
                    }
                }
            }

            return result;
        }
    }
}
