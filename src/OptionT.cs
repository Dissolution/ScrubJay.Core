using ScrubJay.Utilities;

namespace ScrubJay;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// Heavily based upon Rust's Option<br/>
/// </remarks>
/// <see href="https://doc.rust-lang.org/std/option/"/>
/// <see href="https://doc.rust-lang.org/std/option/enum.Option.html"/>
public readonly struct Option<T> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Option<T>, Option<T>, bool>,
    IEqualityOperators<Option<T>, T, bool>,         // + flipped input types
    //IEqualityOperators<Option<T>, None, bool>,    // + flipped input types
    //IEqualityOperators<Option<T>, object, bool>,  // + flipped input types
#endif
    IEquatable<Option<T>>,
    IEquatable<T>,
    //IEquatable<None>,
    IEnumerable<T>
{
    public static implicit operator Option<T>(None _) => None;
    public static explicit operator Option<T>(T value) => Some(value);

    public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);
    public static bool operator !=(Option<T> left, Option<T> right) => !left.Equals(right);
    public static bool operator ==(Option<T> left, T? right) => left.Equals(right);
    public static bool operator !=(Option<T> left, T? right) => !left.Equals(right);
    public static bool operator ==(Option<T> left, None right) => left.Equals(right);
    public static bool operator !=(Option<T> left, None right) => !left.Equals(right);
    public static bool operator ==(Option<T> left, object? right) => left.Equals(right);
    public static bool operator !=(Option<T> left, object? right) => !left.Equals(right);
    public static bool operator ==(T? right, Option<T> left) => left.Equals(right);
    public static bool operator !=(T? right, Option<T> left) => !left.Equals(right);
    public static bool operator ==(None right, Option<T> left) => left.Equals(right);
    public static bool operator !=(None right, Option<T> left) => !left.Equals(right);
    public static bool operator ==(object? right, Option<T> left) => left.Equals(right);
    public static bool operator !=(object? right, Option<T> left) => !left.Equals(right);
    
    public static readonly Option<T> None = default(Option<T>);
    
    public static Option<T> Some(T value) => new Option<T>(value);
    
    private readonly bool _isSome;
    private readonly T? _value;

    internal Option(T value)
    {
        _isSome = true;
        _value = value;
    }

    /// <summary>
    /// Returns <c>true</c> if this <see cref="Option{T}"/> is a <see cref="None"/> value
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsNone() => !_isSome;

    /// <summary>
    /// Returns <c>true</c> if this <see cref="Option{T}"/> is a <see cref="Some"/> value
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSome() => _isSome;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSome(out T some)
    {
        some = _value!;
        return _isSome;
    }

    public T Expect(string? errorMessage = null)
    {
        if (_isSome)
            return _value!;
        throw new InvalidOperationException(errorMessage ?? $"Expected Option.Some<{typeof(T).Name}>, found Option.None");
    }

    public T Unwrap()
    {
        if (_isSome)
            return _value!;
        throw new InvalidOperationException($"Expected Option.Some<{typeof(T).Name}>, found Option.None");
    }

    public T UnwrapOr(T valueIfNone)
    {
        if (_isSome)
            return _value!;
        return valueIfNone!;
    }

    public T UnwrapOrElse(Func<T> createValueIfNone)
    {
        if (_isSome)
            return _value!;
        return createValueIfNone();
    }

    [return: MaybeNull]
    public T UnwrapOrDefault()
    {
        if (_isSome)
            return _value!;
        return default(T);
    }

    public void Match(Action<T> ifSome, Action<None> ifNone)
    {
        if (_isSome)
        {
            ifSome.Invoke(_value!);
        }
        else
        {
            ifNone.Invoke(default(None));
        }
    }
    public TReturn Match<TReturn>(Func<T, TReturn> ifSome, Func<None, TReturn> ifNone)
    {
        if (_isSome)
        {
            return ifSome.Invoke(_value!);
        }
        else
        {
            return ifNone.Invoke(default(None));
        }
    }

    public Option<U> Map<U>(Func<T, U> convertSome)
    {
        if (_isSome)
        {
            return Option<U>.Some(convertSome(_value!));
        }
        return Option<U>.None;
    }

    public Option<T> Inspect(Action<T> ifSome)
    {
        if (_isSome)
        {
            ifSome.Invoke(_value!);
        }
        return this;
    }

    public U MapOr<U>(U valueIfNone, Func<T, U> convertSome)
    {
        if (_isSome)
        {
            return convertSome(_value!);
        }
        return valueIfNone;
    }

    public U MapOrElse<U>(Func<U> createValueIfNone, Func<T, U> convertSome)
    {
        if (_isSome)
        {
            return convertSome(_value!);
        }
        return createValueIfNone();
    }

    public Option<T> Filter(Func<T, bool> someValuePredicate)
    {
        if (_isSome && someValuePredicate(_value!))
            return Some(_value!);
        return None;
    }

/*
    public Result<T> OkOr(Exception error)
    {
        if (_isSome)
        {
            return Result<T>.Ok(_someValue!);
        }
        return Result<T>.Error(error);
    }

    public Result<T> OkOrElse(Func<Exception> createError)
    {
        if (_isSome)
        {
            return Result<T>.Ok(_someValue!);
        }
        return Result<T>.Error(createError());
    }
*/

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<T> GetEnumerator()
    {
        if (IsSome(out var some))
        {
            yield return some!;
        }
    }

    public bool Equals(Option<T> option)
    {
        if (IsSome(out var some))
        {
            if (option.IsSome(out var optSome))
            {
                return EqualityComparer<T>.Default.Equals(some!, optSome!);
            }
            return false;
        }
        return option.IsNone();
    }
    public bool Equals(T? value) => IsSome(out var some) && EqualityComparer<T>.Default.Equals(some!, value!);
    public bool Equals(None _) => IsNone();
    public override bool Equals(object? obj) => obj switch
    {
        Option<T> option => Equals(option),
        T value => Equals(value),
        None none => Equals(none),
        _ => false,
    };
    public override int GetHashCode()
    {
        return _isSome ? Hasher.GetHashCode<T>(_value) : 0;
    }
    public override string ToString()
    {
        if (_isSome)
            return $"Option<{typeof(T).Name}>.Some({_value})";
        else
            return $"Option<{typeof(T).Name}>.None";
    }
}