// Prefix generic type parameter with T
// Do not declare static methods on generic types
// Do not catch Exception

using ScrubJay.Text.Rendering;

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
/// 🦀 Heavily inspired by Rust's Result type 🦀
/// </remarks>
/// <seealso href="https://doc.rust-lang.org/std/result/enum.Result.html"/>
[PublicAPI]
[AsyncMethodBuilder(typeof(ResultAsyncMethodBuilder<>))]
[StructLayout(LayoutKind.Auto)]
public readonly struct Result<T> :
    /* All commented out interfaces are implemented, but cannot be declared per CS0695
     * 'Result<T>' cannot implement both 'X' and 'Y' because they may unify for some type parameter substitutions
     */
#if NET7_0_OR_GREATER
    IEqualityOperators<Result<T>, Result<T>, bool>,
    IEqualityOperators<Result<T>, T, bool>,
#endif
    IEquatable<Result<T>>,
    IEquatable<T>,
    IEquatable<Compat.Ok<T>>,
    //IEquatable<bool>,
    IFormattable,
#if NET6_0_OR_GREATER
    ISpanFormattable,
#endif
    IEnumerable<T>
{
#region Operators

    /// <summary>
    /// Implicitly convert a <see cref="Result{T}"/> into <c>true</c> if it is <c>Ok</c> and <c>false</c> if it is <c>Error</c>
    /// </summary>
    public static implicit operator bool(Result<T> result) => result._isOk;

    /// <summary>
    /// Implicitly convert a <typeparamref name="T"/> <paramref name="value"/> to a <see cref="Result{T}.Ok"/>
    /// </summary>
    public static implicit operator Result<T>(T value) => Ok(value);

    /// <summary>
    /// Implicitly convert an <see cref="Exception"/> to a <see cref="Result{T}.Error"/>
    /// </summary>
    public static implicit operator Result<T>(Exception ex) => Error(ex);

    /// <summary>
    /// Implicitly convert a standalone <see cref="Compat.Ok{T}"/> to an <see cref="Result{T}.Ok(T)"/>
    /// </summary>
    public static implicit operator Result<T>(Compat.Ok<T> ok) => Ok(ok.Value);

    /// <summary>
    /// Implicitly convert a standalone <see cref="Compat.Error{E}"/> to an <see cref="Result{T}.Error"/>
    /// </summary>
    public static implicit operator Result<T>(Compat.Error<Exception> error) => Error(error._value);

    // We pass equality down to T values

    public static bool operator ==(Result<T> left, Result<T> right) => left.Equals(right);
    public static bool operator !=(Result<T> left, Result<T> right) => !left.Equals(right);
    public static bool operator ==(Result<T> result, T? value) => result.Equals(value);
    public static bool operator !=(Result<T> result, T? value) => !result.Equals(value);
    public static bool operator ==(T? value, Result<T> result) => result.Equals(value);
    public static bool operator !=(T? value, Result<T> result) => !result.Equals(value);

#endregion


    /// <summary>
    /// Creates <see cref="Result{T}"/>.Ok(<paramref name="value"/>)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Ok(T value) => new Result<T>(true, value, null);

    /// <summary>
    /// Creates <see cref="Result{T}"/>.Error(<paramref name="ex"/>)
    /// </summary>
    public static Result<T> Error(Exception ex) => new Result<T>(false, default, ex);

    /* These exact fields were chosen so that
     * default(Result<>) would be a failure (same as default(bool) == false)
     */

    private readonly bool       _isOk;
    private readonly T?         _value;
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


    public bool IsOk([MaybeNullWhen(false)] out T value)
    {
        value = _value;
        return _isOk;
    }


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
        throw new InvalidOperationException(ToString());
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
    public T OkOr(Fn<T> getFallback)
    {
        if (_isOk)
        {
            return _value!;
        }
        return getFallback();
    }


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


    public bool IsError([MaybeNullWhen(false)] out Exception error)
    {
        error = _error;
        return !_isOk;
    }


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
            throw new InvalidOperationException(ToString());
        }
    }

