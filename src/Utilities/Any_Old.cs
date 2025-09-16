// using System.Reflection;
// using System.Reflection.Emit;
//
// namespace ScrubJay.Utilities;
//
// [PublicAPI]
// public static class Any
// {
//     public static string ToString<T>(T? value)
// #if NET9_0_OR_GREATER
//         where T : allows ref struct
// #endif
//     {
//         return Any<T>.ToString(value);
//     }
//
//     public static bool Equals<T>(T? left, T? right)
// #if NET9_0_OR_GREATER
//         where T : allows ref struct
// #endif
//     {
//         return Any<T>.Equals(left, right);
//     }
//
//     public static int GetHashCode<T>(T? value)
// #if NET9_0_OR_GREATER
//         where T : allows ref struct
// #endif
//     {
//         return Any<T>.GetHashCode(value);
//     }
//
//     public static int Compare<T>(T? left, T? right)
// #if NET9_0_OR_GREATER
//         where T : allows ref struct
// #endif
//     {
//         return Any<T>.Compare(left, right);
//     }
// }
//
// public static class Any<T>
// #if NET9_0_OR_GREATER
//     where T : allows ref struct
// #endif
// {
// #if NET9_0_OR_GREATER
//     // IEquatable<T> and Object
//     private static readonly Func<T?, T?, bool> _equals;
//     private static readonly Func<T, int> _getHashCode;
//
//     // IComparable<T>
//     private static readonly Func<T?, T?, int> _compare;
//
//     // Object
//     private static readonly Func<T, string> _toString;
//
//     // IFormattable
//     private static readonly Func<T, string?, IFormatProvider?> _format;
//
//
//
//     static Any()
//     {
//         _equals = CreateEquals();
//         _getHashCode = CreateGetHashCode();
//         _compare = CreateCompare();
//         _toString = CreateToString();
//         _format = CreateFormat();
//     }
//
//     private static D EmitDelegate<D>(string name, Action<ILGenerator> emit)
//         where D : Delegate
//     {
//         var invokeMethod = typeof(D).GetMethod("Invoke", BindingFlags.Public | BindingFlags.Instance);
//         if (invokeMethod is null)
//             throw new MissingMethodException(typeof(D).FullName, "Invoke");
//
//         DynamicMethod method = new DynamicMethod(
//             name,
//             MethodAttributes.Public | MethodAttributes.Static,
//             CallingConventions.Standard,
//             invokeMethod.ReturnType,
//             invokeMethod.GetParameters().ConvertAll(p => p.ParameterType),
//             typeof(Unit).Module,
//             true);
//
//         var gen = method.GetILGenerator();
//         emit(gen);
//         return method.CreateDelegate<D>();
//     }
//
//     private static Func<T, string> CreateToString()
//     {
//         var type = typeof(T);
//
//         // Look for `string instance.ToString()`
//
//         MethodInfo? toStringMethod = type
//             .GetMethod("ToString", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
//
//         if (toStringMethod is null)
//         {
//             //
//
//             return (_ => type.Name);
//         }
//
//         var func = EmitDelegate<Func<T, string>>($"{typeof(T).Name}_ToString", gen =>
//         {
//             gen.Emit(OpCodes.Ldarg_0);
//             if (type.IsByRef || type.IsByRefLike)
//             {
//                 gen.Emit(OpCodes.Constrained, typeof(T));
//                 gen.Emit(OpCodes.Callvirt, toStringMethod);
//             }
//             else if (type.IsValueType)
//             {
//                 //gen.Emit(OpCodes.Constrained, typeof(T));
//                 //gen.Emit(OpCodes.Callvirt, toStringMethod);
//                 gen.Emit(OpCodes.Call, toStringMethod);
//             }
//             else if (type.IsClass || type.IsInterface)
//             {
//                 gen.Emit(OpCodes.Ldind_Ref);
//                 gen.Emit(OpCodes.Callvirt, toStringMethod);
//             }
//             else
//             {
//                 Debugger.Break();
//                 throw new NotImplementedException();
//             }
//
//             gen.Emit(OpCodes.Ret);
//         });
//
//         return func;
//     }
//
//     private static Func<T, string?, IFormatProvider?> CreateFormat()
//     {
//         throw new NotImplementedException();
//     }
//
//     private static Func<T, int> CreateGetHashCode()
//     {
//         var type = typeof(T);
//
//         MethodInfo? getHashCodeMethod = type
//             .GetMethod("GetHashCode",
//                 BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
//         if (getHashCodeMethod is null)
//             return (_ => 0);
//
//         var func = EmitDelegate<Func<T, int>>($"{typeof(T).Name}_GetHashCode", gen =>
//         {
//             gen.Emit(OpCodes.Ldarg_0);
//             if (type.IsByRef || type.IsByRefLike)
//             {
//                 gen.Emit(OpCodes.Constrained, typeof(T));
//                 gen.Emit(OpCodes.Callvirt, getHashCodeMethod);
//             }
//             else if (type.IsValueType)
//             {
//                 //gen.Emit(OpCodes.Constrained, typeof(T));
//                 //gen.Emit(OpCodes.Callvirt, toStringMethod);
//                 gen.Emit(OpCodes.Call, getHashCodeMethod);
//             }
//             else if (type.IsClass || type.IsInterface)
//             {
//                 gen.Emit(OpCodes.Ldind_Ref);
//                 gen.Emit(OpCodes.Callvirt, getHashCodeMethod);
//             }
//             else
//             {
//                 Debugger.Break();
//                 throw new NotImplementedException();
//             }
//
//             gen.Emit(OpCodes.Ret);
//         });
//
//         return func;
//     }
//
//     private static Func<T?, T?, bool> CreateEquals()
//     {
//         var type = typeof(T);
//
//         var equalsMethod = type
//             .GetMethods(BindingFlags.Public | BindingFlags.Instance)
//             .Where(static method => method.Name == "Equals")
//             .Where(static method => method.ReturnType == typeof(bool))
//             .Where(method =>
//             {
//                 var parameters = method.GetParameters();
//                 if (parameters.Length != 1)
//                     return false;
//                 if (parameters[0].ParameterType != typeof(T))
//                     return false;
//                 return true;
//             })
//             .OneOrDefault();
//
//         if (equalsMethod is null)
//         {
//             var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
//
//             Debugger.Break();
//             throw new NotImplementedException();
//         }
//
//         var func = EmitDelegate<Func<T?, T?, bool>>($"{typeof(T).Name}_Equals", gen =>
//         {
//             gen.Emit(OpCodes.Ldarg_0);
//             // might have to deref
//             if (type.IsClass || type.IsInterface)
//                 gen.Emit(OpCodes.Ldind_Ref, type);
//
//             gen.Emit(OpCodes.Ldarg_1);
//             if (type.IsByRef || type.IsByRefLike)
//             {
//                 gen.Emit(OpCodes.Constrained, typeof(T));
//                 gen.Emit(OpCodes.Callvirt, equalsMethod);
//             }
//             else if (type.IsValueType)
//             {
//                 //gen.Emit(OpCodes.Constrained, typeof(T));
//                 //gen.Emit(OpCodes.Callvirt, toStringMethod);
//                 gen.Emit(OpCodes.Call, equalsMethod);
//             }
//             else if (type.IsClass || type.IsInterface)
//             {
//                 gen.Emit(OpCodes.Callvirt, equalsMethod);
//             }
//             else
//             {
//                 Debugger.Break();
//                 throw new NotImplementedException();
//             }
//
//             gen.Emit(OpCodes.Ret);
//         });
//
//         return func;
//     }
//
//     private static Func<T?, T?, int> CreateCompare()
//     {
//         var type = typeof(T);
//
//         var compareToMethod = type
//             .GetMethods(BindingFlags.Public | BindingFlags.Instance)
//             .Where(static method => method.Name == "CompareTo")
//             .Where(static method => method.ReturnType == typeof(int))
//             .Where(method =>
//             {
//                 var parameters = method.GetParameters();
//                 if (parameters.Length != 1)
//                     return false;
//                 if (parameters[0].ParameterType != typeof(T))
//                     return false;
//                 return true;
//             })
//             .OneOrDefault();
//
//         if (compareToMethod is null)
//         {
//             throw new NotImplementedException();
//         }
//
//         var func = EmitDelegate<Func<T?, T?, int>>($"{typeof(T).Name}_CompareTo", gen =>
//         {
//             gen.Emit(OpCodes.Ldarg_0);
//             // might have to deref
//             if (type.IsClass || type.IsInterface)
//                 gen.Emit(OpCodes.Ldind_Ref, type);
//
//             gen.Emit(OpCodes.Ldarg_1);
//             if (type.IsByRef || type.IsByRefLike)
//             {
//                 gen.Emit(OpCodes.Constrained, typeof(T));
//                 gen.Emit(OpCodes.Callvirt, compareToMethod);
//             }
//             else if (type.IsValueType)
//             {
//                 //gen.Emit(OpCodes.Constrained, typeof(T));
//                 //gen.Emit(OpCodes.Callvirt, toStringMethod);
//                 gen.Emit(OpCodes.Call, compareToMethod);
//             }
//             else if (type.IsClass || type.IsInterface)
//             {
//                 gen.Emit(OpCodes.Callvirt, compareToMethod);
//             }
//             else
//             {
//                 Debugger.Break();
//                 throw new NotImplementedException();
//             }
//
//             gen.Emit(OpCodes.Ret);
//         });
//
//         return func;
//     }
//
//
// #endif
//
//     public static string ToString(T? value)
//     {
//         if (value is null)
//             return string.Empty;
//
// #if NET9_0_OR_GREATER
//         return _toString(value);
// #else
//         return value.ToString() ?? string.Empty;
// #endif
//     }
//
//     public static bool Equals(T? left, T? right)
//     {
// #if NET9_0_OR_GREATER
//         return _equals(left, right);
// #else
//         return EqualityComparer<T>.Default.Equals(left!, right!);
// #endif
//     }
//
//     public static int GetHashCode(T? value)
//     {
//         if (value is null)
//             return 0;
// #if NET9_0_OR_GREATER
//         return _getHashCode(value);
// #else
//         return value.GetHashCode();
// #endif
//     }
//
//     public static int Compare(T? left, T? right)
//     {
// #if NET9_0_OR_GREATER
//         return _compare(left, right);
// #else
//         return Comparer<T>.Default.Compare(left!, right!);
// #endif
//     }
// }