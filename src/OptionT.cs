// ReSharper disable InconsistentNaming

namespace ScrubJay;

// <a href="https://doc.rust-lang.org/std/option/index.html">Rust's Option</a>

/// <summary>
/// An Option represents an optional value, every Option is either:<br/>
/// <see cref="Some"/> and contains a <typeparamref name="T"/> value
/// or <see cref="None"/>, and does not.
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of value associated with a <see cref="Some"/> Option
/// </typeparam>
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
    IEnumerable
{
#region Operators
    // Treat Option like a boolean: some == true, none == false
    // Allow us to cast from None or a T value directly to an Option
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator bool(Option<T> option) => option._isSome;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Option<T>(None _) => None;

    public static bool operator true(Option<T> option) => option._isSome;
    public static bool operator false(Option<T> option) => !option._isSome;

    // We pass equality and comparison down to T values
    
    public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);
    public static bool operator !=(Option<T> left, Option<T> right) => !left.Equals(right);
    public static bool operator >(Option<T> left, Option<T> right) => left.CompareTo(right) > 0;
    public static bool operator >=(Option<T> left, Option<T> right) => left.CompareTo(right) >= 0;
    public static bool operator <(Option<T> left, Option<T> right) => left.CompareTo(right) < 0;
    public static bool operator <=(Option<T> left, Option<T> right) => left.CompareTo(right) <= 0;

    public static bool operator ==(Option<T> option, None none) => option.IsNone();
    public static bool operator !=(Option<T> option, None none) => option.IsSome();
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
    /// Gets the None option
    /// </summary>
    public static readonly Option<T> None; // == default == default(None) == None.Default
    
    /// <summary>
    /// Creates a new <see cref="Option{T}"/>.Some containing <paramref name="value"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Some(T value) => new(value);

    
    // Is this Option.Some?
    // if someone does default(Option), this will be false, so default(Option) == None
    private readonly bool _isSome;
    // If this is Option.Some, the value
    private readonly T? _value;

    private Option(T value)
    {
        _isSome = true;
        _value = value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.is_some"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSome() => _isSome;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.is_some_and"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSomeAnd(Func<T, bool> predicate) => _isSome && predicate(_value!);


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.is_none"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsNone() => !_isSome;


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.unwrap"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T SomeOrThrow(string? errorMessage = null)
    {
        if (_isSome)
            return _value!;
        throw new InvalidOperationException(errorMessage ?? "This option is not some");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.unwrap_or"/>
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
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.unwrap_or_default"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T? SomeOrDefault()
    {
        if (_isSome)
                return _value!;
        return default(T);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="getValue"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.unwrap_or_else"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T SomeOrElse(Func<T> getValue)
    {
        if (_isSome)
            return _value!;
        return getValue();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="err"></param>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.ok_or"/>
    public Result<T, TError> OkOr<TError>(TError err)
    {
        if (_isSome)
            return Result<T, TError>.Ok(_value!);
        return Result<T, TError>.Error(err);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="getErr"></param>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.ok_or_else"/>
    public Result<T, TError> OkOrElse<TError>(Func<TError> getErr)
    {
        if (_isSome)
            return Result<T, TError>.Ok(_value!);
        return Result<T, TError>.Error(getErr());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.filter"/>
    public Option<T> Filter(Func<T, bool> predicate)
    {
        if (IsSome(out var value) && predicate(value))
            return this;
        return None;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>
    /// <typeparam name="TNew"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.map"/>
    public Option<TNew> Map<TNew>(Func<T, TNew> map)
    {
        if (IsSome(out var value))
        {
            return Some<TNew>(map(value));
        }

        return Option<TNew>.None;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>
    /// <param name="defaultValue"></param>
    /// <typeparam name="TNew"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.map_or"/>
    public TNew MapOr<TNew>(Func<T, TNew> map, TNew defaultValue)
    {
        if (IsSome(out var value))
        {
            return map(value);
        }

        return defaultValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>
    /// <param name="getDefaultValue"></param>
    /// <typeparam name="TNew"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.map_or_else"/>
    public TNew MapOrElse<TNew>(Func<T, TNew> map, Func<TNew> getDefaultValue)
    {
        if (IsSome(out var value))
        {
            return map(value);
        }

        return getDefaultValue();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none)
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
    public TResult Match<TResult>(Func<T, TResult> some, Func<None, TResult> none)
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

    public Result<T, Exception> AsResult()
    {
        if (_isSome)
            return _value!;
        return new InvalidOperationException("None");
    }

#region Compare

    public int CompareTo(Option<T> other)
    {
        // None compares as less than any Some
        if (_isSome)
        {
            if (other._isSome)
            {
                return Comparer<T>.Default.Compare(_value!, other._value!);
            }
            else // y is none
            {
                return 1; // I am greater
            }
        }
        else // x is none
        {
            if (other._isSome)
            {
                return -1; // I am lesser
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
        else
        {
            // None compares as less than any Some
            return -1;
        }
    }

    public int CompareTo(None none)
    {
        // None compares as less than any Some
        if (_isSome)
        {
            return 1; // I am greater
        }
        else // x is none
        {
            // None == None
            return 0;
        }
    }

    public int CompareTo(object? obj)
    {
        return obj switch
        {
            Option<T> option => CompareTo(option),
            T some => CompareTo(some),
            None none => CompareTo(none),
            _ => 1, // unknown values sort before
        };
    }

#endregion

#region Equals

    public bool Equals(Option<T> other)
    {
        if (_isSome)
        {
            if (other._isSome)
            {
                return EqualityComparer<T>.Default.Equals(_value!, other._value!);
            }
            else // y is none
            {
                return false;
            }
        }
        else // x is none
        {
            if (other._isSome)
            {
                return false;
            }
            else
            {
                // None == None
                return true;
            }
        }
    }

    public bool Equals(T? value)
    {
        return _isSome && EqualityComparer<T>.Default.Equals(_value!, value!);
    }

    public bool Equals(None none) => !_isSome;

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj switch
        {
            Option<T> option => Equals(option),
            T value => Equals(value),
            None none => Equals(none),
            _ => false,
        };
    }

#endregion

    public override int GetHashCode()
    {
        if (_isSome)
            return Hasher.GetHashCode<T>(_value);
        return 0;
    }

    public override string ToString()
    {
        return Match(static value => $"Some({value})", static () => nameof(None));
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.iter"/>
    public OptionEnumerator GetEnumerator() => new OptionEnumerator(this);

    [MustDisposeResource(false)]
    [StructLayout(LayoutKind.Auto)]
    public struct OptionEnumerator : IEnumerator<T>, IEnumerator, IDisposable
    {
        private bool _yielded;
        private readonly T _value;

        object? IEnumerator.Current => _value;
        public T Current => _value;

        public OptionEnumerator(Option<T> option)
        {
            if (option._isSome)
            {
                _value = option._value!;
                _yielded = false;
            }
            else
            {
                _value = default!;
                _yielded = true;
            }
        }

        public bool MoveNext()
        {
            if (_yielded)
                return false;
            _yielded = true;
            return true;
        }

        void IEnumerator.Reset() => throw new NotSupportedException();

        void IDisposable.Dispose()
        {
            // Do nothing
        }
    }
}