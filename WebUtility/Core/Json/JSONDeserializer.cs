using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Cvv.WebUtility.Mvc.Provider;

namespace Cvv.WebUtility.Core.Json
{
    class JSONDeserializer : IDeserializerProvider
    {
        public object Parse(string stringValue, Type targetType)
        {
            if (targetType.Name.EndsWith("&"))
                throw new JSONException("Not support this type: " + targetType.Name);

            return ParseValue(stringValue, targetType);
        }

        private static Dictionary<Type, TokenId> _baseType = new Dictionary<Type, TokenId>();

        static JSONDeserializer()
        {
            _baseType.Add(typeof(Boolean), TokenId.Boolean);
            _baseType.Add(typeof(Byte), TokenId.Byte);
            _baseType.Add(typeof(SByte), TokenId.SByte);
            _baseType.Add(typeof(Char), TokenId.Char);
            _baseType.Add(typeof(Decimal), TokenId.Decimal);
            _baseType.Add(typeof(Double), TokenId.Double);
            _baseType.Add(typeof(Single), TokenId.Single);
            _baseType.Add(typeof(Int32), TokenId.Int32);
            _baseType.Add(typeof(UInt32), TokenId.UInt32);
            _baseType.Add(typeof(Int64), TokenId.Int64);
            _baseType.Add(typeof(UInt64), TokenId.UInt64);
            _baseType.Add(typeof(Object), TokenId.Object);
            _baseType.Add(typeof(Int16), TokenId.Int16);
            _baseType.Add(typeof(UInt16), TokenId.UInt16);
            _baseType.Add(typeof(String), TokenId.String);
            _baseType.Add(typeof(DateTime), TokenId.DateTime);
            _baseType.Add(typeof(Guid), TokenId.Guid);

            _baseType.Add(typeof(Boolean[]), TokenId.ArrayBoolean);
            _baseType.Add(typeof(Byte[]), TokenId.ArrayByte);
            _baseType.Add(typeof(SByte[]), TokenId.ArraySByte);
            _baseType.Add(typeof(Char[]), TokenId.ArrayChar);
            _baseType.Add(typeof(Decimal[]), TokenId.ArrayDecimal);
            _baseType.Add(typeof(Double[]), TokenId.ArrayDouble);
            _baseType.Add(typeof(Single[]), TokenId.ArraySingle);
            _baseType.Add(typeof(Int32[]), TokenId.ArrayInt32);
            _baseType.Add(typeof(UInt32[]), TokenId.ArrayUInt32);
            _baseType.Add(typeof(Int64[]), TokenId.ArrayInt64);
            _baseType.Add(typeof(UInt64[]), TokenId.ArrayUInt64);
            _baseType.Add(typeof(Object[]), TokenId.ArrayObject);
            _baseType.Add(typeof(Int16[]), TokenId.ArrayInt16);
            _baseType.Add(typeof(UInt16[]), TokenId.ArrayUInt16);
            _baseType.Add(typeof(String[]), TokenId.ArrayString);
            _baseType.Add(typeof(DateTime[]), TokenId.ArrayDateTime);
            _baseType.Add(typeof(Guid[]), TokenId.ArrayGuid);

            _baseType.Add(typeof(IList<Boolean>), TokenId.ArrayBoolean);
            _baseType.Add(typeof(IList<Byte>), TokenId.ArrayByte);
            _baseType.Add(typeof(IList<SByte>), TokenId.ArraySByte);
            _baseType.Add(typeof(IList<Char>), TokenId.ArrayChar);
            _baseType.Add(typeof(IList<Decimal>), TokenId.ArrayDecimal);
            _baseType.Add(typeof(IList<Double>), TokenId.ArrayDouble);
            _baseType.Add(typeof(IList<Single>), TokenId.ArraySingle);
            _baseType.Add(typeof(IList<Int32>), TokenId.ArrayInt32);
            _baseType.Add(typeof(IList<UInt32>), TokenId.ArrayUInt32);
            _baseType.Add(typeof(IList<Int64>), TokenId.ArrayInt64);
            _baseType.Add(typeof(IList<UInt64>), TokenId.ArrayUInt64);
            _baseType.Add(typeof(IList<Object>), TokenId.ArrayObject);
            _baseType.Add(typeof(IList<Int16>), TokenId.ArrayInt16);
            _baseType.Add(typeof(IList<UInt16>), TokenId.ArrayUInt16);
            _baseType.Add(typeof(IList<String>), TokenId.ArrayString);
            _baseType.Add(typeof(IList<DateTime>), TokenId.ArrayDateTime);
            _baseType.Add(typeof(IList<Guid>), TokenId.ArrayGuid);
        }

        public static object ParseValue(string stringValue, Type targetType)
        {
            if (stringValue == null)
                stringValue = string.Empty;

            TokenId tokenId;

