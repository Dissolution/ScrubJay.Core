using ScrubJay.Utilities;

namespace ScrubJay;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// None only exists to support <see cref="Option"/>'s ability to return a <see cref="M:Option{T}.None"/>
/// without needing the generic type information 
/// </remarks>
[StructLayout(LayoutKind.Explicit, Size = 0)]
public readonly struct None :
#if NET7_0_OR_GREATER
    IEqualityOperators<None, None, bool>,
#endif
    IEquatable<None>
{
    // All Nones are the same
    public static bool operator ==(None left, None right) => true;
    public static bool operator !=(None left, None right) => false;

    // All Nones are the same
    public bool Equals(None _) => true;
    public override bool Equals(object? obj) => obj is None;
    public override int GetHashCode() => 0;
    public override string ToString() => nameof(None);
}

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
    IEqualityOperators<Option<T>, T, bool>,
    IBitwiseOperators<Option<T>, Option<T>, Option<T>>,
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

    public static Option<T> operator &(Option<T> left, Option<T> right)
    {
        return left.IsNone() ? right : None;
    }
    public static Option<T> operator |(Option<T> left, Option<T> right)
    {
        return left.IsSome() ? left : right;
    }
    public static Option<T> operator ^(Option<T> left, Option<T> right)
    {
        if (left.IsSome())
        {
            return right.IsSome() ? None : left;
        }
        else
        {
            return right.IsSome() ? right : None;
        }
    }
#if NET7_0_OR_GREATER
    [Obsolete("Option<T> does not support the ~ operator", true)]
    public static Option<T> operator ~(Option<T> value) => throw new NotSupportedException();
#endif

    public static Option<T> None { get; } = default;
    public static Option<T> Some(T value) => new Option<T>(value);
    public static Option<T> NotNull(T? value) => value is null ? None : Some(value);


    private readonly bool _isSome;
    private readonly T? _someValue;

    private Option(T? someValue)
    {
        _isSome = true;
        _someValue = someValue;
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
        some = _someValue!;
        return _isSome;
    }

    public T Expect(string? errorMessage = null)
    {
        if (_isSome)
            return _someValue!;
        throw new InvalidOperationException(errorMessage ?? $"Expected Option.Some<{typeof(T).Name}>, found Option.None");
    }

    public T Unwrap()
    {
        if (_isSome)
            return _someValue!;
        throw new InvalidOperationException($"Expected Option.Some<{typeof(T).Name}>, found Option.None");
    }

    public T UnwrapOr(T valueIfNone)
    {
        if (_isSome)
            return _someValue!;
        return valueIfNone!;
    }

    public T UnwrapOrElse(Func<T> createValueIfNone)
    {
        if (_isSome)
            return _someValue!;
        return createValueIfNone();
    }

    [return: MaybeNull]
    public T UnwrapOrDefault()
    {
        if (_isSome)
            return _someValue!;
        return default(T);
    }

    public void Match(Action<T> ifSome, Action<None> ifNone)
    {
        if (_isSome)
        {
            ifSome.Invoke(_someValue!);
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
            return ifSome.Invoke(_someValue!);
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
            return Option<U>.Some(convertSome(_someValue!));
        }
        return Option<U>.None;
    }

    public Option<T> Inspect(Action<T> ifSome)
    {
        if (_isSome)
        {
            ifSome.Invoke(_someValue!);
        }
        return this;
    }

    public U MapOr<U>(U valueIfNone, Func<T, U> convertSome)
    {
        if (_isSome)
        {
            return convertSome(_someValue!);
        }
        return valueIfNone;
    }

    public U MapOrElse<U>(Func<U> createValueIfNone, Func<T, U> convertSome)
    {
        if (_isSome)
        {
            return convertSome(_someValue!);
        }
        return createValueIfNone();
    }

    public Option<T> Filter(Func<T, bool> someValuePredicate)
    {
        if (_isSome && someValuePredicate(_someValue!))
            return Some(_someValue!);
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
        return _isSome ? Hasher.GetHashCode<T>(_someValue) : 0;
    }
    public override string ToString()
    {
        return _isSome ? $"Some<{typeof(T).Name}>({_someValue})" : "None";
    }
}

/// <summary>
/// &lt;Using Include="Option" Static="true"/&gt;
/// </summary>
public static class Option
{
    public static readonly None None = default(None);
    public static Option<T> Some<T>(T value) => Option<T>.Some(value);
    public static Option<T> NotNull<T>([AllowNull] T value)
        where T : notnull
        => Option<T>.NotNull(value);
}