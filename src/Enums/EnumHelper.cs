// using System.Reflection;
// using System.Runtime.Serialization;
//
// #if NET6_0_OR_GREATER
// using System.ComponentModel.DataAnnotations;
// #endif
//
// namespace ScrubJay.Enums;
//
// public static partial class EnumHelper
// {
//     private static readonly char[] _splitChars = new char[3] { ' ', ',', '|' };
//
//     public static bool TryParse<TEnum>([AllowNull, NotNullWhen(true)] string? str, out TEnum @enum)
//         where TEnum : struct, Enum
//     {
//         @enum = default;
//         if (string.IsNullOrEmpty(str))
//             return false;
//         if (Enum.TryParse<TEnum>(str, true, out @enum))
//             return true;
//
//         @enum = default;
//         var split = str!.Split(_splitChars, StringSplitOptions.RemoveEmptyEntries);
//         foreach (var segment in split)
//         {
//             if (Enum.TryParse<TEnum>(segment, true, out var flag))
//             {
//                 @enum.AddFlag(flag);
//             }
//             else
//             {
//                 // could not parse this segment, invalid
//                 @enum = default;
//                 return false;
//             }
//         }
//
//         return true;
//     }
// }
//
// public static partial class EnumHelper
// {
//     public static string? GetName(Type enumType, object value)
//     {
//         throw new NotImplementedException();
//     }
// }
//
// public static partial class EnumHelper<TEnum>
//     where TEnum : struct, Enum
// {
//     private static readonly Dictionary<TEnum, string> _names = new();
//
//     static EnumHelper()
//     {
//         var enumType = typeof(TEnum);
//         var enumMemberFields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
//         foreach (var memberField in enumMemberFields)
//         {
//             if (memberField.FieldType != enumType)
//                 continue;
//             TEnum value = (TEnum)memberField.GetValue(null)!;
//             HashSet<string> names = new();
//             var attrs = Attribute.GetCustomAttributes(memberField);
//             foreach (var attr in attrs)
//             {
//                 string? name = null;
//                 switch (attr)
//                 {
//                     case EnumMemberAttribute enumMemberAttribute:
//                         name = enumMemberAttribute.Value;
//                         break;
//                     case DisplayNameAttribute displayNameAttribute:
//                         name = displayNameAttribute.DisplayName;
//                         break;
//                     case DescriptionAttribute descriptionAttribute:
//                         name = descriptionAttribute.Description;
//                         break;
// #if NET6_0_OR_GREATER
//                     case DisplayAttribute displayAttribute:
//                         name = displayAttribute.Name;
//                         break;
// #endif
//                     default:
//                         continue;
//                 }
//
//                 if (!string.IsNullOrWhiteSpace(name))
//                 {
//                     names.Add(name);
//                 }
//             }
//
//             if (names.Count == 0)
//             {
//                 _names.Add(value, memberField.Name);
//             }
//             else if (names.Count == 1)
//             {
//                 _names.Add(value, names.First());
//             }
//
//             throw new NotImplementedException();
//         }
//     }
// }