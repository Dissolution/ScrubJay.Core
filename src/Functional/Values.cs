// Exception to Identifiers Require Correct Suffix

#pragma warning disable CA1710

namespace ScrubJay.Functional;

/// <summary>
/// A small, immutable collection of zero or more <typeparamref name="T"/> values
/// </summary>
/// <typeparam name="T"></typeparam>
[PublicAPI]
public readonly struct Values<T> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Values<T>, Values<T>, bool>,
    IEqualityOperators<Values<T>, T, bool>,
    IEqualityOperators<Values<T>, T[], bool>,
    IAdditionOperators<Values<T>, Values<T>, Values<T>>,
    IAdditionOperators<Values<T>, T[], Values<T>>,
    IAdditionOperators<Values<T>, T, Values<T>>,
#endif
#if NET6_0_OR_GREATER
    ISpanFormattable,
#endif
    IReadOnlyList<T>,
    IReadOnlyCollection<T>,
    IEnumerable<T>,
    IEquatable<Values<T>>,
    IEquatable<T[]>,
    IEquatable<T>,
    IFormattable
{
    public static implicit operator Values<T>(Unit _) => new();

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

    public static Values<T> operator +(Values<T> left, Values<T> right)
        => left.With(right);

    public static Values<T> operator +(Values<T> left, T[] right)
        => left.With(right);

    public static Values<T> operator +(Values<T> left, T right)
        => left.With(right);


    /// <summary>
    /// Returns an empty <see cref="Values{T}"/>
    /// </summary>
    public static readonly Values<T> Empty;

    public static Values<T> Create() => Empty;

    public static Values<T> Create(T value) => new(value);

    public static Values<T> Create(params T[]? values)
    {
        if (values is null)
            return Empty;

        return values.Length switch
        {
            0 => Empty,
            1 => new(values[0]),
            _ => new(values),
        };
    }

    public static Values<T> Create(IEnumerable<T>? values)
    {
        if (values is null)
            return Empty;

        if (values is ICollection<T> collection)
        {
            var array = new T[collection.Count];
            collection.CopyTo(array, 0);
            return array.Length switch
            {
                0 => Empty,
                1 => new(array[0]),
                _ => new(array),
            };
        }

        using var e = values.GetEnumerator();
        if (!e.MoveNext())
            return Empty;

        T value = e.Current;
        if (!e.MoveNext())
            return new(value);

        using var buffer = new Buffer<T>();
        buffer.Add(value);
        buffer.Add(e.Current);
        while (e.MoveNext())
        {
            buffer.Add(e.Current);
        }
        return new(buffer.ToArray());
    }

    /// <summary>
    /// Boxed value(s)
    /// </summary>
    private readonly object? _obj;

    public T this[int index] => TryGetValueAt(index).OkOrThrow();

    /// <summary>
    /// Get the number of stored values
    /// </summary>
    public int Count => Match(static () => 0, static _ => 1, static values => values.Length);

    public bool IsEmpty => _obj is null;

    public bool IsValue => _obj is T;

    public bool IsValues => _obj is T[];


    private Values(T value)
    {
        _obj = value;
    }

    private Values(params T[] values)
    {
        Debug.Assert(values is not null);
        Debug.Assert(values!.Length > 1);
        _obj = values;
    }

    public bool TryGetValue([MaybeNullWhen(false)] out T value) => _obj.Is(out value);

    public bool TryGetValues([MaybeNullWhen(false)] out T[] values) => _obj.Is(out values);

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
    public Result<T> TryGetValueAt(Index index) => Match(
        onEmpty: static () => new InvalidOperationException("Values is empty"),
        onValue: val => Validate.Index(index, 1).Select(_ => val),
        onValues: vals => Validate.Index(index, vals.Length).Select(i => vals[i])
    );


    public bool Contains(T value) => Match(
        static () => false,
        val => Equate.Values(val, value),
        vals => Array.IndexOf<T>(vals, value) >= 0);

    public bool Contains(T value, IEqualityComparer<T>? itemComparer) => Match(
        static () => false,
        val => Equate.Values(value, val, itemComparer),
        vals => Sequence.Contains(vals, value, itemComparer));

    public bool Contains(T[] values) => Match(
        static () => false,
        val => (values.Length == 1) && Equate.Values(val, values[0]),
        vals => Sequence.Contains<T>(vals, values));

    public Values<T> With() => this;

    public Values<T> With(T value)
    {
        if (_obj is null)
            return new(value);

        if (_obj is T val)
            return new(val, value);

        T[] vals = Notsafe.As<object, T[]>(_obj);
        return new([..vals, value,]);
    }

    public Values<T> With(params T[]? values)
        => With(Create(values));

    public Values<T> With(IEnumerable<T>? values)
        => With(Create(values));

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
                return new Values<T>(thisVal, otherVal);
            }
            else
            {
                Debug.Assert(otherObj is T[]);
                return new Values<T>([thisVal, ..(T[])otherObj!, ]);
            }
        }
        else
        {
            Debug.Assert(thisObj is T[]);
            if (otherObj is T otherVal)
            {
                return new Values<T>([..(T[])thisObj, otherVal, ]);
            }
            else
            {
                Debug.Assert(otherObj is T[]);
                return new Values<T>([..(T[])thisObj, ..(T[])otherObj!, ]);
            }
        }
    }


    internal void FastCopyTo(Span<T> span)
    {
        object? obj = _obj;
        if (obj is null) { }
        else if (obj is T value)
        {
            span[0] = value;
        }
        else
        {
            Debug.Assert(obj is T[]);
            T[] values = Notsafe.As<object, T[]>(obj);
            Sequence.CopyTo(values, span);
        }
    }

    public Result<int> TryCopyTo(Span<T> span)
    {
        object? obj = _obj;

        if (obj is null)
            return Ok(0);

        if (obj is T value)
        {
            if (span.Length == 0)
                return new ArgumentException(null, nameof(span));
            span[0] = value;
            return Ok(1);
        }

        var values = Notsafe.As<object, T[]>(obj);
        if (span.Length < values.Length)
            return new ArgumentException(null, nameof(span));
        Sequence.CopyTo(values, span);
        return Ok(values.Length);
    }

    public ReadOnlySpan<T> AsSpan()
    {
        object? obj = _obj;
        if (obj is null)
            return [];
        if (obj is T)
            return Notsafe.AsReadOnlySpan<T>(obj);
        Debug.Assert(obj is T[]);
        return new(Notsafe.As<object, T[]>(obj));
    }

    public T[] ToArray() => Match(static () => [], static value => [value, ], static values => values);


    public void Match(Action onEmpty, Action<T> onValue, Action<T[]> onValues)
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
            onValues(Notsafe.As<object, T[]>(obj));
        }
    }

    public R Match<R>(Fn<R> onEmpty, Fn<T, R> onValue, Fn<T[], R> onValues)
    {
        object? obj = _obj;
        if (obj == null)
            return onEmpty();
        if (obj is T value)
            return onValue(value);
        Debug.Assert(obj is T[]);
        return onValues(Notsafe.As<object, T[]>(obj));
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<T> GetEnumerator() => Match(
        static () => Enumerator.Empty<T>(),
        static value => Enumerator.One<T>(value),
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
            var myValues = Notsafe.As<object, T[]>(obj);
            return values.TryGetValues(out var otherValues) &&
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
            static value => Hasher.Hash<T>(value),
            static values => Hasher.HashMany<T>(values));

    public string ToString(string? format, IFormatProvider? provider = null)
    {
        return TextBuilder.New
            .Append('[')
            .IfFormat(_obj.Is<T>(), format, provider)
            .If(_obj.Is<T[]>(), (tb, vals) => tb.EnumerateAndDelimit(vals,
                (t, value) => t.Format(value, format, provider),
                ", "))
            .Append(']')
            .ToStringAndDispose();
    }

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        text format = default,
        IFormatProvider? provider = null)
    {
        var writer = new TryFormatWriter(destination);
        writer.Add('[');
        if (_obj is T value)
        {
            writer.Add(value, format, provider);
        }
        else if (_obj is T[] values)
        {
            int len = values.Length;
            Debug.Assert(len > 1);
            writer.Add(values[0], format, provider);
            for (int i = 1; i < len; i++)
            {
                writer.Add(',');
                writer.Add(values[i], format, provider);
            }
        }
        writer.Add(']');
        return writer.Wrote(out charsWritten);
    }

    public override string ToString() => ToString(null);
}
