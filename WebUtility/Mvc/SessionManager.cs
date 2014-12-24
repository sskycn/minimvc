using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Cvv.WebUtility.Mvc
{
    public static class SessionManager
    {
        private static SessionProperty CreateSessionProperty(FieldInfo field, string key, object defaultValue, bool createNew)
        {
            ConstructorInfo constructor;
            Type genericType = field.FieldType.GetGenericArguments()[0];

            if (createNew)
            {
                constructor = field.FieldType.GetConstructor(new Type[] { typeof(string) });

                SessionProperty value = (SessionProperty)constructor.Invoke(new object[] { key });

                value.CreateNew = createNew;

                return value;
            }
            else if (defaultValue != null)
            {
                constructor = field.FieldType.GetConstructor(new Type[] { typeof(string), genericType });

                return (SessionProperty)constructor.Invoke(new object[] { key, defaultValue });
            }
            else
            {
                constructor = field.FieldType.GetConstructor(new Type[] { typeof(string) });

                return (SessionProperty)constructor.Invoke(new string[] { key });
            }
        }


        public static void ClearSessionProperties(Type type)
        {
            for (; type != typeof(object); type = type.BaseType)
            {
                FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);

                foreach (FieldInfo field in fields)
                {
                    Type fieldType = field.FieldType;

                    if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(SessionProperty<>))
                    {
                        string key = null;

                        if (field.IsDefined(typeof(SessionProperty.KeyAttribute), false))
                        {
                            SessionProperty.KeyAttribute attr = ((SessionProperty.KeyAttribute[])field.GetCustomAttributes(typeof(SessionProperty.KeyAttribute), false))[0];

                            key = attr.Key;
                        }

                        if (key == null)
                        {
                            key = BuildSessionKey(field);
                        }

                        HttpContextBase.Current.Session.Remove(key);
                    }
                }
            }
        }

        public static void CreateSessionProperties(Type type)
        {
            CreateSessionProperties(null, type);
        }

        public static void CreateSessionProperties(object obj)
        {
            CreateSessionProperties(obj, obj.GetType());
        }

        private static void CreateSessionProperties(object obj, Type type)
        {
            for (; type != typeof(object); type = type.BaseType)
            {
                FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

                foreach (FieldInfo field in fields)
                {
                    Type fieldType = field.FieldType;

                    if (obj == null && !field.IsStatic)
                        continue;

                    if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(SessionProperty<>))
                    {
                        if (field.GetValue(field.IsStatic ? null : obj) == null)
                        {
                            string key = null;
                            object defaultValue = null;
                            bool createNew = false;

                            if (field.IsDefined(typeof(SessionProperty.KeyAttribute), false))
                            {
                                SessionProperty.KeyAttribute attr = ((SessionProperty.KeyAttribute[])field.GetCustomAttributes(typeof(SessionProperty.KeyAttribute), false))[0];

                                key = attr.Key;
                            }

                            if (field.IsDefined(typeof(SessionProperty.DefaultValueAttribute), false))
                            {
                                SessionProperty.DefaultValueAttribute attr = ((SessionProperty.DefaultValueAttribute[])field.GetCustomAttributes(typeof(SessionProperty.DefaultValueAttribute), false))[0];

                                defaultValue = attr.Value;
                            }

                            if (field.IsDefined(typeof(SessionProperty.AutoCreateNewAttribute), false))
                            {
                                createNew = true;
                            }

                            if (key == null)
                            {
                                key = BuildSessionKey(field);
                            }

                            field.SetValue(field.IsStatic ? null : obj, CreateSessionProperty(field, key, defaultValue, createNew));
                        }
                    }
                }
            }
        }

        private static string BuildSessionKey(FieldInfo field)
        {
            return field.DeclaringType.FullName + "." + field.Name;
        }
    }
}