            if (_baseType.TryGetValue(targetType, out tokenId))
            {
                switch (tokenId)
                {
                    case TokenId.Boolean:
                        return CreateBoolean(stringValue);
                    case TokenId.Byte:
                        return CreateByte(stringValue);
                    case TokenId.SByte:
                        return CreateSByte(stringValue);
                    case TokenId.Char:
                        return CreateChar(stringValue);
                    case TokenId.Decimal:
                        return CreateDecimal(stringValue);
                    case TokenId.Double:
                        return CreateDouble(stringValue);
                    case TokenId.Single:
                        return CreateSingle(stringValue);
                    case TokenId.Int32:
                        return CreateInt32(stringValue);
                    case TokenId.UInt32:
                        return CreateUInt32(stringValue);
                    case TokenId.Int64:
                        return CreateInt64(stringValue);
                    case TokenId.UInt64:
                        return CreateUInt64(stringValue);
                    case TokenId.Object:
                        return CreateObject(stringValue);
                    case TokenId.Int16:
                        return CreateInt16(stringValue);
                    case TokenId.UInt16:
                        return CreateUInt16(stringValue);
                    case TokenId.String:
                        return CreateString(stringValue);
                    case TokenId.DateTime:
                        return CreateDateTime(stringValue);
                    case TokenId.Guid:
                        return CreateGuid(stringValue);
                    case TokenId.ArrayBoolean:
                        return CreateArrayBoolean(stringValue);
                    case TokenId.ArrayByte:
                        return CreateArrayByte(stringValue);
                    case TokenId.ArraySByte:
                        return CreateArraySByte(stringValue);
                    case TokenId.ArrayChar:
                        return CreateArrayChar(stringValue);
                    case TokenId.ArrayDecimal:
                        return CreateArrayDecimal(stringValue);
                    case TokenId.ArrayDouble:
                        return CreateArrayDouble(stringValue);
                    case TokenId.ArraySingle:
                        return CreateArraySingle(stringValue);
                    case TokenId.ArrayInt32:
                        return CreateArrayInt32(stringValue);
                    case TokenId.ArrayUInt32:
                        return CreateArrayUInt32(stringValue);
                    case TokenId.ArrayInt64:
                        return CreateArrayInt64(stringValue);
                    case TokenId.ArrayUInt64:
                        return CreateArrayUInt64(stringValue);
                    case TokenId.ArrayObject:
                        return CreateArrayObject(stringValue);
                    case TokenId.ArrayInt16:
                        return CreateArrayInt16(stringValue);
                    case TokenId.ArrayUInt16:
                        return CreateArrayUInt16(stringValue);
                    case TokenId.ArrayString:
                        return CreateArrayString(stringValue);
                    case TokenId.ArrayDateTime:
                        return CreateArrayDateTime(stringValue);
                    case TokenId.ArrayGuid:
                        return CreateArrayGuid(stringValue);
                    default:
                        return CreateObject(stringValue);
                }
            }
            else if (targetType.IsInterface)
            {
                if (targetType.IsGenericType)
                {
                    int index = 0;
                    char[] json = stringValue.ToCharArray();

                    if (json.Length > 0)
                    {
                        Advance(json, ref index, Token.LBracket);
                    }

                    return ParseList(json, ref index, targetType);
                }

                throw new JSONException("MiniJSON not supported this interface '" + targetType.Name + "'");
            }
            else if (typeof(Array).IsAssignableFrom(targetType))
            {
                int index = 0;
                char[] json = stringValue.ToCharArray();

                if (json.Length > 0)
                {
                    Advance(json, ref index, Token.LBracket);
                }

                return ParseArray(json, ref index, targetType);
            }
            else
            {
                int index = 0;

                char[] json = stringValue.ToCharArray();

                if (json.Length > 0)
                {
                    Advance(json, ref index, Token.LCurly);
                }

                return ParseObject(json, ref index, targetType);
            }
        }

        private static object ParseValue(char[] json, ref int index, Type targetType)
        {
            switch (Advance(json, ref index))
            {
                case Token.Number:
                    {
                        string value = ParseNumber(json, ref index);
                        return ParseValue(value, targetType);
                    }
                case Token.String:
                    {
                        string value = ParseString(json, ref index);
                        return ParseValue(value, targetType);
                    }
                case Token.LCurly:
                    return ParseObject(json, ref index, targetType);
                case Token.LBracket:
                    return ParseList(json, ref index, targetType);
                case Token.True:
                    return true;
                case Token.False:
                    return false;
                case Token.Null:
                    return null;
            }

            throw new JSONException("Unrecognized token at index " + index);
        }

        private static object ParseObject(char[] json, ref int index, Type targetType)
        {
            ConstructorInfo[] constructors = targetType.GetConstructors();

            if (constructors.Length != 2)
            {
                throw new JSONException("Model must be 2 constructors, first with zero parameters, another with full parameters as properties");
            }

            if (json.Length == 0)
            {
                return Activator.CreateInstance(targetType);
            }

            ConstructorInfo ctr = constructors[1];

            ParameterInfo[] parameters = ctr.GetParameters();

            Dictionary<string, Type> argsType = new Dictionary<string, Type>(StringComparer.InvariantCulture);

            foreach (ParameterInfo parameter in parameters)
            {
                argsType.Add(parameter.Name, parameter.ParameterType);
            }

            Dictionary<string, object> table = new Dictionary<string, object>();