#endregion


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


    public R Match<R>(Fn<T, R> onOk, Fn<Exception, R> onError)
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
            Equate.Values(_value!, result._value!);
    }

    public bool Equals(Compat.Ok<T> ok)
        => _isOk && Equate.Values(_value, ok.Value);

    public bool Equals(T? value)
        => _isOk && Equate.Values(_value, value);

    public bool Equals(T? value, IEqualityComparer<T>? valueComparer)
        => _isOk && Equate.Values(_value, value, valueComparer);

    public bool Equals<E>(Compat.Error<E> _) => !_isOk;

    public bool Equals(Exception? _) => !_isOk;

    public bool Equals(bool isOk) => _isOk == isOk;

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj switch
        {
            Result<T> result => Equals(result),
            T value => Equals(value),
            Exception ex => Equals(ex),
            bool isOk => Equals(isOk),
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
        using var builder = new TextBuilder();

        if (_isOk)
        {
            builder.Append("Ok(")
                .Format(_value, format, provider)
                .Append(')');
        }
        else
        {
            builder.Render(_error?.GetType())
                .Append('(')
                .Append(_error?.Message)
                .Append(')');
        }

        return builder.ToString();
    }

    public bool TryFormat(
        Span<char> destination,
        out int charsWritten,
        text format = default,
        IFormatProvider? provider = null)
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
            writer.Add(_error?.RenderType());
            writer.Add('(');
            writer.Add(_error?.Message);
            writer.Add(')');
        }

        return writer.Wrote(out charsWritten);
    }

    public override string ToString() => ToString(null);

#endregion


#region Linq + Async duck typing

    public Result<N> Select<N>(Fn<T, N> selector)
    {
        if (_isOk)
        {
            return new(true, selector(_value!), null);
        }
        return new(false, default, _error);
    }

    public Result<N> Select<N>(Fn<T, Option<N>> selector)
    {
        if (_isOk && selector(_value!).IsSome(out var value))
        {
            return new(true, value, null);
        }
        return new(false, default, _error);
    }

    public Result<N> Select<N>(Fn<T, Result<N>> selector)
    {
        if (_isOk)
        {
            return selector(_value!);
        }
        return new(false, default, _error);
    }

    public Result<N> Select<X, N>(X state, Fn<X, T, N> selector)
    {
        if (_isOk)
        {
            return new(true, selector(state, _value!), null);
        }
        return new(false, default, _error);
    }

    public Result<N> SelectMany<N>(Fn<T, Result<N>> newSelector)
    {
        if (_isOk)
        {
            return newSelector(_value!);
        }
        return new(false, default, _error);
    }

    public Result<N> SelectMany<K, N>(Fn<T, Result<K>> keySelector, Fn<T, K, N> newSelector)
    {
        if (_isOk && keySelector(_value!).IsOk(out var key))
        {
            return new(true, newSelector(_value!, key), null);
        }

        return new(false, default, _error);
    }

    /// <summary>
    /// Support for <c>await</c> syntax in order to support early return from <c>async</c> methods
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ResultAwaiter<T> GetAwaiter() => new(this);

#region IEnumerable

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    [MustDisposeResource(false)]
    public ResultEnumerator GetEnumerator() => new ResultEnumerator(this);

    [PublicAPI]
    [MustDisposeResource(false)]
    public readonly struct ResultEnumerator : IEnumerator<T>, IEnumerator, IDisposable
    {
        private readonly Result<T> _result;

        object? IEnumerator.Current => (object?)Current;

        public T Current => _result.OkOrThrow();

        public ResultEnumerator(Result<T> result)
        {
            _result = result;
        }

        void IDisposable.Dispose()
        {
            /* Do Nothing */
        }

        void IEnumerator.Reset()
        {
            /* Do Nothing */
        }

        public bool MoveNext() => _result;
    }

#endregion

#endregion
}
