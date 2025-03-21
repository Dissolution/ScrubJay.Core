using System.Reflection;
using ScrubJay.Extensions;
using ScrubJay.Functional;

namespace ScrubJay.Tests;

public static class TestHelper
{
    public static IReadOnlyList<object?> TestObjects { get; } =
    [
        // null
        (object?)null,
        // enum
        BindingFlags.Public | BindingFlags.NonPublic,
        // primitive
        (byte)147,
        // custom primitive
        default(None),
        // struct
        IntPtr.Zero,
        (Nullable<int>)147,
        // char
        '❤',
        // string
        string.Empty,
        "Sphinx of black quartz, judge my vow.",
        "❤️",
        // array
        new byte[4] { 0, 147, 13, 101 },
        // delegate
        new Action(static () => { }),
        // object itself
        new object(),
        // type
        typeof(TestHelper),
        // exception
        new InvalidOperationException("TheoryHelper.TheoryObjects"),
        // old net stuff
        DBNull.Value,
        // anonymous object
        new
        {
            Id = 147,
            Name = "TJ",
            IsAdmin = true,
        },
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
    ];

    public static IReadOnlyCollection<Type> AllKnownTypes { get; }

    public static IReadOnlyCollection<TestClassRecord?> TestClassRecords { get; }

    static TestHelper()
    {
        AllKnownTypes = AppDomain
            .CurrentDomain
            .GetAssemblies()
            // Assemblies can be tricky to access
            .SelectMany(assembly => Result.TryFunc(assembly, static a => a.GetTypes()).OkOr([]))
            .ToHashSet();

        var rand = new Random();

        TestClassRecords = Enumerable
            .Range(0, 20)
            .Select(i => new TestClassRecord(rand.Next(), $"{i}", i.IsEven()))
            .Append(null)
            .ToList();
    }


}

[PublicAPI]
public record class TestClassRecord(int Id, string Name, bool IsAdmin);
