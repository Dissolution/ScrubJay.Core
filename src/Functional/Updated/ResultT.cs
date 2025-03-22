// Prefix generic type parameter with T
// Do not declare static methods on generic types
// Do not catch Exception

#pragma warning disable CA1715, CA1000, CA1031


namespace ScrubJay.Functional;

/// <summary>
/// Represents the results of a function - <c>(?) -> T</c><br/>
/// imitating a discriminated union like:
/// <code>
/// Result
/// {
///     Ok(T),
///     Error(Exception),
/// }
/// </code>
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// This result type also uses c# syntax to be more fluent<br/>
/// TODO!!!
/// 🦀 Heavily inspired by Rust's Result type 🦀
/// </remarks>
/// <seealso href="https://doc.rust-lang.org/std/result/enum.Result.html"/>
[PublicAPI]
#if !NETFRAMEWORK && !NETSTANDARD2_0
[AsyncMethodBuilder(typeof(ResultAsyncMethodBuilder<>))]
#endif
[StructLayout(LayoutKind.Auto)]
public readonly struct Result<T> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Result<T>, Result<T>, bool>,
    IEqualityOperators<Result<T>, T, bool>,
#endif
    IEquatable<Result<T>>,
    IEquatable<T>,
    IFormattable,
    ISpanFormattable,
    IEnumerable<T>
{
#region Operators

    /// <summary>
    /// Implicitly convert a <see cref="Result{T}"/> into <c>true</c> if it is <c>Ok</c> and <c>false</c> if it is <c>Error</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator bool(Result<T> result) => result._isOk;

    /// <summary>
    /// Implicitly convert an <see cref="Exception"/> to a <see cref="Result{T}.Error"/>
    /// </summary>
    /// <remarks>
    /// This exists (but the equivalent implicit convert from T does not) only to support
    /// implicit conversions from supertypes
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Result<T>(Exception ex) => Error(ex);

    /// <summary>
    /// Implicitly convert a standalone <see cref="Ok{T}"/> to an <see cref="Result{T}.Ok(T)"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Result<T>(Compat.Ok<T> ok) => Ok(ok._value);

    /// <summary>
    /// Implicitly convert a standalone <see cref="Error{E}"/> to an <see cref="Result{T}.Error(E)"/>
    /// </summary>
    public static implicit operator Result<T>(Compat.Error<Exception> error) => Error(error._value);

    // We pass equality down to T values

    public static bool operator ==(Result<T> left, Result<T> right) => left.Equals(right);

    public static bool operator !=(Result<T> left, Result<T> right) => !left.Equals(right);

    public static bool operator ==(Result<T> result, T? value) => result.Equals(value);

    public static bool operator !=(Result<T> result, T? value) => !result.Equals(value);

#endregion

    private static InvalidOperationException GetDefaultError()
        => new($"Result<{typeof(T).NameOf()}>.Error");

    /// <summary>
    /// Creates a new Ok <see cref="Result{T}"/>
    /// </summary>
    /// <param name="ok">The Ok value</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Ok(T ok) => new Result<T>(true, ok, default);

    /// <summary>
    /// Creates a new Error <see cref="Result{T}"/>
    /// </summary>
    /// <param name="ex">The Error value</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Error(Exception ex) => new Result<T>(false, default, ex);


    // is this Result.Ok?
    // default(Result) implies !_isOk, thus default(Result) == None)]
    private readonly bool _isOk;

    // if this is Result.Ok, the Ok Value
    private readonly T? _value;

    // if this is Result.Error, the Error Value
    private readonly Exception? _error;

    /// <summary>
    /// Result may only be constructed through <see cref="Ok(T)"/>, <see cref="Error"/>,
    /// or through implicit conversion from <see cref="Compat.Ok{T}"/> or <see cref="Compat.Error{E}"/>
    /// </summary>
    private Result(bool isOk, T? value, Exception? error)
    {
        _isOk = isOk;
        _value = value;
        _error = error;
    }


#region Ok

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<T> IsOk()
    {
        if (_isOk)
        {
            return Some<T>(_value!);
        }
        else
        {
            return None<T>();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOk([MaybeNullWhen(false)] out T value)
    {
        value = _value;
        return _isOk;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOkWithError([MaybeNullWhen(false)] out T ok, [MaybeNullWhen(true)] out Exception error)
    {
        ok = _value;
        error = _error;
        return _isOk;
    }


    /// <summary>
    /// Returns <c>true</c> if this Result is Ok and the value inside of it matches a predicate
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.is_ok_and"/>
    public bool IsOkAnd(Fn<T, bool> predicate) => _isOk && predicate(_value!);


    [StackTraceHidden]
    public T OkOrThrow()
    {
        if (_isOk)
        {
            return _value!;
        }
        if (_error is not null)
        {
            throw _error;
        }
        throw GetDefaultError();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="fallback"></param>
    /// <returns></returns>
    /// <seealso href="https://doc.rust-lang.org/std/option/enum.Result.html#method.unwrap_or"/><br/>
    /// <seealso href="https://doc.rust-lang.org/std/option/enum.Result.html#method.unwrap_or_default"/><br/>
    public T OkOr(T fallback)
    {
        if (_isOk)
        {
            return _value!;
        }
        return fallback;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="getFallback"></param>
    /// <returns></returns>
    /// <seealso href="https://doc.rust-lang.org/std/option/enum.Result.html#method.unwrap_or_else"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T OkOr(Fn<T> getFallback)
    {
        if (_isOk)
        {
            return _value!;
        }
        return getFallback();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T? OkOrDefault()
    {
        if (_isOk)
        {
            return _value;
        }
        return default(T);
    }

#endregion

#region Error

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<Exception> IsError()
    {
        if (!_isOk)
        {
            return Some(_error!);
        }
        else
        {
            return None<Exception>();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsError([MaybeNullWhen(false)] out Exception error)
    {
        error = _error;
        return !_isOk;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsErrorWithOk([MaybeNullWhen(false)] out Exception error, [MaybeNullWhen(true)] out T ok)
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
    public bool IsErrorAnd(Fn<Exception, bool> errorPredicate) => !_isOk && errorPredicate(_error!);

    public Exception ErrorOr(Exception fallback)
    {
        if (!_isOk && _error is not null)
            return _error;
        return fallback;
    }

    public Exception ErrorOr(Fn<Exception> getFallback)
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
            throw GetDefaultError();
        }
    }


#endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Match<TResult>(Fn<T, TResult> onOk, Fn<Exception, TResult> onError)
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

    public Option<T> AsOption()
    {
        if (_isOk)
        {
            return Option<T>.Some(_value!);
        }
        else
        {
            return Option<T>.None();
        }
    }

#region Equality

    public bool Equals(Result<T> result)
    {
        if (!_isOk)
            return !result._isOk;
        return result._isOk &&
            EqualityComparer<T>.Default.Equals(_value!, result._value!);
    }

    public bool Equals(T? ok)
        => _isOk && EqualityComparer<T>.Default.Equals(_value!, ok!);

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj switch
        {
            Result<T> result => Equals(result),
            T ok => Equals(ok),
            Exception => !_isOk,
            bool isOk => (_isOk == isOk),
            _ => false,
        };


    public override int GetHashCode()
    {
        if (_isOk)
            return Hasher.Hash<T>(_value);
        return Hasher.EmptyHash;
    }

#endregion

    #region ToString / TryFormat

    public string ToString(string? format, IFormatProvider? provider = null)
    {
        Buffer<char> text = stackalloc char[256];
        if (_isOk)
        {
            text.Write("Ok(");
            text.Write(_value, format, provider);
            text.Write(')');
        }
        else
        {
            text.WriteTypeName(_error!.GetType());
            text.Write('(');
            text.Write(_error.Message);
            text.Write(')');
        }
        return text.ToStringAndDispose();
    }

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        text format = default,
        IFormatProvider? provider = default)
    {
        var writer = new TryFormatWriter(destination);
        if (_isOk)
        {
            writer.Add("Ok(");
            writer.Add(_value, format, provider);
            writer.Add(')');
        }
        else
        {
            writer.Add(_error!.GetType().NameOf());
            writer.Add('(');
            writer.Add(_error.Message);
            writer.Add(')');
        }
        return writer.GetResult(out charsWritten);
    }

    public override string ToString() => ToString(default, default);
#endregion


#region LINQ

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<N> Select<N>(Fn<T, N> selector)
    {
        if (_isOk)
        {
            return new(true, selector(_value!), default);
        }
        return new(false, default, _error);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<N> Select<N>(Fn<T, Option<N>> selector)
    {
        if (_isOk && selector(_value!).HasSome(out var value))
        {
            return new(true, value, default);
        }
        return new(false, default, _error);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<N> Select<N>(Fn<T, Result<N>> selector)
    {
        if (_isOk)
        {
            return selector(_value!);
        }
        return new(false, default, _error);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<N> Select<X, N>(X state, Fn<X, T, N> selector)
    {
        if (_isOk)
        {
            return new(true, selector(state, _value!), default);
        }
        return new(false, default, _error);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<N> SelectMany<N>(Fn<T, Result<N>> newSelector)
    {
        if (_isOk)
        {
            return newSelector(_value!);
        }
        return new(false, default, _error);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<N> SelectMany<K, N>(Fn<T, Result<K>> keySelector, Fn<T, K, N> newSelector)
    {
        if (_isOk && keySelector(_value!).IsOk(out var key))
        {
            return new(true, newSelector(_value!, key), default);
        }

        return new(false, default, _error);
    }

    /// <summary>
    /// Support for <c>await</c> syntax in order to support early-return / short-circuiting
    /// </summary>
    /// <returns></returns>
    public ResultAwaiter<T> GetAwaiter() => new(this);

#region IEnumerable
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    [MustDisposeResource(false)]
    public ResultEnumerator GetEnumerator() => new ResultEnumerator(this);

    [PublicAPI]
    [MustDisposeResource(false)]
    public sealed class ResultEnumerator : IEnumerator<T>, IEnumerator, IDisposable
    {
        private readonly Result<T> _result;

        private bool _canYield;

        object? IEnumerator.Current => (object?)Current;

        public T Current
        {
            get
            {
                Throw.IfBadEnumerationState(!_canYield);
                return _result.OkOrThrow();
            }
        }

        public ResultEnumerator(Result<T> result)
        {
            _result = result;
            _canYield = result._isOk;
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
            _canYield = true;
        }

        void IDisposable.Dispose()
        {
            /* Do Nothing */
        }
    }
#endregion
#endregion
}
