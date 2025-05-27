// CA1716: Identifiers should not match keywords

#pragma warning disable CA1716

// CA1710: Identifiers should have correct suffix
#pragma warning disable CA1710

// CA1000: Do not declare static members on generic types
#pragma warning disable CA1000

using static ScrubJay.SpanDelegates;

namespace ScrubJay.Functional;

/// <summary>
/// An Option represents an optional value, every Option is either:<br/>
/// <see cref="Some"/> and contains a <typeparamref name="T"/> value
/// or <see cref="None"/>, and does not.
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of value associated with a <see cref="Some"/> Option
/// </typeparam>
///  <seealso href="https://doc.rust-lang.org/std/option/index.html"/>
[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly ref struct OptionROS<T> :
    /* All listed interfaces are implemented, but cannot be declared because they may unify for some type parameter substitutions */
    //IEqualityOperators<OptionROS<T>, OptionROS<T>, bool>,
    //IEqualityOperators<OptionROS<T>, None, bool>,
    //IEqualityOperators<OptionROS<T>, T, bool>,
    //IComparisonOperators<OptionROS<T>, OptionROS<T>, bool>,
    //IComparisonOperators<OptionROS<T>, None, bool>,
    //IComparisonOperators<OptionROS<T>, T, bool>,
#if NET9_0_OR_GREATER
    IEquatable<OptionROS<T>>,
    IComparable<OptionROS<T>>,
#endif
    IEquatable<None>,
    //IEquatable<T>,
    IComparable<None>,
    //IComparable<T>,
    IFormattable
{
#region Operators

    /// <summary>
    /// Implicitly convert an <see cref="Option{T}"/> into <c>true</c> if it is Some and <c>false</c> if it is None
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator bool(OptionROS<T> option) => option._isSome;

    /// <summary>
    /// Implicitly convert an <see cref="Option{T}"/> into <c>true</c> if it is Some and <c>false</c> if it is None
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator true(OptionROS<T> option) => option._isSome;

    /// <summary>
    /// Implicitly convert an <see cref="Option{T}"/> into <c>true</c> if it is Some and <c>false</c> if it is None
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator false(OptionROS<T> option) => !option._isSome;

    /// <summary>
    /// Implicitly convert a standalone <see cref="None"/> to an <see cref="Option{T}"/>.<see cref="Option{T}.None"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator OptionROS<T>(None _) => None();

    // We pass equality and comparison down to T values

    public static bool operator ==(OptionROS<T> left, OptionROS<T> right) => left.Equals(right);

    public static bool operator !=(OptionROS<T> left, OptionROS<T> right) => !left.Equals(right);

    public static bool operator >(OptionROS<T> left, OptionROS<T> right) => left.CompareTo(right) > 0;

    public static bool operator >=(OptionROS<T> left, OptionROS<T> right) => left.CompareTo(right) >= 0;

    public static bool operator <(OptionROS<T> left, OptionROS<T> right) => left.CompareTo(right) < 0;

    public static bool operator <=(OptionROS<T> left, OptionROS<T> right) => left.CompareTo(right) <= 0;

    public static bool operator ==(OptionROS<T> option, None _) => option.IsNone();

    public static bool operator !=(OptionROS<T> option, None _) => option._isSome;

    public static bool operator >(OptionROS<T> option, None none) => option.CompareTo(none) > 0;

    public static bool operator >=(OptionROS<T> option, None none) => option.CompareTo(none) >= 0;

    public static bool operator <(OptionROS<T> option, None none) => option.CompareTo(none) < 0;

    public static bool operator <=(OptionROS<T> option, None none) => option.CompareTo(none) <= 0;

    public static bool operator ==(OptionROS<T> option, T? some) => option.Equals(some);

    public static bool operator !=(OptionROS<T> option, T? some) => !option.Equals(some);

    public static bool operator >(OptionROS<T> option, T some) => option.CompareTo(some) > 0;

    public static bool operator >=(OptionROS<T> option, T some) => option.CompareTo(some) >= 0;

    public static bool operator <(OptionROS<T> option, T some) => option.CompareTo(some) < 0;

    public static bool operator <=(OptionROS<T> option, T some) => option.CompareTo(some) <= 0;

#endregion

    /// <summary>
    /// Gets <see cref="Option{T}"/>.None, which represents the lack of a value
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OptionROS<T> None() => default;

    /// <summary>
    /// Get an <see cref="Option{T}"/>.Some containing a <paramref name="value"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OptionROS<T> Some(ReadOnlySpan<T> value) => new(value);


    // Is this Option.Some?
    // if someone does default(Option), this will be false, so default(Option) == None
    private readonly bool _isSome;

    // If this is Option.Some, the value
    private readonly ReadOnlySpan<T> _span;

    // option can only be constructed with None(), Some(), or implicitly
    private OptionROS(ReadOnlySpan<T> value)
    {
        _isSome = true;
        _span = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsNone() => !_isSome;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSome() => _isSome;

    /// <summary>
    /// Does this <see cref="Option{T}"/> contain <see cref="Some"/> value?
    /// </summary>
    /// <param name="value">
    /// If this is <see cref="Some"/>, this will be the contained value<br/>
    /// if this is <see cref="None"/>, it will be <c>default(T)</c>
    /// </param>
    /// <returns>
    /// <c>true</c> and fills <paramref name="value"/> if this is <see cref="Some"/><br/>
    /// <c>false</c> if it is <see cref="None"/>
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSome([MaybeNullWhen(false)] out ReadOnlySpan<T> value)
    {
        if (_isSome)
        {
            value = _span!;
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <seealso href="https://doc.rust-lang.org/std/option/enum.Option.html#method.unwrap"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> SomeOrThrow(string? errorMessage = null)
    {
        if (_isSome)
            return _span!;
        throw new InvalidOperationException(errorMessage ?? $"Option<{typeof(T)}> is None");
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="fallback"></param>
    /// <returns></returns>
    /// <seealso href="https://doc.rust-lang.org/std/option/enum.Option.html#method.unwrap_or"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> SomeOr(ReadOnlySpan<T> fallback)
    {
        if (_isSome)
            return _span!;
        return fallback;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <seealso href="https://doc.rust-lang.org/std/option/enum.Option.html#method.unwrap_or_default"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> SomeOrDefault()
    {
        if (_isSome)
            return _span!;
        return default;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Match(ActionROS<T> onSome, Action onNone)
    {
        if (_isSome)
        {
            onSome(_span!);
        }
        else
        {
            onNone();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Match(ActionROS<T> onSome, Action<None> onNone)
    {
        if (_isSome)
        {
            onSome(_span!);
        }
        else
        {
            onNone(default);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Match<R>(FuncROS<T, R> some, Fn<R> none)
    {
        if (_isSome)
        {
            return some(_span!);
        }
        else
        {
            return none();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public R Match<R>(FuncROS<T, R> some, Fn<None, R> none)
    {
        if (_isSome)
        {
            return some(_span!);
        }
        else
        {
            return none(default);
        }
    }

#region Compare

    /* None always compares as less than any Some */

    public int CompareTo(OptionROS<T> other)
    {
        if (_isSome)
        {
            if (other._isSome)
            {
                // We both are Some, compare our values
                return Sequence.Compare(_span, other._span);
            }
            else // other is none
            {
                return 1; // My Some is greater than their None
            }
        }
        else // this is None
        {
            if (other._isSome)
            {
                return -1; // My None is lesser than their Some
            }
            else
            {
                // None == None
                return 0;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(ReadOnlySpan<T> other)
    {
        if (_isSome)
        {
            return Sequence.Compare(_span!, other!);
        }

        // My None is less than a Some value
        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int CompareTo(None none) =>
        // Some > None, None == None
        _isSome ? 1 : 0;

    public int CompareTo(object? obj)
        => obj switch
        {
            // OptionROS<T> option => CompareTo(option),
            T[] array => CompareTo(array.AsSpan()),
            None none => CompareTo(none),
            _ => 1, // Unknown | Null | None values sort before
        };

#endregion

#region Equality

    public bool Equals(OptionROS<T> other)
    {
        // If I am Some
        if (_isSome)
        {
            // Other has to be Some and our values have to be equal
            return other._isSome && Sequence.Equal(_span!, other._span!);
        }

        // Both of us must be None
        return !other._isSome;
    }

    public bool Equals(ReadOnlySpan<T> value) => _isSome && Sequence.Equal(_span!, value!);

    public bool Equals(None none) => !_isSome;

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj switch
        {
            //OptionROS<T> option => Equals(option),
            T[] array => Equals(array.AsSpan()),
            None none => Equals(none),
            bool isSome => _isSome == isSome,
            _ => false,
        };
    }

    public override int GetHashCode()
    {
        if (_isSome)
        {
            return Hasher.HashMany<T>(_span);
        }
        return Hasher.EmptyHash;
    }

#endregion

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        text format = default,
        IFormatProvider? provider = default)
    {
        if (_isSome)
        {
            var fmt = new TryFormatWriter(destination);
            fmt.Add("Some[");
            var span = _span;
            int len = span.Length;
            if (len > 0)
            {
                fmt.Add(span[0], format, provider);
                for (int i = 1; i < len; i++)
                {
                    fmt.Add(',');
                    fmt.Add(span[i], format, provider);
                }
            }
            fmt.Add(']');
            return fmt.GetResult(out charsWritten);
        }

        // None
        if (destination.Length >= 4)
        {
            Notsafe.Text.CopyBlock("None", destination, 4);
            charsWritten = 4;
            return true;
        }

        charsWritten = 0;
        return false;
    }

    public string ToString(string? format, IFormatProvider? provider = null)
    {
        if (_isSome)
        {
            return TextBuilder.New
                .Append("Some[")
                .EnumerateAndDelimit(_span, (tb,v) => tb.Append(v, format, provider), ',')
                .Append(']')
                .ToStringAndDispose();
        }
        return nameof(None);
    }

    public override string ToString()
    {
        if (_isSome)
        {
            return TextBuilder.New
                .Append("Some[")
                .EnumerateAppendAndDelimit(_span, ',')
                .Append(']')
                .ToStringAndDispose();
        }
        return nameof(None);
    }
}