            while (true)
            {
                switch (Advance(json, ref index))
                {
                    case Token.Comma:
                        break;
                    case Token.RCurly:
                        {
                            object[] args = new object[parameters.Length];

                            for (int i = 0; i < args.Length; i++)
                            {
                                object val;
                                string propertyName = parameters[i].Name;

                                if (table.TryGetValue(propertyName, out val))
                                    args[i] = val;
                            }

                            return Activator.CreateInstance(targetType, args);
                        }
                    default:
                        {
                            string name = CamelCase(ParseString(json, ref index));
                            Advance(json, ref index, Token.Colon);

                            Type outType;

                            if (argsType.TryGetValue(name, out outType))
                            {
                                object value = ParseValue(json, ref index, outType);
                                table[name] = value;
                            }
                            else
                            {
                                object value = ParseValue(json, ref index, typeof(object));
                                table[name] = value;
                            }
                        }
                        break;
                }
            }

        }

        private static Array ParseArray(char[] json, ref int index, Type targetType)
        {
            Type elementType = targetType.GetElementType();

            ArrayList array = new ArrayList();

            if (json.Length == 0)
            {
                return array.ToArray(elementType);
            }

            while (true)
            {
                switch (Advance(json, index))
                {
                    case Token.Comma:
                        Advance(json, ref index);
                        break;
                    case Token.RBracket:
                        {
                            Advance(json, ref index);
                            return array.ToArray(elementType);
                        }
                    default:
                        array.Add(ParseValue(json, ref index, elementType));
                        break;
                }
            }
        }

        private static IList ParseList(char[] json, ref int index, Type targetType)
        {
            Type[] typeArguments = targetType.GetGenericArguments();

            if (typeArguments.Length != 1 || !typeof(IList<>).MakeGenericType(typeArguments).IsAssignableFrom(targetType))
            {
                throw new JSONException("MiniJSON not implemented this interface '" + targetType.Name + "'");
            }

            IList array = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(typeArguments));

            if (json.Length == 0)
            {
                return array;
            }

            while (true)
            {
                switch (Advance(json, index))
                {
                    case Token.Comma:
                        Advance(json, ref index);
                        break;
                    case Token.RBracket:
                        Advance(json, ref index);
                        return array;
                    default:
                        array.Add(ParseValue(json, ref index, typeArguments[0]));
                        break;
                }
            }
        }

        private static Boolean CreateBoolean(string stringValue)
        {
            Boolean val;

            Boolean.TryParse(stringValue, out val);

            return val;
        }

