// ReSharper disable InconsistentNaming

using JetBrains.Annotations;
using ScrubJay.Utilities;

namespace ScrubJay;

/// <summary>
/// <c>Result&lt;T, E&gt;</c> is the type used for returning and propagating errors<br/>
/// It has two variants:<br/>
/// <see cref="Ok"/>, representing success and containing a <typeparamref name="T"/> ok value<br/>
/// <see cref="Error"/>, representing error and containing a <typeparamref name="E"/> error value<br/>
/// </summary>
/// <typeparam name="T">The generic <see cref="Type"/> for an Ok value</typeparam>
/// <typeparam name="E">The generic <see cref="Type"/> for an Err value</typeparam>
/// <remarks>
/// Heavily inspired by Rust's Result type<br/>
/// <a href="https://doc.rust-lang.org/std/result/"/><br/>
/// <a href="https://doc.rust-lang.org/std/result/enum.Result.html"/><br/>
/// </remarks>
public readonly struct Result<T, E> :
/* All listed interfaces are implemented, but cannot be declared because they may unify for some type parameter substitutions */
#if NET7_0_OR_GREATER
    IEqualityOperators<Result<T, E>, Result<T, E>, bool>, 
    //IEqualityOperators<Result<T, E>, T, bool>,
    //IEqualityOperators<Result<T, E>, E, bool>,
#endif
    IEquatable<Result<T, E>>,
    //IEquatable<T>,
    //IEquatable<E>,
#if NET7_0_OR_GREATER
    IComparisonOperators<Result<T, E>, Result<T, E>, bool>,
    //IComparisonOperators<Result<T, E>, T, bool>,
    //IComparisonOperators<Result<T, E>, E, bool>,
