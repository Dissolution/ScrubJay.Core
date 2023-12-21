using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScrubJay.Tests;

public static class TestHelper
{
    public static JsonSerializerOptions AggressiveSerialization { get; } = new JsonSerializerOptions
    {
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        IgnoreReadOnlyFields = false,
        IgnoreReadOnlyProperties = false,
        IncludeFields = true,
    };
    
    
    public static IReadOnlyList<object?> TestObjects { get; } = new List<object?>
    {
        null,
        ulong.MaxValue - 13UL,
        (string?)string.Empty,
        (string?)"ABC",
        default(Nothing),
        new byte[4]{0,147,13,101},
        new Action(() => { }),
        IntPtr.Zero,
        new object(),
        new Exception("BLAH"),
        (int?)147,
        Guid.NewGuid(),
        new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() },
        BindingFlags.Public | BindingFlags.NonPublic,
        typeof(TestHelper),
        new { Name = "Joe", Age = 40},
        DBNull.Value,
        new Dictionary<int,string>
        {
            {1, "one"},
            {2, "two"},
            {3, "three"},
        },
        DateTime.UtcNow,
        TimeSpan.MaxValue,
        AppDomain.CurrentDomain,
        '❤',
        "❤️",
    };
    
    static TestHelper()
    {

    }
    
    public static IEnumerable<object[]> ToEnumerableObjects<T>(int arrayLength, params T[] values)
    {
        int v = 0;
        while (true)
        {
            object[] objects = new object[arrayLength];
            for (var o = 0; o < arrayLength; o++)
            {
                v += 1;
                if (!values.TryGetItem(v, out var value)) yield break;
                objects[o] = value!;
            }
            yield return objects;
        }
    }

    
    
    public static IEnumerable<object[]> ToEnumerableObjects<T>(IEnumerable<T> values, int arrayCount)
    {
        using var e = values.GetEnumerator();

        while (true)
        {
            object[] objects = new object[arrayCount];
            for (var i = 0; i < arrayCount; i++)
            {
                if (!e.TryMoveNext(out var value)) yield break;
                objects[i] = value!;
            }

            yield return objects;
        }
    }

    public static IEnumerable<object?[]> ToEnumerableNullableObjects<T>(IEnumerable<T> values, int arrayCount)
    {
        using var e = values.GetEnumerator();

        while (true)
        {
            object?[] objects = new object?[arrayCount];
            for (var i = 0; i < arrayCount; i++)
            {
                if (!e.TryMoveNext(out var value)) yield break;
                objects[i] = value;
            }

            yield return objects;
        }
    }
}