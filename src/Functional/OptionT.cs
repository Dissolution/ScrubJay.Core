// CA1716: Identifiers should not match keywords
#pragma warning disable CA1716

// CA1710: Identifiers should have correct suffix
#pragma warning disable CA1710

// CA1000: Do not declare static members on generic types
#pragma warning disable CA1000

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
public readonly struct Option<T> :
/* All listed interfaces are implemented, but cannot be declared because they may unify for some type parameter substitutions */
#if NET7_0_OR_GREATER
    IEqualityOperators<Option<T>, Option<T>, bool>,
    IEqualityOperators<Option<T>, None, bool>,
    //IEqualityOperators<Option<T>, T, bool>,
#endif
    IEquatable<Option<T>>,
    IEquatable<None>,
    //IEquatable<T>,
#if NET7_0_OR_GREATER
    IComparisonOperators<Option<T>, Option<T>, bool>,
    IComparisonOperators<Option<T>, None, bool>,
    //IComparisonOperators<Option<T>, T, bool>,
#endif
    IComparable<Option<T>>,
    IComparable<None>,
    //IComparable<T>,
    IEnumerable<T>,
    IEnumerable,
    IFormattable
{
#region Operators

    /// <summary>
    /// Implicitly convert an <see cref="Option{T}"/> into <c>true</c> if it is Some and <c>false</c> if it is None
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator bool(Option<T> option) => option._isSome;

    /// <summary>
    /// Implicitly convert an <see cref="Option{T}"/> into <c>true</c> if it is Some and <c>false</c> if it is None
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator true(Option<T> option) => option._isSome;

    /// <summary>
    /// Implicitly convert an <see cref="Option{T}"/> into <c>true</c> if it is Some and <c>false</c> if it is None
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator false(Option<T> option) => !option._isSome;


    /// <summary>
    /// Implicitly convert a standalone <typeparamref name="T"/> <paramref name="value"/> to an
    /// <see cref="Option{T}"/>.<see cref="Option{T}.Some"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Option<T>(T value) => Some(value);

    /// <summary>
    /// Implicitly convert a standalone <see cref="Functional.None"/> to an <see cref="Option{T}"/>.<see cref="Option{T}.None"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Option<T>(None _) => None();

    /// <summary>
    /// Implicitly convert a standalone <see cref="Some{TSome}"/> to an <see cref="Option{T}"/>.<see cref="Option{T}.Some"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Option<T>(Some<T> some) => Some(some.Value);


    // We pass equality and comparison down to T values

    public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);
    public static bool operator !=(Option<T> left, Option<T> right) => !left.Equals(right);
    public static bool operator >(Option<T> left, Option<T> right) => left.CompareTo(right) > 0;
    public static bool operator >=(Option<T> left, Option<T> right) => left.CompareTo(right) >= 0;
    public static bool operator <(Option<T> left, Option<T> right) => left.CompareTo(right) < 0;
    public static bool operator <=(Option<T> left, Option<T> right) => left.CompareTo(right) <= 0;

    public static bool operator ==(Option<T> option, None _) => option.IsNone;
    public static bool operator !=(Option<T> option, None _) => option.IsSome;
    public static bool operator >(Option<T> option, None none) => option.CompareTo(none) > 0;
    public static bool operator >=(Option<T> option, None none) => option.CompareTo(none) >= 0;
    public static bool operator <(Option<T> option, None none) => option.CompareTo(none) < 0;
    public static bool operator <=(Option<T> option, None none) => option.CompareTo(none) <= 0;

    public static bool operator ==(Option<T> option, T? some) => option.Equals(some);
    public static bool operator !=(Option<T> option, T? some) => !option.Equals(some);
    public static bool operator >(Option<T> option, T some) => option.CompareTo(some) > 0;
    public static bool operator >=(Option<T> option, T some) => option.CompareTo(some) >= 0;
    public static bool operator <(Option<T> option, T some) => option.CompareTo(some) < 0;
    public static bool operator <=(Option<T> option, T some) => option.CompareTo(some) <= 0;