#endif
    IComparable<Result<T, E>>,
    //IComparable<T>,
    //IComparable<E>,
    IEnumerable<T>,
    IEnumerable
{
    public static implicit operator bool(Result<T, E> result) => result._isOk;
    public static implicit operator Result<T, E>(T ok) => Ok(ok);
    public static implicit operator Result<T, E>(E error) => Error(error);

    public static bool operator true(Result<T, E> result) => result._isOk;
    public static bool operator false(Result<T, E> result) => !result._isOk;

    public static bool operator ==(Result<T, E> x, Result<T, E> y) => x.Equals(y);
    public static bool operator !=(Result<T, E> x, Result<T, E> y) => !x.Equals(y);
    public static bool operator >(Result<T, E> x, Result<T, E> y) => x.CompareTo(y) > 0;
    public static bool operator >=(Result<T, E> x, Result<T, E> y) => x.CompareTo(y) >= 0;
    public static bool operator <(Result<T, E> x, Result<T, E> y) => x.CompareTo(y) < 0;
    public static bool operator <=(Result<T, E> x, Result<T, E> y) => x.CompareTo(y) <= 0;

    public static bool operator ==(Result<T, E> result, T? ok) => result.Equals(ok);
    public static bool operator !=(Result<T, E> result, T? ok) => !result.Equals(ok);
    public static bool operator >(Result<T, E> result, T? ok) => result.CompareTo(ok) > 0;
    public static bool operator >=(Result<T, E> result, T? ok) => result.CompareTo(ok) >= 0;
    public static bool operator <(Result<T, E> result, T? ok) => result.CompareTo(ok) < 0;
    public static bool operator <=(Result<T, E> result, T? ok) => result.CompareTo(ok) <= 0;

    public static bool operator ==(Result<T, E> result, E? err) => result.Equals(err);
    public static bool operator !=(Result<T, E> result, E? err) => !result.Equals(err);
    public static bool operator >(Result<T, E> result, E? err) => result.CompareTo(err) > 0;
    public static bool operator >=(Result<T, E> result, E? err) => result.CompareTo(err) >= 0;
    public static bool operator <(Result<T, E> result, E? err) => result.CompareTo(err) < 0;
    public static bool operator <=(Result<T, E> result, E? err) => result.CompareTo(err) <= 0;


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


    private readonly bool _isOk;
    private readonly T? _value;
    private readonly E? _error;

    private Result(bool isOk, T? value, E? error)
    {
        _isOk = isOk;
        _value = value;
        _error = error;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.is_ok"/>
    public bool IsOk() => _isOk;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.is_ok_and"/>
    public bool IsOkAnd(Func<T, bool> predicate) => _isOk && predicate(_value!);

    public bool IsOk([MaybeNullWhen(false)] out T value)
    {
        if (_isOk)
        {
            value = _value!;
            return true;
        }
        else
        {
            value = default!;
            return false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.ok"/>
    public Option<T> AsOk()
    {
        if (_isOk)
            return Some(_value!);
        return None;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.is_err"/>
    public bool IsError() => !_isOk;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.is_err_and"/>
    public bool IsErrorAnd(Func<E, bool> predicate) => !_isOk && predicate(_error!);

    public bool IsError([MaybeNullWhen(false)] out E error)
    {
        if (!_isOk)
        {
            error = _error!;
            return true;
        }
        else
        {
            error = default!;
            return false;
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.err"/>
    public Option<E> AsError()
    {
        if (!_isOk)
            return Some(_error!);
        return None;
    }

    public bool IsSuccess([MaybeNullWhen(false)] out T ok, [MaybeNullWhen(true)] out E error)
    {
        if (_isOk)
        {
            ok = _value!;
            error = _error;
            return true;
        }
        else
        {
            ok = _value;
            error = _error!;
            return false;
        }
    }
    
    public bool IsFailure([MaybeNullWhen(false)] out E error, [MaybeNullWhen(true)] out T ok)
    {
        if (!_isOk)
        {
            error = _error!;
            ok = _value;
            return true;
        }
        else
        {
            error = _error;
            ok = _value!;
            return false;
        }
    }
    
    
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="noneErrorMessage"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Result.html#method.expect"/>
    public T Expect(string noneErrorMessage)
    {
        if (_isOk)
            return _value!;
        throw new InvalidOperationException(noneErrorMessage);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Result.html#method.unwrap"/>
    public T Unwrap()
    {
        if (_isOk)
            return _value!;
        throw (_error as Exception) ?? new InvalidOperationException("This Result is not Ok");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Result.html#method.unwrap_or"/>
    public T UnwrapOr(T value)
    {
        if (_isOk)
            return _value!;
        return value;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Result.html#method.unwrap_or_default"/>
    public T? UnwrapOrDefault()
    {
        {
            if (_isOk)
                return _value!;
            return default(T);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="getValue"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Result.html#method.unwrap_or_else"/>
    public T UnwrapOrElse(Func<T> getValue)
    {
        if (_isOk)
            return _value!;
        return getValue();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="notAnErrorExceptionMessage"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.expect_err"/>
    public E ExpectError(string notAnErrorExceptionMessage)
    {
        if (!_isOk)
            return _error!;
        throw new InvalidOperationException(notAnErrorExceptionMessage);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.unwrap_err"/>
    public E UnwrapError()
    {
        if (!_isOk)
            return _error!;
        throw new InvalidOperationException("This Result is not Ok");
    }


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


    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>
    /// <typeparam name="U"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.map"/>
    public Result<U, E> Map<U>(Func<T, U> map)
    {
        if (_isOk)
        {
            return Result<U, E>.Ok(map(_value!));
        }
        else
        {
            return Result<U, E>.Error(_error!);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mapError"></param>
    /// <typeparam name="F"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.map_err"/>
    public Result<T, F> MapErr<F>(Func<E, F> mapError)
    {
        if (_isOk)
        {
            return Result<T, F>.Ok(_value!);
        }
        else
        {
            return Result<T, F>.Error(mapError(_error!));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>
    /// <param name="defaultValue"></param>
    /// <typeparam name="U"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.map_or"/>
    public U MapOr<U>(Func<T, U> map, U defaultValue)
    {
        if (_isOk)
        {
            return map(_value!);
        }

        return defaultValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>
    /// <param name="getDefaultValue"></param>
    /// <typeparam name="U"></typeparam>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Result.html#method.map_or_else"/>
    public U MapOrElse<U>(Func<T, U> map, Func<U> getDefaultValue)
    {
        if (_isOk)
        {
            return map(_value!);
        }
        else
        {
            return getDefaultValue();
        }
    }

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
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    public ResultEnumerator GetEnumerator() => new ResultEnumerator(this);


    public int CompareTo(Result<T, E> other)
    {
        // An Ok compares as less than any Err
        // while two Ok or two Err compare their containing values

        if (_isOk)
        {
            if (other._isOk)
            {
                // compare ok values
                return Comparer<T>.Default.Compare(_value!, other._value!);
            }
            else
            {
                return -1; // my Ok is less than their Err
            }
        }
        else // i'm Err
        {
            if (other._isOk)
            {
                return 1; // my Err is greater than their Ok
            }
            else
            {
                // compare err values
                return Comparer<E>.Default.Compare(_error!, other._error!);
            }
        }
    }

    public int CompareTo(T? value)
    {
        if (_isOk)
            return Comparer<T>.Default.Compare(_value!, value!);
        return 1; // my Err is greater than an Ok value
    }

    public int CompareTo(E? error)
    {
        if (!_isOk)
            return Comparer<E>.Default.Compare(_error!, error!);
        return -1; // my Ok is less than an Err value
    }

    public bool Equals(Result<T, E> other)
    {
        if (_isOk)
        {
            if (other._isOk)
            {
                // compare ok values
                return EqualityComparer<T>.Default.Equals(_value!, other._value!);
            }
            else
            {
                return false;
            }
        }
        else // i'm Err
        {
            if (other._isOk)
            {
                return false;
            }
            else
            {
                // compare err values
                return EqualityComparer<E>.Default.Equals(_error!, other._error!);
            }
        }
    }

    public bool Equals(T? ok)
    {
        if (_isOk)
            return EqualityComparer<T>.Default.Equals(_value!, ok!);
        return false;
    }

    public bool Equals(E? err)
    {
        if (!_isOk)
            return EqualityComparer<E>.Default.Equals(_error!, err!);
        return false;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj switch
        {
            Result<E, E> result => Equals(result),
            T ok => Equals(ok),
            E err => Equals(err),
            _ => false,
        };

    public override int GetHashCode()
    {
        if (_isOk)
            return Hasher.GetHashCode<T>(_value);
        return Hasher.GetHashCode<E>(_error);
    }

    public override string ToString()
    {
        return Match(ok => $"Ok({ok})", err => $"Err({err})");
    }
    
    
    [MustDisposeResource(false)]
    public struct ResultEnumerator : IEnumerator<T>, IEnumerator, IDisposable
    {
        private bool _yielded;
        private readonly T _value;

        object? IEnumerator.Current => _value;
        public T Current => _value;

        public ResultEnumerator(Result<T, E> result)
        {
            if (result._isOk)
            {
                _value = result._value!;
                _yielded = false;
            }
            else
            {
                _value = default!;
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