// using System.Reflection;
// using ScrubJay.Functional.IMPL;
//
// namespace ScrubJay.Functional.Tests;
//
// public static class CommonTheoryData
// {
//     private static readonly string?[,] _2dStringArray;
//
//     public static MiscTheoryData Values { get; } = new()
//     {
//         // null
//         (object?)null,
//         // enum
//         BindingFlags.Public | BindingFlags.NonPublic,
//         // primitive
//         (byte)147,
//         // custom primitive
//         default(None),
//         // struct
//         IntPtr.Zero,
//         // Nullable
//         (Nullable<int>)null,
//         (Nullable<int>)147,
//         // char
//         '❤',
//         // string
//         (string?)null,
//         string.Empty,
//         "Sphinx of black quartz, judge my vow!",
//         "🏃🏻‍➡️ 🟫 🦊 🦘 🆙 🦥 🐕",
//         // delegate
//         new Action(static () => { }),
//         // object itself
//         new object(),
//         // type
//         typeof(CommonTheoryData),
//         // exception
//         new Exception(nameof(CommonTheoryData)),
//         // old net stuff
//         DBNull.Value,
//         // anonymous object
//         new { Id = 147, Name = "TJ", IsAdmin = true, },
//         // array
//         new byte[4] { 0, 147, 13, 101 },
//         _2dStringArray,
//         // simple dictionary
//         new Dictionary<int, string>
//         {
//             { 1, "one" },
//             { 2, "two" },
//             { 3, "three" },
//         },
//         // complex list
//         new List<List<(string, object?)>>
//         {
//             new()
//             {
//                 ("a", 0),
//                 ("b", null),
//                 ("c", false),
//             },
//             new()
//             {
//                 ("a", 1),
//                 ("b", new object()),
//                 ("c", true),
//             },
//         },
//         // complex class
//         AppDomain.CurrentDomain,
//     };
//
//     public static TheoryData<Type> AllKnownTypes { get; }
//
//     static CommonTheoryData()
//     {
//         HashSet<Type> allTypes = new();
//         var assemblies = AppDomain.CurrentDomain.GetAssemblies();
//         for (var a = 0; a < assemblies.Length; a++)
//         {
//             Assembly assembly;
//             Type[] assemblyTypes;
//             try
//             {
//                 assembly = assemblies[a];
//                 assemblyTypes = assembly.GetTypes(); // All types, not just Public
//                 for (var t = 0; t < assemblyTypes.Length; t++)
//                 {
//                     allTypes.Add(assemblyTypes[t]);
//                 }
//             }
//             catch (Exception)
//             {
//                 // Ignore any exceptions, Assemblies can be tricky
//             }
//         }
//         AllKnownTypes = new TheoryData<Type>(allTypes);
//
//         _2dStringArray = new string?[2, 2];
//         _2dStringArray[0, 0] = "TRJ";
//         _2dStringArray[0, 1] = Guid.NewGuid().ToString();
//         _2dStringArray[1, 0] = "147";
//         _2dStringArray[1, 1] = null;
//     }
// }