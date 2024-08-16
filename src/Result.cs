using ScrubJay.Utilities;

namespace ScrubJay;

public readonly struct Result<TOk, TError> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Result<TOk, TError>, Result<TOk, TError>, bool>,
#endif
    IEquatable<Result<TOk, TError>>,
    IEnumerable<TOk>,
    IFormattable
{
    public static implicit operator Result<TOk, TError>(TOk okValue) => Ok(okValue);
    public static implicit operator Result<TOk, TError>(TError errorValue) => Error(errorValue);
    public static implicit operator bool(Result<TOk, TError> result) => result._isOk;
   
    public static bool operator ==(Result<TOk, TError> left, Result<TOk, TError> right) => left.Equals(right);
    public static bool operator !=(Result<TOk, TError> left, Result<TOk, TError> right) => !left.Equals(right);

    /// <summary>
    /// Returns an Ok Result containing the given <paramref name="value"/>
    /// </summary>
    public static Result<TOk, TError> Ok(TOk value) => new(true, value, default(TError));

    /// <summary>
    /// Returns an Error Result containing the given <paramref name="value"/>
    /// </summary>
    public static Result<TOk, TError> Error(TError value) => new(false, default(TOk), value);


    private readonly bool _isOk;
    private readonly TOk? _ok;
    private readonly TError? _error;

    private Result(bool isOk, TOk? ok, TError? error)
    {
        _isOk = isOk;
        _ok = ok;
        _error = error;
    }

    /// <summary>
    /// Is this an Ok Result?
    /// </summary>
    /// <returns>
    /// <c>true</c> if this is Result.<see cref="Ok"/><br/>
    /// <c>false</c> if this is Result.<see cref="Error"/><br/>
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOk() => _isOk;

    /// <summary>
    /// Determines if this <see cref="Result{TOk,TError}"/> is <see cref="Ok"/> and output its <paramref name="okValue"/>
    /// </summary>
    /// <returns>
    /// <c>true</c> and sets <paramref name="okValue"/> if this is Result.<see cref="Ok"/><br/>
    /// <c>false</c> and sets <paramref name="okValue"/> to <c>default</c> if this is Result.<see cref="Error"/><br/>
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOk([MaybeNullWhen(false)] out TOk okValue)
    {
        okValue = _ok;
        return _isOk;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TOk OkOrFallback(TOk fallback)
    {
        return _isOk ? _ok! : fallback;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsError() => !_isOk;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsError([MaybeNullWhen(false)] out TError error)
    {
        error = _error;
        return !_isOk;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TError ErrorOrFallback(TError fallback)
    {
        return _isOk ? fallback : _error!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOkIncludeError(
        [MaybeNullWhen(false)] out TOk ok,
        [MaybeNullWhen(true)] out TError error)
    {
        ok = _ok;
        error = _error;
        return _isOk;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsErrorIncludeOk(
        [MaybeNullWhen(false)] out TError error,
        [MaybeNullWhen(true)] out TOk ok)
    {
        ok = _ok;
        error = _error;
        return !_isOk;
    }

    /// <summary>
    /// Performs a matching operation on this <see cref="Result{TOk,TError}"/>
    /// </summary>
    /// <param name="onOk">
    /// Invoked on the <typeparamref name="TOk"/> value if this is a Result.Ok&lt;TOk&gt;
    /// </param>
    /// <param name="onError">
    /// Invoked on the <typeparamref name="TError"/> value if this is a Result.Error&lt;TOk&gt;
    /// </param>
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

    /// <summary>
    /// Performs a matching operation on this <see cref="Result{TOk,TError}"/> and returns a result
    /// </summary>
    /// <param name="onOk">
    /// Invoked on the <typeparamref name="TOk"/> value if this is a Result.Ok&lt;TOk&gt;
    /// </param>
    /// <param name="onError">
    /// Invoked on the <typeparamref name="TError"/> value if this is a Result.Error&lt;TOk&gt;
    /// </param>
    /// <returns>
    /// The result of either <paramref name="onOk"/> or <paramref name="onError"/>
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TReturn Match<TReturn>(Func<TOk, TReturn> onOk, Func<TError, TReturn> onError)
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

    /// <summary>
    /// Converts this <see cref="Result{TOk,TError}"/> to an <see cref="Option{T}"/>
    /// </summary>
    /// <returns>
    /// <see cref="Option{T}">Option.Some(ok)</see> if this is a Result.Ok&lt;TOk&gt;<br/>
    /// <see cref="Option{T}">Option.None</see> if this is a Result.Error&lt;TError&gt;<br/>
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<TOk> AsOption()
    {
        if (_isOk)
        {
            return Option<TOk>.Some(_ok!);
        }
        else
        {
            return Option<TOk>.None;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<TOk> GetEnumerator()
    {
        if (_isOk)
        {
            yield return _ok!;
        }
    }

    public bool Equals(Result<TOk, TError> result)
    {
        if (_isOk)
        {
            return result._isOk &&
                EqualityComparer<TOk>.Default.Equals(_ok!, result._ok!);
        }
        else
        {
            return !result._isOk &&
                EqualityComparer<TError>.Default.Equals(_error!, result._error!);
        }
    }
    public bool Equals(TOk? ok) => _isOk && EqualityComparer<TOk>.Default.Equals(_ok!, ok!);
    public bool Equals(TError? error) => !_isOk && EqualityComparer<TError>.Default.Equals(_error!, error!);
    public bool Equals(bool isOk) => _isOk == isOk;
    public override bool Equals(object? obj)
    {
        return obj switch
        {
            Result<TOk, TError> result => Equals(result),
            TOk ok => Equals(ok),
            TError error => Equals(error),
            bool isOk => Equals(isOk),
            _ => false,
        };
    }

    public override int GetHashCode() => Match<int>(
        static ok => Hasher.GetHashCode<TOk>(ok),
        static error => Hasher.GetHashCode<TError>(error));

    public string ToString(string? format, IFormatProvider? provider = default) => Match(
        okValue =>
        {
            string? okString;
            // ReSharper disable once MergeCastWithTypeCheck
            if (okValue is IFormattable)
            {
                okString = ((IFormattable)okValue).ToString(format, provider);
            }
            else
            {
                okString = okValue?.ToString();
            }
            return $"Result.Ok<{typeof(TOk).Name}>({okString})";
        },
        errorValue =>
        {
            string? errorString;
            // ReSharper disable once MergeCastWithTypeCheck
            if (errorValue is IFormattable)
            {
                errorString = ((IFormattable)errorValue).ToString(format, provider);
            }
            else
            {
                errorString = errorValue?.ToString();
            }
            return $"Result.Error<{typeof(TError).Name}>({errorString})";
        });

    public override string ToString() => Match(
        ok => $"Ok<{ok?.GetType().Name}>({ok})",
        error => $"Error<{error?.GetType().Name}>({error})");
}