// Prefix generic type parameter with T
#pragma warning disable CA1715

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
    IComparisonOperators<Result<T, E>, Result<T, E>, bool>,
    // IComparisonOperators<Result<T, E>, T, bool>,
    // IComparisonOperators<Result<T, E>, E, bool>,
#endif
    IEquatable<Result<T, E>>,
    // IEquatable<T>,
    // IEquatable<E>,
    IComparable<Result<T, E>>,
    // IComparable<T>,
    // IComparable<E>,
    IEnumerable<T>
{
#region Operators

    /// <summary>
    /// Implicitly convert a <see cref="Result{T,E}"/> into <c>true</c> if it is <c>Ok</c> and <c>false</c> if it is <c>Error</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator bool(Result<T, E> result) => result._isOk;

    /// <summary>
    /// Implicitly convert a <see cref="Result{T,E}"/> into <c>true</c> if it is <c>Ok</c> and <c>false</c> if it is <c>Error</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator true(Result<T, E> result) => result._isOk;

    /// <summary>
    /// Implicitly convert a <see cref="Result{T,E}"/> into <c>true</c> if it is <c>Ok</c> and <c>false</c> if it is <c>Error</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator false(Result<T, E> result) => !result._isOk;

    /// <summary>
    /// Implicitly convert a standalone <typeparamref name="T"/> <paramref name="value"/> to an
    /// <see cref="Result{T, E}.Ok"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Result<T, E>(T value) => Ok(value);

    /// <summary>
    /// Implicitly convert a standalone <typeparamref name="E"/> <paramref name="value"/> to an
    /// <see cref="Result{T, E}.Error"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Result<T, E>(E value) => Error(value);

    /// <summary>
    /// Implicitly convert a standalone <see cref="Ok{T}"/> to an <see cref="Result{T, E}.Ok(T)"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Result<T, E>(Compat.Ok<T> ok) => Ok(ok._value);

    /// <summary>
    /// Implicitly convert a standalone <see cref="Error{E}"/> to an <see cref="Result{T, E}.Error(E)"/>
    /// </summary>
    public static implicit operator Result<T, E>(Compat.Error<E> error) => Error(error._value);

    // We pass equality and comparison down to T values

    public static bool operator ==(Result<T, E> left, Result<T, E> right) => left.Equals(right);
    public static bool operator !=(Result<T, E> left, Result<T, E> right) => !left.Equals(right);
    public static bool operator >(Result<T, E> left, Result<T, E> right) => left.CompareTo(right) > 0;
    public static bool operator >=(Result<T, E> left, Result<T, E> right) => left.CompareTo(right) >= 0;
    public static bool operator <(Result<T, E> left, Result<T, E> right) => left.CompareTo(right) < 0;
    public static bool operator <=(Result<T, E> left, Result<T, E> right) => left.CompareTo(right) <= 0;

    public static bool operator ==(Result<T, E> result, T? ok) => result.Equals(ok);
    public static bool operator !=(Result<T, E> result, T? ok) => !result.Equals(ok);
    public static bool operator >(Result<T, E> result, T? ok) => result.CompareTo(ok) > 0;
    public static bool operator >=(Result<T, E> result, T? ok) => result.CompareTo(ok) >= 0;
    public static bool operator <(Result<T, E> result, T? ok) => result.CompareTo(ok) < 0;
    public static bool operator <=(Result<T, E> result, T? ok) => result.CompareTo(ok) <= 0;

    public static bool operator ==(Result<T, E> result, E? error) => result.Equals(error);
    public static bool operator !=(Result<T, E> result, E? error) => !result.Equals(error);
    public static bool operator >(Result<T, E> result, E? error) => result.CompareTo(error) > 0;
    public static bool operator >=(Result<T, E> result, E? error) => result.CompareTo(error) >= 0;
    public static bool operator <(Result<T, E> result, E? error) => result.CompareTo(error) < 0;
    public static bool operator <=(Result<T, E> result, E? error) => result.CompareTo(error) <= 0;

#endregion

    /// <summary>
    /// Creates a new Ok <see cref="Result{T,E}"/>
    /// </summary>
    /// <param name="ok">The Ok value</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, E> Ok(T ok) => new Result<T, E>(true, ok, default);

    /// <summary>
    /// Creates a new Error <see cref="Result{T,E}"/>
    /// </summary>
    /// <param name="error">The Error value</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    /// <summary>
    /// Returns <c>true</c> if this Result is Ok<br/>
    /// </summary>
    /// <seealso href="https://doc.rust-lang.org/std/result/enum.Result.html#method.is_ok"/>
    public bool IsOk
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _isOk;
    }

    /// <summary>
    /// Returns <c>true</c> and <paramref name="value"/> if this Result is Ok
    /// </summary>
    /// <param name="value">
    /// If this is an Ok result, the Ok value, otherwise default(<typeparamref name="T"/>)
    /// </param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasOk([MaybeNullWhen(false)] out T value)
    {
        if (_isOk)
        {
            value = _value!;
            return true;
        }

        value = default!;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<T> HasOk() => _isOk ? Some(_value!) : None<T>();

    /// <summary>
    /// Returns <c>true</c> if this Result is Ok and the value inside of it matches a predicate
    /// </summary>
    /// <param name="okPredicate"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.is_ok_and"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOkAnd(Func<T, bool> okPredicate) => _isOk && okPredicate(_value!);

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T OkOrThrow(string? errorMessage = null)
    {
        if (_isOk)
            return _value!;
        throw (_error as Exception) ?? new InvalidOperationException(errorMessage ?? "This Result is not Ok");
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="fallbackOk"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Result.html#method.unwrap_or"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T OkOr(T fallbackOk)
    {
        if (_isOk)
            return _value!;
        return fallbackOk;
    }


    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Result.html#method.unwrap_or_default"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T? OkOrDefault()
    {
        if (_isOk)
            return _value!;
        return default(T);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="getOk"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Result.html#method.unwrap_or_else"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T OkOrElse(Func<T> getOk)
    {
        if (_isOk)
            return _value!;
        return getOk();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasOkOrError([MaybeNullWhen(false)] out T ok, [MaybeNullWhen(true)] out E error)
    {
        if (_isOk)
        {
            ok = _value!;
            error = _error;
            return true;
        }

        ok = _value;
        error = _error!;
        return false;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="map"></param>
    /// <typeparam name="TNewOk"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.map"/>
    public Result<TNewOk, E> OkSelect<TNewOk>(Func<T, TNewOk> map)
    {
        if (_isOk)
        {
            return Result<TNewOk, E>.Ok(map(_value!));
        }

        return Result<TNewOk, E>.Error(_error!);
    }

    public Result<TNewOk, E> OkSelect<TNewOk>(Func<T, Result<TNewOk, E>> map)
    {
        if (_isOk)
            return map(_value!);
        return Error<E>(_error!);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="mapOk"></param>
    /// <param name="defaultOk"></param>
    /// <typeparam name="TNewOk"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.map_or"/>
    public TNewOk OkSelectOr<TNewOk>(Func<T, TNewOk> mapOk, TNewOk defaultOk)
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
    /// <typeparam name="TNewOk"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Result.html#method.map_or_else"/>
    public TNewOk OkSelectOrElse<TNewOk>(Func<T, TNewOk> mapOk, Func<TNewOk> getOk)
    {
        if (_isOk)
        {
            return mapOk(_value!);
        }

        return getOk();
    }

#endregion

#region Error

    /// <summary>
    /// Returns <c>true</c> if this Result is Error<br/>
    /// </summary>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.is_err"/>
    public bool IsError
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !_isOk;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasError([MaybeNullWhen(false)] out E error)
    {
        if (!_isOk)
        {
            error = _error!;
            return true;
        }

        error = default!;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<E> HasError() => !_isOk ? Some(_error!) : None<E>();

    /// <summary>
    /// Returns <c>true</c> if this Result is Error and the value inside of it matches a predicate
    /// </summary>
    /// <param name="errorPredicate"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.is_err_and"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsErrorAnd(Func<E, bool> errorPredicate) => !_isOk && errorPredicate(_error!);

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.unwrap_err"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public E ErrorOrThrow(string? errorMessage = null)
    {
        if (!_isOk)
            return _error!;
        throw new InvalidOperationException(errorMessage ?? "This Result is not an Error");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public E ErrorOr(E error)
    {
        if (!_isOk)
            return _error!;
        return error;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public E? ErrorOrDefault()
    {
        if (!_isOk)
            return _error!;
        return default(E);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public E ErrorOrElse(Func<E> getError)
    {
        if (_isOk)
            return _error!;
        return getError();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasErrorOrOk([MaybeNullWhen(false)] out E error, [MaybeNullWhen(true)] out T ok)
    {
        if (!_isOk)
        {
            error = _error!;
            ok = _value;
            return true;
        }

        error = _error;
        ok = _value!;
        return false;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="mapError"></param>
    /// <typeparam name="TNewError"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.map_err"/>
    public Result<T, TNewError> ErrorSelect<TNewError>(Func<E, TNewError> mapError)
    {
        if (_isOk)
        {
            return Result<T, TNewError>.Ok(_value!);
        }

        return Result<T, TNewError>.Error(mapError(_error!));
    }

    public TNewError ErrorSelectOr<TNewError>(Func<E, TNewError> mapError, TNewError defaultError)
    {
        if (!_isOk)
        {
            return mapError(_error!);
        }

        return defaultError;
    }

    public TNewError ErrorSelectOrElse<TNewError>(Func<E, TNewError> mapError, Func<TNewError> getError)
    {
        if (!_isOk)
            return mapError(_error!);

        return getError();
    }

#endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Match<TResult>(Func<T, TResult> onOk, Func<E, TResult> onError)
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

#region Compare

    public int CompareTo(Result<T, E> result)
    {
        // An Ok compares as less than any Err
        // while two Ok or two Err compare their containing values

        if (_isOk)
        {
            if (result._isOk)
            {
                // compare ok values
                return Comparer<T>.Default.Compare(_value!, result._value!);
            }

            return -1; // my Ok is less than their Error
        }

        // i'm Error
        if (result._isOk)
        {
            return 1; // my Error is greater than their Ok
        }

        // compare err values
        return Comparer<E>.Default.Compare(_error!, result._error!);
    }

    public int CompareTo(T? ok)
    {
        if (_isOk)
        {
            return Comparer<T>.Default.Compare(_value!, ok!);
        }

        return 1; // my Err is greater than an Ok value
    }

    public int CompareTo(E? error)
    {
        if (!_isOk)
        {
            return Comparer<E>.Default.Compare(_error!, error!);
        }

        return -1; // my Ok is less than an Err value
    }

    public int CompareTo(object? obj)
    {
        return obj switch
        {
            Result<E, E> result => CompareTo(result),
            T ok => CompareTo(ok),
            E error => CompareTo(error),
            _ => 1, // null and unknown values sort before
        };
    }

#endregion

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

#endregion

#region LINQ + IEnumerable

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TNewOk, E> Select<TNewOk>(Func<T, TNewOk> selector)
    {
        if (_isOk)
            return Ok<TNewOk, E>(selector(_value!));
        return Error<TNewOk, E>(_error!);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<N, E> Select<N>(Func<T, Option<N>> selector)
    {
        if (_isOk && selector(_value!).HasSome(out var value))
        {
            return Result<N, E>.Ok(value);
        }
        return Error<N, E>(_error!);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<N, E> Select<N>(Func<T, Result<N, E>> selector)
    {
        if (_isOk)
        {
            return selector(_value!);
        }
        return Error<N, E>(_error!);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TNewOk, E> Select<TState, TNewOk>(TState state, Func<TState, T, TNewOk> selector)
    {
        if (_isOk)
            return Ok<TNewOk, E>(selector(state, _value!));
        return Error<TNewOk, E>(_error!);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TNewOk, E> SelectMany<TNewOk>(
        Func<T, Result<TNewOk, E>> newSelector)
    {
        if (_isOk)
        {
            return newSelector(_value!);
        }
        return Error<TNewOk, E>(_error!);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TNewOk, E> SelectMany<TKey, TNewOk>(
        Func<T, Result<TKey, E>> keySelector,
        Func<T, TKey, TNewOk> newSelector)
    {
        if (_isOk && keySelector(_value!).HasOk(out var key))
        {
            return Ok<TNewOk, E>(newSelector(_value!, key));
        }

        return Error<TNewOk, E>(_error!);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    [MustDisposeResource(false)]
    public ResultEnumerator GetEnumerator() => new ResultEnumerator(this);

    [MustDisposeResource(false)]
    [StructLayout(LayoutKind.Auto)]
    public struct ResultEnumerator : IEnumerator<T>, IEnumerator, IDisposable
    {
        private bool _canYield;
        private readonly T _value;

        object? IEnumerator.Current => _value;
        public T Current => _value;

        public ResultEnumerator(Result<T, E> result)
        {
            if (result._isOk)
            {
                _value = result._value!;
                _canYield = true;
            }
            else
            {
                _value = default!;
                _canYield = false;
            }
        }

        public bool MoveNext()
        {
            if (!_canYield)
                return false;
            _canYield = false;
            return true;
        }

        void IEnumerator.Reset() => throw new NotSupportedException();

        void IDisposable.Dispose()
        {
            // Do nothing
        }
    }

#endregion

    public override int GetHashCode()
    {
        if (_isOk)
            return Hasher.Hash<T>(_value);
        return Hasher.Hash<E>(_error);
    }

    public override string ToString()
    {
        if (_isOk)
            return $"Ok({_value})";
        return $"Error({_error})";
    }
}