#endregion

    /// <summary>
    /// Gets <see cref="Option{T}"/>.None, which represents the lack of a value
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> None() => default;

    /// <summary>
    /// Get an <see cref="Option{T}"/>.Some containing a <paramref name="value"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Some(T value) => new(value);


    // Is this Option.Some?
    // if someone does default(Option), this will be false, so default(Option) == None
    private readonly bool _isSome;

    // If this is Option.Some, the value
    private readonly T? _value;

    // option can only be constructed with None(), Some(), or implicitly
    private Option(T value)
    {
        _isSome = true;
        _value = value;
    }

    /// <summary>
    /// Does this <see cref="Option{T}"/> contain <see cref="Some"/> value?
    /// </summary>
    /// <returns>
    /// <c>true</c> if this is Some, <c>false</c> if it is None
    /// </returns>
    /// <seealso href="https://doc.rust-lang.org/std/option/enum.Option.html#method.is_some"/>
    public bool IsSome
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _isSome;
    }

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
    public bool HasSome([MaybeNullWhen(false)] out T value)
    {
        if (_isSome)
        {
            value = _value!;
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    /// <seealso href="https://doc.rust-lang.org/std/option/enum.Option.html#method.is_some_and"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSomeAnd(Fun<T, bool> predicate) => _isSome && predicate(_value!);


    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <seealso href="https://doc.rust-lang.org/std/option/enum.Option.html#method.is_none"/>
    public bool IsNone
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !_isSome;
    }


    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <seealso href="https://doc.rust-lang.org/std/option/enum.Option.html#method.unwrap"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T SomeOrThrow(string? errorMessage = null)
    {
        if (_isSome)
            return _value!;
        throw new InvalidOperationException(errorMessage ?? $"Option<{typeof(T)}> is None");
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <seealso href="https://doc.rust-lang.org/std/option/enum.Option.html#method.unwrap_or"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T SomeOr(T value)
    {
        if (_isSome)
            return _value!;
        return value;
    }


    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <seealso href="https://doc.rust-lang.org/std/option/enum.Option.html#method.unwrap_or_default"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T? SomeOrDefault()
    {
        if (_isSome)
            return _value!;
        return default;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="getValue"></param>
    /// <returns></returns>
    /// <seealso href="https://doc.rust-lang.org/std/option/enum.Option.html#method.unwrap_or_else"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T SomeOrElse(Fun<T> getValue)
    {
        if (_isSome)
            return _value!;
        return getValue();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="map"></param>
    /// <param name="defaultValue"></param>
    /// <typeparam name="TNew"></typeparam>
    /// <returns></returns>
    /// <seealso href="https://doc.rust-lang.org/std/option/enum.Option.html#method.map_or"/>
    public TNew SelectOr<TNew>(Fun<T, TNew> map, TNew defaultValue)
    {
        if (HasSome(out var value))
        {
            return map(value);
        }

        return defaultValue;
    }

    public TNew? SelectOrDefault<TNew>(Fun<T, TNew> map)
    {
        if (HasSome(out var value))
        {
            return map(value);
        }

        return default(TNew);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="map"></param>
    /// <param name="getDefaultValue"></param>
    /// <typeparam name="TNew"></typeparam>
    /// <returns></returns>
    /// <seealso href="https://doc.rust-lang.org/std/option/enum.Option.html#method.map_or_else"/>
    public TNew SelectOrElse<TNew>(Fun<T, TNew> map, Fun<TNew> getDefaultValue)
    {
        if (HasSome(out var value))
        {
            return map(value);
        }

        return getDefaultValue();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Match(Act<T> onSome, Act onNone)
    {
        if (_isSome)
        {
            onSome(_value!);
        }
        else
        {
            onNone();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Match(Act<T> onSome, Act<None> onNone)
    {
        if (_isSome)
        {
            onSome(_value!);
        }
        else
        {
            onNone(default);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Match<TResult>(Fun<T, TResult> some, Fun<TResult> none)
    {
        if (_isSome)
        {
            return some(_value!);
        }
        else
        {
            return none();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Match<TResult>(Fun<T, TResult> some, Fun<None, TResult> none)
    {
        if (_isSome)
        {
            return some(_value!);
        }
        else
        {
            return none(default);
        }
    }

    public Result<T, Exception> AsResult(string? errorMessage = null)
    {
        if (_isSome)
            return _value!;
        return new InvalidOperationException(errorMessage ?? $"Option<{typeof(T)}> is None");
    }

#region Compare

    /* None always compares as less than any Some */

    public int CompareTo(Option<T> other)
    {
        if (_isSome)
        {
            if (other._isSome)
            {
                // We both are Some, compare our values
                return Comparer<T>.Default.Compare(_value!, other._value!);
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
    public int CompareTo(T? other)
    {
        if (_isSome)
            return Comparer<T>.Default.Compare(_value!, other!);

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
            Option<T> option => CompareTo(option),
            T some => CompareTo(some),
            None none => CompareTo(none),
            _ => 1, // Unknown | Null | None values sort before
        };

#endregion

#region Equals

    public bool Equals(Option<T> other)
    {
        // If I am Some
        if (_isSome)
        {
            // Other has to be Some and our values have to be equal
            return other._isSome && EqualityComparer<T>.Default.Equals(_value!, other._value!);
        }

        // Both of us must be None
        return !other._isSome;
    }

    public bool Equals(T? value) => _isSome && EqualityComparer<T>.Default.Equals(_value!, value!);

    public bool Equals(None none) => !_isSome;

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj switch
        {
            Option<T> option => Equals(option),
            T value => Equals(value),
            None none => Equals(none),
            bool isSome => _isSome == isSome,
            _ => false,
        };
    }

#endregion

#region LINQ + IEnumerable

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<TNew> Select<TNew>(Fun<T, TNew> selector)
    {
        if (_isSome)
            return Some<TNew>(selector(_value!));
        return None<TNew>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<TNew> SelectMany<TNew>(Fun<T, Option<TNew>> newSelector)
    {
        if (_isSome)
        {
            return newSelector(_value!);
        }

        return None<TNew>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<TNew> SelectMany<TKey, TNew>(
        Fun<T, Option<TKey>> keySelector,
        Fun<T, TKey, TNew> newSelector)
    {
        if (_isSome && keySelector(_value!).HasSome(out var key))
        {
            return Some<TNew>(newSelector(_value!, key));
        }

        return None<TNew>();
    }

    /// <summary>
    /// Returns <see cref="None"/> if this <see cref="Option{T}"/> is <see cref="None"/>,<br/>
    /// otherwise calls <paramref name="predicate"/> with the wrapped value and returns:<br/>
    /// <see cref="Some"/> if <paramref name="predicate"/> returns <c>true</c> (with the wrapped value),<br/>
    /// and <see cref="None"/> if <paramref name="predicate"/> returns <c>false</c><br/>
    /// This function works similar to <c>Enumerable.Where</c><br/>
    /// You can imagine this <see cref="Option{T}"/> being an iterator over one or zero elements<br/>
    /// <see cref="Where"/> lets you decide which elements to keep<br/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    /// <seealso href="https://doc.rust-lang.org/std/option/enum.Option.html#method.filter"/>
    public Option<T> Where(Fun<T, bool> predicate)
    {
        if (HasSome(out var value) && predicate(value))
            return Some<T>(_value!);
        return None();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    /// <seealso href="https://doc.rust-lang.org/std/option/enum.Option.html#method.iter"/>
    [MustDisposeResource(false)]
    public OptionEnumerator GetEnumerator() => new OptionEnumerator(this);

    [MustDisposeResource(false)]
    [StructLayout(LayoutKind.Auto)]
    public struct OptionEnumerator : IEnumerator<T>, IEnumerator, IDisposable
    {
        private bool _canYield;
        private readonly T _value;

        object? IEnumerator.Current => _value;
        public T Current => _value;

        public OptionEnumerator(Option<T> option)
        {
            if (option._isSome)
            {
                _value = option._value!;
                _canYield = true;
            }
            else
            {
                _value = default!;
                _canYield = false;
            }
        }

        public bool MoveNext()
        {
            if (!_canYield)
                return false;
            _canYield = false;
            return true;
        }

        void IEnumerator.Reset() => throw new NotSupportedException();

        void IDisposable.Dispose()
        {
            // Do nothing
        }
    }

#endregion

    public override int GetHashCode()
    {
        if (_isSome)
            return Hasher.GetHashCode<T>(_value);
        return Hasher.EmptyHash;
    }

    public string ToString(string? format, IFormatProvider? formatProvider = null)
    {
        if (_isSome)
        {
            DefaultInterpolatedStringHandler text = new(6, 1, formatProvider);
            text.AppendLiteral("Some(");
            text.AppendFormatted<T>(_value!, format);
            text.AppendLiteral(")");
            return text.ToStringAndClear();
        }

        return nameof(None);
    }

    public override string ToString() => _isSome ? $"Some({_value})" : nameof(None);
}
