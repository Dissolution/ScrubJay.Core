using ScrubJay.Utilities;

namespace ScrubJay.Scratch;

public readonly struct Result<TOk, TError> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Result<TOk, TError>, Result<TOk, TError>, bool>,
    IEqualityOperators<Result<TOk, TError>, Ok<TOk>, bool>,
    IEqualityOperators<Result<TOk, TError>, Error<TError>, bool>,
    IEqualityOperators<Result<TOk, TError>, bool, bool>,
#endif
    IEquatable<Result<TOk, TError>>,
    IEquatable<Ok<TOk>>,
    IEquatable<Error<TError>>,
    IEquatable<bool>,
    IEnumerable<TOk>,
    IFormattable
{
    public static implicit operator Result<TOk, TError>(TOk okValue) => new(okValue);
    public static implicit operator Result<TOk, TError>(Ok<TOk> ok) => new(ok.Value);
    public static implicit operator Result<TOk, TError>(TError errorValue) => new(errorValue);
    public static implicit operator Result<TOk, TError>(Error<TError> error) => new(error._value);

    public static bool operator true(Result<TOk, TError> result) => result._isOk;
    public static bool operator false(Result<TOk, TError> result) => !result._isOk;
    public static bool operator !(Result<TOk, TError> result) => !result._isOk;
    
    public static bool operator ==(Result<TOk, TError> left, Result<TOk, TError> right) => left.Equals(right);
    public static bool operator ==(Result<TOk, TError> result, Ok<TOk> ok) => result.Equals(ok);
    public static bool operator ==(Result<TOk, TError> result, Error<TError> error) => result.Equals(error);
    public static bool operator ==(Result<TOk, TError> result, bool isOk) => result.Equals(isOk);
    public static bool operator ==(Ok<TOk> ok, Result<TOk, TError> result) => result.Equals(ok);
    public static bool operator ==(Error<TError> error, Result<TOk, TError> result) => result.Equals(error);
    public static bool operator ==(bool isOk, Result<TOk, TError> result) => result.Equals(isOk);
    
    public static bool operator !=(Result<TOk, TError> left, Result<TOk, TError> right) => !left.Equals(right);
    public static bool operator !=(Result<TOk, TError> result, bool isOk) => !result.Equals(isOk);
    public static bool operator !=(Result<TOk, TError> result, Ok<TOk> ok) => !result.Equals(ok);
    public static bool operator !=(Result<TOk, TError> result, Error<TError> error) => !result.Equals(error);
    public static bool operator !=(Ok<TOk> ok, Result<TOk, TError> result) => !result.Equals(ok);
    public static bool operator !=(Error<TError> error, Result<TOk, TError> result) => !result.Equals(error);
    public static bool operator !=(bool isOk, Result<TOk, TError> result) => !result.Equals(isOk);

    /// <summary>
    /// Returns an Ok Result containing the given <paramref name="okValue"/>
    /// </summary>
    public static Result<TOk, TError> Ok(TOk okValue) => new(okValue);
    
    /// <summary>
    /// Returns an Error Result containing the given <paramref name="errorValue"/>
    /// </summary>
    public static Result<TOk, TError> Error(TError errorValue) => new(errorValue);

    private readonly bool _isOk;
    private readonly TOk? _okValue;
    private readonly TError? _errorValue;

    private Result(TOk ok)
    {
        _isOk = true;
        _okValue = ok;
        _errorValue = default;
    }
    private Result(TError error)
    {
        _isOk = false;
        _okValue = default;
        _errorValue = error;
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
        if (_isOk)
        {
            okValue = _okValue!;
            return true;
        }
        okValue = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsError() => !_isOk;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsError([MaybeNullWhen(false)] out TError error)
    {
        if (!_isOk)
        {
            error = _errorValue!;
            return true;
        }
        error = default;
        return false;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOkAndError(
        [MaybeNullWhen(false)] out TOk ok, 
        [MaybeNullWhen(true)] out TError error)
    {
        ok = _okValue;
        error = _errorValue;
        return _isOk;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsErrorAndOk(
        [MaybeNullWhen(false)] out TError error, 
        [MaybeNullWhen(true)] out TOk ok)
    {
        ok = _okValue;
        error = _errorValue;
        return !_isOk;
    }
    
    /// <summary>
    /// Performs a matching operation on this <see cref="Result{TOk, TError}"/>
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
            onOk(_okValue!);
        }
        else
        {
            onError(_errorValue!);
        }
    }
    
    /// <summary>
    /// Performs a matching operation on this <see cref="Result{TOk, TError}"/> and returns a result
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
            return onOk(_okValue!);
        }
        else
        {
            return onError(_errorValue!);
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
            return Option<TOk>.Some(_okValue!);
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
            yield return _okValue!;
        }
    }

    public bool Equals(Result<TOk, TError> result)
    {
        if (_isOk)
        {
            return result._isOk &&
                EqualityComparer<TOk>.Default.Equals(_okValue!, result._okValue!);
        }
        else
        {
            return !result._isOk &&
                EqualityComparer<TError>.Default.Equals(_errorValue!, result._errorValue!);
        }
    }
    public bool Equals(Ok<TOk> ok) => _isOk && EqualityComparer<TOk>.Default.Equals(_okValue!, ok.Value!);
    public bool Equals(Error<TError> error) => !_isOk && EqualityComparer<TError>.Default.Equals(_errorValue!, error._value!);
    public bool Equals(bool isOk) => _isOk == isOk;
    public override bool Equals(object? obj)
    {
        return obj switch
        {
            Result<TOk, TError> result => Equals(result),
            Ok<TOk> ok => Equals(ok),
            Error<TError> error => Equals(error),
            bool isOk => Equals(isOk),
            _ => false,
        };
    }

    public override int GetHashCode() => _isOk ? Hasher.GetHashCode<TOk>(_okValue) : Hasher.GetHashCode<TError>(_errorValue);
    
    public string ToString(string? format, IFormatProvider? provider = default)
    {
        return Match(okValue =>
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
        }, errorValue =>
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
    }

    public override string ToString() => Match(
        ok => $"Result.Ok<{typeof(TOk).Name}>({ok})",
        error => $"Result.Error<{typeof(TError).Name}>({error})");
}


