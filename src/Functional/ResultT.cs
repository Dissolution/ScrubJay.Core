// Prefix generic type parameter with T
// Do not declare static methods on generic types
// Do not catch Exception

#pragma warning disable CA1715, CA1000, CA1031


namespace ScrubJay.Functional;

/// <summary>
/// A Result type holding a returned <typeparamref name="T"/> value or <see cref="Exception"/>.
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of value stored with an <c>Ok</c> Result
/// </typeparam>
/// <remarks> 
/// This emulates a discriminated union:
/// <code>
/// Result
/// {
///     Ok(T),
///     Error(Exception),
/// }
/// </code>
/// 🦀 Heavily inspired by Rust's Result type! 🦀
/// </remarks>
/// <seealso href="https://en.wikipedia.org/wiki/Result_type">Result Type on Wikipedia</seealso>
/// <seealso href="https://doc.rust-lang.org/std/result/enum.Result.html">Rust's Result Type</seealso>
[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly struct Result<T> :
    /* All commented out interfaces are implemented, but cannot be declared per CS0695:
     * 'Result<T>' cannot implement both 'X' and 'Y' because they may unify for some type parameter substitutions
     */
#if NET7_0_OR_GREATER
    IEqualityOperators<Result<T>, Result<T>, bool>,
    IEqualityOperators<Result<T>, T, bool>,
    // IEqualityOperators<Result<T>, Exception, bool>,
    IComparisonOperators<Result<T>, Result<T>, bool>,
    IComparisonOperators<Result<T>, T, bool>,
#endif
    IEquatable<Result<T>>,
    IEquatable<T>,
    // IEquatable<Exception>,
    IComparable<Result<T>>,
    IComparable<T>,
    IEnumerable<T>,
    IFormattable
{
#region Operators

    /// <summary>
    /// Implicitly convert a <see cref="Result{T}"/> into a <c>bool</c> (Ok -> <c>true</c>, Error -> <c>false</c>) 
    /// </summary>
    public static implicit operator bool(Result<T> result) => result._isOk;

    /// <summary>
    /// Implicitly convert a <see cref="Result{T}"/> into a <see cref="Result"/> (Ok(T) -> Ok, Error -> Error) 
    /// </summary>
    public static implicit operator Result(Result<T> result) =>
        result.IsError(out var error) ? Result.Error(error) : Result.Ok;

    /// <summary>
    /// Implicitly convert a <typeparamref name="T"/> <paramref name="value"/> into an <see cref="Ok"/> <see cref="Result{T}"/>
    /// </summary>
    public static implicit operator Result<T>(T value) => Ok(value);

    /// <summary>
    /// Implicitly convert an <see cref="Exception"/> into an <see cref="Error"/> <see cref="Result{T}"/>
    /// </summary>
    public static implicit operator Result<T>(Exception ex) => Error(ex);

    /// <summary>
    /// Implicitly convert an <see cref="IMPL.Ok{T}"/> into an <see cref="Ok"/> <see cref="Result{T}"/>
    /// </summary>
    public static implicit operator Result<T>(IMPL.Ok<T> ok) => Ok(ok.Value);

    /// <summary>
    /// Implicitly convert an <see cref="IMPL.Error{T}"/> into an <see cref="Error"/> <see cref="Result{T}"/>
    /// </summary>
    public static implicit operator Result<T>(IMPL.Error<Exception> error) => Error(error.Value);

    /// <summary>
    /// <see cref="Result{T}"/> evaluates to <c>true</c> if it is <see cref="Ok"/>
    /// </summary>
    public static bool operator true(Result<T> result) => result._isOk;

    /// <summary>
    /// <see cref="Result{T}"/> evaluates to <c>false</c> if it is <see cref="Error"/>
    /// </summary>
    public static bool operator false(Result<T> result) => !result._isOk;

    public static bool operator ==(Result<T> left, Result<T> right) => left.Equals(right);
    public static bool operator !=(Result<T> left, Result<T> right) => !left.Equals(right);
    public static bool operator ==(Result<T> result, T? value) => result.Equals(value);
    public static bool operator !=(Result<T> result, T? value) => !result.Equals(value);
    public static bool operator ==(Result<T> result, Exception? error) => result.Equals(error);
    public static bool operator !=(Result<T> result, Exception? error) => !result.Equals(error);

    public static bool operator >(Result<T> left, Result<T> right) => left.CompareTo(right) > 0;
    public static bool operator >=(Result<T> left, Result<T> right) => left.CompareTo(right) >= 0;
    public static bool operator <(Result<T> left, Result<T> right) => left.CompareTo(right) < 0;
    public static bool operator <=(Result<T> left, Result<T> right) => left.CompareTo(right) <= 0;
    public static bool operator >(Result<T> left, T right) => left.CompareTo(right) > 0;
    public static bool operator >=(Result<T> left, T right) => left.CompareTo(right) >= 0;
    public static bool operator <(Result<T> left, T right) => left.CompareTo(right) < 0;
    public static bool operator <=(Result<T> left, T right) => left.CompareTo(right) <= 0;

#endregion

    /// <summary>
    /// Creates an Ok <see cref="Result{T}"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Ok(T value) => new Result<T>(true, value, null);

    /// <summary>
    /// Creates <see cref="Result{T}"/>.Error(<paramref name="ex"/>)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Error(Exception? ex) => new Result<T>(false, default, ex ?? new InvalidOperationException());

    // is this ok or error?
#if DEBUG
    internal
#else
    private
#endif
        readonly bool _isOk;
    
    
    // possible ok value
#if DEBUG
    internal
#else
    private
#endif
        readonly T? _value;

    // possible error exception
#if DEBUG
    internal
#else
    private
#endif
        readonly Exception? _error;
    
    /// <remarks>
    /// <see cref="Result{T}"/> may only be constructed with <see cref="Ok"/>, <see cref="Error"/>,
    /// or an implicit conversion from a <typeparamref name="T"/> or <see cref="Exception"/>.
    /// </remarks>
    private Result(bool isOk, T? value, Exception? error)
    {
        _isOk = isOk;
        _value = value;
        _error = error;
    }

#region Ok

    /// <summary>
    /// Is this an Ok <see cref="Result{T}"/>?
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOk() => _isOk;

    /// <summary>
    /// Is this an Ok <see cref="Result{T}"/>?
    /// </summary>
    /// <param name="value">
    /// If this is an Ok <see cref="Result{T}"/>, the Ok value;
    /// otherwise <c>default(T)</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if this is an Ok <see cref="Result{T}"/>; otherwise <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOk([MaybeNullWhen(false)] out T value)
    {
        value = _value;
        return _isOk;
    }

    /// <summary>
    /// Is this an Ok <see cref="Result{T}"/>?
    /// </summary>
    /// <param name="value">
    /// If this is an Ok <see cref="Result{T}"/>, the Ok value;
    /// otherwise <c>default(T)</c>.
    /// </param>
    /// <param name="error">
    /// If this is an Error <see cref="Result{T}"/>, the Error <see cref="Exception"/>;
    /// otherwise <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if this is an Ok <see cref="Result{T}"/>; otherwise <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOk([MaybeNullWhen(false)] out T value, [NotNullWhen(false)] out Exception? error)
    {
        value = _value;
        error = _error;
        return _isOk;
    }

    public bool IsOkAnd(Func<T, bool> okPredicate)
    {
        return _isOk && okPredicate(_value!);
    }

    public T OkOr(T fallback)
    {
        if (_isOk)
            return _value!;
        return fallback;
    }

    public T OkOr(Func<T> getFallback)
    {
        if (_isOk)
            return _value!;
        return getFallback();
    }

    public T? OkOrDefault()
    {
        if (_isOk)
            return _value!;
        return default(T);
    }

    public T OkOrThrow()
    {
        if (_isOk)
            return _value!;
        throw _error!;
    }

#endregion

#region Error

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsError() => !_isOk;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsError([NotNullWhen(true)] out Exception? error)
    {
        error = _error;
        return !_isOk;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsError([NotNullWhen(true)] out Exception? error, [MaybeNullWhen(true)] out T ok)
    {
        error = _error;
        ok = _value;
        return !_isOk;
    }

    public bool IsErrorAnd(Func<Exception, bool> errorPredicate)
    {
        return !_isOk && errorPredicate(_error!);
    }

    public Exception ErrorOr(Exception fallback)
    {
        if (!_isOk)
            return _error!;
        return fallback;
    }

    public Exception ErrorOr(Func<Exception> getFallback)
    {
        if (!_isOk)
            return _error!;
        return getFallback();
    }

    public void ThrowIfError()
    {
        if (!_isOk)
        {
            throw _error!;
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
#if NET9_0_OR_GREATER
        where R : allows ref struct
#endif
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
            return Option<T>.Some(_value!);
        }
        else
        {
            return Option<T>.None;
        }
    }

#region Comparison

    public int CompareTo(Result<T> other)
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
                return Comparer<Exception>.Default.Compare(_error!, other._error!);
            }
        }
    }

    public int CompareTo(T? ok)
    {
        if (_isOk)
        {
            return Comparer<T>.Default.Compare(_value!, ok!);
        }

        return 1; // Error < Ok
    }

