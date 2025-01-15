// ReSharper disable InvokeAsExtensionMethod

using System.Collections;
using ScrubJay.Extensions;

namespace ScrubJay.Tests.ExtensionsTests;

public class EnumerableExtensionsTests
{
    public static TheoryData<object?> Enumerables { get;} = new()
    { 
        (object?)null!,
        (object?)Array.Empty<object?>(),
        (object?)"abc".ToCharArray(),
        (object?)"11234566".Distinct(),
        (object?)"kgbhfeeekg".ToCharArray().Select(c => c.ToString()).ToHashSet(StringComparer.OrdinalIgnoreCase),
        (object?)Enumerable.Empty<int>(),
        (object?)Array.CreateInstance(typeof(byte), 0),
        (object?)new string?[4] { null, string.Empty, "c", "ABC" },
        (object?)Enumerable.Empty<int?>().Append(1).Append(null).Append(4).Append(null).Append(7),
    };


    [Theory]
    [MemberData(nameof(Enumerables))]
    public void WhereNotNullWorks<T>(object? obj)
    {
        var enumerable = obj as IEnumerable<T>;
        var output = EnumerableExtensions.WhereNotNull(enumerable);
        Assert.NotNull(output);
        foreach (var value in output)
        {
            Assert.NotNull(value);
        }
    }


    private sealed class BadEnumerableNull : IEnumerable<int>, IEnumerable
    {
        IEnumerator IEnumerable.GetEnumerator() => null!;
        public IEnumerator<int> GetEnumerator() => null!;
    }

    private sealed class BadEnumerableThrow : IEnumerable<int>, IEnumerable
    {
        IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();
        public IEnumerator<int> GetEnumerator() => throw new NotSupportedException();
    }

    private sealed class BadEnumerableBad : IEnumerable<int>, IEnumerable
    {
        IEnumerator IEnumerable.GetEnumerator() => new BadEnumerator();
        public IEnumerator<int> GetEnumerator() => new BadEnumerator();
    }

    private sealed class BadEnumerator : IEnumerator<int>, IEnumerator
    {
        public int Current => throw new NotSupportedException();
        object IEnumerator.Current => throw new NotSupportedException();
        public bool MoveNext() => throw new NotSupportedException();
        public void Reset() => throw new NotSupportedException();
        public void Dispose() => throw new NotSupportedException();
    }

    public static TheoryData<IEnumerable<int>?> SwallowedData { get; } = new()
    {
        (IEnumerable<int>?)null!,
        (IEnumerable<int>?)new int[3] { 0, 1, 2 },
        (IEnumerable<int>?)new BadEnumerableNull(),
        (IEnumerable<int>?)new List<int> { 1, 2, 3 },
        (IEnumerable<int>?)new BadEnumerableThrow(),
        (IEnumerable<int>?)Enumerable.Range(0, 5),
        (IEnumerable<int>?)new BadEnumerableBad(),
    };

    [Theory]
    [MemberData(nameof(SwallowedData))]
    public void SwallowedWorks(IEnumerable<int>? enumerable)
    {
        var a = enumerable.UnbreakableEnumerate().ToList();
        // No exception should be thrown
        Assert.NotNull(a);
    }

    [Theory]
    [MemberData(nameof(SwallowedData))]
    public void SwallowedIsTransitive(IEnumerable<int>? enumerable)
    {
        // ReSharper disable PossibleMultipleEnumeration
        var stringList = enumerable
            .UnbreakableEnumerate()
            .Select(i => i.ToString())
            .ToList();
        Assert.NotNull(stringList);
        // No exception thrown

        if (enumerable is null)
        {
            return;
        }

        var objList = enumerable
            .Select<int, object?>(_ => throw new InvalidOperationException())
            .UnbreakableEnumerate()
            .ToList();
        Assert.NotNull(objList);
        // No Exception Thrown
        // ReSharper restore PossibleMultipleEnumeration
    }
}
