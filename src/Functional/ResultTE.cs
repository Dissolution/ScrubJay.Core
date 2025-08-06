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
    /* All listed interfaces are implemented, but cannot be declared because they may unify for some type parameter substitutions */
#if NET7_0_OR_GREATER
    IEqualityOperators<Result<T, E>, Result<T, E>, bool>,
    // IEqualityOperators<Result<T, E>, T, bool>,
    // IEqualityOperators<Result<T, E>, E, bool>,
#endif
    IEquatable<Result<T, E>>,
    // IEquatable<T>,
    // IEquatable<E>,
    IFormattable,
#if NET6_0_OR_GREATER
    ISpanFormattable,
#endif
    IEnumerable<T>
{
#region Operators

    /// <summary>
    /// Implicitly convert a <see cref="Result{T,E}"/> into <c>true</c> if it is <c>Ok</c> and <c>false</c> if it is <c>Error</c>
    /// </summary>
    public static implicit operator bool(Result<T, E> result) => result._isOk;

    /// <summary>
    /// Implicitly convert a standalone <see cref="Compat.Ok{T}"/> to an <see cref="Result{T, E}.Ok(T)"/>
    /// </summary>
    public static implicit operator Result<T, E>(Compat.Ok<T> ok) => Ok(ok.Value);

    /// <summary>
    /// Implicitly convert a standalone <see cref="Compat.Error{E}"/> to an <see cref="Result{T, E}.Error(E)"/>
    /// </summary>
    public static implicit operator Result<T, E>(Compat.Error<E> error) => Error(error._value);

    // We pass equality and comparison down to T values

    public static bool operator ==(Result<T, E> left, Result<T, E> right) => left.Equals(right);
    public static bool operator !=(Result<T, E> left, Result<T, E> right) => !left.Equals(right);
    public static bool operator ==(Result<T, E> result, T? ok) => result.Equals(ok);
    public static bool operator !=(Result<T, E> result, T? ok) => !result.Equals(ok);
    public static bool operator ==(Result<T, E> result, E? error) => result.Equals(error);
    public static bool operator !=(Result<T, E> result, E? error) => !result.Equals(error);

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
    /// or through implicit conversion from <see cref="Compat.Ok{T}"/> or <see cref="Compat.Error{E}"/>
    /// </summary>
    private Result(bool isOk, T? value, E? error)
    {
        _isOk = isOk;
        _value = value;
        _error = error;
    }

#region Ok

    public Option<T> IsOk()
    {
        if (_isOk)
            return Some(_value!);
        else
            return None<T>();
    }

    /// <summary>
    /// Returns <c>true</c> and <paramref name="value"/> if this Result is Ok
    /// </summary>
    /// <param name="value">
    /// If this is an Ok result, the Ok value, otherwise default(<typeparamref name="T"/>)
    /// </param>
    /// <returns></returns>
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

    public bool IsOkWithError([MaybeNullWhen(false)] out T ok, [MaybeNullWhen(true)] out E error)
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
    public bool IsOkAnd(Fn<T, bool> okPredicate) => _isOk && okPredicate(_value!);

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
    public T OkOrThrow(string? errorMessage = null)
    {
        if (_isOk)
            return _value!;
        if (_error is Exception ex)
            throw ex;
        throw new InvalidOperationException(errorMessage ?? ToString());
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="fallbackOk"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Result.html#method.unwrap_or"/>
    public T OkOr(T fallbackOk)
    {
        if (_isOk)
            return _value!;
        return fallbackOk;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="getOk"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Result.html#method.unwrap_or_else"/>
    public T OkOr(Fn<T> getOk)
    {
        if (_isOk)
            return _value!;
        return getOk();
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
    ///
    /// </summary>
    /// <param name="map"></param>
    /// <typeparam name="N"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.map"/>
    public Result<N, E> OkSelect<N>(Fn<T, N> map)
    {
        if (_isOk)
        {
            return Result<N, E>.Ok(map(_value!));
        }

        return Result<N, E>.Error(_error!);
    }

    public Result<N, E> OkSelect<N>(Fn<T, Result<N, E>> map)
    {
        if (_isOk)
            return map(_value!);
        return Result<N, E>.Error(_error!);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="mapOk"></param>
    /// <param name="defaultOk"></param>
    /// <typeparam name="N"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.map_or"/>
    public N OkSelectOr<N>(Fn<T, N> mapOk, N defaultOk)
    {
        if (_isOk)
        {
            return mapOk(_value!);
        }

        return defaultOk;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="mapOk"></param>
    /// <param name="getOk"></param>
    /// <typeparam name="N"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Result.html#method.map_or_else"/>
    public N OkSelectOrElse<N>(Fn<T, N> mapOk, Fn<N> getOk)
    {
        if (_isOk)
        {
            return mapOk(_value!);
        }

        return getOk();
    }

#endregion

#region Error

    public Option<E> IsError()
    {
        if (!_isOk)
            return Some(_error!);
        else
            return None<E>();
    }

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

    public bool IsErrorWithOk([MaybeNullWhen(false)] out E error, [MaybeNullWhen(true)] out T ok)
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
    public bool IsErrorAnd(Fn<E, bool> errorPredicate) => !_isOk && errorPredicate(_error!);


    public E ErrorOr(E error)
    {
        if (!_isOk)
            return _error!;
        return error;
    }


    public E ErrorOr(Fn<E> getError)
    {
        if (_isOk)
            return _error!;
        return getError();
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
    public E ErrorOrThrow()
    {
        if (!_isOk)
            return _error!;
        throw new InvalidOperationException(ToString());
    }

#endregion


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


    public R Match<R>(Fn<T, R> onOk, Fn<E, R> onError)
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

#region Equal

    public bool Equals(Result<T, E> result)
    {
        if (_isOk)
        {
            if (result._isOk)
            {
                return EqualityComparer<T>.Default.Equals(_value!, result._value!);
            }

            return false;
        }

        if (result._isOk)
        {
            return false;
        }

        return EqualityComparer<E>.Default.Equals(_error!, result._error!);
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
            return Hasher.Hash<T>(_value);
        return Hasher.Hash<E>(_error);
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
            builder.Append("Error(")
                .Format(_error, format, provider)
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
        if (_isOk)
        {
            return new TryFormatWriter(destination)
            {
                "Ok(",
                {
                    _value, format, provider
                },
                ')',
            }.Wrote(out charsWritten);
        }
        else
        {
            return new TryFormatWriter(destination)
            {
                "Error(",
                {
                    _error, format, provider
                },
                ')',
            }.Wrote(out charsWritten);
        }
    }

    public override string ToString()
    {
        if (_isOk)
        {
            return $"Ok({_value})";
        }
        else
        {
            return $"Error({_error})";
        }
    }

#endregion


#region LINQ + IEnumerable

    public Result<N, E> Select<N>(Fn<T, N> selector)
    {
        if (_isOk)
        {
            return new(true, selector(_value!), default);
        }

        return new(false, default, _error);
    }


    public Result<N, E> Select<N>(Fn<T, Option<N>> selector)
    {
        if (_isOk && selector(_value!).IsSome(out var value))
        {
            return new(true, value, default);
        }

        return new(false, default, _error);
    }


    public Result<N, E> Select<N>(Fn<T, Result<N, E>> selector)
    {
        if (_isOk)
        {
            return selector(_value!);
        }

        return new(false, default, _error);
    }


    public Result<N, E> Select<X, N>(X state, Fn<X, T, N> selector)
    {
        if (_isOk)
        {
            return new(true, selector(state, _value!), default);
        }

        return new(false, default, _error);
    }


    public Result<N, E> SelectMany<N>(Fn<T, Result<N, E>> newSelector)
    {
        if (_isOk)
        {
            return newSelector(_value!);
        }

        return new(false, default, _error);
    }


    public Result<N, E> SelectMany<K, N>(
        Fn<T, Result<K, E>> keySelector,
        Fn<T, K, N> newSelector)
    {
        if (_isOk && keySelector(_value!).IsOk(out var key))
        {
            return new(true, newSelector(_value!, key), default);
        }

        return new(false, default, _error);
    }

#region IEnumerable

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    [MustDisposeResource(false)]
    public ResultEnumerator GetEnumerator() => new ResultEnumerator(this);

    [PublicAPI]
    [MustDisposeResource(false)]
    public sealed class ResultEnumerator : IEnumerator<T>, IEnumerator, IDisposable
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

#endregion
}