#pragma warning disable CA1710 // Identifiers should have correct suffix

namespace ScrubJay.Collections;

[PublicAPI]
public static class Values
{
    /// <summary>
    /// Concatenates two specified instances of <see cref="Values{T}"/>.
    /// </summary>
    /// <param name="values1">The first <see cref="Values{T}"/> to concatenate.</param>
    /// <param name="values2">The second <see cref="Values{T}"/> to concatenate.</param>
    /// <returns>The concatenation of <paramref name="values1"/> and <paramref name="values2"/>.</returns>
    public static Values<T> Concat<T>(Values<T> values1, Values<T> values2)
    {
        int count1 = values1.Count;
        int count2 = values2.Count;

        if (count1 == 0)
        {
            return values2;
        }

        if (count2 == 0)
        {
            return values1;
        }

        var combined = new T[count1 + count2];
        values1.FastCopyTo(combined);
        values2.FastCopyTo(combined.AsSpan(count1));
        return new Values<T>(combined);
    }

    /// <summary>
    /// Concatenates specified instance of <see cref="Values{T}"/> with specified <see cref="string"/>.
    /// </summary>
    /// <param name="values">The <see cref="Values{T}"/> to concatenate.</param>
    /// <param name="value">The <see cref="string" /> to concatenate.</param>
    /// <returns>The concatenation of <paramref name="values"/> and <paramref name="value"/>.</returns>
    public static Values<T> Concat<T>(in Values<T> values, T? value)
    {
        if (value is null)
        {
            return values;
        }

        int count = values.Count;
        if (count == 0)
        {
            return new Values<T>(value);
        }

        var combined = new T[count + 1];
        values.FastCopyTo(combined);
        combined[count] = value;
        return new Values<T>(combined);
    }

    /// <summary>
    /// Concatenates specified instance of <see cref="string"/> with specified <see cref="Values{T}"/>.
    /// </summary>
    /// <param name="value">The <see cref="string" /> to concatenate.</param>
    /// <param name="values">The <see cref="Values{T}"/> to concatenate.</param>
    /// <returns>The concatenation of <paramref name="values"/> and <paramref name="values"/>.</returns>
    public static Values<T> Concat<T>(T? value, in Values<T> values)
    {
        if (value is null)
        {
            return values;
        }

        int count = values.Count;
        if (count == 0)
        {
            return new Values<T>(value);
        }

        var combined = new T[count + 1];
        combined[0] = value;
        values.FastCopyTo(combined.AsSpan(1));
        return new Values<T>(combined);
    }
}

[PublicAPI]
public readonly struct Values<T> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Values<T>, Values<T>, bool>,
    IEqualityOperators<Values<T>, T, bool>,
    IEqualityOperators<Values<T>, T[], bool>,
    //IEqualityOperators<Values<T>, object, bool>,
