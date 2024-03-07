global using static ScrubJay.StaticImports;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using ScrubJay.Comparison;

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
        new byte[4] { 0, 147, 13, 101 },
        new Action(() => { }),
        IntPtr.Zero,
        new object(),
        new Exception("BLAH"),
        (int?)147,
        Guid.NewGuid(),
        new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() },
        BindingFlags.Public | BindingFlags.NonPublic,
        typeof(TestHelper),
        new { Name = "Joe", Age = 40 },
        DBNull.Value,
        new Dictionary<int, string>
        {
            { 1, "one" },
            { 2, "two" },
            { 3, "three" },
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


    private static IEnumerable<object?[]> SingleMemberDataIterator<T>(T?[] items)
    {
        var itemCount = items.Length;

        for (var i = 0; i < itemCount; i++)
        {
            yield return new object?[1] { (object?)items[i] };
        }
    }

    private static IEnumerable<object?[]> DoubleMemberDataIterator<T>(T?[] items)
    {
        var itemCount = items.Length;
        if (itemCount < 2)
            yield break;

        int offset = 0;
        while ((offset + 2) <= itemCount)
        {
            object?[] objects = new object?[2]
            {
                (object?)items[offset],
                (object?)items[offset + 1]
            };
            yield return objects;
            offset += 2;
        }
    }

    private static IEnumerable<object?[]> MemberDataIterator<T>(int size, T?[] items)
    {
        var itemCount = items.Length;
        if (itemCount < size)
            yield break;

        int offset = 0;
        while ((offset + size) <= itemCount)
        {
            object?[] objects = new object?[size];
            for (var i = 0; i < size; i++)
            {
                objects[i] = (object?)items[offset + i];
            }

            yield return objects;
            offset += size;
        }
    }

    public static IEnumerable<object[]> ToMemberData<T>(int itemsPerArray, params T?[] items)
    {
        return (itemsPerArray switch
        {
            < 1 => throw new ArgumentOutOfRangeException(nameof(itemsPerArray)),
            1 => SingleMemberDataIterator<T>(items),
            2 => DoubleMemberDataIterator<T>(items),
            _ => MemberDataIterator<T>(itemsPerArray, items)
        })!;
    }


    public static IEnumerable<object?[]> ToNullableMemberData<T>(int itemsPerArray, params T?[] items)
    {
        return (itemsPerArray switch
        {
            < 1 => throw new ArgumentOutOfRangeException(nameof(itemsPerArray)),
            1 => SingleMemberDataIterator<T>(items),
            2 => DoubleMemberDataIterator<T>(items),
            _ => MemberDataIterator<T>(itemsPerArray, items)
        });
    }

    public static IEnumerable<object?[]> Double<T>(params T?[] items)
    {
        int itemCount = items.Length;
        for (var i = 0; i < itemCount; i++)
        for (var j = 0; j < itemCount; j++)
        {
            yield return new object?[2] { items[i], items[j] };
        }
    }
    
    public static IEnumerable<object?[]> AllCombinations<T>(int argCount, params T?[] items)
    {
        int itemCount = items.Length;
        if (itemCount == 0)
            yield break;
        if (argCount < 1)
            yield break;

        int combinations = argCount * argCount;
        var mdi = new MultiDimensionalIndex(argCount, itemCount);
        bool incremented;
        for (int c = 0; c < combinations; c++)
        {
            object?[] comb = new object?[argCount];
            for (var a = 0; a < argCount; a++)
            {
                comb[a] = (object?)items[mdi.Indices[a]];
                incremented = mdi.TryIncrement();
                Debug.Assert(incremented);
            }
            yield return comb;
        }
        incremented = mdi.TryIncrement();
        Debug.Assert(!incremented);
    }
}

public sealed class MultiDimensionalIndex
{
    public int Dimensions { get; }

    public (int Lower, int Upper)[] DimensionBounds { get; }

    public int[] Indices { get; }

    public MultiDimensionalIndex(int dimensions, int upperBound)
    {
        if (dimensions < 1)
            throw new ArgumentOutOfRangeException(nameof(dimensions));
        this.Dimensions = dimensions;
        this.DimensionBounds = new (int Lower, int Upper)[dimensions];
        this.DimensionBounds.Initialize((0, upperBound));
        this.Indices = new int[dimensions];
        Reset();
    }
    
    public MultiDimensionalIndex(int dimensions, params int[] upperBounds)
    {
        if (dimensions < 1)
            throw new ArgumentOutOfRangeException(nameof(dimensions));
        if (upperBounds.Length != dimensions)
            throw new ArgumentOutOfRangeException(nameof(upperBounds));
        this.Dimensions = dimensions;
        this.DimensionBounds = Array.ConvertAll<int, (int,int)>(upperBounds, upper => new(0, upper));
        this.Indices = new int[dimensions];
        Reset();
    }
    
    public MultiDimensionalIndex(int dimensions, params (int Lower, int Upper)[] bounds)
    {
        if (dimensions < 1)
            throw new ArgumentOutOfRangeException(nameof(dimensions));
        if (bounds.Length != dimensions)
            throw new ArgumentOutOfRangeException(nameof(bounds));
        this.Dimensions = dimensions;
        this.DimensionBounds = bounds;
        this.Indices = new int[dimensions];
        Reset();
    }
    

    private bool TryIncrementAt(int rank)
    {
        if (rank < 0 || rank >= Dimensions) return false;

        var indices = this.Indices;
        var rankIndex = indices[rank];
        var rankBounds = this.DimensionBounds[rank];
        
        // Am I at the limits?
        if (rankIndex >= rankBounds.Upper)
        {
            // Try to increment the next higher dimension (right to left)
            if (!TryIncrementAt(rank - 1))
            {
                // Nothing can increment
                return false;
            }
            // Someone else incremented, reset this rank
            indices[rank] = rankBounds.Lower;
            return true;
        }
        // I can increment
        indices[rank] = rankIndex + 1;
        return true;
    }
    
    public void Reset()
    {
        for (var i = 0; i < Dimensions; i++)
        {
            Indices[i] = DimensionBounds[i].Lower;
        }
    }

    public bool TryIncrement()
    {
        // right to left
        return TryIncrementAt(Dimensions - 1);
    }
}