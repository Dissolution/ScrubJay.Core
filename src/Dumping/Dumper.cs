// using System.Collections.Concurrent;
// using System.Reflection;
//
// namespace ScrubJay.Dumping;
//
// public static class Dumper
// {
//     extension(TextBuilder builder)
//     {
//         public TextBuilder Dump<T>(T? value, DumpMode mode = DumpMode.Default)
// #if NET9_0_OR_GREATER
//             where T : allows ref struct
// #endif
//         {
//             DumpTo<T>(builder, value, mode);
//             return builder;
//         }
//     }
//
//     extension<T>(T? value)
//     {
//         public string Dump()
//         {
//             using var builder = new TextBuilder();
//             builder.Dump<T>(value)
//         }
//     }
//
//     static Dumper()
//     {
//
//     }
//
//     private static readonly ConcurrentBag<object> _dumpers = [];
//
//     internal static readonly MethodInfo _dumpEnumMethod = typeof(EnumDumper)
//         .GetMethods(BindingFlags.Public | BindingFlags.Instance)
//         .Where(static method => method.Name == nameof(DumpTo) && method.GetGenericArguments().Length == 1)
//         .FirstOrDefault()
//         .ThrowIfNull();
//
//     internal static readonly MethodInfo _findDumpMethod = typeof(Dumper)
//         .GetMethod(nameof(FindDump), BindingFlags.NonPublic | BindingFlags.Static)
//         .ThrowIfNull();
//
//
//     private static DumpTo<T>? FindDump<T>()
// #if NET9_0_OR_GREATER
//         where T : allows ref struct
// #endif
//     {
//         var type = typeof(T);
//
//         if (type == typeof(object))
//         {
//             return static (tb, value, mode) => DumpTo(tb, Notsafe.Box(value), mode);
//         }
//
//         if (type.IsEnum)
//         {
//             return Delegate.CreateDelegate<DumpTo<T>>(_dumpEnumMethod.MakeGenericMethod(type));
//         }
//
//         foreach (var dumper in _dumpers)
//         {
//             // If this is a cached delegate that can handle this type (as the delegate is `in T`, delegates will handle supertypes)
//             // use that delegate directly
//             if (dumper is DumpTo<T> dumpTo)
//             {
//                 return dumpTo;
//             }
//
//             // If this is an IDumper<T> instance (that can also handle supertypes as above)
//             // we can use the delegate from that instance
//             if (dumper is IValueDumper<T> valueDumper)
//             {
//                 return valueDumper.DumpTo;
//             }
//         }
//
//         // no dumper has been associated with this type
//         return null;
//     }
//
//     private static void DumpNull<T>(TextBuilder builder, DumpMode mode)
// #if NET9_0_OR_GREATER
//         where T : allows ref struct
// #endif
//     {
//         if (mode == DumpMode.Extended)
//         {
//             builder.Append("`null");
//         }
//
//         if (mode.HasFlags(DumpMode.Types))
//         {
//             builder.Append('(')
//                 .Dump(typeof(T))
//                 .Append(")null");
//         }
//     }
//
//
//     public static void Add<T>(DumpTo<T> dumpTo)
//     {
//         Throw.IfNull(dumpTo);
//         _dumpers.Add(dumpTo);
//     }
//
//     public static void Add<T>(IValueDumper<T> valueDumper)
//     {
//         Throw.IfNull(valueDumper);
//         _dumpers.Add(valueDumper);
//     }
//
//     public static void DumpTo(TextBuilder builder, object? obj, DumpMode mode = DumpMode.Default)
//     {
//         if (obj is null)
//         {
//             DumpNull<object>(builder, mode);
//             return;
//         }
//
//         Type  type = obj.GetType();
//         if (type == typeof(object))
//         {
//             // stop infinite recursion
//             builder.Append("(object)");
//             return;
//         }
//
//         // search for a compat
//         _findDumpMethod.MakeGenericMethod(type)
//             .Invoke(null, null)
//             // will be a delegate
//             .ThrowIfNot<Delegate>()
//             // invoke it
//             .DynamicInvoke(builder, obj, mode);
//         // return is void
//     }
//
//     public static void DumpTo<T>(TextBuilder builder, T? value, DumpMode mode = DumpMode.Default)
// #if NET9_0_OR_GREATER
//         where T : allows ref struct
// #endif
//     {
//         if (value is null)
//         {
//             DumpNull<T>(builder, mode);
//             return;
//         }
//
//         // Find a function that lets us dump this value
//         var dumpTo = FindDump<T>();
//         if (dumpTo is not null)
//         {
//             dumpTo(builder, value, mode);
//             return;
//         }
//
//         // no registered function for dumping
//         // just call ToString
//         builder.Append<T>(value);
//     }
//
//
//     public static string Dump<T>(T? value, DumpMode mode = DumpMode.Default)
// #if NET9_0_OR_GREATER
//         where T : allows ref struct
// #endif
//     {
//         using var builder = new TextBuilder();
//         DumpTo<T>(builder, value, mode);
//         return builder.ToString();
//     }
// }