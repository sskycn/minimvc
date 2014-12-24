using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Cvv.WebUtility.Core
{
    public static class Base64Serializer
    {
        public static string Serialize(object obj)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(memStream, obj);

                return Convert.ToBase64String(memStream.ToArray());
            }
        }

        public static T Deserialize<T>(string s)
        {
            using (MemoryStream memStream = new MemoryStream(Convert.FromBase64String(s)))
            {
                return (T)new BinaryFormatter().Deserialize(memStream);
            }
        }
    }
}