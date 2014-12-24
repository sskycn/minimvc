using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;

namespace Cvv.WebUtility
{
    public static class StringHelper
    {
        public static string Base64StringEncode(string input)
        {
            byte[] encbuff = System.Text.Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(encbuff);
        }

        public static string Base64StringDecode(string input)
        {
            byte[] decbuff = Convert.FromBase64String(input);
            return System.Text.Encoding.UTF8.GetString(decbuff);
        }

        public static string CaseInsensitiveReplace(string input, string oldValue, string newValue)
        {
            Regex regEx = new Regex(oldValue, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return regEx.Replace(input, newValue);
        }

        public static string ReplaceFirst(string input, string oldValue, string newValue)
        {
            Regex regEx = new Regex(oldValue, RegexOptions.Multiline);
            return regEx.Replace(input, newValue, 1);
        }

        public static string ReplaceLast(string input, string oldValue, string newValue)
        {
            int index = input.LastIndexOf(oldValue);
            if (index < 0)
            {
                return input;
            }
            else
            {
                StringBuilder sb = new StringBuilder(input.Length - oldValue.Length + newValue.Length);
                sb.Append(input.Substring(0, index));
                sb.Append(newValue);
                sb.Append(input.Substring(index + oldValue.Length, input.Length - index - oldValue.Length));
                return sb.ToString();
            }
        }

        public static string FilterWords(string input, params string[] filterWords)
        {
            return StringHelper.FilterWords(input, char.MinValue, filterWords);
        }

        public static string FilterWords(string input, char mask, params string[] filterWords)
        {
            string stringMask = mask == char.MinValue ? string.Empty : mask.ToString();
            string totalMask = stringMask;

            foreach (string s in filterWords)
            {
                Regex regEx = new Regex(s, RegexOptions.IgnoreCase | RegexOptions.Multiline);

                if (stringMask.Length > 0)
                {
                    for (int i = 1; i < s.Length; i++)
                        totalMask += stringMask;
                }

                input = regEx.Replace(input, totalMask);

                totalMask = stringMask;
            }

            return input;
        }

        public static MatchCollection HasWords(string input, params string[] hasWords)
        {
            StringBuilder sb = new StringBuilder(hasWords.Length + 50);


            foreach (string s in hasWords)
            {
                sb.AppendFormat("({0})|", StringHelper.HtmlSpecialEntitiesEncode(s.Trim()));
            }

            string pattern = sb.ToString();
            pattern = pattern.TrimEnd('|');

            Regex regEx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return regEx.Matches(input);
        }

        public static string HtmlSpecialEntitiesEncode(string input)
        {
            return HttpUtility.HtmlEncode(input);
        }

        public static string HtmlSpecialEntitiesDecode(string input)
        {
            return HttpUtility.HtmlDecode(input);
        }

        public static string MD5String(string input)
        {
            MD5 md5Hasher = MD5.Create();

            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public static string MD5String(Stream input)
        {
            MD5 md5Hasher = MD5.Create();

            byte[] data = md5Hasher.ComputeHash(input);

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public static bool MD5VerifyString(string input, string hash)
        {
            string hashOfInput = StringHelper.MD5String(input);

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string Sha1String(string input)
        {
            SHA1 shaHasher = SHA1.Create();
            byte[] data = shaHasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static string Sha1String(Stream input)
        {
            SHA1 shaHasher = SHA1.Create();
            byte[] data = shaHasher.ComputeHash(input);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static string Sha256String(string input)
        {
            SHA256 shaHasher = SHA256.Create();
            byte[] data = shaHasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static string Sha256String(Stream input)
        {
            SHA256 shaHasher = SHA256.Create();
            byte[] data = shaHasher.ComputeHash(input);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static bool Sha256VerifyString(string input, string hash)
        {
            string hashOfInput = StringHelper.Sha256String(input);

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string Sha512String(string input)
        {
            SHA512 shaHasher = SHA512.Create();
            byte[] data = shaHasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static string Sha512String(Stream input)
        {
            SHA512 shaHasher = SHA512.Create();
            byte[] data = shaHasher.ComputeHash(input);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static bool Sha512VerifyString(string input, string hash)
        {
            string hashOfInput = StringHelper.Sha512String(input);

            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string PadLeftHtmlSpaces(string input, int totalSpaces)
        {
            string space = "&nbsp;";
            return PadLeft(input, space, totalSpaces * space.Length);
        }

        public static string PadLeft(string input, string pad, int totalWidth)
        {
            return StringHelper.PadLeft(input, pad, totalWidth, false);
        }

        public static string PadLeft(string input, string pad, int totalWidth, bool cutOff)
        {
            if (input.Length >= totalWidth)
                return input;

            int padCount = pad.Length;
            string paddedString = input;

            while (paddedString.Length < totalWidth)
            {
                paddedString += pad;
            }


            if (cutOff)
                paddedString = paddedString.Substring(0, totalWidth);

            return paddedString;
        }

        public static string PadRightHtmlSpaces(string input, int totalSpaces)
        {
            string space = "&nbsp;";
            return PadRight(input, space, totalSpaces * space.Length);
        }

        public static string PadRight(string input, string pad, int totalWidth)
        {
            return StringHelper.PadRight(input, pad, totalWidth, false);
        }

        public static string PadRight(string input, string pad, int totalWidth, bool cutOff)
        {
            if (input.Length >= totalWidth)
                return input;

            string paddedString = string.Empty;

            while (paddedString.Length < totalWidth - input.Length)
            {
                paddedString += pad;
            }


            if (cutOff)
                paddedString = paddedString.Substring(0, totalWidth - input.Length);

            paddedString += input;

            return paddedString;
        }

        public static string RemoveNewLines(string input)
        {
            return StringHelper.RemoveNewLines(input, false);
        }

        public static string RemoveNewLines(string input, bool addSpace)
        {
            string replace = string.Empty;
            if (addSpace)
                replace = " ";

            string pattern = @"[\r|\n]";
            Regex regEx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            return regEx.Replace(input, replace);
        }

        public static string Reverse(string input)
        {
            char[] reverse = new char[input.Length];
            for (int i = 0, k = input.Length - 1; i < input.Length; i++, k--)
            {
                if (char.IsSurrogate(input[k]))
                {
                    reverse[i + 1] = input[k--];
                    reverse[i++] = input[k];
                }
                else
                {
                    reverse[i] = input[k];
                }
            }
            return new System.String(reverse);
        }

        public static string SentenceCase(string input)
        {
            if (input.Length < 1)
                return input;

            string sentence = input.ToLower();
            return sentence[0].ToString().ToUpper() + sentence.Substring(1);
        }

        public static string SpaceToNbsp(string input)
        {
            string space = "&nbsp;";
            return input.Replace(" ", space);
        }

        public static string StripTags(string input)
        {
            Regex stripTags = new Regex("<(.|\n)+?>");
            return stripTags.Replace(input, "");
        }

        public static string TitleCase(string input)
        {
            return TitleCase(input, true);
        }

        public static string TitleCase(string input, bool ignoreShortWords)
        {
            List<string> ignoreWords = null;
            if (ignoreShortWords)
            {

                ignoreWords = new List<string>();
                ignoreWords.Add("a");
                ignoreWords.Add("is");
                ignoreWords.Add("was");
                ignoreWords.Add("the");
            }

            string[] tokens = input.Split(' ');
            StringBuilder sb = new StringBuilder(input.Length);
            foreach (string s in tokens)
            {
                if (ignoreShortWords == true
                    && s != tokens[0]
                    && ignoreWords.Contains(s.ToLower()))
                {
                    sb.Append(s + " ");
                }
                else
                {
                    sb.Append(s[0].ToString().ToUpper());
                    sb.Append(s.Substring(1).ToLower());
                    sb.Append(" ");
                }
            }

            return sb.ToString().Trim();
        }

        public static string TrimIntraWords(string input)
        {
            Regex regEx = new Regex(@"[\s]+");
            return regEx.Replace(input, " ");
        }

        public static string TrimSpace(string input)
        {
            Regex regEx = new Regex(@"[\s]+");
            return regEx.Replace(input, "");
        }

        public static string NewLineToBreak(string input)
        {
            Regex regEx = new Regex(@"[\n|\r]+");
            return regEx.Replace(input, "<br />");
        }

        public static string WordWrap(string input, int charCount)
        {
            return StringHelper.WordWrap(input, charCount, false, Environment.NewLine);
        }

        public static string WordWrap(string input, int charCount, bool cutOff)
        {
            return StringHelper.WordWrap(input, charCount, cutOff, Environment.NewLine);
        }

        public static string WordWrap(string input, int charCount, bool cutOff, string breakText)
        {
            StringBuilder sb = new StringBuilder(input.Length + 100);
            int counter = 0;

            if (cutOff)
            {
                while (counter < input.Length)
                {
                    if (input.Length > counter + charCount)
                    {
                        sb.Append(input.Substring(counter, charCount));
                        sb.Append(breakText);
                    }
                    else
                    {
                        sb.Append(input.Substring(counter));
                    }
                    counter += charCount;
                }
            }
            else
            {
                string[] strings = input.Split(' ');
                for (int i = 0; i < strings.Length; i++)
                {
                    counter += strings[i].Length + 1;
                    if (i != 0 && counter > charCount)
                    {
                        sb.Append(breakText);
                        counter = 0;
                    }

                    sb.Append(strings[i] + ' ');
                }
            }
            return sb.ToString().TrimEnd();
        }
    }
}
