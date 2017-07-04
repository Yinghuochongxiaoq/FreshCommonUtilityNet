using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

// ReSharper disable once CheckNamespace
namespace FreshCommonUtility.DeepCopy
{
    /// <summary>
    /// Deep copy helper
    /// </summary>
    public static class DeepCopyHelper
    {
        #region [1、Deep Copy use emit]
        /// <summary>
        /// Cache struct
        /// </summary>
        struct Identity
        {
            // ReSharper disable once NotAccessedField.Local
            private int _hashcode;
            // ReSharper disable once NotAccessedField.Local
            private RuntimeTypeHandle _type;


            public Identity(int hashcode, RuntimeTypeHandle type)
            {
                _hashcode = hashcode;
                _type = type;
            }
        }

        /// <summary>
        /// 缓存对象复制的方法
        /// </summary>
        static Dictionary<Type, Func<object, Dictionary<Identity, object>, object>> _methods1 = new Dictionary<Type, Func<object, Dictionary<Identity, object>, object>>();

        /// <summary>
        /// 缓存对象复制的方法
        /// </summary>
        static Dictionary<Type, Action<object, Dictionary<Identity, object>, object>> _methods2 = new Dictionary<Type, Action<object, Dictionary<Identity, object>, object>>();

        /// <summary>
        /// Get settable fiedls
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        static List<FieldInfo> GetSettableFields(Type t)
        {
            return t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();
        }

