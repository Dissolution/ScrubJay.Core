// using System.Text;
// #pragma warning disable CS8618
//
// public static class Dumper
// {
//     private static bool IsSimple(Type type)
//     {
//         return type.IsPrimitive
//                || type == typeof(decimal)
//                || type == typeof(DateTime)
//                || type == typeof(DateTimeOffset)
//                || type == typeof(TimeSpan)
//                || type == typeof(Guid);
//     }
//
//
//     private class DumperInstance : IDisposable
//     {
//         private readonly TextBuilder _builder = new();
//         private readonly HashSet<object> _visited = [];
//
//         public string Dump(object? obj,
//             [CallerArgumentExpression(nameof(obj))]
//             string? objName = null)
//         {
//             _builder.Clear();
//             _visited.Clear();
//
//             if (objName is not null)
//             {
//                 _builder.Append(objName);
//                 _builder.AppendLine(":");
//                 Dump_Step1(obj, IndentSize);
//             }
//             else
//             {
//                 Dump_Step1(obj, 0);
//             }
//
//             return _builder.ToString();
//         }
//
//         private void Dump_Step1(object? obj, int indent = 0, int preIndents = 0)
//         {
//             _builder.Repeat(preIndents, "  "); // two spaces
//
//             // Handle null
//             if (obj == null)
//             {
//                 _builder.AppendLine("`null");
//                 return;
//             }
//
//             var type = obj.GetType();
//
//             if (IsSimple(type))
//             {
//                 DumpPrimitive(obj);
//                 return;
//             }
//
//             // Handle strings that need special formatting
//             if (obj is string str)
//             {
//                 // escape this string value
//                 DumpString(str, true);
//                 return;
//             }
//
//             if (obj is Enum e)
//             {
//                 DumpEnum(e);
//                 return;
//             }
//
//             if (obj is ITuple tuple)
//             {
//                 DumpTuple(tuple);
//                 return;
//             }
//
//             // circular references
//             if (!type.IsValueType)
//             {
//                 if (!_visited.Add(obj))
//                 {
//                     _builder.AppendLine("<🔁>");
//                     return;
//                 }
//             }
//
//             // Handle dictionaries
//             if (obj is IDictionary dict)
//             {
//                 DumpDictionary(dict, indent);
//                 return;
//             }
//
//             // Handle enumerables (lists, arrays, etc.)
//             if (obj is IEnumerable enumerable)
//             {
//                 DumpEnumerable(enumerable, indent);
//                 return;
//             }
//
//             // Handle complex objects
//             DumpObject(obj, indent);
//         }
//

//


//
//
//
//
//
//
//
//
//
//
//

//
//
//


//         private static string FormatString(string str)
//         {
//             // Handle multi-line strings
//             if (str.Contains('\n') || str.Contains('\r'))
//             {
//                 return "|\n" + string.Join("\n",
//                     str.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
//                         .Select(line => "  " + line));
//             }
//
//             // Handle strings that need quoting
//             if (NeedsQuoting(str))
//             {
//                 return $"\"{str.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"";
//             }
//
//             return str;
//         }
//
//
//
//     }
//
// }
//
//
// // Example usage
// public class Example
// {
//     public static void Main()
//     {

//     }
// }
//