#endregion

#region Equality

    public bool Equals(Result<T> other)
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
                return EqualityComparer<Exception>.Default.Equals(_error!, other._error!);
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

    public bool Equals(Exception? error)
    {
        if (!_isOk)
        {
            return EqualityComparer<Exception>.Default.Equals(_error!, error!);
        }

        return false;
    }

    public bool Equals(bool isOk) => _isOk;

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
#if NETFRAMEWORK || NETSTANDARD2_0
        if (_isOk)
        {
            if (_value is not null)
                return _value.GetHashCode();

            return typeof(T).GetHashCode();
        }
        else
        {
            if (_error is not null)
                return _error.GetHashCode();
            return typeof(Exception).GetHashCode();
        }
#else
        return HashCode.Combine(_isOk, _value, _error);
#endif
    }

#endregion

#region Formatting

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

            return $"Ok({str})";
        }
        else
        {
            return $"Error({_error})";
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
        private readonly Result<T> _result;
        private bool _canYield;

        object? IEnumerator.Current => _result.OkOrThrow();

        public T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _result.OkOrThrow();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ResultEnumerator(Result<T> result)
        {
            _result = result;
            _canYield = result._isOk;
        }

        void IDisposable.Dispose()
        {
            /* Do Nothing */
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

#region Linq

    public Result<N> Select<N>(Func<T, N> selector)
    {
        if (_isOk)
        {
            return Result<N>.Ok(selector(_value!));
        }
        else
        {
            return Result<N>.Error(_error!);
        }
    }

    public Result<N> Select<N>(Func<T, Result<N>> selector)
    {
        if (_isOk)
        {
            return selector(_value!);
        }
        else
        {
            return Result<N>.Error(_error!);
        }
    }

    public Result<N> Select<N>(Func<T, Option<N>> selector)
    {
        if (_isOk)
        {
            if (selector(_value!).IsSome(out var some))
            {
                return Result<N>.Ok(some);
            }
            else
            {
                return Result<N>.Error(new InvalidOperationException());
            }
        }
        else
        {
            return Result<N>.Error(_error!);
        }
    }

    public Result<N> SelectMany<K, N>(Func<T, Result<K>> keySelector, Func<T, K, N> newSelector)
    {
        if (_isOk)
        {
            var keySelectResult = keySelector(_value!);
            if (keySelectResult._isOk)
            {
                var newSelectResult = newSelector(_value!, keySelectResult._value!);
                return Result<N>.Ok(newSelectResult);
            }

            return Result<N>.Error(keySelectResult._error!);
        }

        return Result<N>.Error(_error!);
    }

#endregion
}