#pragma warning disable MA0077, MA0094, CA1034

// ReSharper disable InconsistentNaming

using ScrubJay.Comparison;
namespace ScrubJay;

[StructLayout(LayoutKind.Auto)]
[PublicAPI]
public readonly ref struct OptionReadOnlySpan<T> //:
/* All listed interfaces are implemented, but cannot be declared because they may unify for some type parameter substitutions */    
#if NET7_0_OR_GREATER
    //IEqualityOperators<OptionReadOnlySpan<T>, OptionReadOnlySpan<T>, bool>,
    //IEqualityOperators<OptionReadOnlySpan<T>, None, bool>,
    //IEqualityOperators<OptionReadOnlySpan<T>, ReadOnlySpan<T>, bool>,
#endif
    //IEquatable<OptionReadOnlySpan<T>>,
    //IEquatable<None>,
    //IEquatable<ReadOnlySpan<T>>,
#if NET7_0_OR_GREATER
    //IComparisonOperators<OptionReadOnlySpan<T>, OptionReadOnlySpan<T>, bool>,
    //IComparisonOperators<OptionReadOnlySpan<T>, None, bool>,
    //IComparisonOperators<OptionReadOnlySpan<T>, ReadOnlySpan<T>, bool>,
#endif
    //IComparable<OptionReadOnlySpan<T>>,
    //IComparable<None>,
    //IComparable<ReadOnlySpan<T>>,
    //IEnumerable<ReadOnlySpan<T>>,
    //IEnumerable
{
#region Operators
    // Treat Option like a boolean: some == true, none == false
    // Allow us to cast from None or a T value directly to an Option
    
    public static implicit operator bool(OptionReadOnlySpan<T> option) => option._isSome;
    public static implicit operator OptionReadOnlySpan<T>(None _) => None();
    public static implicit operator OptionReadOnlySpan<T>(ReadOnlySpan<T> some) => Some(some);

    public static bool operator true(OptionReadOnlySpan<T> option) => option._isSome;
    public static bool operator false(OptionReadOnlySpan<T> option) => !option._isSome;

    // We pass equality and comparison down to T values
    
    public static bool operator ==(OptionReadOnlySpan<T> left, OptionReadOnlySpan<T> right) => left.Equals(right);
    public static bool operator !=(OptionReadOnlySpan<T> left, OptionReadOnlySpan<T> right) => !left.Equals(right);
    public static bool operator >(OptionReadOnlySpan<T> left, OptionReadOnlySpan<T> right) => left.CompareTo(right) > 0;
    public static bool operator >=(OptionReadOnlySpan<T> left, OptionReadOnlySpan<T> right) => left.CompareTo(right) >= 0;
    public static bool operator <(OptionReadOnlySpan<T> left, OptionReadOnlySpan<T> right) => left.CompareTo(right) < 0;
    public static bool operator <=(OptionReadOnlySpan<T> left, OptionReadOnlySpan<T> right) => left.CompareTo(right) <= 0;

    public static bool operator ==(OptionReadOnlySpan<T> option, None none) => option.IsNone();
    public static bool operator !=(OptionReadOnlySpan<T> option, None none) => option.IsSome();
    public static bool operator >(OptionReadOnlySpan<T> option, None none) => option.CompareTo(none) > 0;
    public static bool operator >=(OptionReadOnlySpan<T> option, None none) => option.CompareTo(none) >= 0;
    public static bool operator <(OptionReadOnlySpan<T> option, None none) => option.CompareTo(none) < 0;
    public static bool operator <=(OptionReadOnlySpan<T> option, None none) => option.CompareTo(none) <= 0;

    public static bool operator ==(OptionReadOnlySpan<T> option, ReadOnlySpan<T> some) => option.Equals(some);
    public static bool operator !=(OptionReadOnlySpan<T> option, ReadOnlySpan<T> some) => !option.Equals(some);
    public static bool operator >(OptionReadOnlySpan<T> option, ReadOnlySpan<T> some) => option.CompareTo(some) > 0;
    public static bool operator >=(OptionReadOnlySpan<T> option, ReadOnlySpan<T> some) => option.CompareTo(some) >= 0;
    public static bool operator <(OptionReadOnlySpan<T> option, ReadOnlySpan<T> some) => option.CompareTo(some) < 0;
    public static bool operator <=(OptionReadOnlySpan<T> option, ReadOnlySpan<T> some) => option.CompareTo(some) <= 0;

#endregion

    /// <summary>
    /// Gets the None option
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OptionReadOnlySpan<T> None() => default;
    
    /// <summary>
    /// Creates a new <see cref="Option{T}"/>.Some containing <paramref name="value"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OptionReadOnlySpan<T> Some(ReadOnlySpan<T> value) => new(value);

    
    // Is this Option.Some?
    // if someone does default(Option), this will be false, so default(Option) == None
    private readonly bool _isSome;
    // If this is Option.Some, the value
    private readonly ReadOnlySpan<T> _value;

    private OptionReadOnlySpan(ReadOnlySpan<T> value)
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
    public bool IsSome(out ReadOnlySpan<T> value)
    {
        if (_isSome)
        {
            value = _value;
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
    public bool IsSomeAnd(RSpanFunc<T, bool> predicate) => _isSome && predicate(_value);

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
    public ReadOnlySpan<T> SomeOrThrow(string? errorMessage = null)
    {
        if (_isSome)
            return _value;
        throw new InvalidOperationException(errorMessage ?? "This option is not some");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.unwrap_or"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> SomeOr(ReadOnlySpan<T> value)
    {
        if (_isSome)
            return _value;
        return value;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.unwrap_or_default"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> SomeOrDefault()
    {
        if (_isSome)
                return _value;
        return ReadOnlySpan<T>.Empty;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="getValue"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.unwrap_or_else"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> SomeOrElse(FuncRSpan<T> getValue)
    {
        if (_isSome)
            return _value!;
        return getValue();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.filter"/>
    public OptionReadOnlySpan<T> Filter(RSpanFunc<T, bool> predicate)
    {
        if (IsSome(out var value) && predicate(value))
            return this;
        return default;
    }
    
    public Option<TNew> Map<TNew>(RSpanFunc<T, TNew> map)
    {
        if (IsSome(out var value))
        {
            return Some<TNew>(map(value));
        }

        return default;
    }
    
    public OptionReadOnlySpan<TNew> Map<TNew>(RSpanFuncRSpan<T, TNew> map)
    {
        if (IsSome(out var value))
        {
            return OptionReadOnlySpan<TNew>.Some(map(value));
        }

        return default;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>
    /// <param name="defaultValue"></param>
    /// <typeparam name="TNew"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.map_or"/>
    public TNew MapOr<TNew>(RSpanFunc<T, TNew> map, TNew defaultValue)
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
    public TNew MapOrElse<TNew>(RSpanFunc<T, TNew> map, Func<TNew> getDefaultValue)
    {
        if (IsSome(out var value))
        {
            return map(value);
        }

        return getDefaultValue();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Match(RSpanAction<T> onSome, Action onNone)
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
    public void Match(RSpanAction<T> onSome, Action<None> onNone)
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
    public TResult Match<TResult>(RSpanFunc<T, TResult> some, Func<TResult> none)
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
    public TResult Match<TResult>(RSpanFunc<T, TResult> some, Func<None, TResult> none)
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


#region Compare

    public int CompareTo(OptionReadOnlySpan<T> other)
    {
        // None compares as less than any Some
        if (_isSome)
        {
            if (other._isSome)
            {
                return Compare.Sequence(_value, other._value);
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

    public int CompareTo(ReadOnlySpan<T> other)
    {
        if (_isSome)
        {
            return Compare.Sequence(_value, other);
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
            T[] array => CompareTo(array.AsSpan()),
            None none => CompareTo(none),
            _ => 1, // unknown values sort before
        };
    }

#endregion

#region Equals

    public bool Equals(OptionReadOnlySpan<T> other)
    {
        if (_isSome)
        {
            if (other._isSome)
            {
                return Equate.Sequence(_value, other._value);
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
                return true;
            }
        }
    }

    public bool Equals(ReadOnlySpan<T> value)
    {
        return _isSome && Equate.Sequence(_value, value);
    }

    public bool Equals(None none) => !_isSome;

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj switch
        {
            T[] array => Equals(array.AsSpan()),
            None none => Equals(none),
            _ => false,
        };
    }

#endregion

    public override int GetHashCode()
    {
        if (_isSome)
            return Hasher.Combine(_value);
        return 0;
    }

    public override string ToString()
    {
        if (_isSome)
            return $"Some({_value.ToString()})";
        return nameof(None);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.iter"/>
    public OptionReadOnlySpanEnumerator GetEnumerator() => new OptionReadOnlySpanEnumerator(this);

    [MustDisposeResource(false)]
    [StructLayout(LayoutKind.Auto)]
    public ref struct OptionReadOnlySpanEnumerator // : IEnumerator<T>, IEnumerator, IDisposable
    {
        private bool _yielded;
        private readonly ReadOnlySpan<T> _value;
        
        public ReadOnlySpan<T> Current => _value;

        public OptionReadOnlySpanEnumerator(OptionReadOnlySpan<T> option)
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
    }
}