using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;
using Cvv.WebUtility.Mvc.Provider;

namespace Cvv.WebUtility.Core.Json
{
    internal class JSONSerializer : ISerializerProvider
    {
        public string Stringify(object obj)
        {
            StringBuilder output = new StringBuilder();
            WriteValue(obj, output);
            return output.ToString();
        }

        private void WriteArray(IEnumerable array, StringBuilder output)
        {
            output.Append("[");
            bool flag = false;
            foreach (object o in array)
            {
                if (flag)
                {
                    output.Append(',');
                }
                WriteValue(o, output);
                flag = true;
            }
            output.Append("]");
        }

        private void WriteDictionary(IDictionary dic, StringBuilder output)
        {
            output.Append("{");
            bool flag = false;
            foreach (DictionaryEntry entry in dic)
            {
                if (flag)
                {
                    output.Append(",");
                }
                WritePair(entry.Key.ToString(), entry.Value, output);
                flag = true;
            }
            output.Append("}");
        }

        private void WriteObject(object obj, StringBuilder output)
        {
            output.Append("{");
            bool flag = false;
            foreach (FieldInfo info in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (flag)
                {
                    output.Append(",");
                }
                WritePair(info.Name, info.GetValue(obj), output);
                flag = true;
            }
            foreach (PropertyInfo info in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (info.CanRead)
                {
                    if (flag)
                    {
                        output.Append(",");
                    }
                    WritePair(info.Name, info.GetValue(obj, null), output);
                    flag = true;
                }
            }
            output.Append("}");
        }

        private void WritePair(string name, object value, StringBuilder output)
        {
            WriteString(name, output);
            output.Append(":");
            WriteValue(value, output);
        }

        private void WriteString(string s, StringBuilder output)
        {
            output.Append('"');
            foreach (char ch in s)
            {
                switch (ch)
                {
                    case '\t':
                        output.Append(@"\t");
                        break;

                    case '\n':
                        output.Append(@"\n");
                        break;

                    case '\r':
                        output.Append(@"\r");
                        break;

                    case '"':
                    case '\\':
                        output.Append(@"\" + ch);
                        break;

                    default:
                        if ((ch >= ' ') && (ch < '\x0080'))
                        {
                            output.Append(ch);
                        }
                        else
                        {
                            output.Append(@"\u" + ((int)ch).ToString("X4"));
                        }
                        break;
                }
            }
            output.Append('"');
        }

        private void WriteValue(object obj, StringBuilder output)
        {
            if (obj == null)
            {
                output.Append("null");
            }
            else if ((((((obj is sbyte) || (obj is byte)) || ((obj is short) || (obj is ushort))) || (((obj is int) || (obj is uint)) || ((obj is long) || (obj is ulong)))) || ((obj is decimal) || (obj is double))) || (obj is float))
            {
                output.Append(Convert.ToString(obj, NumberFormatInfo.InvariantInfo));
            }
            else if (obj is bool)
            {
                output.Append(obj.ToString().ToLower());
            }
            else if (((obj is char) || (obj is Enum)) || (obj is Guid))
            {
                WriteString("" + obj, output);
            }
            else if (obj is DateTime)
            {
                TimeSpan span = (TimeSpan)(((DateTime)obj) - new DateTime(0x7b2, 1, 1));
                output.Append("new Date(" + span.TotalMilliseconds.ToString("0") + ")");
            }
            else if (obj is string)
            {
                WriteString((string)obj, output);
            }
            else if (obj is IDictionary)
            {
                WriteDictionary((IDictionary)obj, output);
            }
            else if (((obj is Array) || (obj is IList)) || (obj is ICollection))
            {
                WriteArray((IEnumerable)obj, output);
            }
            else
            {
                WriteObject(obj, output);
            }
        }
    }
}
