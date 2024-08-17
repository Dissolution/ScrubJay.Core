using JetBrains.Annotations;
using ScrubJay.Utilities;

namespace ScrubJay;

// <a href="https://doc.rust-lang.org/std/option/index.html">Rust's Option</a>

public readonly struct Option<T> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Option<T>, Option<T>, bool>,
    IEqualityOperators<Option<T>, Option.None, bool>,
    //IEqualityOperators<Option<T>, T, bool>,
#endif
    IEquatable<Option<T>>,
    IEquatable<Option.None>,
    //IEquatable<T>,
#if NET7_0_OR_GREATER
    IComparisonOperators<Option<T>, Option<T>, bool>,
    IComparisonOperators<Option<T>, Option.None, bool>,
    //IComparisonOperators<Option<T>, T, bool>,
#endif
    IComparable<Option<T>>,
    IComparable<Option.None>,
    //IComparable<T>,
    IEnumerable<T>,
    IEnumerable
{
    public static implicit operator bool(Option<T> option) => option._isSome;
    public static implicit operator Option<T>(Option.None none) => None;
    public static implicit operator Option<T>(T value) => Some(value);

    public static bool operator true(Option<T> option) => option._isSome;
    public static bool operator false(Option<T> option) => !option._isSome;

    public static bool operator ==(Option<T> x, Option<T> y) => x.Equals(y);
    public static bool operator !=(Option<T> x, Option<T> y) => !x.Equals(y);
    public static bool operator >(Option<T> x, Option<T> y) => x.CompareTo(y) > 0;
    public static bool operator >=(Option<T> x, Option<T> y) => x.CompareTo(y) >= 0;
    public static bool operator <(Option<T> x, Option<T> y) => x.CompareTo(y) < 0;
    public static bool operator <=(Option<T> x, Option<T> y) => x.CompareTo(y) <= 0;

    public static bool operator ==(Option<T> option, Option.None none) => option.IsNone();
    public static bool operator !=(Option<T> option, Option.None none) => option.IsSome();
    public static bool operator >(Option<T> option, Option.None none) => option.CompareTo(none) > 0;
    public static bool operator >=(Option<T> option, Option.None none) => option.CompareTo(none) >= 0;
    public static bool operator <(Option<T> option, Option.None none) => option.CompareTo(none) < 0;
    public static bool operator <=(Option<T> option, Option.None none) => option.CompareTo(none) <= 0;

    public static bool operator ==(Option<T> left, T? right) => left.Equals(right);
    public static bool operator !=(Option<T> left, T? right) => !left.Equals(right);
    public static bool operator >(Option<T> left, T right) => left.CompareTo(right) > 0;
    public static bool operator >=(Option<T> left, T right) => left.CompareTo(right) >= 0;
    public static bool operator <(Option<T> left, T right) => left.CompareTo(right) < 0;
    public static bool operator <=(Option<T> left, T right) => left.CompareTo(right) <= 0;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.or"/>
    public static Option<T> operator |(Option<T> x, Option<T> y) => x.IsSome() ? x : y;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.xor"/>
    public static Option<T> operator ^(Option<T> x, Option<T> y)
    {
        if (x._isSome)
        {
            if (y._isSome)
                return None;
            return x;
        }
        else
        {
            if (y._isSome)
                return y;
            return None;
        }
    }

    public static Option<T> None => default;
    public static Option<T> Some(T value) => new(value);

    private readonly bool _isSome;
    
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
    public bool IsSome() => _isSome;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.is_some_and"/>
    public bool IsSomeAnd(Func<T, bool> predicate) => _isSome && predicate(_value!);

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
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.is_none"/>
    public bool IsNone() => !_isSome;

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

    public void Match(Action<T> onSome, Action<Option.None> onNone)
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

    public TResult Match<TResult>(Func<T, TResult> some, Func<Option.None, TResult> none)
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="noneErrorMessage"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.expect"/>
    public T Expect(string noneErrorMessage)
    {
        if (_isSome)
            return _value!;
        throw new InvalidOperationException(noneErrorMessage);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.unwrap"/>
    public T Unwrap()
    {
        if (_isSome)
            return _value!;
        throw new InvalidOperationException("This option is not some");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.unwrap_or"/>
    public T UnwrapOr(T value)
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
    [return: MaybeNull]
    public T UnwrapOrDefault()
    {
        {
            if (_isSome)
                return _value!;
            return default(T);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="getValue"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.unwrap_or_else"/>
    public T UnwrapOrElse(Func<T> getValue)
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
            return ScrubJay.Result<T, TError>.Ok(_value!);
        return ScrubJay.Result<T, TError>.Error(err);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="getErr"></param>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.ok_or_else"/>
    public ScrubJay.Result<T, TError> OkOrElse<TError>(Func<TError> getErr)
    {
        if (_isSome)
            return ScrubJay.Result<T, TError>.Ok(_value!);
        return ScrubJay.Result<T, TError>.Error(getErr());
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
    /// <typeparam name="U"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.map"/>
    public Option<U> Map<U>(Func<T, U> map)
    {
        if (IsSome(out var value))
        {
            return Some<U>(map(value));
        }

        return Option<U>.None;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>
    /// <param name="defaultValue"></param>
    /// <typeparam name="U"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.map_or"/>
    public U MapOr<U>(Func<T, U> map, U defaultValue)
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
    /// <typeparam name="U"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.map_or_else"/>
    public U MapOrElse<U>(Func<T, U> map, Func<U> getDefaultValue)
    {
        if (IsSome(out var value))
        {
            return map(value);
        }

        return getDefaultValue();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.or"/>
    public Option<T> Or(Option<T> other) => _isSome ? this : other;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.or_else"/>
    public Option<T> OrElse(Func<Option<T>> other) => _isSome ? this : other();

    public Result<T, Exception> AsResult()
    {
        if (_isSome)
            return _value!;
        return new InvalidOperationException("Option.None");
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.iter"/>
    public Enumerator GetEnumerator() => new Enumerator(this);

    [MustDisposeResource(false)]
    public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
    {
        private bool _yielded;
        private readonly T _value;

        object? IEnumerator.Current => _value;
        public T Current => _value;

        public Enumerator(Option<T> option)
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

    public int CompareTo(Option.None none)
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
            if (_isSome)
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

    public bool Equals(Option.None none) => !_isSome;

    public bool Equals(T? value)
    {
        return _isSome && EqualityComparer<T>.Default.Equals(_value!, value!);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj switch
        {
            Option<T> option => Equals(option),
            Option.None none => Equals(none),
            T value => Equals(value),
            _ => false,
        };
    }

    public override int GetHashCode()
    {
        if (_isSome)
            return Hasher.Combine(_value);
        return 0;
    }

    public override string ToString()
    {
        return Match(value => $"Some({value})", () => nameof(None));
    }
}