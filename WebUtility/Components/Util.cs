using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Cvv.WebUtility
{
    class Util
    {
        public static Type[] FindCompatibleTypes(Assembly assembly, Type baseType)
        {
            List<Type> types = new List<Type>();

            foreach (Type type in assembly.GetTypes())
            {
                if (type != baseType && baseType.IsAssignableFrom(type))
                {
                    types.Add(type);
                }
            }

            return types.ToArray();
        }


        public static T ConvertString<T>(string stringValue)
        {
            object o = ConvertString(stringValue, typeof(T));

            if (o == null)
                return default(T);
            else
                return (T)o;
        }

        public static object ConvertString(string stringValue, Type targetType)
        {
            if (stringValue == null)
                return null;

            if (targetType == typeof(string))
                return stringValue;

            object value = stringValue;

            if (IsNullable(targetType))
            {
                if (stringValue.Trim().Length == 0)
                    return null;

                targetType = GetRealType(targetType);
            }

            if (targetType != typeof(string))
            {
                if (targetType == typeof(double) || targetType == typeof(float))
                {
                    double doubleValue;

                    if (!double.TryParse(stringValue.Replace(',', '.'), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out doubleValue))
                        value = null;
                    else
                        value = doubleValue;
                }
                else if (targetType == typeof(decimal))
                {
                    decimal decimalValue;

                    if (!decimal.TryParse(stringValue.Replace(',', '.'), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out decimalValue))
                        value = null;
                    else
                        value = decimalValue;
                }
                else if (targetType == typeof(Int32) || targetType == typeof(Int16) || targetType == typeof(Int64) || targetType == typeof(SByte) || targetType.IsEnum)
                {
                    long longValue;

                    if (!long.TryParse(stringValue, out longValue))
                        value = null;
                    else
                        value = longValue;
                }
                else if (targetType == typeof(UInt32) || targetType == typeof(UInt16) || targetType == typeof(UInt64) || targetType == typeof(Byte))
                {
                    ulong longValue;

                    if (!ulong.TryParse(stringValue, out longValue))
                        value = null;
                    else
                        value = longValue;
                }
                else if (targetType == typeof(DateTime))
                {
                    DateTime dateTime;

                    if (!DateTime.TryParseExact(stringValue, new string[] { "yyyyMMdd", "yyyy-MM-dd", "yyyy.MM.dd", "yyyy/MM/dd" }, null, DateTimeStyles.NoCurrentDateDefault, out dateTime))
                        value = null;
                    else
                        value = dateTime;
                }
                else if (targetType == typeof(bool))
                {
                    value = (stringValue == "1" || stringValue.ToUpper() == "Y" || stringValue.ToUpper() == "YES" || stringValue.ToUpper() == "T" || stringValue.ToUpper() == "TRUE");
                }
                else
                {
                    value = null;
                }
            }

            if (value == null)
                return null;

            if (targetType.IsValueType)
            {
                if (!targetType.IsGenericType)
                {
                    if (targetType.IsEnum)
                        return Enum.ToObject(targetType, value);
                    else
                        return Convert.ChangeType(value, targetType);
                }

                if (targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    Type sourceType = value.GetType();

                    Type underlyingType = targetType.GetGenericArguments()[0];

                    if (sourceType == underlyingType)
                        return value;

                    if (underlyingType.IsEnum)
                    {
                        return Enum.ToObject(underlyingType, value);
                    }
                    else
                    {
                        return Convert.ChangeType(value, underlyingType);
                    }
                }
            }

            return value;
        }

        public static object ConvertType(object value, Type targetType)
        {
            if (value == null)
                return null;

            if (value.GetType() == targetType)
                return value;

            if (targetType.IsValueType)
            {
                if (!targetType.IsGenericType)
                {
                    if (targetType.IsEnum)
                        return Enum.ToObject(targetType, value);
                    else
                        return Convert.ChangeType(value, targetType);
                }

                if (targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    Type underlyingType = targetType.GetGenericArguments()[0];

                    return ConvertType(value, underlyingType);
                }
            }

            if (targetType.IsAssignableFrom(value.GetType()))
                return value;
            else
                return Convert.ChangeType(value, targetType);
        }

        public static bool IsNullable(Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static Type GetRealType(Type type)
        {
            if (IsNullable(type))
                return type.GetGenericArguments()[0];

            return type;
        }

        public static bool ToBool(object value)
        {
            if (value == null)
            {
                return false;
            }
            else if (value is bool)
            {
                return (bool)value;
            }
            else if (IsInteger(value))
            {
                return (Convert.ToInt64(value) != 0);
            }
            else if (value is string)
            {
                return ((string)value).Trim().Length > 0;
            }
            else if (value is decimal)
            {
                return ((decimal)value) != 0m;
            }
            else if (IsDouble(value))
            {
                return (Convert.ToDouble(value) != 0.0);
            }
            else if (value is DateTime)
            {
                return ((DateTime)value) != DateTime.MinValue;
            }
            else if (value is ICollection)
            {
                return ((ICollection)value).Count > 0;
            }
            else
            {
                return true;
            }
        }

        public static double ToDouble(object obj)
        {
            if (obj is double)
                return (double)obj;
            else
                return Convert.ToDouble(obj);
        }

        public static bool IsInteger(object value)
        {
            return (value is Int32 || value is Int16 || value is Int64 || value is UInt16 || value is UInt32 || value is Byte || value is SByte);
        }

        public static bool IsDouble(object value)
        {
            return (value is double || value is float);
        }

        public static bool IsNumber(object value)
        {
            return (value is double || value is float || value is decimal || IsInteger(value));
        }

        public static bool IsString(object value)
        {
            return (value is string || value is char) ;
        }
    }
}
