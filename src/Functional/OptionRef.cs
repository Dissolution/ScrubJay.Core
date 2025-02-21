#if NET7_0_OR_GREATER

#pragma warning disable CA1000
#pragma warning disable CA1034
#pragma warning disable CA1045

// ReSharper disable InconsistentNaming

namespace ScrubJay.Functional;

[StructLayout(LayoutKind.Auto)]
[PublicAPI]
public readonly ref struct OptionRef<T>
{
#region Operators

    // Treat Option like a boolean: some == true, none == false
    // Allow us to cast from None or a T value directly to an Option

    public static implicit operator bool(OptionRef<T> option) => option._isSome;
    public static implicit operator OptionRef<T>(None _) => None();

    public static bool operator true(OptionRef<T> option) => option._isSome;
    public static bool operator false(OptionRef<T> option) => !option._isSome;

    // We pass equality and comparison down to T values

    public static bool operator ==(OptionRef<T> left, OptionRef<T> right) => left.Equals(right);
    public static bool operator !=(OptionRef<T> left, OptionRef<T> right) => !left.Equals(right);
    public static bool operator >(OptionRef<T> left, OptionRef<T> right) => left.CompareTo(right) > 0;
    public static bool operator >=(OptionRef<T> left, OptionRef<T> right) => left.CompareTo(right) >= 0;
    public static bool operator <(OptionRef<T> left, OptionRef<T> right) => left.CompareTo(right) < 0;
    public static bool operator <=(OptionRef<T> left, OptionRef<T> right) => left.CompareTo(right) <= 0;

    public static bool operator ==(OptionRef<T> option, None _) => option.IsNone;
    public static bool operator !=(OptionRef<T> option, None _) => option.IsSome;
    public static bool operator >(OptionRef<T> option, None none) => option.CompareTo(none) > 0;
    public static bool operator >=(OptionRef<T> option, None none) => option.CompareTo(none) >= 0;
    public static bool operator <(OptionRef<T> option, None none) => option.CompareTo(none) < 0;
    public static bool operator <=(OptionRef<T> option, None none) => option.CompareTo(none) <= 0;
#endregion

    /// <summary>
    /// Gets the None option
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OptionRef<T> None() => default;

    /// <summary>
    /// Creates a new <see cref="Option{T}"/>.Some containing <paramref name="value"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OptionRef<T> Some(ref T value) => new(ref value);


    // Is this Option.Some?
    // if someone does default(Option), this will be false, so default(Option) == None
    private readonly bool _isSome;

    // If this is Option.Some, the value
    private readonly ref T _value;

    private OptionRef(ref T value)
    {
        _isSome = true;
        _value = ref value;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.is_some"/>
    public bool IsSome
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _isSome;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasSome(ref T value)
    {
        if (_isSome)
        {
            value = ref _value;
            return true;
        }

        value = ref Notsafe.NullRef<T>();
        return false;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.is_none"/>
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
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.unwrap"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T SomeOrThrow(string? errorMessage = null)
    {
        if (_isSome)
            return ref _value;
        throw new InvalidOperationException(errorMessage ?? "This option is not some");
    }

    public delegate void ActionRef(ref T value);
    public delegate TReturn FuncRef<out TReturn>(ref T value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Match(ActionRef onSome, Action onNone)
    {
        if (_isSome)
        {
            onSome(ref _value);
        }
        else
        {
            onNone();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Match(ActionRef onSome, Action<None> onNone)
    {
        if (_isSome)
        {
            onSome(ref _value);
        }
        else
        {
            onNone(default);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Match<TResult>(FuncRef<TResult> some, Func<TResult> none)
    {
        if (_isSome)
        {
            return some(ref _value);
        }
        else
        {
            return none();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Match<TResult>(FuncRef<TResult> some, Func<None, TResult> none)
    {
        if (_isSome)
        {
            return some(ref _value);
        }
        else
        {
            return none(default);
        }
    }



#region Compare

    public int CompareTo(OptionRef<T> other)
    {
        // None compares as less than any Some
        if (_isSome)
        {
            if (other._isSome)
            {
                return Compare.Values(_value, other._value);
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

    public int CompareTo(T other)
    {
        if (_isSome)
        {
            return Compare.Values(_value, other);
        }
        else
        {
            // None compares as less than any Some
            return -1;
        }
    }

    public int CompareTo(None _)
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
            T value => CompareTo(value),
            None none => CompareTo(none),
            _ => 1, // unknown values sort before
        };
    }

#endregion

#region Equals

    public bool Equals(OptionRef<T> other)
    {
        if (_isSome)
        {
            if (other._isSome)
            {
                return Equate.Values(_value, other._value);
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

    public bool Equals(T value) => _isSome && Equate.Values(_value, value);

    public bool Equals(None _) => !_isSome;

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj switch
        {
            T value => Equals(value),
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
        {
            T value = _value!;
            return $"Some({value})";
        }
        return nameof(None);
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.iter"/>
    public OptionRefEnumerator GetEnumerator() => new(this);

    [MustDisposeResource(false)]
    [StructLayout(LayoutKind.Auto)]
    public ref struct OptionRefEnumerator // : IEnumerator<T>, IEnumerator, IDisposable
    {
        private bool _yielded;
        private readonly ref T _value;

        public readonly T Current => _value;

        public OptionRefEnumerator(OptionRef<T> option)
        {
            if (option._isSome)
            {
                _value = ref option._value!;
                _yielded = false;
            }
            else
            {
                _value = ref Notsafe.NullRef<T>();
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
#endif
