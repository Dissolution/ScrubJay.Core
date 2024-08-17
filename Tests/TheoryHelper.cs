using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ScrubJay.Collections;

namespace ScrubJay.Tests;

public sealed class MiscTheoryData : IReadOnlyCollection<object?[]>
{
    private readonly List<object?[]> _rows;

    public int Count => _rows.Count;

    public MiscTheoryData()
    {
        _rows = new(64);
    }

    public void Add<T>(T? value)
    {
        _rows.Add(new object?[1] { (object?)value });
    }

    public void Add(object? obj)
    {
        _rows.Add(new object?[1] { obj });
    }

    public void AddRow(params object?[] objects)
    {
        _rows.Add(objects);
    }

    public MiscTheoryData Combinations(int columns)
    {
        if (columns < 1) 
            throw new ArgumentOutOfRangeException(nameof(columns));

        var rows = _rows;
        if (rows.Count == 0) 
            return new MiscTheoryData();

        var lengths = new int[columns];
        lengths.AsSpan().Fill(rows.Count);
        BoundedIndices boundedIndices = BoundedIndices.Lengths(lengths);

        var data = new MiscTheoryData();

        while (boundedIndices.TryMoveNext(out var indices))
        {
            var row = new object?[columns];
            for (var c = 0; c < columns; c++)
            {
                row[c] = rows[indices[c]][0];
            }
            data.AddRow(row);
        }

        return data;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<object?[]> GetEnumerator() => _rows.GetEnumerator();
}

public static class TheoryHelper
{


    public static class Data
    {
        public static object?[] Objects => new object?[]
        {
            // null
            (object?)null,
            // enum
            BindingFlags.Public | BindingFlags.NonPublic,
            // primitive
            (byte)147,
            // custom primitive
            default(Option.None),
            // struct
            IntPtr.Zero,
            // Nullable
            //(Nullable<int>)null,
            (Nullable<int>)147,
            // char
            '❤',
            // string
            //(string?)null,
            string.Empty,
            "Sphinx of black quartz, judge my vow.",
            "❤️",
            // array
            new byte[4] { 0, 147, 13, 101 },
            new Action(static () => { }),
            // object itself
            new object(),
            // type
            typeof(TheoryHelper),
            // exception
            new Exception("TheoryHelper.TheoryObjects"),
            // old net stuff
            DBNull.Value,
            // anonymous object
            new { Id = 147, Name = "TJ", IsAdmin = true, },
            // simple dictionary
            new Dictionary<int, string>
            {
                { 1, "one" },
                { 2, "two" },
                { 3, "three" },
            },
            // complex list
            new List<List<(string, object?)>>
            {
                new()
                {
                    ("a", 0),
                    ("b", null),
                    ("c", false),
                },
                new()
                {
                    ("a", 1),
                    ("b", new object()),
                    ("c", true),
                },
            },
            // complex class
            AppDomain.CurrentDomain,
        };

        public static TheoryData<Type> AllKnownTypes { get; }

        static Data()
        {
            HashSet<Type> allTypes = new();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (var a = 0; a < assemblies.Length; a++)
            {
                Assembly assembly;
                Type[] assemblyTypes;
                try
                {
                    assembly = assemblies[a];
                    assemblyTypes = assembly.GetTypes(); // All types, not just Public
                    for (var t = 0; t < assemblyTypes.Length; t++)
                    {
                        allTypes.Add(assemblyTypes[t]);
                    }
                }
                catch (Exception)
                {
                    // Ignore any exceptions, Assemblies can be tricky
                }
            }
            AllKnownTypes = new TheoryData<Type>(allTypes);
        }
    }

    internal static void Shuffle<T>(Span<T> items)
    {
#if NET8_0_OR_GREATER
        Random.Shared.Shuffle(items);
#else
#if NET6_0_OR_GREATER
        var rand = Random.Shared;
#else
        var rand = new Random();
#endif

        int n = items.Length;

        for (int i = 0; i < n - 1; i++)
        {
            int j = rand.Next(i, n);

            if (j != i)
            {
                T temp = items[i];
                items[i] = items[j];
                items[j] = temp;
            }
        }
#endif
    }

    [return: NotNullIfNotNull(nameof(array))]
    internal static T[]? ReverseShallowCopy<T>(T[]? array)
    {
        if (array is null) return null;
        int len = array.Length;
        T[] copy = new T[len];
        for (int s = 0, e = len - 1; s < len && e >= 0; s++, e--)
        {
            copy[e] = array[s];
        }
        return copy;
    }


    public static TheoryData<T> OneParamCombinations<T>(params T[] values)
    {
        return new TheoryData<T>(values);
    }

    public static TheoryData<T, T> TwoParamCombinations<T>(params T[] values)
    {
        var data = new TheoryData<T, T>();
        int count = values.Length;
        if (count == 0)
            return data;
        for (var i = 0; i < count; i++)
        for (var j = 0; j < count; j++)
        {
            data.Add(values[i], values[j]);
        }
        return data;
    }

    public static TheoryData<T, T, T> ThreeParamCombinations<T>(params T[] values)
    {
        var data = new TheoryData<T, T, T>();
        int count = values.Length;
        if (count == 0)
            return data;
        for (var i = 0; i < count; i++)
        for (var j = 0; j < count; j++)
        for (var k = 0; k < count; k++)
        {
            data.Add(values[i], values[j], values[k]);
        }
        return data;
    }


//    public static IEnumerable<object?[]> AllCombinations<T>(int argCount, params T?[] items)
//    {
//        if (argCount < 1)
//            yield break;
//        int itemCount = items.Length;
//        if (itemCount == 0)
//            yield break;
//
//        long combCount = (long)Math.Pow(itemCount, argCount);
//        var indexer = new CombinationIndex(argCount, itemCount);
//        bool got;
//        int[]? indices;
//        for (var c = 0L; c < combCount; c += 1L)
//        {
//            object?[] args = new object?[argCount];
//            got = indexer.TryGetNext(out indices);
//            Debug.Assert(got);
//            Debug.Assert(indices is not null);
//            for (var a = 0; a < argCount; a++)
//            {
//                args[a] = (object?)items[indices![a]];
//            }
//            yield return args;
//        }
//        got = indexer.TryGetNext(out indices);
//        Debug.Assert(!got);
//        Debug.Assert(indices is null);
//    }

    private static int[] GetIndices(int count)
    {
        var indices = new int[count];
        for (var i = count - 1; i >= 0; i--)
        {
            indices[i] = i;
        }
        return indices;
    }

    public static IEnumerable<object?[]> ShuffledCombinations<T>(int argCount, params T?[] items)
    {
        if (argCount < 1)
            yield break;
        int itemCount = items.Length;
        if (itemCount == 0)
            yield break;

        var indices = new List<int[]>();
        for (var a = 0; a < argCount; a++)
        {
            var randIndices = GetIndices(itemCount);
            Shuffle<int>(randIndices);
            indices.Add(randIndices);
        }
        for (var i = 0; i < itemCount; i++)
        {
            object?[] args = new object?[argCount];
            for (var a = 0; a < argCount; a++)
            {
                args[a] = indices[a][i];
            }
            yield return args;
        }
    }
}