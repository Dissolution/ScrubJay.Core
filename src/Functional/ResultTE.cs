// Prefix generic type parameter with T
// Do not declare static methods on generic types

#pragma warning disable CA1715, CA1000

namespace ScrubJay.Functional;

/// <summary>
/// <c>Result&lt;T, E&gt;</c> is used for return and error propagation<br/>
/// It acts like a discriminated union with two values:<br/>
/// <c>Ok&lt;T&gt;</c> -> Indicates success with a contained <typeparamref name="T"/> value<br/>
/// <c>Error&lt;E&gt;</c> -> Indicates failure with a contained <typeparamref name="E"/> value
/// </summary>
/// <typeparam name="T">The <see cref="Type"/> of value contained in an <c>Ok</c></typeparam>
/// <typeparam name="E">The <see cref="Type"/> of value contained in an <c>Error</c></typeparam>
/// <remarks>
/// 🦀 Heavily inspired by Rust's Result type 🦀
/// </remarks>
/// <seealso href="https://doc.rust-lang.org/std/result/enum.Result.html"/>
[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly struct Result<T, E> :
    /* All listed interfaces are implemented,
     * but cannot be declared because they may unify for some type parameter substitutions
     */
#if NET7_0_OR_GREATER
    IEqualityOperators<Result<T, E>, Result<T, E>, bool>,
    IEqualityOperators<Result<T, E>, T, bool>,
    //IEqualityOperators<Result<T, E>, E, bool>,
    IComparisonOperators<Result<T, E>, Result<T, E>, bool>,
    IComparisonOperators<Result<T, E>, T, bool>,
    //IComparisonOperators<Result<T, E>, E, bool>,
#endif
#if NET6_0_OR_GREATER
    ISpanFormattable,
#endif
    IEquatable<Result<T, E>>,
    IEquatable<T>,
    //IEquatable<E>,
    IComparable<Result<T, E>>,
    IComparable<T>,
    //IComparable<E>,
    IEnumerable<T>,
    IFormattable
{
#region Operators

    public static implicit operator bool(Result<T, E> result) => result._isOk;
    public static implicit operator Result<T, E>(IMPL.Ok<T> ok) => Ok(ok.Value);
    public static implicit operator Result<T, E>(IMPL.Error<E> error) => Error(error.Value);

    // We pass equality and comparison down to T values

    public static bool operator ==(Result<T, E> left, Result<T, E> right) => left.Equals(right);
    public static bool operator !=(Result<T, E> left, Result<T, E> right) => !left.Equals(right);
    public static bool operator ==(Result<T, E> result, T? ok) => result.Equals(ok);
    public static bool operator !=(Result<T, E> result, T? ok) => !result.Equals(ok);
    public static bool operator ==(Result<T, E> result, E? error) => result.Equals(error);
    public static bool operator !=(Result<T, E> result, E? error) => !result.Equals(error);

    public static bool operator >(Result<T, E> left, Result<T, E> right)
        => left.CompareTo(right) > 0;

    public static bool operator >=(Result<T, E> left, Result<T, E> right)
        => left.CompareTo(right) >= 0;

    public static bool operator <(Result<T, E> left, Result<T, E> right)
        => left.CompareTo(right) < 0;

    public static bool operator <=(Result<T, E> left, Result<T, E> right)
        => left.CompareTo(right) <= 0;

    public static bool operator >(Result<T, E> result, T? ok)
        => result.CompareTo(ok) > 0;

    public static bool operator >=(Result<T, E> result, T? ok)
        => result.CompareTo(ok) >= 0;

    public static bool operator <(Result<T, E> result, T? ok)
        => result.CompareTo(ok) < 0;

    public static bool operator <=(Result<T, E> result, T? ok)
        => result.CompareTo(ok) <= 0;

    public static bool operator >(Result<T, E> result, E? error)
        => result.CompareTo(error) > 0;

    public static bool operator >=(Result<T, E> result, E? error)
        => result.CompareTo(error) >= 0;

    public static bool operator <(Result<T, E> result, E? error)
        => result.CompareTo(error) < 0;

    public static bool operator <=(Result<T, E> result, E? error)
        => result.CompareTo(error) <= 0;

#endregion

    /// <summary>
    /// Creates a new Ok <see cref="Result{T,E}"/>
    /// </summary>
    /// <param name="ok">The Ok value</param>
    /// <returns></returns>
    public static Result<T, E> Ok(T ok) => new Result<T, E>(true, ok, default);

    /// <summary>
    /// Creates a new Error <see cref="Result{T,E}"/>
    /// </summary>
    /// <param name="error">The Error value</param>
    /// <returns></returns>
    public static Result<T, E> Error(E error) => new Result<T, E>(false, default, error);


    // is this Result.Ok?
    // default(Result) implies !_isOk, thus default(Result) == None
    private readonly bool _isOk;

    // if this is Result.Ok, the Ok Value
    private readonly T? _value;

    // if this is Result.Error, the Error Value
    private readonly E? _error;

    /// <summary>
    /// Result may only be constructed through <see cref="Ok(T)"/>, <see cref="Error(E)"/>,
    /// or through implicit conversion from <see cref="IMPL.Ok{T}"/> or <see cref="IMPL.Error{E}"/>
    /// </summary>
    private Result(bool isOk, T? value, E? error)
    {
        _isOk = isOk;
        _value = value;
        _error = error;
    }

#region Ok

    public bool IsOk() => _isOk;

    public bool IsOk([MaybeNullWhen(false)] out T value)
    {
        if (_isOk)
        {
            value = _value!;
            return true;
        }

        value = default!;
        return false;
    }

    public bool IsOk([MaybeNullWhen(false)] out T ok, [MaybeNullWhen(true)] out E error)
    {
        ok = _value;
        error = _error;
        return _isOk;
    }

    /// <summary>
    /// Returns <c>true</c> if this Result is Ok and the value inside of it matches a predicate
    /// </summary>
    /// <param name="okPredicate"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.is_ok_and"/>
    public bool IsOkAnd(Func<T, bool> okPredicate) => _isOk && okPredicate(_value!);

    /// <summary>
    ///
    /// </summary>
    /// <param name="fallback"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Result.html#method.unwrap_or"/>
    public T OkOr(T fallback)
    {
        if (_isOk)
            return _value!;
        return fallback;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="getFallback"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Result.html#method.unwrap_or_else"/>
    public T OkOr(Func<T> getFallback)
    {
        if (_isOk)
            return _value!;
        return getFallback();
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Result.html#method.unwrap_or_default"/>
    public T? OkOrDefault()
    {
        if (_isOk)
            return _value!;
        return default(T);
    }

    /// <summary>
    /// Returns the contained Ok value
    /// </summary>
    /// <returns>
    ///
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the value is an Error
    /// </exception>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Result.html#method.unwrap"/>
    public T OkOrThrow(string? exceptionMessage = null)
    {
        if (_isOk)
            return _value!;
        if (_error is Exception ex)
            throw ex;
        throw new InvalidOperationException(exceptionMessage ?? $"{ToString()} is not Ok");
    }

#endregion

#region Error

    public bool IsError() => !_isOk;

    public bool IsError([MaybeNullWhen(false)] out E error)
    {
        if (!_isOk)
        {
            error = _error!;
            return true;
        }

        error = default!;
        return false;
    }

    public bool IsError([MaybeNullWhen(false)] out E error, [MaybeNullWhen(true)] out T ok)
    {
        error = _error;
        ok = _value;
        return !_isOk;
    }

    /// <summary>
    /// Returns <c>true</c> if this Result is Error and the value inside of it matches a predicate
    /// </summary>
    /// <param name="errorPredicate"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.is_err_and"/>
    public bool IsErrorAnd(Func<E, bool> errorPredicate) => !_isOk && errorPredicate(_error!);


    public E ErrorOr(E fallback)
    {
        if (!_isOk)
            return _error!;
        return fallback;
    }


    public E ErrorOr(Func<E> getFallback)
    {
        if (_isOk)
            return _error!;
        return getFallback();
    }

    public E? ErrorOrDefault()
    {
        if (!_isOk)
            return _error!;
        return default(E);
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.unwrap_err"/>
    public E ErrorOrThrow(string? exceptionMessage = null)
    {
        if (!_isOk)
            return _error!;
        throw new InvalidOperationException(exceptionMessage ?? $"{ToString()} is not Error");
    }

#endregion

#region Match

    public void Match(Action<T> onOk, Action<E> onError)
    {
        if (_isOk)
        {
            onOk(_value!);
        }
        else
        {
            onError(_error!);
        }
    }


    public R Match<R>(Func<T, R> onOk, Func<E, R> onError)
    {
        if (_isOk)
        {
            return onOk(_value!);
        }
        else
        {
            return onError(_error!);
        }
    }

#endregion

    public Option<T> AsOption()
    {
        if (_isOk)
        {
            return Some(_value!);
        }
        else
        {
            return None;
        }
    }

#region Compare

    public int CompareTo(Result<T, E> other)
    {
        if (_isOk)
        {
            if (other._isOk)
            {
                return Comparer<T>.Default.Compare(_value!, other._value!);
            }
            else
            {
                return -1; // Ok < Error
            }
        }
        else
        {
            if (other._isOk)
            {
                return 1; // Error > Ok
            }
            else
            {
                return Comparer<E>.Default.Compare(_error!, other._error!);
            }
        }
    }

    public int CompareTo(T? ok)
    {
        if (_isOk)
        {
            return Comparer<T>.Default.Compare(_value!, ok!);
        }
        else
        {
            // Error > Ok
            return 1;
        }
    }

    public int CompareTo(E? error)
    {
        if (_isOk)
        {
            // Ok < Error
            return -1;
        }
        else
        {
            return Comparer<E>.Default.Compare(_error!, error!);
        }
    }

#endregion

#region Equal

    public bool Equals(Result<T, E> other)
    {
        if (_isOk)
        {
            if (other._isOk)
            {
                return EqualityComparer<T>.Default.Equals(_value!, other._value!);
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (other._isOk)
            {
                return false;
            }
            else
            {
                return EqualityComparer<E>.Default.Equals(_error!, other._error!);
            }
        }
    }

    public bool Equals(T? ok)
    {
        if (_isOk)
        {
            return EqualityComparer<T>.Default.Equals(_value!, ok!);
        }

        return false;
    }

    public bool Equals(E? error)
    {
        if (!_isOk)
        {
            return EqualityComparer<E>.Default.Equals(_error!, error!);
        }

        return false;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj switch
        {
            Result<E, E> result => Equals(result),
            T ok => Equals(ok),
            E error => Equals(error),
            bool isOk => _isOk == isOk,
            _ => false,
        };

    public override int GetHashCode()
    {
        if (_isOk)
        {
            if (_value is not null)
            {
                return _value.GetHashCode();
            }

            return typeof(T).GetHashCode();
        }
        else
        {
            if (_error is not null)
            {
                return _error.GetHashCode();
            }

            return typeof(E).GetHashCode();
        }
    }

#endregion

#region ToString / TryFormat

    public string ToString(string? format, IFormatProvider? provider = null)
    {
        string? str;

        if (_isOk)
        {
            if (_value is IFormattable)
            {
                str = ((IFormattable)_value!).ToString(format, provider);
            }
            else
            {
                str = _value?.ToString();
            }

            return $"Result<{typeof(T)}, {typeof(E)}>.Ok({str})";
        }
        else
        {
            if (_error is IFormattable)
            {
                str = ((IFormattable)_error!).ToString(format, provider);
            }
            else
            {
                str = _error?.ToString();
            }

            return $"Result<{typeof(T)}, {typeof(E)}>.Error({str})";
        }
    }

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        ReadOnlySpan<char> format = default,
        IFormatProvider? provider = null)
    {
        // todo: make this better
        var str = ToString(format.ToString(), provider);
        if (str.TryCopyTo(destination))
        {
            charsWritten = str.Length;
            return true;
        }

        charsWritten = 0;
        return false;
        ;
    }

    public override string ToString()
    {
        if (_isOk)
        {
            return $"Result<{typeof(T)}, {typeof(E)}>.Ok({_value})";
        }
        else
        {
            return $"Result<{typeof(T)}, {typeof(E)}>.Error({_error})";
        }
    }

#endregion

#region Linq

    public Result<N, E> Select<N>(Func<T, N> selector)
    {
        if (_isOk)
        {
            return Result<N, E>.Ok(selector(_value!));
        }

        return Error<E>(_error!);
    }

    public Result<N, E> Select<N>(Func<T, Result<N, E>> selector)
    {
        if (_isOk)
        {
            return selector(_value!);
        }

        return Error<E>(_error!);
    }

    public Result<N, E> SelectMany<K, N>(
        Func<T, Result<K, E>> keySelector,
        Func<T, K, N> newSelector)
    {
        if (IsOk(out var value, out var error))
        {
            var keyResult = keySelector(value!);
            if (keyResult.IsOk(out var key, out error))
            {
                var newSelect = newSelector(value, key);
                return Result<N, E>.Ok(newSelect);
            }
            else
            {
                return Result<N, E>.Error(error);
            }
        }
        else
        {
            return Result<N, E>.Error(error);
        }
    }

#endregion

#region IEnumerable

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    [MustDisposeResource(false)]
    public ResultEnumerator GetEnumerator() => new ResultEnumerator(this);

    [PublicAPI]
    [MustDisposeResource(false)]
    public struct ResultEnumerator : IEnumerator<T>, IEnumerator, IDisposable
    {
        private readonly Result<T, E> _result;
        private bool _canYield;

        object? IEnumerator.Current => _result.OkOrThrow();

        public T Current => _result.OkOrThrow();

        public ResultEnumerator(Result<T, E> result)
        {
            _result = result;
            _canYield = result._isOk;
        }

        void IDisposable.Dispose()
        {
            /* Do Nothing */
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
            _canYield = _result._isOk;
        }
    }

#endregion
}