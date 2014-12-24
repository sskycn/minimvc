using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

namespace Cvv.WebUtility.Mini
{
    public class XMLWriter
    {
        private XMLWriter() { }

        public static void WriteDifinition(StreamWriter sw)
        {
            sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        }

        public static void WriteDifinition(StreamWriter sw, string encoding)
        {
            sw.WriteLine("<?xml version=\"1.0\" encoding=\"{0}\"?>", encoding);
        }

        public static string XMLEncode(string text)
        {
            Regex r = new Regex("&(?![a-zA-Z]{2,6};|#[0-9]{2,4};)");
            text = r.Replace(text, "&amp;");

            return text.Replace("<", "&lt;").Replace("\"", "&quot;").Replace(">", "&gt;");
        }

        public static string XMLDecode(string text)
        {
            return text.Replace("&lt;", "<").Replace("&quot;", "\"").Replace("&gt;", ">").Replace("&amp;", "&");
        }

        public static void WriteValue(StreamWriter sw, object obj)
        {
            int level = 0;
            WriteValue(sw, obj, ref level);
        }

        public static void WriteValue(StreamWriter sw, object obj, ref int level)
        {
            if (obj == null)
            {
                sw.Write("null");
            }
            else if (obj is string)
            {
                WriteString(sw, (string)obj, ref level);
            }
            else if ((((((obj is sbyte) || (obj is byte)) || ((obj is short) || (obj is ushort))) || (((obj is int) || (obj is uint)) || ((obj is long) || (obj is ulong)))) || ((obj is decimal) || (obj is double))) || (obj is float))
            {
                sw.Write(Convert.ToString(obj, NumberFormatInfo.InvariantInfo));
            }
            else if (obj is bool)
            {
                sw.Write(obj.ToString().ToLower());
            }
            else if (((obj is char) || (obj is Enum)) || (obj is Guid))
            {
                WriteString(sw, "" + obj, ref level);
            }
            else if (obj is DateTime)
            {
                WriteString(sw, ((DateTime)obj).ToString(), ref level);
            }
            else if (obj is IDictionary)
            {
                WriteDictionary(sw, (IDictionary)obj, ref level);
            }
            else if ((obj is Array) || (obj is IList) || (obj is ICollection))
            {
                WriteArray(sw, (IEnumerable)obj, ref level);
            }
            else
            {
                WriteObject(sw, obj, ref level);
            }
        }

        private static void WriteString(StreamWriter sw, string s, ref int level)
        {
            sw.Write(XMLEncode(s));
        }

        private static void WriteObject(StreamWriter sw, object obj, ref int level)
        {
            Type type = obj.GetType();

            string ident = CreateIdent(level++);

            sw.Write(ident);
            sw.Write("<{0}>\r\n", type.Name);

            foreach (FieldInfo fInfo in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                sw.Write(ident + "  ");
                WritePair(sw, fInfo.Name, fInfo.GetValue(obj), ref level);
            }

            foreach (PropertyInfo pInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (pInfo.CanRead)
                {
                    sw.Write(ident + "  ");
                    WritePair(sw, pInfo.Name, pInfo.GetValue(obj, null), ref level);
                }
            }

            sw.Write(ident);
            sw.Write("</{0}>\r\n", type.Name);
        }

        private static void WriteArray(StreamWriter sw, IEnumerable array, ref int level)
        {
            foreach (object value in array)
            {
                WriteValue(sw, value, ref level);
            }
        }

        private static void WriteDictionary(StreamWriter sw, IDictionary dic, ref int level)
        {
            foreach (DictionaryEntry entry in dic)
            {
                WritePair(sw, entry.Key.ToString(), entry.Value, ref level);
            }
        }

        private static void WritePair(StreamWriter sw, string name, object value, ref int level)
        {
            sw.Write("<{0} type=\"{1}\">", name, value.GetType().Name);
            WriteValue(sw, value, ref level);
            sw.Write("</{0}>\r\n", name);
        }

        private static string CreateIdent(int level)
        {
            string ident = string.Empty;

            for (int i = 0; i < level; i++)
            {
                ident += "  ";
            }

            return ident;
        }
    }
}
