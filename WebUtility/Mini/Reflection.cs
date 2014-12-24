using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using System.Collections;

namespace Cvv.WebUtility.Mini
{
    internal struct Getters
    {
        public string Name;
        public Reflection.GenericGetter Getter;
        public Type PropertyType;
    }

    internal sealed class Reflection
    {
        public readonly static Reflection Instance = new Reflection();

        private Reflection()
        {
        }

        public bool ShowReadOnlyProperties = false;
        internal delegate object GenericSetter(object target, object value);
        internal delegate object GenericGetter(object obj);
        private delegate object CreateObject();

        private SafeDictionary<Type, string> _typeNames = new SafeDictionary<Type, string>();
        private SafeDictionary<string, Type> _typeCaches = new SafeDictionary<string, Type>();
        private SafeDictionary<Type, CreateObject> _constrCaches = new SafeDictionary<Type, CreateObject>();
        private SafeDictionary<Type, List<Getters>> _gettersCaches = new SafeDictionary<Type, List<Getters>>();

        internal string GetTypeAssemblyName(Type type)
        {
            string val = "";
            if (_typeNames.TryGetValue(type, out val))
            {
                return val;
            }
            else
            {
                string s = type.AssemblyQualifiedName;
                _typeNames.Add(type, s);
                return s;
            }
        }

        internal Type GetTypeFromCache(string typename)
        {
            Type val = null;
            if (_typeCaches.TryGetValue(typename, out val))
                return val;
            else
            {
                Type t = Type.GetType(typename);
                _typeCaches.Add(typename, t);
                return t;
            }
        }

        internal object FastCreateInstance(Type objType)
        {
            try
            {
                CreateObject createObject = null;

                if (_constrCaches.TryGetValue(objType, out createObject))
                {
                    return createObject();
                }
                else
                {
                    if (objType.IsClass)
                    {
                        DynamicMethod dynMethod = new DynamicMethod("_", objType, null);
                        ILGenerator ilGen = dynMethod.GetILGenerator();
                        ilGen.Emit(OpCodes.Newobj, objType.GetConstructor(Type.EmptyTypes));
                        ilGen.Emit(OpCodes.Ret);
                        createObject = (CreateObject)dynMethod.CreateDelegate(typeof(CreateObject));
                        _constrCaches.Add(objType, createObject);
                    }
                    else // structs
                    {
                        DynamicMethod dynMethod = new DynamicMethod("_", typeof(object), null);
                        ILGenerator ilGen = dynMethod.GetILGenerator();
                        var lv = ilGen.DeclareLocal(objType);
                        ilGen.Emit(OpCodes.Ldloca_S, lv);
                        ilGen.Emit(OpCodes.Initobj, objType);
                        ilGen.Emit(OpCodes.Ldloc_0);
                        ilGen.Emit(OpCodes.Box, objType);
                        ilGen.Emit(OpCodes.Ret);
                        createObject = (CreateObject)dynMethod.CreateDelegate(typeof(CreateObject));
                        _constrCaches.Add(objType, createObject);
                    }
                    return createObject();
                }
            }
            catch (Exception exc)
            {
                throw new Exception(string.Format("Failed to fast create instance for type '{0}' from assemebly '{1}'",
                    objType.FullName, objType.AssemblyQualifiedName), exc);
            }
        }