        private static Byte CreateByte(string stringValue)
        {
            Byte val;

            if (stringValue.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
            {
                Byte.TryParse(stringValue.Substring(2), NumberStyles.HexNumber, null, out val);
            }
            else
            {
                Byte.TryParse(stringValue, out val);
            }

            return val;
        }

        private static SByte CreateSByte(string stringValue)
        {
            SByte val;

            if (stringValue.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
            {
                SByte.TryParse(stringValue.Substring(2), NumberStyles.HexNumber, null, out val);
            }
            else
            {
                SByte.TryParse(stringValue, out val);
            }

            return val;
        }

        private static Char CreateChar(string stringValue)
        {
            Char val;

            Char.TryParse(stringValue, out val);

            return val;
        }

        private static Decimal CreateDecimal(string stringValue)
        {
            Decimal val;

            Decimal.TryParse(stringValue, out val);

            return val;
        }

        private static Double CreateDouble(string stringValue)
        {
            Double val;

            Double.TryParse(stringValue, out val);

            return val;
        }

        private static Single CreateSingle(string stringValue)
        {
            Single val;

            Single.TryParse(stringValue, out val);

            return val;
        }

        private static Int32 CreateInt32(string stringValue)
        {
            Int32 val;

            if (stringValue.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
            {
                Int32.TryParse(stringValue.Substring(2), NumberStyles.HexNumber, null, out val);
            }
            else
            {
                Int32.TryParse(stringValue, out val);
            }

            return val;
        }

        private static UInt32 CreateUInt32(string stringValue)
        {
            UInt32 val;

            if (stringValue.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
            {
                UInt32.TryParse(stringValue.Substring(2), NumberStyles.HexNumber, null, out val);
            }
            else
            {
                UInt32.TryParse(stringValue, out val);
            }

            return val;
        }

        private static Int64 CreateInt64(string stringValue)
        {
            Int64 val;

            if (stringValue.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
            {
                Int64.TryParse(stringValue.Substring(2), NumberStyles.HexNumber, null, out val);
            }
            else
            {
                Int64.TryParse(stringValue, out val);
            }

            return val;
        }

        private static UInt64 CreateUInt64(string stringValue)
        {
            UInt64 val;

            if (stringValue.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
            {
                UInt64.TryParse(stringValue.Substring(2), NumberStyles.HexNumber, null, out val);
            }
            else
            {
                UInt64.TryParse(stringValue, out val);
            }

            return val;
        }

        private static Object CreateObject(string stringValue)
        {
            return stringValue;
        }

        private static Int16 CreateInt16(string stringValue)
        {
            Int16 val;

            Int16.TryParse(stringValue, out val);

            return val;
        }

        private static UInt16 CreateUInt16(string stringValue)
        {
            UInt16 val;

            UInt16.TryParse(stringValue, out val);

            return val;
        }

        private static String CreateString(string stringValue)
        {
            return stringValue;
        }

        private static DateTime CreateDateTime(string stringValue)
        {
            DateTime val;

            DateTime.TryParse(stringValue, out val);

            return val;
        }

        private static Guid CreateGuid(string stringValue)
        {
            Guid val;

            try
            {
                val = new Guid(stringValue);
            }
            catch (Exception)
            {
                val = Guid.Empty;
            }

            return val;
        }

        private static Boolean[] CreateArrayBoolean(string stringValue)
        {
            Boolean[] rval;

            if (stringValue.TrimStart().StartsWith("["))
            {
                int index = 0;
                rval = ParseArrayBoolean(stringValue.ToCharArray(), ref index);
            }
            else
            {
                string[] array = StringToArray(stringValue);

                rval = new Boolean[array.Length];

                for (int i = 0; i < array.Length; i++)
                {
                    rval[i] = CreateBoolean(array[i]);
                }
            }

            return rval;
        }

        private static Byte[] CreateArrayByte(string stringValue)
        {
            Byte[] rval;

            if (stringValue.TrimStart().StartsWith("["))
            {
                int index = 0;
                rval = ParseArrayByte(stringValue.ToCharArray(), ref index);
            }
            else
            {
                string[] array = StringToArray(stringValue);

                rval = new Byte[array.Length];

                for (int i = 0; i < array.Length; i++)
                {
                    rval[i] = CreateByte(array[i]);
                }
            }

            return rval;
        }

        private static SByte[] CreateArraySByte(string stringValue)
        {
            SByte[] rval;

            if (stringValue.TrimStart().StartsWith("["))
            {
                int index = 0;
                rval = ParseArraySByte(stringValue.ToCharArray(), ref index);
            }
            else
            {
                string[] array = StringToArray(stringValue);

                rval = new SByte[array.Length];

                for (int i = 0; i < array.Length; i++)
                {
                    rval[i] = CreateSByte(array[i]);
                }
            }

            return rval;
        }

        private static Char[] CreateArrayChar(string stringValue)
        {
            Char[] rval;

            if (stringValue.TrimStart().StartsWith("\""))
            {
                int index = 0;
                string s = ParseString(stringValue.ToCharArray(), ref index);
                rval = s.ToCharArray();
            }
            else
            {
                rval = stringValue.ToCharArray();
            }

            return rval;
        }

        private static Decimal[] CreateArrayDecimal(string stringValue)
        {
            Decimal[] rval;

            if (stringValue.TrimStart().StartsWith("["))
            {
                int index = 0;
                rval = ParseArrayDecimal(stringValue.ToCharArray(), ref index);
            }
            else
            {
                string[] array = StringToArray(stringValue);

                rval = new Decimal[array.Length];

                for (int i = 0; i < array.Length; i++)
                {
                    rval[i] = CreateDecimal(array[i]);
                }
            }

            return rval;
        }

        private static Double[] CreateArrayDouble(string stringValue)
        {
            Double[] rval;

            if (stringValue.TrimStart().StartsWith("["))
            {
                int index = 0;
                rval = ParseArrayDouble(stringValue.ToCharArray(), ref index);
            }
            else
            {
                string[] array = StringToArray(stringValue);

                rval = new Double[array.Length];

                for (int i = 0; i < array.Length; i++)
                {
                    rval[i] = CreateDouble(array[i]);
                }
            }

            return rval;
        }

        private static Single[] CreateArraySingle(string stringValue)
        {
            Single[] rval;

            if (stringValue.TrimStart().StartsWith("["))
            {
                int index = 0;
                rval = ParseArraySingle(stringValue.ToCharArray(), ref index);
            }
            else
            {
                string[] array = StringToArray(stringValue);

                rval = new Single[array.Length];

                for (int i = 0; i < array.Length; i++)
                {
                    rval[i] = CreateSingle(array[i]);
                }
            }

            return rval;
        }

        private static Int32[] CreateArrayInt32(string stringValue)
        {
            Int32[] rval;

            if (stringValue.TrimStart().StartsWith("["))
            {
                int index = 0;
                rval = ParseArrayInt32(stringValue.ToCharArray(), ref index);
            }
            else
            {
                string[] array = StringToArray(stringValue);

                rval = new Int32[array.Length];

                for (int i = 0; i < array.Length; i++)
                {
                    rval[i] = CreateInt32(array[i]);
                }
            }

            return rval;
        }

        private static UInt32[] CreateArrayUInt32(string stringValue)
        {
            UInt32[] rval;

            if (stringValue.TrimStart().StartsWith("["))
            {
                int index = 0;
                rval = ParseArrayUInt32(stringValue.ToCharArray(), ref index);
            }
            else
            {
                string[] array = StringToArray(stringValue);

                rval = new UInt32[array.Length];

                for (int i = 0; i < array.Length; i++)
                {
                    rval[i] = CreateUInt32(array[i]);
                }
            }

            return rval;
        }

        private static Int64[] CreateArrayInt64(string stringValue)
        {
            Int64[] rval;

            if (stringValue.TrimStart().StartsWith("["))
            {
                int index = 0;
                rval = ParseArrayInt64(stringValue.ToCharArray(), ref index);
            }
            else
            {
                string[] array = StringToArray(stringValue);

                rval = new Int64[array.Length];

                for (int i = 0; i < array.Length; i++)
                {
                    rval[i] = CreateInt64(array[i]);
                }
            }

            return rval;
        }

        private static UInt64[] CreateArrayUInt64(string stringValue)
        {
            UInt64[] rval;

            if (stringValue.TrimStart().StartsWith("["))
            {
                int index = 0;
                rval = ParseArrayUInt64(stringValue.ToCharArray(), ref index);
            }
            else
            {
                string[] array = StringToArray(stringValue);

                rval = new UInt64[array.Length];

                for (int i = 0; i < array.Length; i++)
                {
                    rval[i] = CreateUInt64(array[i]);
                }
            }

            return rval;
        }

        private static Object[] CreateArrayObject(string stringValue)
        {
            throw new NotImplementedException();
        }

        private static Int16[] CreateArrayInt16(string stringValue)
        {
            Int16[] rval;

            if (stringValue.TrimStart().StartsWith("["))
            {
                int index = 0;
                rval = ParseArrayInt16(stringValue.ToCharArray(), ref index);
            }
            else
            {
                string[] array = StringToArray(stringValue);

                rval = new Int16[array.Length];

                for (int i = 0; i < array.Length; i++)
                {
                    rval[i] = CreateInt16(array[i]);
                }
            }

            return rval;
        }

        private static UInt16[] CreateArrayUInt16(string stringValue)
        {
            UInt16[] rval;

            if (stringValue.TrimStart().StartsWith("["))
            {
                int index = 0;
                rval = ParseArrayUInt16(stringValue.ToCharArray(), ref index);
            }
            else
            {
                string[] array = StringToArray(stringValue);

                rval = new UInt16[array.Length];

                for (int i = 0; i < array.Length; i++)
                {
                    rval[i] = CreateUInt16(array[i]);
                }
            }

            return rval;
        }

        private static String[] CreateArrayString(string stringValue)
        {
            string[] rval;

            if (stringValue.TrimStart().StartsWith("["))
            {
                int index = 0;
                rval = ParseArrayString(stringValue.ToCharArray(), ref index);
            }
            else
            {
                rval = StringToArray(stringValue);
            }

            return rval;
        }

        private static DateTime[] CreateArrayDateTime(string stringValue)
        {
            throw new NotImplementedException();
        }

        private static Guid[] CreateArrayGuid(string stringValue)
        {
            throw new NotImplementedException();
        }

        private static string[] StringToArray(string stringValue)
        {
            if (string.IsNullOrEmpty(stringValue))
            {
                return new string[0];
            }
            else
            {
                return stringValue.Split(',');
            }
        }

        private static Boolean[] ParseArrayBoolean(char[] json, ref int index)
        {
            List<Boolean> array = new List<Boolean>();

            Advance(json, ref index, Token.LBracket);

            while (true)
            {
                switch (Advance(json, index))
                {
                    case Token.Comma:
                        Advance(json, ref index);
                        break;

                    case Token.RBracket:
                        Advance(json, ref index);
                        return array.ToArray();

                    default:
                        array.Add(ParseBoolean(json, ref index));
                        break;
                }
            }
        }

        private static Boolean ParseBoolean(char[] json, ref int index)
        {
            Token token = Advance(json, ref index);

            Boolean rval;

            if (token == Token.True)
            {
                rval = true;
            }
            else if (token == Token.False)
            {
                rval = false;
            }
            else if (token == Token.String)
            {
                string stringValue = ParseString(json, ref index);
                rval = CreateBoolean(stringValue);
            }
            else
            {
                rval = false;
            }

            return rval;
        }

        private static Byte[] ParseArrayByte(char[] json, ref int index)
        {
            List<Byte> array = new List<Byte>();

            Advance(json, ref index, Token.LBracket);

            while (true)
            {
                switch (Advance(json, index))
                {
                    case Token.Comma:
                        Advance(json, ref index);
                        break;

                    case Token.RBracket:
                        Advance(json, ref index);
                        return array.ToArray();

                    default:
                        array.Add(ParseByte(json, ref index));
                        break;
                }
            }
        }

        private static Byte ParseByte(char[] json, ref int index)
        {
            Token token = Advance(json, ref index);

            Byte rval;
            
            if (token == Token.String)
            {
                string stringValue = ParseString(json, ref index);
                rval = CreateByte(stringValue);
            }
            else
            {
                string stringValue = ParseNumber(json, ref index);
                rval = CreateByte(stringValue);
            }

            return rval;
        }

        private static SByte[] ParseArraySByte(char[] json, ref int index)
        {
            List<SByte> array = new List<SByte>();

            Advance(json, ref index, Token.LBracket);

            while (true)
            {
                switch (Advance(json, index))
                {
                    case Token.Comma:
                        Advance(json, ref index);
                        break;

                    case Token.RBracket:
                        Advance(json, ref index);
                        return array.ToArray();

                    default:
                        array.Add(ParseSByte(json, ref index));
                        break;
                }
            }
        }

        private static SByte ParseSByte(char[] json, ref int index)
        {
            Token token = Advance(json, ref index);

            SByte rval;

            if (token == Token.String)
            {
                string stringValue = ParseString(json, ref index);
                rval = CreateSByte(stringValue);
            }
            else
            {
                string stringValue = ParseNumber(json, ref index);
                rval = CreateSByte(stringValue);
            }

            return rval;
        }

        private static Decimal[] ParseArrayDecimal(char[] json, ref int index)
        {
            List<Decimal> array = new List<Decimal>();

            Advance(json, ref index, Token.LBracket);

            while (true)
            {
                switch (Advance(json, index))
                {
                    case Token.Comma:
                        Advance(json, ref index);
                        break;

                    case Token.RBracket:
                        Advance(json, ref index);
                        return array.ToArray();

                    default:
                        array.Add(ParseDecimal(json, ref index));
                        break;
                }
            }
        }

        private static Decimal ParseDecimal(char[] json, ref int index)
        {
            Token token = Advance(json, ref index);

            Decimal rval;

            if (token == Token.String)
            {
                string stringValue = ParseString(json, ref index);
                rval = CreateDecimal(stringValue);
            }
            else
            {
                string stringValue = ParseNumber(json, ref index);
                rval = CreateDecimal(stringValue);
            }

            return rval;
        }

        private static Double[] ParseArrayDouble(char[] json, ref int index)
        {
            List<Double> array = new List<Double>();

            Advance(json, ref index, Token.LBracket);

            while (true)
            {
                switch (Advance(json, index))
                {
                    case Token.Comma:
                        Advance(json, ref index);
                        break;

                    case Token.RBracket:
                        Advance(json, ref index);
                        return array.ToArray();

                    default:
                        array.Add(ParseDouble(json, ref index));
                        break;
                }
            }
        }

        private static Double ParseDouble(char[] json, ref int index)
        {
            Token token = Advance(json, ref index);

            Double rval;

            if (token == Token.String)
            {
                string stringValue = ParseString(json, ref index);
                rval = CreateDouble(stringValue);
            }
            else
            {
                string stringValue = ParseNumber(json, ref index);
                rval = CreateDouble(stringValue);
            }

            return rval;
        }

        private static Single[] ParseArraySingle(char[] json, ref int index)
        {
            List<Single> array = new List<Single>();

            Advance(json, ref index, Token.LBracket);

            while (true)
            {
                switch (Advance(json, index))
                {
                    case Token.Comma:
                        Advance(json, ref index);
                        break;

                    case Token.RBracket:
                        Advance(json, ref index);
                        return array.ToArray();

                    default:
                        array.Add(ParseSingle(json, ref index));
                        break;
                }
            }
        }

        private static Single ParseSingle(char[] json, ref int index)
        {
            Token token = Advance(json, ref index);

            Single rval;

            if (token == Token.String)
            {
                string stringValue = ParseString(json, ref index);
                rval = CreateSingle(stringValue);
            }
            else
            {
                string stringValue = ParseNumber(json, ref index);
                rval = CreateSingle(stringValue);
            }

            return rval;
        }

        private static Int32[] ParseArrayInt32(char[] json, ref int index)
        {
            List<Int32> array = new List<Int32>();

            Advance(json, ref index, Token.LBracket);

            while (true)
            {
                switch (Advance(json, index))
                {
                    case Token.Comma:
                        Advance(json, ref index);
                        break;

                    case Token.RBracket:
                        Advance(json, ref index);
                        return array.ToArray();

                    default:
                        array.Add(ParseInt32(json, ref index));
                        break;
                }
            }
        }

        private static Int32 ParseInt32(char[] json, ref int index)
        {
            Token token = Advance(json, ref index);

            Int32 rval;

            if (token == Token.String)
            {
                string stringValue = ParseString(json, ref index);
                rval = CreateInt32(stringValue);
            }
            else
            {
                string stringValue = ParseNumber(json, ref index);
                rval = CreateInt32(stringValue);
            }

            return rval;
        }
        private static UInt32[] ParseArrayUInt32(char[] json, ref int index)
        {
            List<UInt32> array = new List<UInt32>();

            Advance(json, ref index, Token.LBracket);

            while (true)
            {
                switch (Advance(json, index))
                {
                    case Token.Comma:
                        Advance(json, ref index);
                        break;

                    case Token.RBracket:
                        Advance(json, ref index);
                        return array.ToArray();

                    default:
                        array.Add(ParseUInt32(json, ref index));
                        break;
                }
            }
        }

        private static UInt32 ParseUInt32(char[] json, ref int index)
        {
            Token token = Advance(json, ref index);

            UInt32 rval;

            if (token == Token.String)
            {
                string stringValue = ParseString(json, ref index);
                rval = CreateUInt32(stringValue);
            }
            else
            {
                string stringValue = ParseNumber(json, ref index);
                rval = CreateUInt32(stringValue);
            }

            return rval;
        }
        private static Int64[] ParseArrayInt64(char[] json, ref int index)
        {
            List<Int64> array = new List<Int64>();

            Advance(json, ref index, Token.LBracket);

            while (true)
            {
                switch (Advance(json, index))
                {
                    case Token.Comma:
                        Advance(json, ref index);
                        break;

                    case Token.RBracket:
                        Advance(json, ref index);
                        return array.ToArray();

                    default:
                        array.Add(ParseInt64(json, ref index));
                        break;
                }
            }
        }

        private static Int64 ParseInt64(char[] json, ref int index)
        {
            Token token = Advance(json, ref index);

            Int64 rval;

            if (token == Token.String)
            {
                string stringValue = ParseString(json, ref index);
                rval = CreateInt64(stringValue);
            }
            else
            {
                string stringValue = ParseNumber(json, ref index);
                rval = CreateInt64(stringValue);
            }

            return rval;
        }
        private static UInt64[] ParseArrayUInt64(char[] json, ref int index)
        {
            List<UInt64> array = new List<UInt64>();

            Advance(json, ref index, Token.LBracket);

            while (true)
            {
                switch (Advance(json, index))
                {
                    case Token.Comma:
                        Advance(json, ref index);
                        break;

                    case Token.RBracket:
                        Advance(json, ref index);
                        return array.ToArray();

                    default:
                        array.Add(ParseUInt64(json, ref index));
                        break;
                }
            }
        }

        private static UInt64 ParseUInt64(char[] json, ref int index)
        {
            Token token = Advance(json, ref index);

            UInt64 rval;

            if (token == Token.String)
            {
                string stringValue = ParseString(json, ref index);
                rval = CreateUInt64(stringValue);
            }
            else
            {
                string stringValue = ParseNumber(json, ref index);
                rval = CreateUInt64(stringValue);
            }

            return rval;
        }
        private static Int16[] ParseArrayInt16(char[] json, ref int index)
        {
            List<Int16> array = new List<Int16>();

            Advance(json, ref index, Token.LBracket);

            while (true)
            {
                switch (Advance(json, index))
                {
                    case Token.Comma:
                        Advance(json, ref index);
                        break;

                    case Token.RBracket:
                        Advance(json, ref index);
                        return array.ToArray();

                    default:
                        array.Add(ParseInt16(json, ref index));
                        break;
                }
            }
        }

        private static Int16 ParseInt16(char[] json, ref int index)
        {
            Token token = Advance(json, ref index);

            Int16 rval;

            if (token == Token.String)
            {
                string stringValue = ParseString(json, ref index);
                rval = CreateInt16(stringValue);
            }
            else
            {
                string stringValue = ParseNumber(json, ref index);
                rval = CreateInt16(stringValue);
            }

            return rval;
        }
        private static UInt16[] ParseArrayUInt16(char[] json, ref int index)
        {
            List<UInt16> array = new List<UInt16>();

            Advance(json, ref index, Token.LBracket);

            while (true)
            {
                switch (Advance(json, index))
                {
                    case Token.Comma:
                        Advance(json, ref index);
                        break;

                    case Token.RBracket:
                        Advance(json, ref index);
                        return array.ToArray();

                    default:
                        array.Add(ParseUInt16(json, ref index));
                        break;
                }
            }
        }

        private static UInt16 ParseUInt16(char[] json, ref int index)
        {
            Token token = Advance(json, ref index);

            UInt16 rval;

            if (token == Token.String)
            {
                string stringValue = ParseString(json, ref index);
                rval = CreateUInt16(stringValue);
            }
            else
            {
                string stringValue = ParseNumber(json, ref index);
                rval = CreateUInt16(stringValue);
            }

            return rval;
        }

        private static string[] ParseArrayString(char[] json, ref int index)
        {
            List<string> array = new List<string>();

            Advance(json, ref index, Token.LBracket);

            while (true)
            {
                switch (Advance(json, ref index))
                {
                    case Token.Comma:
                        break;
                    case Token.RBracket:
                        return array.ToArray();
                    default:
                        array.Add(ParseString(json, ref index));
                        break;
                }
            }
        }

        private static string ParseString(char[] json, ref int index)
        {
            StringBuilder s = new StringBuilder();

            int runIndex = -1;

            while (index < json.Length)
            {
                char c = json[index++];

                if (c == '"')
                {
                    if (runIndex != -1)
                    {
                        if (s.Length == 0)
                            return new string(json, runIndex, index - runIndex - 1);

                        s.Append(json, runIndex, index - runIndex - 1);
                    }

                    return s.ToString();
                }

                if (c != '\\')
                {
                    if (runIndex == -1)
                        runIndex = index - 1;

                    continue;
                }

                if (index == json.Length) break;

                if (runIndex != -1)
                {
                    s.Append(json, runIndex, index - runIndex - 1);
                    runIndex = -1;
                }

                switch (json[index++])
                {
                    case '"':
                        s.Append('"');
                        break;

                    case '\\':
                        s.Append('\\');
                        break;

                    case '/':
                        s.Append('/');
                        break;

                    case 'b':
                        s.Append('\b');
                        break;

                    case 'f':
                        s.Append('\f');
                        break;

                    case 'n':
                        s.Append('\n');
                        break;

                    case 'r':
                        s.Append('\r');
                        break;

                    case 't':
                        s.Append('\t');
                        break;

                    case 'u':
                        {
                            int remainingLength = json.Length - index;
                            if (remainingLength < 4) break;

                            uint codePoint = ParseUnicode(json[index], json[index + 1], json[index + 2], json[index + 3]);
                            s.Append((char)codePoint);

                            index += 4;
                        }
                        break;
                }
            }

            throw new JSONException("Unexpectedly reached end of string");
        }

        private static string ParseNumber(char[] json, ref int index)
        {
            var startIndex = index - 1;

            do
            {
                if (index == json.Length)
                    break;

                char c = json[index];

                if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F') || c == '.' || c == '-' || c == '+' || c == 'e' || c == 'E' || c == 'x' || c == 'X')
                {
                    if (++index == json.Length)
                        break;

                    continue;
                }

                break;
            } while (true);

            return new string(json, startIndex, index - startIndex);
        }

        private static uint ParseSingleChar(char c1, uint multipliyer)
        {
            uint p1 = 0;
            if (c1 >= '0' && c1 <= '9')
                p1 = (uint)(c1 - '0') * multipliyer;
            else if (c1 >= 'A' && c1 <= 'F')
                p1 = (uint)((c1 - 'A') + 10) * multipliyer;
            else if (c1 >= 'a' && c1 <= 'f')
                p1 = (uint)((c1 - 'a') + 10) * multipliyer;
            return p1;
        }

        private static uint ParseUnicode(char c1, char c2, char c3, char c4)
        {
            uint p1 = ParseSingleChar(c1, 0x1000);
            uint p2 = ParseSingleChar(c2, 0x100);
            uint p3 = ParseSingleChar(c3, 0x10);
            uint p4 = ParseSingleChar(c4, 1);

            return p1 + p2 + p3 + p4;
        }

        private static void Advance(char[] json, ref int index, Token expecToken)
        {
            Token curtok = Advance(json, ref index);

            if (curtok != expecToken)
                throw new JSONException(string.Format("Expect token: {0}, but got token: {1}", expecToken, curtok));
        }

        private static Token Advance(char[] json, int index)
        {
            return Advance(json, ref index);
        }

        private static Token Advance(char[] json, ref int index)
        {
            char c;

            while (index < json.Length)
            {
                c = json[index];

                if (c > ' ') break;
                if (c != ' ' && c != '\t' && c != '\n' && c != '\r') break;

                index++;
            }

            if (index == json.Length)
            {
                return Token.Eof;
            }

            c = json[index];

            index++;

            switch (c)
            {
                case '{':
                    return Token.LCurly;

                case '}':
                    return Token.RCurly;

                case '[':
                    return Token.LBracket;

                case ']':
                    return Token.RBracket;

                case ',':
                    return Token.Comma;
                case '"':
                    return Token.String;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                case '+':
                case '.':
                    return Token.Number;
                case ':':
                    return Token.Colon;

                case 'f':
                    if (json.Length - index >= 4 &&
                        json[index + 0] == 'a' &&
                        json[index + 1] == 'l' &&
                        json[index + 2] == 's' &&
                        json[index + 3] == 'e')
                    {
                        index += 4;
                        return Token.False;
                    }
                    break;
                case 't':
                    if (json.Length - index >= 3 &&
                        json[index + 0] == 'r' &&
                        json[index + 1] == 'u' &&
                        json[index + 2] == 'e')
                    {
                        index += 3;
                        return Token.True;
                    }
                    break;
                case 'n':
                    if (json.Length - index >= 3 &&
                        json[index + 0] == 'u' &&
                        json[index + 1] == 'l' &&
                        json[index + 2] == 'l')
                    {
                        index += 3;
                        return Token.Null;
                    }
                    break;
            }

            throw new JSONException("Could not find token at index " + --index);
        }

        public static string CamelCase(string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            if (name.Length < 2)
                return name.ToLower();

            return char.ToLower(name[0]) + name.Substring(1);
        }

        public static string PascalCase(string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            if (name.Length < 2)
                return name.ToUpper();

            return char.ToUpper(name[0]) + name.Substring(1);
        }

        enum Token
        {
            Eof = -1,
            LCurly,
            RCurly,
            LBracket,
            RBracket,
            Colon,
            Comma,
            String,
            Number,
            True,
            False,
            Null,
        }

        enum TokenId
        {
            Boolean,
            Byte,
            SByte,
            Char,
            Decimal,
            Double,
            Single,
            Int32,
            UInt32,
            Int64,
            UInt64,
            Object,
            Int16,
            UInt16,
            String,
            DateTime,
            Guid,
            ArrayBoolean,
            ArrayByte,
            ArraySByte,
            ArrayChar,
            ArrayDecimal,
            ArrayDouble,
            ArraySingle,
            ArrayInt32,
            ArrayUInt32,
            ArrayInt64,
            ArrayUInt64,
            ArrayObject,
            ArrayInt16,
            ArrayUInt16,
            ArrayString,
            ArrayDateTime,
            ArrayGuid,
        }
    }
}