#endif
    IReadOnlyList<T>,
    IReadOnlyCollection<T>,
    IEnumerable<T>,
    IEquatable<Values<T>>,
    IEquatable<T>,
    IEquatable<T[]>
{
    public static implicit operator Values<T>(T value) => new(value);
    public static implicit operator Values<T>(T[] values) => new(values);

    public static bool operator ==(Values<T> left, Values<T> right) => left.Equals(right);
    public static bool operator !=(Values<T> left, Values<T> right) => !left.Equals(right);

    public static bool operator ==(Values<T> left, T? right) => left.Equals(right);
    public static bool operator !=(Values<T> left, T? right) => !left.Equals(right);
    public static bool operator ==(Values<T> left, T[]? right) => left.Equals(right);
    public static bool operator !=(Values<T> left, T[]? right) => !left.Equals(right);
    public static bool operator ==(Values<T> left, object? right) => left.Equals(right);
    public static bool operator !=(Values<T> left, object? right) => !left.Equals(right);

    public static bool operator ==(T? right, Values<T> left) => left.Equals(right);
    public static bool operator !=(T? right, Values<T> left) => !left.Equals(right);
    public static bool operator ==(T[]? right, Values<T> left) => left.Equals(right);
    public static bool operator !=(T[]? right, Values<T> left) => !left.Equals(right);
    public static bool operator ==(object? right, Values<T> left) => left.Equals(right);
    public static bool operator !=(object? right, Values<T> left) => !left.Equals(right);

    public static readonly Values<T> Empty;


    private readonly object? _obj;

    public T this[int index] => TryGet(index).OkOrThrow();

    public int Count => Match(static () => 0, static _ => 1, static values => values.Length);

    public bool IsNull => _obj is null;

    public bool IsNullOrEmpty => Match(static () => true, static _ => false, static values => values.Length == 0);


    public Values()
    {
        _obj = null;
    }

    public Values(T value)
    {
        _obj = (object?)value;
    }

    public Values(params T[]? values)
    {
        _obj = (object?)values;
    }

    private Result<T, Exception> TryGet(int index)
    {
        object? obj = _obj;

        if (obj is null)
        {
            return new InvalidOperationException("Values is empty");
        }

        if (obj is T value)
        {
            if (index == 0)
            {
                return value;
            }

            return new ArgumentOutOfRangeException(nameof(index), index, "Values only contains one item");
        }

        var values = Unsafe.As<T[]>(obj);
        return Validate.Index(index, values.Length).OkSelect(i => values[i]);
    }


    public bool IsValue([MaybeNullWhen(false)] out T value)
    {
        return _obj.Is(out value);
    }

    public Option<T> HasValue()
    {
        if (_obj is T value)
            return Some(value);
        return None<T>();
    }

    public bool IsValues([MaybeNullWhen(false)] out T[] values)
    {
        return _obj.Is(out values);
    }

    public Option<T[]> HasValues()
    {
        if (_obj is T[] values)
            return Some(values);
        return None<T[]>();
    }

    public bool Contains(T item)
    {
        return Match(
            static () => false,
            value => EqualityComparer<T>.Default.Equals(item, value),
            values => Array.IndexOf<T>(values, item) != -1);
    }

    internal void FastCopyTo(Span<T> span)
    {
        object? obj = _obj;
        if (obj is null)
            return;
        if (obj is T value)
        {
            span[0] = value;
        }
        else
        {
            Sequence.CopyTo(Unsafe.As<T[]>(obj), span);
        }
    }

    public Result<int, Exception> TryCopyTo(Span<T> span)
    {
        object? obj = _obj;
        if (obj is null)
        {
            return 0;
        }
        else if (obj is T value)
        {
            if (span.Length == 0)
                return new ArgumentException(default, nameof(span));
            span[0] = value;
            return 1;
        }
        else
        {
            var values = Unsafe.As<T[]>(obj);
            if (span.Length < values.Length)
                return new ArgumentException(default, nameof(span));
            Sequence.CopyTo(values, span);
            return values.Length;
        }
    }

    public ReadOnlySpan<T> AsSpan()
    {
        object? obj = _obj;
        if (obj is null)
            return [];
        if (obj is T)
            return Notsafe.AsReadOnlySpan<T>(obj);
        return new(Unsafe.As<T[]>(obj));
    }

    public T[] ToArray() => Match(static () => [], static value => [value], static values => values);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Match(Act onEmpty, Act<T> onValue, Act<T[]> onValues)
    {
        object? obj = _obj;
        if (obj is null)
        {
            onEmpty();
        }
        else if (obj is T value)
        {
            onValue(value);
        }
        else
        {
            onValues(Unsafe.As<T[]>(obj));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Match<TResult>(Fun<TResult> onEmpty, Fun<T, TResult> onValue, Fun<T[], TResult> onValues)
    {
        object? obj = _obj;

        if (obj is null)
            return onEmpty();

        if (obj is T value)
            return onValue(value);

        return onValues(Unsafe.As<T[]>(obj));
    }


    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<T> GetEnumerator()
    {
        object? obj = _obj;
        if (obj is null)
            return Enumerator.Empty<T>();
        if (obj is T value)
            return Enumerator.Single<T>(value);
        return Enumerator.ForArray<T>(Unsafe.As<T[]>(obj));
    }

    public bool Equals(T? value)
    {
        return _obj is T myValue && EqualityComparer<T>.Default.Equals(myValue, value!);
    }

    public bool Equals(params T[]? values)
    {
        return _obj is T[] myValues && Sequence.Equal(myValues, values!);
    }

    public bool Equals(Values<T> values)
    {
        object? obj = _obj;
        if (obj is null)
            return values._obj is null;
        if (obj is T value)
            return values._obj is T otherValue && EqualityComparer<T>.Default.Equals(value, otherValue);
        var myValues = Unsafe.As<T[]>(obj);
        return values.IsValues(out var otherValues) && Sequence.Equal(myValues, otherValues);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj switch
        {
            null => _obj is null,
            T value => Equals(value),
            T[] values => Equals(values),
            Values<T> tValues => Equals(tValues),
            _ => false,
        };
    }

    public override int GetHashCode()
        => Match(
            static () => Hasher.NullHash,
            static value => Hasher.GetHashCode(value),
            static values => Hasher.Combine(values));

    public override string ToString()
        => Match(
            static () => string.Empty,
            static value => value!.ToString()!,
            static values => values.Length switch
            {
                0 => string.Empty,
                1 => values[0]!.ToString()!,
                _ => string.Join<T>(", ", values),
            });
}