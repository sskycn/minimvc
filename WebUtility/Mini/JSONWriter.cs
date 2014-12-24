using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Cvv.WebUtility.Mini
{
    class JSONWriter
    {
        private JSONWriter() { }

        public static void WriteValue(StringBuilder sb, object obj)
        {
            if (obj == null)
            {
                sb.Append("null");
            }
            else if (obj is string)
            {
                WriteString(sb, (string)obj);
            }
            else if ((((((obj is sbyte) || (obj is byte)) || ((obj is short) || (obj is ushort))) || (((obj is int) || (obj is uint)) || ((obj is long) || (obj is ulong)))) || ((obj is decimal) || (obj is double))) || (obj is float))
            {
                sb.Append(Convert.ToString(obj, NumberFormatInfo.InvariantInfo));
            }
            else if (obj is bool)
            {
                sb.Append(obj.ToString().ToLower());
            }
            else if (((obj is char) || (obj is Enum)) || (obj is Guid))
            {
                WriteString(sb, "" + obj);
            }
            else if (obj is DateTime)
            {
                sb.Append("\"");
                WriteString(sb, ((DateTime)obj).ToString());
                sb.Append("\"");
            }
            else if (obj is IDictionary)
            {
                WriteDictionary(sb, (IDictionary)obj);
            }
            else if ((obj is Array) || (obj is IList) || (obj is ICollection))
            {
                WriteArray(sb, (IEnumerable)obj);
            }
            else
            {
                WriteObject(sb, obj);
            }
        }

        private static void WriteString(StringBuilder sb, string s)
        {
            sb.Append(s);
        }

        private static void WriteObject(StringBuilder sb, object obj)
        {
            bool flag = false;
            Type type = obj.GetType();

            sb.Append('{');
            foreach (FieldInfo fInfo in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (flag)
                {
                    sb.Append(",");
                }
                else
                {
                    flag = true;
                }
                WritePair(sb, fInfo.Name, fInfo.GetValue(obj));
            }

            foreach (PropertyInfo pInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (pInfo.CanRead)
                {
                    if (flag)
                    {
                        sb.Append(",");
                    }
                    else
                    {
                        flag = true;
                    }
                    WritePair(sb, pInfo.Name, pInfo.GetValue(obj, null));
                }
            }
            sb.Append('}');
        }

        private static void WriteArray(StringBuilder sb, IEnumerable array)
        {
            bool flag = false;
            sb.Append('[');
            foreach (object value in array)
            {
                if (flag)
                {
                    sb.Append(',');
                }
                else
                {
                    flag = true;
                }

                if (value is string)
                {
                    sb.Append('"');
                    WriteValue(sb, value);
                    sb.Append('"');
                }
                else
                {
                    WriteValue(sb, value);
                }
            }
            sb.Append(']');
        }

        private static void WriteDictionary(StringBuilder sb, IDictionary dic)
        {
            bool flag = false;
            sb.Append('{');
            foreach (DictionaryEntry entry in dic)
            {
                if (flag)
                {
                    sb.Append(",");
                }
                else
                {
                    flag = true;
                }
                WritePair(sb, entry.Key.ToString(), entry.Value);
            }
            sb.Append('}');
        }

        private static void WritePair(StringBuilder sb, string name, object value)
        {
            sb.Append('"');
            WriteString(sb, name);
            sb.Append("\":");

            if (value is string)
            {
                sb.Append('"');
                WriteValue(sb, value);
                sb.Append('"');
            }
            else
            {
                WriteValue(sb, value);
            }
        }
    }
}
