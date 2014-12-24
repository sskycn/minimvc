using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Cvv.WebUtility.Mvc;

namespace Cvv.WebUtility.Expression
{
    class HtmlStatement : StatementNode
    {
        private ExpressionNode _expression;

        public HtmlStatement(Token relatedToken)
            : base(relatedToken)
        {

        }

        public ExpressionNode Expression
        {
            get { return _expression; }
            set { _expression = value; }
        }

        public override object Execute(IContext context, Stream writer)
        {
            if (_expression != null)
            {
                object data = _expression.Execute(context, writer);

                if (data != null)
                {
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
