// CA1716: Identifiers should not match keywords

using ScrubJay.Functional.IMPL;

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
/// <remarks>
/// All <c>None</c> values are equal, <c>Some</c> values equate and compare the contained values,
/// <c>None</c> compares less than <c>Some</c>.
/// </remarks>
[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly struct Option<T> :
    /* All listed interfaces are implemented,
     * but cannot be declared because they may unify for some type parameter substitutions
     */
#if NET7_0_OR_GREATER
    IEqualityOperators<Option<T>, Option<T>, bool>,
    IEqualityOperators<Option<T>, None, bool>,
    //IEqualityOperators<Option<T>, T, bool>,
    IComparisonOperators<Option<T>, Option<T>, bool>,
    IComparisonOperators<Option<T>, None, bool>,
    //IComparisonOperators<Option<T>, T, bool>,
#endif
    IEquatable<Option<T>>,
    IEquatable<None>,
    //IEquatable<T>,
    IComparable<Option<T>>,
    IComparable<None>,
    //IComparable<T>,
    IEnumerable<T>,
#if NET6_0_OR_GREATER
    ISpanFormattable,
#endif
    IFormattable
{
#region Operators

    public static implicit operator bool(Option<T> option) => option._isSome;
    public static implicit operator Option<T>(None _) => None;

    public static bool operator true(Option<T> option) => option._isSome;
    public static bool operator false(Option<T> option) => !option._isSome;


    public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);
    public static bool operator !=(Option<T> left, Option<T> right) => !left.Equals(right);
    public static bool operator >(Option<T> left, Option<T> right) => left.CompareTo(right) > 0;
    public static bool operator >=(Option<T> left, Option<T> right) => left.CompareTo(right) >= 0;
    public static bool operator <(Option<T> left, Option<T> right) => left.CompareTo(right) < 0;
    public static bool operator <=(Option<T> left, Option<T> right) => left.CompareTo(right) <= 0;

    public static bool operator ==(Option<T> option, None _) => option.IsNone();
    public static bool operator !=(Option<T> option, None _) => option._isSome;
    public static bool operator >(Option<T> option, None none) => option.CompareTo(none) > 0;
    public static bool operator >=(Option<T> option, None none) => option.CompareTo(none) >= 0;
    public static bool operator <(Option<T> option, None none) => option.CompareTo(none) < 0;
    public static bool operator <=(Option<T> option, None none) => option.CompareTo(none) <= 0;

    public static bool operator ==(Option<T> option, T some) => option.Equals(some);
    public static bool operator !=(Option<T> option, T some) => !option.Equals(some);
    public static bool operator >(Option<T> option, T some) => option.CompareTo(some) > 0;
    public static bool operator >=(Option<T> option, T some) => option.CompareTo(some) >= 0;
    public static bool operator <(Option<T> option, T some) => option.CompareTo(some) < 0;
    public static bool operator <=(Option<T> option, T some) => option.CompareTo(some) <= 0;

#endregion

    /// <summary>
    /// Gets <see cref="Option{T}"/>.None, which represents the lack of a value
    /// </summary>
    public static readonly Option<T> None;

    /// <summary>
    /// Get an <see cref="Option{T}"/>.Some containing a <paramref name="value"/>
    /// </summary>
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

    public bool IsNone() => !_isSome;

#region Some-ness

    public bool IsSome() => _isSome;

    public bool IsSome([MaybeNullWhen(false)] out T value)
    {
        if (_isSome)
        {
            value = _value!;
            return true;
        }

        value = default;
        return false;
    }

    public bool IsSomeAnd(Func<T, bool> predicate) => _isSome && predicate(_value!);

    public T SomeOr(T fallback)
    {
        if (_isSome)
            return _value!;
        return fallback;
    }

    public T SomeOr(Func<T> getFallback)
    {
        if (_isSome)
            return _value!;
        return getFallback();
    }

    public T? SomeOrDefault()
    {
        if (_isSome)
            return _value!;
        return default;
    }

    public T SomeOrThrow(string? errorMessage = null)
    {
        if (_isSome)
            return _value!;
        throw new InvalidOperationException(errorMessage ?? $"{ToString()} is not Some");
    }


#endregion

#region Match

    public void Match(Action<T> onSome, Action onNone)
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


    public void Match(Action<T> onSome, Action<None> onNone)
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


    public R Match<R>(Func<T, R> some, Func<R> none)
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


    public R Match<R>(Func<T, R> some, Func<None, R> none)
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

#endregion

#region Compare

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

    public int CompareTo(Option<T> other, IComparer<T>? comparer)
    {
        if (_isSome)
        {
            if (other._isSome)
            {
                // We both are Some, compare our values
                return (comparer ?? Comparer<T>.Default).Compare(_value!, other._value!);
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

    public int CompareTo(T? other)
    {
        if (_isSome)
        {
            return Comparer<T>.Default.Compare(_value!, other!);
        }

        // My None is less than a Some value
        return -1;
    }

    public int CompareTo(T? other, IComparer<T>? comparer)
    {
        if (_isSome)
        {
            return (comparer ?? Comparer<T>.Default).Compare(_value!, other!);
        }

        // My None is less than a Some value
        return -1;
    }


    public int CompareTo(None none) =>
        // Some > None, None == None
        _isSome ? 1 : 0;

    public int CompareTo(object? obj)
        => obj switch
        {
            Option<T> option => CompareTo(option),
            T some => CompareTo(some),
            None none => CompareTo(none),
            _ => 1, // unknown values are always less
        };

#endregion

#region Equality

    public bool Equals(Option<T> other)
    {
        if (_isSome)
        {
            return other._isSome && EqualityComparer<T>.Default.Equals(_value!, other._value!);
        }

        return !other._isSome;
    }

    public bool Equals(Option<T> other, IEqualityComparer<T>? comparer)
    {
        if (_isSome)
        {
            return other._isSome && (comparer ?? EqualityComparer<T>.Default).Equals(_value!, other._value!);
        }

        return !other._isSome;
    }

    public bool Equals(T? value)
    {
        if (_isSome)
        {
            return EqualityComparer<T>.Default.Equals(_value!, value!);
        }

        return false;
    }

    public bool Equals(T? value, IEqualityComparer<T>? comparer)
    {
        if (_isSome)
        {
            return (comparer ?? EqualityComparer<T>.Default).Equals(_value!, value!);
        }

        return false;
    }

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

    public override int GetHashCode()
    {
        if (_isSome)
        {
            if (_value is not null)
            {
                return _value.GetHashCode();
            }

            return 0;
        }

        return -1;
    }

#endregion

#region LINQ + IEnumerable

    public Option<N> Select<N>(Func<T, N> selector)
    {
        if (_isSome)
            return Some<N>(selector(_value!));
        return Option<N>.None;
    }

    public Option<N> SelectMany<N>(Func<T, Option<N>> newSelector)
    {
        if (_isSome)
        {
            return newSelector(_value!);
        }

        return Option<N>.None;
    }

    public Option<N> SelectMany<K, N>(
        Func<T, K> keySelector,
        Func<T, K, N> newSelector)
    {
        if (_isSome)
        {
            var key = keySelector(_value!);
            var newValue = newSelector(_value!, key);
            return Some<N>(newValue);
        }

        return Option<N>.None;
    }

    public Option<N> SelectMany<K, N>(
        Func<T, Option<K>> keySelector,
        Func<T, K, N> newSelector)
    {
        if (_isSome)
        {
            var key = keySelector(_value!);
            if (key.IsSome(out var k))
            {
                var newValue = newSelector(_value!, k);
                return Some<N>(newValue);
            }
        }

        return Option<N>.None;
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
    public Option<T> Where(Func<T, bool> predicate)
    {
        if (_isSome)
        {
            if (predicate(_value!))
            {
                return this;
            }
        }

        return None;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    [MustDisposeResource(false)]
    public OptionEnumerator GetEnumerator() => new OptionEnumerator(this);

    [PublicAPI]
    [MustDisposeResource(false)]
    public struct OptionEnumerator : IEnumerator<T>, IEnumerator, IDisposable
    {
        private readonly Option<T> _option;
        private bool _canYield;

        readonly object? IEnumerator.Current => _option.SomeOrThrow();
        public readonly T Current => _option.SomeOrThrow();

        public OptionEnumerator(Option<T> option)
        {
            _option = option;
            _canYield = option._isSome;
        }

        public bool MoveNext()
        {
            if (!_canYield)
            {
                return false;
            }
            else
            {
                _canYield = false;
                return true;
            }
        }

        public void Reset()
        {
            _canYield = _option._isSome;
        }

        readonly void IDisposable.Dispose()
        {
            /* Do Nothing */
        }
    }

#endregion

#region Formatting

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format = default,
        IFormatProvider? provider = null)
    {
        // todo: Make this more efficient
        string fmt = ToString(format.ToString(), provider);
        if (fmt.TryCopyTo(destination))
        {
            charsWritten = fmt.Length;
            return true;
        }

        charsWritten = 0;
        return false;
    }

    public string ToString(string? format, IFormatProvider? provider = null)
    {
        if (_isSome)
        {
            string? str;
            if (_value is IFormattable)
            {
                str = ((IFormattable)_value).ToString(format, provider);
            }
            else
            {
                str = _value?.ToString();
            }

            return $"Option<{typeof(T).Name}>.Some({str})";
        }

        return $"Option<{typeof(T).Name}>.None";
    }

    public override string ToString()
    {
        if (_isSome)
        {
            return $"Option<{typeof(T)}>.Some({_value})";
        }

        return $"Option<{typeof(T)}>.None";
    }

#endregion
}