        internal static GenericSetter CreateSetField(Type type, FieldInfo fieldInfo)
        {
            Type[] arguments = new Type[2];
            arguments[0] = arguments[1] = typeof(object);

            DynamicMethod dynamicSet = new DynamicMethod("_", typeof(object), arguments, type);

            ILGenerator il = dynamicSet.GetILGenerator();

            if (!type.IsClass) // structs
            {
                var lv = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloca_S, lv);
                il.Emit(OpCodes.Ldarg_1);
                if (fieldInfo.FieldType.IsClass)
                    il.Emit(OpCodes.Castclass, fieldInfo.FieldType);
                else
                    il.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
                il.Emit(OpCodes.Stfld, fieldInfo);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Box, type);
                il.Emit(OpCodes.Ret);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                if (fieldInfo.FieldType.IsValueType)
                    il.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
                il.Emit(OpCodes.Stfld, fieldInfo);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ret);
            }
            return (GenericSetter)dynamicSet.CreateDelegate(typeof(GenericSetter));
        }

        internal static GenericSetter CreateSetMethod(Type type, PropertyInfo propertyInfo)
        {
            MethodInfo setMethod = propertyInfo.GetSetMethod();
            if (setMethod == null)
                return null;

            Type[] arguments = new Type[2];
            arguments[0] = arguments[1] = typeof(object);

            DynamicMethod setter = new DynamicMethod("_", typeof(object), arguments);
            ILGenerator il = setter.GetILGenerator();

            if (!type.IsClass) // structs
            {
                var lv = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloca_S, lv);
                il.Emit(OpCodes.Ldarg_1);
                if (propertyInfo.PropertyType.IsClass)
                    il.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
                else
                    il.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                il.EmitCall(OpCodes.Call, setMethod, null);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Box, type);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
                il.Emit(OpCodes.Ldarg_1);
                if (propertyInfo.PropertyType.IsClass)
                    il.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
                else
                    il.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                il.EmitCall(OpCodes.Callvirt, setMethod, null);
                il.Emit(OpCodes.Ldarg_0);
            }

            il.Emit(OpCodes.Ret);

            return (GenericSetter)setter.CreateDelegate(typeof(GenericSetter));
        }

        internal static GenericGetter CreateGetField(Type type, FieldInfo fieldInfo)
        {
            DynamicMethod dynamicGet = new DynamicMethod("_", typeof(object), new Type[] { typeof(object) }, type);

            ILGenerator il = dynamicGet.GetILGenerator();

            if (!type.IsClass) // structs
            {
                var lv = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloca_S, lv);
                il.Emit(OpCodes.Ldfld, fieldInfo);
                if (fieldInfo.FieldType.IsValueType)
                    il.Emit(OpCodes.Box, fieldInfo.FieldType);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, fieldInfo);
                if (fieldInfo.FieldType.IsValueType)
                    il.Emit(OpCodes.Box, fieldInfo.FieldType);
            }

            il.Emit(OpCodes.Ret);

            return (GenericGetter)dynamicGet.CreateDelegate(typeof(GenericGetter));
        }

        internal static GenericGetter CreateGetMethod(Type type, PropertyInfo propertyInfo)
        {
            MethodInfo getMethod = propertyInfo.GetGetMethod();
            if (getMethod == null)
                return null;

            DynamicMethod getter = new DynamicMethod("_", typeof(object), new Type[] { typeof(object) }, type);

            ILGenerator il = getter.GetILGenerator();

            if (!type.IsClass) // structs
            {
                var lv = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloca_S, lv);
                il.EmitCall(OpCodes.Call, getMethod, null);
                if (propertyInfo.PropertyType.IsValueType)
                    il.Emit(OpCodes.Box, propertyInfo.PropertyType);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
                il.EmitCall(OpCodes.Callvirt, getMethod, null);
                if (propertyInfo.PropertyType.IsValueType)
                    il.Emit(OpCodes.Box, propertyInfo.PropertyType);
            }

            il.Emit(OpCodes.Ret);

            return (GenericGetter)getter.CreateDelegate(typeof(GenericGetter));
        }

        internal List<Getters> GetGetters(Type type)
        {
            List<Getters> val = null;
            if (_gettersCaches.TryGetValue(type, out val))
                return val;

            PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<Getters> getters = new List<Getters>();
            foreach (PropertyInfo p in props)
            {
                if (!p.CanWrite && ShowReadOnlyProperties == false) continue;

                object[] att = p.GetCustomAttributes(typeof(System.Xml.Serialization.XmlIgnoreAttribute), false);
                if (att != null && att.Length > 0)
                    continue;

                GenericGetter g = CreateGetMethod(type, p);
                if (g != null)
                {
                    Getters gg = new Getters();
                    gg.Name = p.Name;
                    gg.Getter = g;
                    gg.PropertyType = p.PropertyType;
                    getters.Add(gg);
                }
            }

            FieldInfo[] fi = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var f in fi)
            {
                object[] att = f.GetCustomAttributes(typeof(System.Xml.Serialization.XmlIgnoreAttribute), false);
                if (att != null && att.Length > 0)
                    continue;

                GenericGetter g = CreateGetField(type, f);
                if (g != null)
                {
                    Getters gg = new Getters();
                    gg.Name = f.Name;
                    gg.Getter = g;
                    gg.PropertyType = f.FieldType;
                    getters.Add(gg);
                }
            }

            _gettersCaches.Add(type, getters);
            return getters;
        }
    }
}
