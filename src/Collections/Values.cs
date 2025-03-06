// Exception to Identifiers Require Correct Suffix
#pragma warning disable CA1710

namespace ScrubJay.Collections;

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

    public T this[int index] => TryGetAt(index).OkOrThrow();

    /// <summary>
    /// Get the number of stored values
    /// </summary>
    public int Count => Match(static () => 0, static _ => 1, static values => values.Length);

    public bool IsEmpty => _obj is null;
    public bool IsValue => _obj is T;
    public bool IsValues => _obj is T[];

    private Values(T[] array, byte _)
    {
        _obj = array;
    }

    public Values()
    {
        _obj = null;
    }

    public Values(T value)
    {
        _obj = value;
    }

    public Values(params T[]? values)
    {
        if (values is null)
        {
            _obj = null;
        }
        else
        {
            _obj = values.Length switch
            {
                0 => null,
                1 => values[0],
                _ => values,
            };
        }
    }

    public Values(IEnumerable<T>? values)
    {
        if (values is null)
        {
            _obj = null;
        }
        else if (values is ICollection<T> collection)
        {
            var array = new T[collection.Count];
            collection.CopyTo(array, 0);
            _obj = array.Length switch
            {
                0 => null,
                1 => array[0],
                _ => array,
            };
        }
        else
        {
            using var e = values.GetEnumerator();
            if (!e.MoveNext())
            {
                _obj = null;
            }
            else
            {
                T value = e.Current;
                if (!e.MoveNext())
                {
                    _obj = value;
                }
                else
                {
                    using var buffer = new Buffer<T>();
                    buffer.Add(value);
                    buffer.Add(e.Current);
                    while (e.MoveNext())
                        buffer.Add(e.Current);
                    _obj = buffer.ToArray();
                }
            }
        }
    }


    public bool HasValue([MaybeNullWhen(false)] out T value) => _obj.Is(out value);

    public bool HasValues([MaybeNullWhen(false)] out T[] values) => _obj.Is(out values);

    public bool Contains(T value) => Match(
        static () => false,
        val => Equate.Values(val, value),
        vals => Array.IndexOf<T>(vals, value) >= 0);

    public bool Contains(T value, IEqualityComparer<T>? itemComparer) => Match(
        static () => false,
        val => Equate.Values(value, val, itemComparer),
        vals => Sequence.Contains(vals, value, itemComparer));


    /// <summary>
    /// Tries to get the value at the given <see cref="Index"/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    /// <remarks>
    /// An <see cref="Empty"/> has no values at any index<br/>
    /// A <typeparamref name="T"/> Value exists only at index 0<br/>
    /// <c>T[]</c> Values are indexed as a standard <see cref="Array"/>
    /// </remarks>
    public Result<T, Exception> TryGetAt(Index index) => Match(
        onEmpty: static () => new InvalidOperationException("Values is empty"),
        onValue: val => Validate.Index(index, 1).Select(_ => val),
        onValues: vals => Validate.Index(index, vals.Length).Select(i => vals[i])
    );

    public Values<T> With(Values<T> values)
    {
        object? thisObj = _obj;
        object? otherObj = values._obj;

        if (thisObj is null)
            return values;
        if (otherObj is null)
            return this;

        if (thisObj is T thisVal)
        {
            if (otherObj is T otherVal)
            {
                return new Values<T>([thisVal, otherVal], default);
            }
            else
            {
                Debug.Assert(otherObj is T[]);
                return new Values<T>([thisVal, ..(T[])otherObj!], default);
            }
        }
        else
        {
            Debug.Assert(thisObj is T[]);
            if (otherObj is T otherVal)
            {
                return new Values<T>([..(T[])thisObj, otherVal], default);
            }
            else
            {
                Debug.Assert(otherObj is T[]);
                return new Values<T>([..(T[])thisObj, ..(T[])otherObj!], default);
            }
        }
    }

    public Values<T> With(T value) => With(new Values<T>(value));

    public Values<T> WithMany(T[] values) => With(new Values<T>(values));
    public Values<T> WithMany(IEnumerable<T> values) => With(new Values<T>(values));

    internal void FastCopyTo(Span<T> span)
    {
        object? obj = _obj;
        if (obj is null)
        {

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
            return values.HasValues(out var otherValues) &&
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
