using System;
using System.Reflection;

namespace Cvv.WebUtility.Core
{
    public static class AttributeHelper
    {
        public static bool HasAttribute<T>(MemberInfo type, bool inherit) where T : Attribute
        {
            return type.IsDefined(typeof(T), inherit);
        }

        public static T GetAttribute<T>(MemberInfo type, bool inherit) where T : Attribute
        {
            T[] attributes = (T[])type.GetCustomAttributes(typeof(T), inherit);

            return attributes.Length > 0 ? attributes[0] : null;
        }

        public static T[] GetAttributes<T>(MemberInfo type, bool inherit) where T : Attribute
        {
            return (T[])type.GetCustomAttributes(typeof(T), inherit);
        }
    }
}