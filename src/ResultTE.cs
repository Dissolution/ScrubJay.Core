namespace ScrubJay;

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
    //IEqualityOperators<Result<T, E>, T, bool>,
    //IEqualityOperators<Result<T, E>, E, bool>,
#endif
    IEquatable<Result<TOk, TError>>,
    //IEquatable<T>,
    //IEquatable<E>,
#if NET7_0_OR_GREATER
    IComparisonOperators<Result<TOk, TError>, Result<TOk, TError>, bool>,
    //IComparisonOperators<Result<T, E>, T, bool>,
    //IComparisonOperators<Result<T, E>, E, bool>,
#endif
    IComparable<Result<TOk, TError>>,
    //IComparable<T>,
    //IComparable<E>,
    IEnumerable<TOk>,
    IEnumerable
{
#region Operators

    public static implicit operator bool(Result<TOk, TError> result) => result._isOk;
    public static implicit operator Result<TOk, TError>(TOk ok) => Ok(ok);
    public static implicit operator Result<TOk, TError>(TError error) => Error(error);

    public static bool operator true(Result<TOk, TError> result) => result._isOk;
    public static bool operator false(Result<TOk, TError> result) => !result._isOk;

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
    public static Result<TOk, TError> Ok(TOk ok) => new Result<TOk, TError>(true, ok, default);

    /// <summary>
    /// Creates a new Error <see cref="Result{T,E}"/>
    /// </summary>
    /// <param name="error">The Error value</param>
    /// <returns></returns>
    public static Result<TOk, TError> Error(TError error) => new Result<TOk, TError>(false, default, error);


    private readonly bool _isOk;
    private readonly TOk? _ok;
    private readonly TError? _error;

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
    public bool IsOk() => _isOk;

    /// <summary>
    /// Returns <c>true</c> and <paramref name="ok"/> if this Result is Ok
    /// </summary>
    /// <param name="ok">
    /// If this is an Ok result, the Ok value, otherwise default(<typeparamref name="TOk"/>)
    /// </param>
    /// <returns></returns>
    public bool IsOk([MaybeNullWhen(false)] out TOk ok)
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
    public TOk OkOrElse(Func<TOk> getOk)
    {
        if (_isOk)
            return _ok!;
        return getOk();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.ok"/>
    public Option<TOk> AsOk()
    {
        if (_isOk)
        {
            return Some(_ok!);
        }

        return None<TOk>();
    }

    public bool IsSuccess([MaybeNullWhen(false)] out TOk ok, [MaybeNullWhen(true)] out TError error)
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
    public Result<TNewOk, TError> MapOk<TNewOk>(Func<TOk, TNewOk> map)
    {
        if (_isOk)
        {
            return Result<TNewOk, TError>.Ok(map(_ok!));
        }

        return Result<TNewOk, TError>.Error(_error!);
    }
    
    public Result<TNewOk, TError> MapOk<TNewOk>(Func<TOk, Result<TNewOk, TError>> map)
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
    public TNewOk MapOkOr<TNewOk>(Func<TOk, TNewOk> mapOk, TNewOk defaultOk)
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
    public TNewOk MapOkOrElse<TNewOk>(Func<TOk, TNewOk> mapOk, Func<TNewOk> getOk)
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
    public bool IsError() => !_isOk;

    public bool IsError([MaybeNullWhen(false)] out TError error)
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
    public bool IsErrorAnd(Func<TError, bool> errorPredicate) => !_isOk && errorPredicate(_error!);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.unwrap_err"/>
    public TError ErrorOrThrow(string? errorMessage = null)
    {
        if (!_isOk)
            return _error!;
        throw new InvalidOperationException(errorMessage ?? "This Result is not an Error");
    }

    public TError ErrorOr(TError error)
    {
        if (!_isOk)
            return _error!;
        return error;
    }

    public TError? ErrorOrDefault()
    {
        if (!_isOk)
            return _error!;
        return default(TError);
    }

    public TError ErrorOrElse(Func<TError> getError)
    {
        if (_isOk)
            return _error!;
        return getError();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.err"/>
    public Option<TError> AsError()
    {
        if (!_isOk)
            return Some(_error!);
        return None<TError>();
    }

    public bool IsFailure([MaybeNullWhen(false)] out TError error, [MaybeNullWhen(true)] out TOk ok)
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
    public Result<TOk, TNewError> MapError<TNewError>(Func<TError, TNewError> mapError)
    {
        if (_isOk)
        {
            return Result<TOk, TNewError>.Ok(_ok!);
        }

        return Result<TOk, TNewError>.Error(mapError(_error!));
    }

    public TNewError MapErrorOr<TNewError>(Func<TError, TNewError> mapError, TNewError defaultError)
    {
        if (!_isOk)
        {
            return mapError(_error!);
        }

        return defaultError;
    }

    public TNewError MapErrorOrElse<TNewError>(Func<TError, TNewError> mapError, Func<TNewError> getError)
    {
        if (!_isOk)
            return mapError(_error!);

        return getError();
    }

#endregion

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

    public TResult Match<TResult>(Func<TOk, TResult> onOk, Func<TError, TResult> onError)
    {
        if (_isOk)
            return onOk(_ok!);
        return onError(_error!);
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
            _ => false,
        };

#endregion

    public override int GetHashCode()
    {
        if (_isOk)
        {
            return Hasher.GetHashCode<TOk>(_ok);
        }

        return Hasher.GetHashCode<TError>(_error);
    }

    public override string ToString()
    {
        if (_isOk)
        {
            return $"Ok({_ok!})";
        }

        return $"Error({_error!})";
    }


    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<TOk> IEnumerable<TOk>.GetEnumerator() => GetEnumerator();

    public ResultEnumerator GetEnumerator() => new ResultEnumerator(this);


    [MustDisposeResource(false)]
    [StructLayout(LayoutKind.Auto)]
    public struct ResultEnumerator : IEnumerator<TOk>, IEnumerator, IDisposable
    {
        private bool _yielded;
        private readonly TOk _ok;

        object? IEnumerator.Current => _ok;
        public TOk Current => _ok;

        public ResultEnumerator(Result<TOk, TError> result)
        {
            if (result._isOk)
            {
                _ok = result._ok!;
                _yielded = false;
            }
            else
            {
                _ok = default!;
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
}