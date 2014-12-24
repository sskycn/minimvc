using System;
using System.Text;

namespace Cvv.WebUtility.Core
{
    public static class BinaryHelper
    {
        public static string ToHex(byte[] bytes, bool upperCase)
        {
            StringBuilder result = new StringBuilder(bytes.Length*2);

            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));

            return result.ToString();
        }

        public static string ToHex(byte[] bytes)
        {
            return ToHex(bytes, false);
        }

        public static byte[] XorWith(byte[] bytes, byte[] xor)
        {
            byte[] result = new byte[bytes.Length];

            for (int i = 0; i < bytes.Length; i++)
                if (i < xor.Length)
                    result[i] = (byte) (bytes[i] ^ xor[i]);
                else
                    result[i] = bytes[i];

            return result;
        }
    }
}