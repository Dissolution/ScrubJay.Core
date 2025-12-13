// Prefix generic type parameter with T
// Do not declare static methods on generic types
// Do not catch Exception

#pragma warning disable CA1715, CA1000, CA1031


namespace ScrubJay.Functional;

/// <summary>
/// Represents the RefResults of a function - <c>(?) -> T</c><br/>
/// imitating a discriminated union like:
/// <code>
/// RefResult
/// {
///     Ok(T),
///     Error(Exception),
/// }
/// </code>
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// This RefResult type also uses c# syntax to be more fluent<br/>
/// 🦀 Heavily inspired by Rust's RefResult type 🦀
/// </remarks>
/// <seealso href="https://doc.rust-lang.org/std/result/enum.Result.html"/>
[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly ref struct RefResult<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
#region Operators

    public static implicit operator bool(in RefResult<T> refResult) => refResult._isOk;
    public static implicit operator RefResult<T>(T value) => Ok(value);
    public static implicit operator RefResult<T>(Exception ex) => Error(ex);
    public static implicit operator RefResult<T>(IMPL.Ok<T> ok) => Ok(ok.Value);
    public static implicit operator RefResult<T>(IMPL.Error<Exception> error) => Error(error.Value);

#endregion


    /// <summary>
    /// Creates <see cref="Result{T}"/>.Ok(<paramref name="value"/>)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RefResult<T> Ok(T value) => new RefResult<T>(true, value, null);

    /// <summary>
    /// Creates <see cref="Result{T}"/>.Error(<paramref name="ex"/>)
    /// </summary>
    public static RefResult<T> Error(Exception ex) => new RefResult<T>(false, default, ex);

    /* These exact fields were chosen so that
     * default(RefResult<>) would be a failure (same as default(bool) == false)
     */

    private readonly bool _isOk;
    private readonly T? _value;
    private readonly Exception? _error;

    /// <summary>
    /// RefResult may only be constructed through <see cref="Ok(T)"/>, <see cref="Error"/>,
    /// or through implicit conversion from <see cref="IMPL.Ok{T}"/> or <see cref="IMPL.Error{E}"/>
    /// </summary>
    private RefResult(bool isOk, T? value, Exception? error)
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

    public bool IsOk([MaybeNullWhen(false)] out T ok, [MaybeNullWhen(true)] out Exception error)
    {
        ok = _value;
        error = _error;
        return _isOk;
    }

    /// <summary>
    /// Returns <c>true</c> if this RefResult is Ok and the value inside of it matches a predicate
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
        throw (_error ?? new InvalidOperationException(exceptionMessage ?? $"{ToString()} is not Ok"));
    }

#endregion

#region Error

    public bool IsError() => _isOk;

    public bool IsError([MaybeNullWhen(false)] out Exception error)
    {
        error = _error;
        return !_isOk;
    }


    public bool IsError([MaybeNullWhen(false)] out Exception error, [MaybeNullWhen(true)] out T ok)
    {
        error = _error;
        ok = _value;
        return !_isOk;
    }

    /// <summary>
    /// Returns <c>true</c> if this RefResult is Error and the value inside of it matches a predicate
    /// </summary>
    /// <param name="errorPredicate"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.is_err_and"/>
    public bool IsErrorAnd(Func<Exception, bool> errorPredicate) => !_isOk && errorPredicate(_error!);

    public Exception ErrorOr(Exception fallback)
    {
        if (!_isOk && _error is not null)
            return _error;
        return fallback;
    }

    public Exception ErrorOr(Func<Exception> getFallback)
    {
        if (!_isOk && _error is not null)
            return _error;
        return getFallback();
    }

    [StackTraceHidden]
    public void ThrowIfError()
    {
        if (!_isOk)
        {
            if (_error is not null)
            {
                throw _error;
            }

            throw new InvalidOperationException($"{ToString()} is not Ok");
        }
    }

#endregion

#region Match

    public void Match(Action<T> onOk, Action<Exception> onError)
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


    public R Match<R>(Func<T, R> onOk, Func<Exception, R> onError)
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

    public RefOption<T> AsOption()
    {
        if (_isOk)
        {
            return RefOption<T>.Some(_value!);
        }
        else
        {
            return None;
        }
    }

#region Linq

    public RefResult<N> Select<N>(Func<T, N> selector)
    {
        if (_isOk)
        {
            return RefResult<N>.Ok(selector(_value!));
        }

        return RefResult<N>.Error(_error!);
    }

    public RefResult<N> Select<N>(Func<T, Option<N>> selector)
    {
        if (_isOk && selector(_value!).IsSome(out var value))
        {
            return RefResult<N>.Ok(value);
        }

        return RefResult<N>.Error(_error!);
    }

#if NET9_0_OR_GREATER
    public RefResult<N> Select<N>(Func<T, RefResult<N>> selector)
    {
        if (_isOk)
        {
            return selector(_value!);
        }

        return RefResult<N>.Error(_error!);
    }
#endif

    public RefResult<N> Select<X, N>(X state, Func<X, T, N> selector)
    {
        if (_isOk)
        {
            return RefResult<N>.Ok(selector(state, _value!));
        }

        return RefResult<N>.Error(_error!);
    }

#if NET9_0_OR_GREATER
    public RefResult<N> SelectMany<N>(Func<T, RefResult<N>> newSelector)
    {
        if (_isOk)
        {
            return newSelector(_value!);
        }

        return RefResult<N>.Error(_error!);
    }

    public RefResult<N> SelectMany<K, N>(Func<T, RefResult<K>> keySelector, Func<T, K, N> newSelector)
    {
        if (_isOk && keySelector(_value!).IsOk(out var key))
        {
            return RefResult<N>.Ok(newSelector(_value!, key));
        }

        return RefResult<N>.Error(_error!);
    }
#endif

#endregion

#region IEnumerable

    [MustDisposeResource(false)]
    public RefResultEnumerator GetEnumerator() => new RefResultEnumerator(this);

    [PublicAPI]
    [MustDisposeResource(false)]
    public ref struct RefResultEnumerator
    {
        private readonly RefResult<T> _result;
        private bool _canYield;

        public T Current => _result.OkOrThrow();

        public RefResultEnumerator(RefResult<T> refResult)
        {
            _result = refResult;
            _canYield = _result._isOk;
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

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return false;
    }

    public override int GetHashCode()
    {
        return 0;
    }

    public override string ToString()
    {
        if (_isOk)
        {
            return $"Ok({_value.Stringify()})";
        }
        else
        {
            return $"Error({_error.Stringify()})";
        }
    }
}