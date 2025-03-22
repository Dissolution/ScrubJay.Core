// Prefix generic type parameter with T
// Do not declare static methods on generic types
// Do not catch Exception

#pragma warning disable CA1715, CA1000, CA1031

using ScrubJay.Functional.Threading;

namespace ScrubJay.Functional;

[PublicAPI]
#if !NETFRAMEWORK && !NETSTANDARD2_0
[AsyncMethodBuilder(typeof(ResultAsyncMethodBuilder<>))]
#endif
[StructLayout(LayoutKind.Auto)]
public readonly struct Result<T> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Result<T>, Result<T>, bool>,
    IEqualityOperators<Result<T>, T, bool>,
    IComparisonOperators<Result<T>, Result<T>, bool>,
    IComparisonOperators<Result<T>, T, bool>,
#endif
    IEquatable<Result<T>>,
    IEquatable<T>,
    IComparable<Result<T>>,
    IComparable<T>,
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
    /// Implicitly convert a <see cref="Result{T}"/> into <c>true</c> if it is <c>Ok</c> and <c>false</c> if it is <c>Error</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator true(Result<T> result) => result._isOk;

    /// <summary>
    /// Implicitly convert a <see cref="Result{T}"/> into <c>true</c> if it is <c>Ok</c> and <c>false</c> if it is <c>Error</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator false(Result<T> result) => !result._isOk;

    /// <summary>
    /// Implicitly convert a <typeparamref name="T"/> <paramref name="value"/> to <see cref="Result{T}.Ok"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Result<T>(T value) => Ok(value);

    /// <summary>
    /// Implicitly convert an <see cref="Exception"/> to a <see cref="Result{T}.Error"/>
    /// </summary>
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

    // We pass equality and comparison down to T values

    public static bool operator ==(Result<T> left, Result<T> right) => left.Equals(right);

    public static bool operator !=(Result<T> left, Result<T> right) => !left.Equals(right);

    public static bool operator >(Result<T> left, Result<T> right) => left.CompareTo(right) > 0;

    public static bool operator >=(Result<T> left, Result<T> right) => left.CompareTo(right) >= 0;

    public static bool operator <(Result<T> left, Result<T> right) => left.CompareTo(right) < 0;

    public static bool operator <=(Result<T> left, Result<T> right) => left.CompareTo(right) <= 0;

    public static bool operator ==(Result<T> result, T? ok) => result.Equals(ok);

    public static bool operator !=(Result<T> result, T? ok) => !result.Equals(ok);

    public static bool operator >(Result<T> result, T? ok) => result.CompareTo(ok) > 0;

    public static bool operator >=(Result<T> result, T? ok) => result.CompareTo(ok) >= 0;

    public static bool operator <(Result<T> result, T? ok) => result.CompareTo(ok) < 0;

    public static bool operator <=(Result<T> result, T? ok) => result.CompareTo(ok) <= 0;

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

    public static Result<T> Try(Func<T>? func)
    {
        if (func is null)
            return new ArgumentNullException(nameof(func));

        try
        {
            T result = func();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return Error(ex);
        }
    }


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

    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T OkOrThrow()
    {
        if (_isOk)
        {
            return _value!;
        }
        throw (_error ?? GetDefaultError());
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
    public bool HasOkOrError([MaybeNullWhen(false)] out T ok, [MaybeNullWhen(true)] out Exception error)
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
    public bool HasError([MaybeNullWhen(false)] out Exception error)
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
    public Option<Exception> HasError() => !_isOk ? Some(_error!) : None<Exception>();

    /// <summary>
    /// Returns <c>true</c> if this Result is Error and the value inside of it matches a predicate
    /// </summary>
    /// <param name="errorPredicate"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.is_err_and"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsErrorAnd(Func<Exception, bool> errorPredicate) => !_isOk && errorPredicate(_error!);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Exception ErrorOr(Exception error)
    {
        if (!_isOk)
            return _error!;
        return error;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Exception? ErrorOrDefault()
    {
        if (!_isOk)
            return _error!;
        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Exception ErrorOrElse(Func<Exception> getError)
    {
        if (_isOk)
            return _error!;
        return getError();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasErrorOrOk([MaybeNullWhen(false)] out Exception error, [MaybeNullWhen(true)] out T ok)
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
    public TResult Match<TResult>(Func<T, TResult> onOk, Func<Exception, TResult> onError)
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

    public int CompareTo(Result<T> result)
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

        // we're both errors
        return 0;
    }

    public int CompareTo(T? ok)
    {
        if (_isOk)
        {
            return Comparer<T>.Default.Compare(_value!, ok!);
        }

        return 1; // my Err is greater than an Ok value
    }

    public int CompareTo(object? obj)
    {
        return obj switch
        {
            Result<T> result => CompareTo(result),
            T ok => CompareTo(ok),
            _ => 1, // null and unknown values sort before
        };
    }

#endregion

#region Equal

    public bool Equals(Result<T> result)
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

        // both are errors
        return true;
    }

    public bool Equals(T? ok)
    {
        if (_isOk)
        {
            return EqualityComparer<T>.Default.Equals(_value!, ok!);
        }

        return false;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj switch
        {
            Result<T> result => Equals(result),
            T ok => Equals(ok),
            bool isOk => _isOk == isOk,
            _ => false,
        };

#endregion

#region LINQ + IEnumerable

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<N> Select<N>(Func<T, N> selector)
    {
        if (_isOk)
            return Result<N>.Ok(selector(_value!));
        return Result<N>.Error(_error!);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<N> Select<N>(Func<T, Option<N>> selector)
    {
        if (_isOk && selector(_value!).HasSome(out var value))
        {
            return Result<N>.Ok(value);
        }
        return Result<N>.Error(_error!);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<N> Select<N>(Func<T, Result<N>> selector)
    {
        if (_isOk)
        {
            return selector(_value!);
        }
        return Result<N>.Error(_error!);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<N> Select<X, N>(X state, Func<X, T, N> selector)
    {
        if (_isOk)
            return Result<N>.Ok(selector(state, _value!));
        return Result<N>.Error(_error!);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<N> SelectMany<N>(Func<T, Result<N>> newSelector)
    {
        if (_isOk)
        {
            return newSelector(_value!);
        }
        return Result<N>.Error(_error!);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<N> SelectMany<K, N>(Func<T, Result<K>> keySelector, Func<T, K, N> newSelector)
    {
        if (_isOk && keySelector(_value!).HasOk(out var key))
        {
            return Result<N>.Ok(newSelector(_value!, key));
        }

        return Result<N>.Error(_error!);
    }


    public ResultAwaiter<T> GetAwaiter() => new(this);

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
            _canYield = result.IsOk;
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

        public void Reset() => _canYield = true;

        void IDisposable.Dispose()
        {
            /* Do Nothing */
        }
    }

#endregion

    public override int GetHashCode()
    {
        if (_isOk)
            return Hasher.Hash<T>(_value);
        return Hasher.EmptyHash;
    }

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
}
