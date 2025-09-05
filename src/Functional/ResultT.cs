// Prefix generic type parameter with T
// Do not declare static methods on generic types
// Do not catch Exception

using System.Diagnostics;

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
    IComparisonOperators<Result<T>, Result<T>, bool>,
    IComparisonOperators<Result<T>, T, bool>,
#endif
#if NET6_0_OR_GREATER
    ISpanFormattable,
#endif
    IEquatable<Result<T>>,
    IEquatable<T>,
    IComparable<Result<T>>,
    IComparable<T>,
    IEnumerable<T>,
    IFormattable
{
#region Operators

    public static implicit operator bool(Result<T> result) => result._isOk;
    public static implicit operator Result<T>(Exception ex) => Error(ex);
    public static implicit operator Result<T>(IMPL.Ok<T> ok) => Ok(ok.Value);
    public static implicit operator Result<T>(IMPL.Error<Exception> error) => Error(error.Value);


    public static bool operator ==(Result<T> left, Result<T> right) => left.Equals(right);
    public static bool operator !=(Result<T> left, Result<T> right) => !left.Equals(right);
    public static bool operator ==(Result<T> result, T? value) => result.Equals(value);
    public static bool operator !=(Result<T> result, T? value) => !result.Equals(value);

    public static bool operator >(Result<T> left, Result<T> right)
        => left.CompareTo(right) > 0;

    public static bool operator >=(Result<T> left, Result<T> right)
        => left.CompareTo(right) >= 0;

    public static bool operator <(Result<T> left, Result<T> right)
        => left.CompareTo(right) < 0;

    public static bool operator <=(Result<T> left, Result<T> right)
        => left.CompareTo(right) <= 0;

    public static bool operator >(Result<T> left, T right)
        => left.CompareTo(right) > 0;

    public static bool operator >=(Result<T> left, T right)
        => left.CompareTo(right) >= 0;

    public static bool operator <(Result<T> left, T right)
        => left.CompareTo(right) < 0;

    public static bool operator <=(Result<T> left, T right)
        => left.CompareTo(right) <= 0;

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

    private readonly bool _isOk;
    private readonly T? _value;
    private readonly Exception? _error;

    /// <summary>
    /// Result may only be constructed through <see cref="Ok(T)"/>, <see cref="Error"/>,
    /// or through implicit conversion from <see cref="IMPL.Ok{T}"/> or <see cref="IMPL.Error{E}"/>
    /// </summary>
    private Result(bool isOk, T? value, Exception? error)
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
        throw (_error ?? new InvalidOperationException(exceptionMessage ?? $"{ToString()} is not Ok"));
    }

#endregion

#region Error

    public bool IsError() => !_isOk;

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
    /// Returns <c>true</c> if this Result is Error and the value inside of it matches a predicate
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

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj switch
        {
            Result<T> result => Equals(result),
            T value => Equals(value),
            Exception ex => Equals(ex),
            bool isOk => isOk == _isOk,
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

            return typeof(Exception).GetHashCode();
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

            return $"Result<{typeof(T)}>.Ok({str})";
        }
        else
        {
            return $"Result<{typeof(T)}>.Error({_error})";
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
            return $"Result<{typeof(T)}>.Ok({_value})";
        }
        else
        {
            return $"Result<{typeof(T)}>.Error({_error})";
        }
    }

#endregion

#region Linq

    public Result<N> Select<N>(Func<T, N> selector)
    {
        if (IsOk(out var value, out var error))
        {
            return Result<N>.Ok(selector(value));
        }
        else
        {
            return error;
        }
    }

    public Result<N> Select<N>(Func<T, Result<N>> selector)
    {
        if (IsOk(out var value, out var error))
        {
            return selector(value!);
        }
        else
        {
            return error;
        }
    }

    public Result<N> Select<N>(Func<T, Option<N>> selector)
    {
        if (IsOk(out var value, out var error))
        {
            if (selector(value!).IsSome(out var some))
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
            return error;
        }
    }

    public Result<N> SelectMany<K, N>(Func<T, Result<K>> keySelector, Func<T, K, N> newSelector)
    {
        if (IsOk(out var value, out var error))
        {
            var keyResult = keySelector(value!);
            if (keyResult.IsOk(out var key, out error))
            {
                var newSelect = newSelector(value, key);
                return Result<N>.Ok(newSelect);
            }
            else
            {
                return error;
            }
        }
        else
        {
            return error;
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

        public T Current => _result.OkOrThrow();

        public ResultEnumerator(Result<T> result)
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

    /// <summary>
    /// Support for <c>await</c> syntax in order to support early return from <c>async</c> methods
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ResultAwaiter<T> GetAwaiter() => new(this);
}