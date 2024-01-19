using ScrubJay.Comparison;
using ScrubJay.Text;

namespace ScrubJay.Collections;

public readonly struct EquatableArray<T> : IReadOnlyList<T>,
#if NET7_0_OR_GREATER
    IEqualityOperators<EquatableArray<T>, EquatableArray<T>, bool>,
#endif
    IEquatable<EquatableArray<T>>,
    IEquatable<T[]>,
    IEnumerable<T>
    where T : IEquatable<T>
{
    public static bool operator ==(EquatableArray<T> left, EquatableArray<T> right) => left.Equals(right);
    public static bool operator !=(EquatableArray<T> left, EquatableArray<T> right) => !left.Equals(right);

    public static readonly EquatableArray<T> Empty = new(Array.Empty<T>());

    public T this[int index] => _array is null ? throw new InvalidOperationException() : _array[index];

    public int Count => _array?.Length ?? 0;

    private readonly T[]? _array;

    public EquatableArray(params T[]? array)
    {
        _array = array;
    }

    public ReadOnlySpan<T> AsSpan() => _array.AsSpan();

    public T[]? AsArray() => _array;

    IEnumerator IEnumerable.GetEnumerator() => (_array ?? Array.Empty<T>()).GetEnumerator();
    public IEnumerator<T> GetEnumerator() => (_array ?? Array.Empty<T>()).AsValid<IEnumerable<T>>().GetEnumerator();

    public bool Equals(EquatableArray<T> array)
    {
        return MemoryExtensions.SequenceEqual<T>(_array.AsSpan(), array.AsSpan());
    }

    public bool Equals(T[]? array)
    {
        return array is not null && MemoryExtensions.SequenceEqual<T>(_array.AsSpan(), array.AsSpan());
    }

    public bool Equals(IEnumerable<T>? values)
    {
        return EnumerableEqualityComparer<T>.Default.Equals(_array, values);
    }

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            EquatableArray<T> equatableArray => Equals(equatableArray),
            T[] array => Equals(array),
            IEnumerable<T> enumerable => Equals(enumerable),
            _ => false
        };
    }
    
    public override int GetHashCode()
    {
        return Hasher.Combine<T>(_array);
    }

    public override string ToString()
    {
        return StringBuilderPool.Rent()
            .Append('[')
            .AppendJoin<T>(", ", _array ?? Array.Empty<T>())
            .Append(']')
            .ToStringAndReturn();
    }
}