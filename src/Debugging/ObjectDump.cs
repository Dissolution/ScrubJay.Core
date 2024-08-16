// using System.Diagnostics;
// using System.Reflection;
// using ScrubJay.Enums;
//
// namespace ScrubJay.Debugging;
//
// #if DEBUG
// public static class DebugDumper
// {
//     [return: NotNullIfNotNull(nameof(obj))]
//     public static object? Dump(
//         this object? obj, 
//         BindingFlags flags = BindingFlags.Public | BindingFlags.Instance,
//         MemberTypes memberTypes = MemberTypes.Property,
//         bool recursive = false)
//     {
//         if (obj is null) return null;
//         Type valueType = obj.GetType();
//         if (valueType.IsPrimitive || valueType.IsPointer || valueType.IsEnum || valueType.IsArray ||
//             valueType == typeof(TimeSpan) || valueType == typeof(DateTime) || valueType == typeof(Guid) ||
//             valueType == typeof(string) ||
//             valueType == typeof(object) ||
//             valueType.Implements<IEnumerable>())
//         {
//             return obj;
//         }
//
//         MemberInfo[] members = valueType.GetMembers(flags);
//         if (members.Length == 0)
//         {
//             Debugger.Break();
//             return obj;
//         }
//         
//         // We're returning a Dict
//         Dictionary<string, object?> memberValues = new();
//        
//         if (memberTypes.HasFlags(MemberTypes.Field))
//         {
//             foreach (var field in members.OfType<FieldInfo>())
//             {
//                 if (!recursive)
//                 {
//                     memberValues[field.Name] = field.GetValue(obj);
//                 }
//                 else
//                 {
//                     object? fieldValue = field.GetValue(obj);
//                     memberValues[field.Name] = Dump(fieldValue, flags, memberTypes, true);
//                 }
//             }
//         }
//
//         if (memberTypes.HasFlags(MemberTypes.Property))
//         {
//             foreach (var property in members.OfType<PropertyInfo>())
//             {
//                 if (property.GetIndexParameters().Length > 0)
//                     continue;
//                 if (!recursive)
//                 {
//                     memberValues[property.Name] = property.GetValue(obj);
//                 }
//                 else
//                 {
//                     object? fieldValue = property.GetValue(obj);
//                     memberValues[property.Name] = Dump(fieldValue, flags, memberTypes, true);
//                 }
//             }
//         }
//
//         return memberValues;
//     }
// }
// #endif