// Identifiers should have correct suffix
#pragma warning disable CA1710
// Remove unnecessary cast (IDE0004)
#pragma warning disable IDE0004

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

/// <summary>
/// A small, immutable collection of Values
/// </summary>
/// <typeparam name="T"></typeparam>
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

    public static bool operator ==(Values<T> left, T? right) => left.Equals(right);
    public static bool operator ==(Values<T> left, T[]? right) => left.Equals(right);
    public static bool operator ==(Values<T> left, Values<T> right) => left.Equals(right);
    public static bool operator ==(Values<T> left, object? right) => left.Equals(right);
    public static bool operator !=(Values<T> left, T? right) => !left.Equals(right);
    public static bool operator !=(Values<T> left, T[]? right) => !left.Equals(right);
    public static bool operator !=(Values<T> left, Values<T> right) => !left.Equals(right);
    public static bool operator !=(Values<T> left, object? right) => !left.Equals(right);


    /// <summary>
    /// Returns an empty <see cref="Values{T}"/>
    /// </summary>
    public static readonly Values<T> Empty;


    /// <summary>
    /// Storage object
    /// </summary>
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


    public Result<T, Exception> TryGet(Index index) => Match(index,
        onEmpty: static _ => new InvalidOperationException("Values is empty"),
        onValue: static (idx, value) => Validate.Index(idx, 1).OkSelect(i => value),
        onValues: static (idx, values) => Validate.Index(idx, values.Length).OkSelect(i => values[i])
        );



    public bool IsValue([MaybeNullWhen(false)] out T value) => _obj.Is(out value);

    public Option<T> HasValue()
    {
        if (_obj is T value)
            return Some(value);
        return None<T>();
    }

    public bool IsValues([MaybeNullWhen(false)] out T[] values) => _obj.Is(out values);

    public Option<T[]> HasValues()
    {
        if (_obj is T[] values)
            return Some(values);
        return None<T[]>();
    }

    public bool Contains(T item) => Match(item,
            static _ => false,
            static (itm, value) => EqualityComparer<T>.Default.Equals(itm, value),
            static (itm, values) => Array.IndexOf<T>(values, itm) != -1
            );

    public bool Contains(T item, IEqualityComparer<T>? itemComparer)
    {
        return Match(
            static () => false,
            value => Equate.With(itemComparer).Equals(value, item),
            values => values.Contains(item, itemComparer));
    }

    internal void FastCopyTo(Span<T> span)
    {
        object? obj = _obj;
        if (obj is null)
        {
            return;
        }
        else if (obj is T value)
        {
            span[0] = value;
        }
        else
        {
            Debug.Assert(obj is T[]);
            T[] values = Notsafe.As<T[]>(obj);
            Sequence.CopyTo(values, span);
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
        Debug.Assert(obj is T[]);
        return new(Notsafe.As<T[]>(obj));
    }

    public T[] ToArray() => Match(static () => [], static value => [value], static values => values);


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
            Debug.Assert(obj is T[]);
            var values = Notsafe.As<T[]>(obj);
            onValues(values);
        }
    }

    public void Match<TState>(TState state, Act<TState> onEmpty, Act<TState, T> onValue, Act<TState, T[]> onValues)
    {
        object? obj = _obj;
        if (obj is null)
        {
            onEmpty(state);
        }
        else if (obj is T value)
        {
            onValue(state, value);
        }
        else
        {
            Debug.Assert(obj is T[]);
            var values = Notsafe.As<T[]>(obj);
            onValues(state, values);
        }
    }

    public TResult Match<TResult>(Fun<TResult> onEmpty, Fun<T, TResult> onValue, Fun<T[], TResult> onValues)
    {
        object? obj = _obj;
        switch (obj)
        {
            case null:
                return onEmpty();
            case T value:
                return onValue(value);
            default:
            {
                Debug.Assert(obj is T[]);
                var values = Notsafe.As<T[]>(obj);
                return onValues(values);
            }
        }
    }

    public TResult Match<TState, TResult>(TState state, Fun<TState, TResult> onEmpty, Fun<TState, T, TResult> onValue, Fun<TState, T[], TResult> onValues)
    {
        object? obj = _obj;
        switch (obj)
        {
            case null:
                return onEmpty(state);
            case T value:
                return onValue(state, value);
            default:
            {
                Debug.Assert(obj is T[]);
                var values = Notsafe.As<T[]>(obj);
                return onValues(state, values);
            }
        }
    }


    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<T> GetEnumerator() => Match(
        static () => Enumerator.Empty<T>(),
        static value => Enumerator.Single<T>(value),
        static values => Enumerator.ForArray<T>(values));
   

    public bool Equals(T? value)
        => _obj is T myValue && EqualityComparer<T>.Default.Equals(myValue, value!);

    public bool Equals(params T[]? values)
        => _obj is T[] myValues && Sequence.Equal(myValues, values!);

    public bool Equals(Values<T> values)
    {
        // Direct code, no Match
        object? obj = _obj;
        if (obj is null)
        {
            return values._obj is null;
        }
        else if (obj is T value)
        {
            return values._obj is T otherValue &&
                EqualityComparer<T>.Default.Equals(value, otherValue);
        }
        else
        {
            Debug.Assert(obj is T[]);
            var myValues = Notsafe.As<T[]>(obj);
            return values.IsValues(out var otherValues) &&
                Sequence.Equal(myValues, otherValues);
        }
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj switch
        {
            null => _obj is null,
            T value => Equals(value),
            T[] valueArray => Equals(valueArray),
            Values<T> values => Equals(values),
            _ => false,
        };
    }

    public override int GetHashCode()
        => Match(
            static () => Hasher.NullHash,
            static value => Hasher.GetHashCode<T>(value),
            static values => Hasher.Combine<T>(values));

    public override string ToString()
        => Match(
            static () => string.Empty,
            static value => value?.ToString() ?? string.Empty,
            static values => values.Length switch
            {
                0 => string.Empty,
                1 => values[0]?.ToString() ?? string.Empty,
                _ => string.Join<T>(", ", values),
            });
}