        /// <summary>
        /// Create clone method function
        /// </summary>
        /// <param name="type"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedParameter.Local
        static Func<object, Dictionary<Identity, object>, object> CreateCloneMethod1(Type type, Dictionary<Identity, object> objects)
        {
            Type tmptype;
            var fields = GetSettableFields(type);
            var dm = new DynamicMethod("Clone" + Guid.NewGuid(), typeof(object), new[] { typeof(object), typeof(Dictionary<Identity, object>) }, true);
            var il = dm.GetILGenerator();
            il.DeclareLocal(type);
            il.DeclareLocal(type);
            il.DeclareLocal(typeof(Identity));
            if (!type.IsArray)
            {
                il.Emit(OpCodes.Newobj, type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null));
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Stloc_1);
                il.Emit(OpCodes.Ldloca_S, 2);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, type);
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Callvirt, typeof(object).GetMethod("GetHashCode"));
                il.Emit(OpCodes.Ldtoken, type);
                il.Emit(OpCodes.Call, typeof(Identity).GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(int), typeof(RuntimeTypeHandle) }, null));
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldloc_2);
                il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Callvirt, typeof(Dictionary<Identity, object>).GetMethod("Add"));
                foreach (var field in fields)
                {
                    if (!field.FieldType.IsValueType && field.FieldType != typeof(String))
                    {
                        //不符合条件的字段，直接忽略，避免报错。  
                        if ((field.FieldType.IsArray && (field.FieldType.GetArrayRank() > 1 || (!(tmptype = field.FieldType.GetElementType()).IsValueType && tmptype != typeof(String) && tmptype.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null) == null))) ||
                            (!field.FieldType.IsArray && field.FieldType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null) == null))
                            break;
                        il.Emit(OpCodes.Ldloc_1);
                        il.Emit(OpCodes.Ldloc_0);
                        il.Emit(OpCodes.Ldfld, field);
                        il.Emit(OpCodes.Ldarg_1);
                        il.EmitCall(OpCodes.Call, typeof(DeepCopyHelper).GetMethod("CopyImpl", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(field.FieldType), null);
                        il.Emit(OpCodes.Stfld, field);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldloc_1);
                        il.Emit(OpCodes.Ldloc_0);
                        il.Emit(OpCodes.Ldfld, field);
                        il.Emit(OpCodes.Stfld, field);
                    }
                }
                for (type = type.BaseType; type != null && type != typeof(object); type = type.BaseType)
                {
                    //只需要查找基类的私有成员，共有或受保护的在派生类中直接被复制过了。  
                    fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToList();
                    foreach (var field in fields)
                    {
                        if (!field.FieldType.IsValueType && field.FieldType != typeof(String))
                        {
                            //不符合条件的字段，直接忽略，避免报错。  
                            if ((field.FieldType.IsArray && (field.FieldType.GetArrayRank() > 1 || (!(tmptype = field.FieldType.GetElementType()).IsValueType && tmptype != typeof(String) && tmptype.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null) == null))) ||
                                (!field.FieldType.IsArray && field.FieldType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null) == null))
                                break;
                            il.Emit(OpCodes.Ldloc_1);
                            il.Emit(OpCodes.Ldloc_0);
                            il.Emit(OpCodes.Ldfld, field);
                            il.Emit(OpCodes.Ldarg_1);
                            il.EmitCall(OpCodes.Call, typeof(DeepCopyHelper).GetMethod("CopyImpl", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(field.FieldType), null);
                            il.Emit(OpCodes.Stfld, field);
                        }
                        else
                        {
                            il.Emit(OpCodes.Ldloc_1);
                            il.Emit(OpCodes.Ldloc_0);
                            il.Emit(OpCodes.Ldfld, field);
                            il.Emit(OpCodes.Stfld, field);
                        }
                    }
                }
            }
            else
            {
                Type arraytype = type.GetElementType();
                var i = il.DeclareLocal(typeof(int));
                var lb1 = il.DefineLabel();
                var lb2 = il.DefineLabel();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, type);
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldlen);
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Sub);
                il.Emit(OpCodes.Stloc, i);
                il.Emit(OpCodes.Newarr, arraytype);
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Stloc_1);
                il.Emit(OpCodes.Ldloca_S, 2);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Callvirt, typeof(object).GetMethod("GetHashCode"));
                il.Emit(OpCodes.Ldtoken, type);
                il.Emit(OpCodes.Call, typeof(Identity).GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(int), typeof(RuntimeTypeHandle) }, null));
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldloc_2);
                il.Emit(OpCodes.Ldloc_1);
                il.Emit(OpCodes.Callvirt, typeof(Dictionary<Identity, object>).GetMethod("Add"));
                il.Emit(OpCodes.Ldloc, i);
                il.Emit(OpCodes.Br, lb1);
                il.MarkLabel(lb2);
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ldloc, i);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ldloc, i);
                il.Emit(OpCodes.Ldelem, arraytype);
                if (!arraytype.IsValueType && arraytype != typeof(String))
                {
                    il.EmitCall(OpCodes.Call, typeof(DeepCopyHelper).GetMethod("CopyImpl", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(arraytype), null);
                }
                il.Emit(OpCodes.Stelem, arraytype);
                il.Emit(OpCodes.Ldloc, i);
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Sub);
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Stloc, i);
                il.MarkLabel(lb1);
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Clt);
                il.Emit(OpCodes.Brfalse, lb2);
            }
            il.Emit(OpCodes.Ret);


            return (Func<object, Dictionary<Identity, object>, object>)dm.CreateDelegate(typeof(Func<object, Dictionary<Identity, object>, object>));
        }

        /// <summary>
        /// Create clone action
        /// </summary>
        /// <param name="type"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedParameter.Local
        static Action<object, Dictionary<Identity, object>, object> CreateCloneMethod2(Type type, Dictionary<Identity, object> objects)
        {
            Type tmptype;
            var fields = GetSettableFields(type);
            var dm = new DynamicMethod("Copy" + Guid.NewGuid(), null, new[] { typeof(object), typeof(Dictionary<Identity, object>), typeof(object) }, true);
            var il = dm.GetILGenerator();
            il.DeclareLocal(type);
            il.DeclareLocal(type);
            il.DeclareLocal(typeof(Identity));
            if (!type.IsArray)
            {
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Castclass, type);
                il.Emit(OpCodes.Stloc_1);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, type);
                il.Emit(OpCodes.Stloc_0);
                foreach (var field in fields)
                {
                    if (!field.FieldType.IsValueType && field.FieldType != typeof(String))
                    {
                        //不符合条件的字段，直接忽略，避免报错。  
                        if ((field.FieldType.IsArray && (field.FieldType.GetArrayRank() > 1 || (!(tmptype = field.FieldType.GetElementType()).IsValueType && tmptype != typeof(String) && tmptype.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null) == null))) ||
                            (!field.FieldType.IsArray && field.FieldType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null) == null))
                            break;
                        il.Emit(OpCodes.Ldloc_1);
                        il.Emit(OpCodes.Ldloc_0);
                        il.Emit(OpCodes.Ldfld, field);
                        il.Emit(OpCodes.Ldarg_1);
                        il.EmitCall(OpCodes.Call, typeof(DeepCopyHelper).GetMethod("CopyImpl", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(field.FieldType), null);
                        il.Emit(OpCodes.Stfld, field);
                    }
                    else
                    {
                        il.Emit(OpCodes.Ldloc_1);
                        il.Emit(OpCodes.Ldloc_0);
                        il.Emit(OpCodes.Ldfld, field);
                        il.Emit(OpCodes.Stfld, field);
                    }
                }
                for (type = type.BaseType; type != null && type != typeof(object); type = type.BaseType)
                {
                    //只需要查找基类的私有成员，共有或受保护的在派生类中直接被复制过了。  
                    fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToList();
                    foreach (var field in fields)
                    {
                        if (!field.FieldType.IsValueType && field.FieldType != typeof(String))
                        {
                            //不符合条件的字段，直接忽略，避免报错。  
                            if ((field.FieldType.IsArray && (field.FieldType.GetArrayRank() > 1 || (!(tmptype = field.FieldType.GetElementType()).IsValueType && tmptype != typeof(String) && tmptype.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null) == null))) ||
                                (!field.FieldType.IsArray && field.FieldType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null) == null))
                                break;
                            il.Emit(OpCodes.Ldloc_1);
                            il.Emit(OpCodes.Ldloc_0);
                            il.Emit(OpCodes.Ldfld, field);
                            il.Emit(OpCodes.Ldarg_1);
                            il.EmitCall(OpCodes.Call, typeof(DeepCopyHelper).GetMethod("CopyImpl", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(field.FieldType), null);
                            il.Emit(OpCodes.Stfld, field);
                        }
                        else
                        {
                            il.Emit(OpCodes.Ldloc_1);
                            il.Emit(OpCodes.Ldloc_0);
                            il.Emit(OpCodes.Ldfld, field);
                            il.Emit(OpCodes.Stfld, field);
                        }
                    }
                }
            }
            else
            {
                Type arraytype = type.GetElementType();
                var i = il.DeclareLocal(typeof(int));
                var lb1 = il.DefineLabel();
                var lb2 = il.DefineLabel();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, type);
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldlen);
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Sub);
                il.Emit(OpCodes.Stloc, i);
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Castclass, type);
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Stloc_1);
                il.Emit(OpCodes.Ldloc, i);
                il.Emit(OpCodes.Br, lb1);
                il.MarkLabel(lb2);
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Ldloc, i);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ldloc, i);
                il.Emit(OpCodes.Ldelem, arraytype);
                if (!arraytype.IsValueType && arraytype != typeof(String))
                {
                    il.EmitCall(OpCodes.Call, typeof(DeepCopyHelper).GetMethod("CopyImpl", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(arraytype), null);
                }
                il.Emit(OpCodes.Stelem, arraytype);
                il.Emit(OpCodes.Ldloc, i);
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Sub);
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Stloc, i);
                il.MarkLabel(lb1);
                il.Emit(OpCodes.Ldc_I4_0);
                il.Emit(OpCodes.Clt);
                il.Emit(OpCodes.Brfalse, lb2);
            }
            il.Emit(OpCodes.Ret);


            return (Action<object, Dictionary<Identity, object>, object>)dm.CreateDelegate(typeof(Action<object, Dictionary<Identity, object>, object>));
        }

        /// <summary>
        /// Copy Set type
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <param name="objects"></param>
        /// <author>FreshMan</author>
        /// <creattime>2017-07-03</creattime>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Local
        static TEntity CopyImpl<TEntity>(TEntity source, Dictionary<Identity, object> objects) where TEntity : class
        {
            //为空则直接返回null  
            if (source == null) return null;

            Type type = source.GetType();
            Identity id = new Identity(source.GetHashCode(), type.TypeHandle);
            object result;
            //如果发现曾经复制过，用之前的，从而停止递归复制。
            if (!objects.TryGetValue(id, out result))
            {
                //最后查找对象的复制方法，如果不存在，创建新的。
                Func<object, Dictionary<Identity, object>, object> method;
                if (!_methods1.TryGetValue(type, out method))
                {
                    method = CreateCloneMethod1(type, objects);
                    _methods1.Add(type, method);
                }
                result = method(source, objects);
            }
            return (TEntity)result;
        }

        /// <summary>
        /// Deep copy object all of field
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source"></param>
        /// <author>FreshMan</author>
        /// <creattime>2017-07-03</creattime>
        /// <returns></returns>
        public static TEntity DeepCopy<TEntity>(this TEntity source) where TEntity : class
        {
            Type type = source.GetType();
            //存放内嵌引用类型的复制链，避免构成一个环。
            Dictionary<Identity, object> objects = new Dictionary<Identity, object>();
            Func<object, Dictionary<Identity, object>, object> method;
            if (!_methods1.TryGetValue(type, out method))
            {
                method = CreateCloneMethod1(type, objects);
                _methods1.Add(type, method);
            }
            return (TEntity)method(source, objects);
        }

        /// <summary>  
        /// source property copy to target
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="source">source data</param> 
        /// <param name="target">target type</param>
        /// <author>FreshMan</author>
        /// <creattime>2017-07-03</creattime>
        /// </summary>  
        public static void DeepCopy<TEntity>(this TEntity source, TEntity target) where TEntity : class
        {
            if (target == null)
                // ReSharper disable once NotResolvedInText
                throw new ArgumentNullException("Target", "The target to be copied is not initialized.");
            Type type = source.GetType();
            if (type != target.GetType())
                throw new ArgumentException("The type of object you want to copy is different and cannot be copied.");
            //存放内嵌引用类型的复制链，避免构成一个环。
            Dictionary<Identity, object> objects = new Dictionary<Identity, object>
            {
                {new Identity(source.GetHashCode(), type.TypeHandle), source}
            };
            Action<object, Dictionary<Identity, object>, object> method;
            if (!_methods2.TryGetValue(type, out method))
            {
                method = CreateCloneMethod2(type, objects);
                _methods2.Add(type, method);
            }
            method(source, objects, target);
        }
        #endregion

        #region [2、Deep Copy use recursion]

        /// <summary>
        /// Recursion DeepCopy object
        /// </summary>
        /// <param name="srcobj"></param>
        /// <returns></returns>
        public static object DeepCopyRecursion(object srcobj)
        {
            if (srcobj == null)
            {
                return null;
            }

            Type srcObjType = srcobj.GetType();

            // Is simple value type, directly assign  
            if (srcObjType.IsValueType)
            {
                return srcobj;
            }
            // Is array  
            if (srcObjType.IsArray)
            {
                return DeepCopyArray(srcobj as Array);
            }
            // is List or map  
            else if (srcObjType.IsGenericType)
            {
                return DeepCopyGenericType(srcobj);
            }
            // is cloneable  
            else if (srcobj is ICloneable)
            {
                // Log informations  
                return (srcobj as ICloneable).Clone();
            }
            else
            {
                // Try to do deep copy, create a new copied instance  
                object deepCopiedObj = Activator.CreateInstance(srcObjType);

                // Find out all fields or properties, do deep copy  
                BindingFlags bflags = BindingFlags.DeclaredOnly | BindingFlags.Public
                | BindingFlags.NonPublic | BindingFlags.Instance;
                MemberInfo[] memberCollection = srcObjType.GetMembers(bflags);

                foreach (MemberInfo member in memberCollection)
                {
                    if (member.MemberType == MemberTypes.Field)
                    {
                        FieldInfo field = (FieldInfo)member;
                        object fieldValue = field.GetValue(srcobj);
                        field.SetValue(deepCopiedObj, DeepCopyRecursion(fieldValue));
                    }
                    else if (member.MemberType == MemberTypes.Property)
                    {
                        PropertyInfo property = (PropertyInfo)member;
                        MethodInfo info = property.GetSetMethod(false);
                        if (info != null)
                        {
                            object propertyValue = property.GetValue(srcobj, null);
                            property.SetValue(deepCopiedObj, DeepCopyRecursion(propertyValue), null);
                        }
                    }
                }

                return deepCopiedObj;
            }
        }

        /// <summary>
        /// Deep copy generic
        /// </summary>
        /// <param name="srcGeneric"></param>
        /// <returns></returns>
        private static object DeepCopyGenericType(object srcGeneric)
        {
            try
            {
                // Is List   
                IList srcList = srcGeneric as IList;
                if (srcList == null || srcList.Count <= 0)
                {
                    return null;
                }

                // Create new List<object> instance  
                IList dstList = Activator.CreateInstance(srcList.GetType()) as IList;
                // deep copy each object in List  
                foreach (object o in srcList)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    dstList.Add(DeepCopyRecursion(o));
                }

                return dstList;
            }
            catch (Exception)
            {
                try
                {
                    IDictionary srcDictionary = srcGeneric as IDictionary;
                    if (srcDictionary == null || srcDictionary.Count <= 0)
                    {
                        return null;
                    }

                    // Create new map instance  
                    IDictionary dstDictionary = Activator.CreateInstance(srcDictionary.GetType()) as IDictionary;
                    // deep copy each object in map  
                    foreach (object o in srcDictionary.Keys)
                    {
                        // ReSharper disable once PossibleNullReferenceException
                        dstDictionary[o] = srcDictionary[o];
                    }
                    return dstDictionary;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Deep copy array set.
        /// </summary>
        /// <param name="srcArray"></param>
        /// <returns></returns>
        private static Array DeepCopyArray(Array srcArray)
        {
            if (srcArray.Length <= 0)
            {
                return null;
            }
            // Create new array instance based on source array  
            Array arrayCopied = Array.CreateInstance(srcArray.GetValue(0).GetType(), srcArray.Length);
            // deep copy each object in array  
            for (int i = 0; i < srcArray.Length; i++)
            {
                object o = DeepCopyRecursion(srcArray.GetValue(i));
                arrayCopied.SetValue(o, i);
            }
            return arrayCopied;
        }
        #endregion
    }
}
