using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Cvv.WebUtility.Mvc;
using System.Collections;

namespace Cvv.WebUtility.Expression
{
    class EchoStatement : StatementNode
    {
        private ExpressionList _expressions;

        public EchoStatement(Token relatedToken)
            : base(relatedToken)
        {

        }

        public ExpressionList Expressions
        {
            get { return _expressions; }
            set { _expressions = value; }
        }

        public override object Execute(IContext context, Stream writer)
        {
            if (_expressions != null)
            {
                object data = _expressions.Execute(context, writer);

                string val;

                if (data is Array)
                {
                    val = WriteArray((Array)data);
                }
                else
                {
                    val = data.ToString();
                }

                byte[] bytes = WebAppConfig.TemplateFileEncoding.GetBytes(val);
                writer.Write(bytes, 0, bytes.Length);
            }

            return null;
        }

        private static string WriteArray(Array array)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < array.Length; i++)
            {
                if (i > 0)
                    sb.Append(',');

                sb.Append(array.GetValue(i));
            }

            return sb.ToString();
        }
    }
}
