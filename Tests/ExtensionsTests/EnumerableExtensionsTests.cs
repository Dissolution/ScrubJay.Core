// ReSharper disable InvokeAsExtensionMethod

using System.Collections;
using ScrubJay.Extensions;

namespace ScrubJay.Tests.ExtensionsTests;

public class EnumerableExtensionsTests
{
    public static TheoryData Enumerables => new TheoryData<IEnumerable?>()
    {
        null!,
        Array.Empty<object?>(),
        "abc".ToCharArray(),
        "11234566".Distinct(),
        "kgbhfeeekg".ToCharArray().Select(c => c.ToString()).ToHashSet(StringComparer.OrdinalIgnoreCase),
        Enumerable.Empty<int>(),
        Array.CreateInstance(typeof(byte), 0),
        new string?[4] { null, string.Empty, "c", "ABC" },
        Enumerable.Empty<int?>().Append(1).Append(null).Append(4).Append(null).Append(7),

    };


    [Theory]
    [MemberData(nameof(Enumerables))]
    public void WhereNotNull_Works<T>(IEnumerable<T>? enumerable)
    {
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        var output = EnumerableExtensions.WhereNotNull(enumerable);
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        Assert.NotNull(output);
        foreach (var value in output)
        {
            Assert.NotNull(value);
        }
    }


    private sealed class BadEnumerable_Null : IEnumerable<int>, IEnumerable
    {
        IEnumerator IEnumerable.GetEnumerator() => null!;
        public IEnumerator<int> GetEnumerator() => null!;
    }

    private sealed class BadEnumerable_Throw : IEnumerable<int>, IEnumerable
    {
        IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();
        public IEnumerator<int> GetEnumerator() => throw new NotSupportedException();
    }

    private sealed class BadEnumerable_Bad : IEnumerable<int>, IEnumerable
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
        null!,
        new int[3] { 0, 1, 2 },
        new BadEnumerable_Null(),
        new List<int> { 1, 2, 3 },
        new BadEnumerable_Throw(),
        Enumerable.Range(0, 5),
        new BadEnumerable_Bad(),
    };

    [Theory]
    [MemberData(nameof(SwallowedData))]
    public void Swallowed_Works(IEnumerable<int>? enumerable)
    {
        var a = enumerable.Swallowed().ToList();
        // No exception should be thrown
        Assert.NotNull(a);
    }

    [Theory]
    [MemberData(nameof(SwallowedData))]
    public void Swallowed_IsTransitive(IEnumerable<int>? enumerable)
    {
        // ReSharper disable PossibleMultipleEnumeration
        var stringList = enumerable
            .Swallowed()
            .Select(i => i.ToString())
            .ToList();
        Assert.NotNull(stringList);
        // No exception thrown

        if (enumerable is null) return;

        var objList = enumerable
            .Select<int, object?>(_ => throw new InvalidOperationException())
            .Swallowed()
            .ToList();
        Assert.NotNull(objList);
        // No Exception Thrown
        // ReSharper restore PossibleMultipleEnumeration
    }
}