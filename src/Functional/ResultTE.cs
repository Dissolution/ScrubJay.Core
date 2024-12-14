#pragma warning disable CA1710, MA0048, CA1000

namespace ScrubJay.Functional;

/// <summary>
/// <c>Result&lt;TOk, TError&gt;</c> is the type used for returning and propagating errors<br/>
/// It has two variants:<br/>
/// <see cref="Ok"/>, representing success and containing a <typeparamref name="TOk"/> ok value<br/>
/// <see cref="Error"/>, representing error and containing a <typeparamref name="TError"/> error value<br/>
/// </summary>
/// <typeparam name="TOk">The generic <see cref="Type"/> for an Ok value</typeparam>
/// <typeparam name="TError">The generic <see cref="Type"/> for an Error value</typeparam>
/// <remarks>
/// Heavily inspired by Rust's Result type<br/>
/// <a href="https://doc.rust-lang.org/std/result/"/><br/>
/// <a href="https://doc.rust-lang.org/std/result/enum.Result.html"/><br/>
/// </remarks>
[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly struct Result<TOk, TError> :
/* All listed interfaces are implemented, but cannot be declared because they may unify for some type parameter substitutions */
#if NET7_0_OR_GREATER
    IEqualityOperators<Result<TOk, TError>, Result<TOk, TError>, bool>,
    // IEqualityOperators<Result<TOk, TError>, TOk, bool>,
    // IEqualityOperators<Result<TOk, TError>, TError, bool>,
#endif
    IEquatable<Result<TOk, TError>>,
    // IEquatable<TOk>,
    // IEquatable<TError>,
#if NET7_0_OR_GREATER
    IComparisonOperators<Result<TOk, TError>, Result<TOk, TError>, bool>,
    // IComparisonOperators<Result<TOk, TError>, TOk, bool>,
    // IComparisonOperators<Result<TOk, TError>, TError, bool>,
#endif
    IComparable<Result<TOk, TError>>,
    // IComparable<TOk>,
    // IComparable<TError>,
    IEnumerable<TOk>
{
#region Operators

    /// <summary>
    /// Implicitly convert a <see cref="Result{TOk,TError}"/> into <c>true</c> if it is <see cref="Ok"/> and <c>false</c> if it is <see cref="Error"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator bool(Result<TOk, TError> result) => result._isOk;

    /// <summary>
    /// Implicitly convert a <see cref="Result{TOk,TError}"/> into <c>true</c> if it is <see cref="Ok"/> and <c>false</c> if it is <see cref="Error"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator true(Result<TOk, TError> result) => result._isOk;

    /// <summary>
    /// Implicitly convert a <see cref="Result{TOk,TError}"/> into <c>true</c> if it is <see cref="Ok"/> and <c>false</c> if it is <see cref="Error"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator false(Result<TOk, TError> result) => !result._isOk;

    /// <summary>
    /// Implicitly convert a standalone <typeparamref name="TOk"/> <paramref name="value"/> to an
    /// <see cref="Result{TOk, TError}.Ok"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Result<TOk, TError>(TOk value) => Ok(value);

    /// <summary>
    /// Implicitly convert a standalone <typeparamref name="TError"/> <paramref name="value"/> to an
    /// <see cref="Result{TOk, TError}.Error"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Result<TOk, TError>(TError value) => Error(value);

    /// <summary>
    /// Implicitly convert a standalone <see cref="Ok{TOk}"/> to an <see cref="Result{TOk, TError}.Ok"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Result<TOk, TError>(Ok<TOk> ok) => Ok(ok.Value);

    /// <summary>
    /// Implicitly convert a standalone <see cref="Error{TError}"/> to an <see cref="Result{TOk, TError}.Error"/>
    /// </summary>
    public static implicit operator Result<TOk, TError>(Error<TError> error) => Error(error.Value);

    // We pass equality and comparison down to T values

    public static bool operator ==(Result<TOk, TError> left, Result<TOk, TError> right) => left.Equals(right);
    public static bool operator !=(Result<TOk, TError> left, Result<TOk, TError> right) => !left.Equals(right);
    public static bool operator >(Result<TOk, TError> left, Result<TOk, TError> right) => left.CompareTo(right) > 0;
    public static bool operator >=(Result<TOk, TError> left, Result<TOk, TError> right) => left.CompareTo(right) >= 0;
    public static bool operator <(Result<TOk, TError> left, Result<TOk, TError> right) => left.CompareTo(right) < 0;
    public static bool operator <=(Result<TOk, TError> left, Result<TOk, TError> right) => left.CompareTo(right) <= 0;

    public static bool operator ==(Result<TOk, TError> result, TOk? ok) => result.Equals(ok);
    public static bool operator !=(Result<TOk, TError> result, TOk? ok) => !result.Equals(ok);
    public static bool operator >(Result<TOk, TError> result, TOk? ok) => result.CompareTo(ok) > 0;
    public static bool operator >=(Result<TOk, TError> result, TOk? ok) => result.CompareTo(ok) >= 0;
    public static bool operator <(Result<TOk, TError> result, TOk? ok) => result.CompareTo(ok) < 0;
    public static bool operator <=(Result<TOk, TError> result, TOk? ok) => result.CompareTo(ok) <= 0;

    public static bool operator ==(Result<TOk, TError> result, TError? error) => result.Equals(error);
    public static bool operator !=(Result<TOk, TError> result, TError? error) => !result.Equals(error);
    public static bool operator >(Result<TOk, TError> result, TError? error) => result.CompareTo(error) > 0;
    public static bool operator >=(Result<TOk, TError> result, TError? error) => result.CompareTo(error) >= 0;
    public static bool operator <(Result<TOk, TError> result, TError? error) => result.CompareTo(error) < 0;
    public static bool operator <=(Result<TOk, TError> result, TError? error) => result.CompareTo(error) <= 0;

#endregion

    /// <summary>
    /// Creates a new Ok <see cref="Result{T,E}"/>
    /// </summary>
    /// <param name="ok">The Ok value</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TOk, TError> Ok(TOk ok) => new Result<TOk, TError>(true, ok, default);

    /// <summary>
    /// Creates a new Error <see cref="Result{T,E}"/>
    /// </summary>
    /// <param name="error">The Error value</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TOk, TError> Error(TError error) => new Result<TOk, TError>(false, default, error);

    // is this Result.Ok?
    // default(Result) implies !_isOk, thus default(Result) == None
    private readonly bool _isOk;

    // if this is Result.Ok, the Ok Value
    private readonly TOk? _ok;

    // if this is Result.Error, the Error Value
    private readonly TError? _error;

    /// <summary>
    /// Result should only be constructed with <see cref="Ok"/>, <see cref="Error"/><br/>
    /// or implicitly cast from
    /// <typeparamref name="TOk"/>,
    /// <see cref="ScrubJay.Functional.Ok{TOk}"/>,
    /// <typeparamref name="TError"/>,
    /// or
    /// <see cref="ScrubJay.Functional.Error{TError}"/>
    /// </summary>
    private Result(bool isOk, TOk? ok, TError? error)
    {
        _isOk = isOk;
        _ok = ok;
        _error = error;
    }

#region Ok

    /// <summary>
    /// Returns <c>true</c> if this Result is Ok<br/>
    /// </summary>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.is_ok"/>
    public bool IsOk
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _isOk;
    }

    /// <summary>
    /// Returns <c>true</c> and <paramref name="ok"/> if this Result is Ok
    /// </summary>
    /// <param name="ok">
    /// If this is an Ok result, the Ok value, otherwise default(<typeparamref name="TOk"/>)
    /// </param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasOk([MaybeNullWhen(false)] out TOk ok)
    {
        if (_isOk)
        {
            ok = _ok!;
            return true;
        }

        ok = default!;
        return false;
    }

    /// <summary>
    /// Returns <c>true</c> if this Result is Ok and the value inside of it matches a predicate
    /// </summary>
    /// <param name="okPredicate"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.is_ok_and"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOkAnd(Func<TOk, bool> okPredicate) => _isOk && okPredicate(_ok!);

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
    public TOk OkOrThrow(string? errorMessage = null)
    {
        if (_isOk)
            return _ok!;
        throw (_error as Exception) ?? new InvalidOperationException(errorMessage ?? "This Result is not Ok");
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="fallbackOk"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Result.html#method.unwrap_or"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TOk OkOr(TOk fallbackOk)
    {
        if (_isOk)
            return _ok!;
        return fallbackOk;
    }


    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Result.html#method.unwrap_or_default"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TOk? OkOrDefault()
    {
        if (_isOk)
            return _ok!;
        return default(TOk);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="getOk"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Result.html#method.unwrap_or_else"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TOk OkOrElse(Func<TOk> getOk)
    {
        if (_isOk)
            return _ok!;
        return getOk();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasOkOrError([MaybeNullWhen(false)] out TOk ok, [MaybeNullWhen(true)] out TError error)
    {
        if (_isOk)
        {
            ok = _ok!;
            error = _error;
            return true;
        }

        ok = _ok;
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
    public Result<TNewOk, TError> OkSelect<TNewOk>(Func<TOk, TNewOk> map)
    {
        if (_isOk)
        {
            return Result<TNewOk, TError>.Ok(map(_ok!));
        }

        return Result<TNewOk, TError>.Error(_error!);
    }

    public Result<TNewOk, TError> OkSelect<TNewOk>(Func<TOk, Result<TNewOk, TError>> map)
    {
        if (_isOk)
            return map(_ok!);
        return _error!;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="mapOk"></param>
    /// <param name="defaultOk"></param>
    /// <typeparam name="TNewOk"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.map_or"/>
    public TNewOk OkSelectOr<TNewOk>(Func<TOk, TNewOk> mapOk, TNewOk defaultOk)
    {
        if (_isOk)
        {
            return mapOk(_ok!);
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
    public TNewOk OkSelectOrElse<TNewOk>(Func<TOk, TNewOk> mapOk, Func<TNewOk> getOk)
    {
        if (_isOk)
        {
            return mapOk(_ok!);
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
    public bool HasError([MaybeNullWhen(false)] out TError error)
    {
        if (!_isOk)
        {
            error = _error!;
            return true;
        }

        error = default!;
        return false;
    }

    /// <summary>
    /// Returns <c>true</c> if this Result is Error and the value inside of it matches a predicate
    /// </summary>
    /// <param name="errorPredicate"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.is_err_and"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsErrorAnd(Func<TError, bool> errorPredicate) => !_isOk && errorPredicate(_error!);

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.unwrap_err"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TError ErrorOrThrow(string? errorMessage = null)
    {
        if (!_isOk)
            return _error!;
        throw new InvalidOperationException(errorMessage ?? "This Result is not an Error");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TError ErrorOr(TError error)
    {
        if (!_isOk)
            return _error!;
        return error;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TError? ErrorOrDefault()
    {
        if (!_isOk)
            return _error!;
        return default(TError);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TError ErrorOrElse(Func<TError> getError)
    {
        if (_isOk)
            return _error!;
        return getError();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasErrorOrOk([MaybeNullWhen(false)] out TError error, [MaybeNullWhen(true)] out TOk ok)
    {
        if (!_isOk)
        {
            error = _error!;
            ok = _ok;
            return true;
        }

        error = _error;
        ok = _ok!;
        return false;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="mapError"></param>
    /// <typeparam name="TNewError"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.map_err"/>
    public Result<TOk, TNewError> ErrorSelect<TNewError>(Func<TError, TNewError> mapError)
    {
        if (_isOk)
        {
            return Result<TOk, TNewError>.Ok(_ok!);
        }

        return Result<TOk, TNewError>.Error(mapError(_error!));
    }

    public TNewError ErrorSelectOr<TNewError>(Func<TError, TNewError> mapError, TNewError defaultError)
    {
        if (!_isOk)
        {
            return mapError(_error!);
        }

        return defaultError;
    }

    public TNewError ErrorSelectOrElse<TNewError>(Func<TError, TNewError> mapError, Func<TNewError> getError)
    {
        if (!_isOk)
            return mapError(_error!);

        return getError();
    }

#endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Match(Action<TOk> onOk, Action<TError> onError)
    {
        if (_isOk)
        {
            onOk(_ok!);
        }
        else
        {
            onError(_error!);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TResult Match<TResult>(Func<TOk, TResult> onOk, Func<TError, TResult> onError)
    {
        if (_isOk)
        {
            return onOk(_ok!);
        }
        else
        {
            return onError(_error!);
        }
    }

    public Option<TOk> AsOption()
    {
        if (_isOk)
        {
            return Option<TOk>.Some(_ok!);
        }
        else
        {
            return Option<TOk>.None();
        }
    }

#region Compare

    public int CompareTo(Result<TOk, TError> result)
    {
        // An Ok compares as less than any Err
        // while two Ok or two Err compare their containing values

        if (_isOk)
        {
            if (result._isOk)
            {
                // compare ok values
                return Comparer<TOk>.Default.Compare(_ok!, result._ok!);
            }

            return -1; // my Ok is less than their Error
        }

        // i'm Error
        if (result._isOk)
        {
            return 1; // my Error is greater than their Ok
        }

        // compare err values
        return Comparer<TError>.Default.Compare(_error!, result._error!);
    }

    public int CompareTo(TOk? ok)
    {
        if (_isOk)
        {
            return Comparer<TOk>.Default.Compare(_ok!, ok!);
        }

        return 1; // my Err is greater than an Ok value
    }

    public int CompareTo(TError? error)
    {
        if (!_isOk)
        {
            return Comparer<TError>.Default.Compare(_error!, error!);
        }

        return -1; // my Ok is less than an Err value
    }

    public int CompareTo(object? obj)
    {
        return obj switch
        {
            Result<TError, TError> result => CompareTo(result),
            TOk ok => CompareTo(ok),
            TError error => CompareTo(error),
            _ => 1, // null and unknown values sort before
        };
    }

#endregion

#region Equal

    public bool Equals(Result<TOk, TError> result)
    {
        if (_isOk)
        {
            if (result._isOk)
            {
                return EqualityComparer<TOk>.Default.Equals(_ok!, result._ok!);
            }

            return false;
        }

        if (result._isOk)
        {
            return false;
        }

        return EqualityComparer<TError>.Default.Equals(_error!, result._error!);
    }

    public bool Equals(TOk? ok)
    {
        if (_isOk)
        {
            return EqualityComparer<TOk>.Default.Equals(_ok!, ok!);
        }

        return false;
    }

    public bool Equals(TError? error)
    {
        if (!_isOk)
        {
            return EqualityComparer<TError>.Default.Equals(_error!, error!);
        }

        return false;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj switch
        {
            Result<TError, TError> result => Equals(result),
            TOk ok => Equals(ok),
            TError error => Equals(error),
            bool isOk => _isOk == isOk,
            _ => false,
        };

#endregion

#region LINQ + IEnumerable

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TNewOk, TError> Select<TNewOk>(Func<TOk, TNewOk> selector)
    {
        if (_isOk)
            return Result<TNewOk, TError>.Ok(selector(_ok!));
        return Result<TNewOk, TError>.Error(_error!);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TNewOk, TError> SelectMany<TNewOk>(
        Func<TOk, Result<TNewOk, TError>> newSelector)
    {
        if (_isOk)
        {
            return newSelector(_ok!);
        }
        return Result<TNewOk, TError>.Error(_error!);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<TNewOk, TError> SelectMany<TKey, TNewOk>(
        Func<TOk, Result<TKey, TError>> keySelector,
        Func<TOk, TKey, TNewOk> newSelector)
    {
        if (_isOk && keySelector(_ok!).HasOk(out var key))
        {
            return Result<TNewOk, TError>.Ok(newSelector(_ok!, key));
        }

        return Result<TNewOk, TError>.Error(_error!);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<TOk> IEnumerable<TOk>.GetEnumerator() => GetEnumerator();

    [MustDisposeResource(false)]
    public ResultEnumerator GetEnumerator() => new ResultEnumerator(this);

    [MustDisposeResource(false)]
    [StructLayout(LayoutKind.Auto)]
    public struct ResultEnumerator : IEnumerator<TOk>, IEnumerator, IDisposable
    {
        private bool _canYield;
        private readonly TOk _value;

        object? IEnumerator.Current => _value;
        public TOk Current => _value;

        public ResultEnumerator(Result<TOk, TError> result)
        {
            if (result._isOk)
            {
                _value = result._ok!;
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
            return Hasher.GetHashCode<TOk>(_ok);
        return Hasher.GetHashCode<TError>(_error);
    }

    public override string ToString()
    {
        if (_isOk)
            return $"Ok({_ok})";
        return $"Error({_error})";
    }
}