public readonly struct Ok<T> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Ok<T>, Ok<T>, bool>,
    IEqualityOperators<Ok<T>, T, bool>,
#endif
    IEquatable<Ok<T>>,
    IEquatable<T>,
    IEnumerable<T>
{
    public static bool operator ==(Ok<T> left, Ok<T> right) => left.Equals(right);
    public static bool operator !=(Ok<T> left, Ok<T> right) => !left.Equals(right);
    public static bool operator ==(Ok<T> left, T? right) => left.Equals(right);
    public static bool operator !=(Ok<T> left, T? right) => !left.Equals(right);
    public static bool operator ==(T? right, Ok<T> left) => left.Equals(right);
    public static bool operator !=(T? right, Ok<T> left) => !left.Equals(right);

    public readonly T Value;

    public Ok(T value)
    {
        this.Value = value;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<T> GetEnumerator()
    {
        yield return Value;
    }

    public bool Equals(Ok<T> ok) => EqualityComparer<T>.Default.Equals(Value!, ok.Value!);

    public bool Equals(T? value) => EqualityComparer<T>.Default.Equals(Value!, value!);

    public override bool Equals(object? obj) => obj switch
    {
        Ok<T> ok => Equals(ok),
        T value => Equals(value),
        _ => false,
    };

    public override int GetHashCode() => Hasher.GetHashCode<T>(Value);

    public override string ToString() => $"Ok<{typeof(T).Name}>({Value})";
}

public readonly struct Error<T> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Error<T>, Error<T>, bool>,
    IEqualityOperators<Error<T>, T, bool>,
#endif
    IEquatable<Error<T>>,
    IEquatable<T>,
    IEnumerable<T>
{
    public static bool operator ==(Error<T> left, Error<T> right) => left.Equals(right);
    public static bool operator !=(Error<T> left, Error<T> right) => !left.Equals(right);
    public static bool operator ==(Error<T> left, T? right) => left.Equals(right);
    public static bool operator !=(Error<T> left, T? right) => !left.Equals(right);
    public static bool operator ==(T? right, Error<T> left) => left.Equals(right);
    public static bool operator !=(T? right, Error<T> left) => !left.Equals(right);

    internal readonly T _value;

    public Error(T value)
    {
        _value = value;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<T> GetEnumerator()
    {
        yield return _value;
    }

    public bool Equals(Error<T> error) => EqualityComparer<T>.Default.Equals(_value!, error._value!);

    public bool Equals(T? value) => EqualityComparer<T>.Default.Equals(_value!, value!);

    public override bool Equals(object? obj) => obj switch
    {
        Error<T> ok => Equals(ok),
        T value => Equals(value),
        _ => false,
    };

    public override int GetHashCode() => Hasher.GetHashCode<T>(_value);

    public override string ToString() => $"Error<{typeof(T).Name}>({_value})";
}

public static class Result
{
    public static Ok<TOk> Ok<TOk>(TOk value) => new Ok<TOk>(value);
    public static Error<TError> Error<TError>(TError error) => new Error<TError>(error);

//    public static Error<Exception> Error<TException>(TException ex)
//        where TException : Exception
//        => new Error<Exception>(ex);

    public static Error<Exception> Errors(params Exception?[]? exceptions)
    {
        var aggEx = new AggregateException(exceptions.WhereNotNull());
        return new Error<Exception>(aggEx);
    }
    
    
    
    public static Result<T, ArgumentNullException> NotNull<T>(
        [AllowNull] T value,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
        where T : notnull
    {
        if (value is not null)
            return value;
        return new ArgumentNullException(valueName);
    }
}

public static class ResultExtensions
{
    public static void ThrowIfError<TOk, TError>(this Result<TOk, TError> result)
        where TError : Exception
    {
        if (result.IsError(out var ex))
            throw ex;
    }
}