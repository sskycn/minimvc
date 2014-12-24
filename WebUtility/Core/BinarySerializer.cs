using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Cvv.WebUtility.Core
{
    public static class BinarySerializer
    {
        public static byte[] Serialize(object obj)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(memStream, obj);

                return memStream.ToArray();
            }
        }

        public static T Deserialize<T>(byte [] bytes)
        {
            using (MemoryStream memStream = new MemoryStream(bytes))
            {
                return (T)new BinaryFormatter().Deserialize(memStream);
            }
        }
    }
}