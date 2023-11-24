namespace ScrubJay.Utilities;

/// <summary>
/// Represents the typed result of an operation as either:<br/>
/// <c>Ok(<typeparamref name="TOk">Value</typeparamref>)</c><br/>
/// <c>Error(<typeparamref name="TError">Error</typeparamref>)</c>
/// </summary>
/// <typeparam name="TOk">
/// The <see cref="Type"/> of the Value stored with an <c>Ok</c> Result
/// </typeparam>
/// <typeparam name="TError">
/// The <see cref="Type"/> of the Value stored with an <c>Error</c> Result 
/// </typeparam>
public readonly struct Result<TOk, TError> :
#if NET7_0_OR_GREATER
    IEqualityOperators<Result<TOk, TError>, Result<TOk, TError>, bool>,
    IEqualityOperators<Result<TOk, TError>, bool, bool>,
    IBitwiseOperators<Result<TOk, TError>, Result<TOk, TError>, bool>,
    IBitwiseOperators<Result<TOk, TError>, bool, bool>,
#endif
    IEquatable<Result<TOk, TError>>,
    IEquatable<bool>,
    IEnumerable<TOk>
{
    public static implicit operator Result<TOk, TError>(TOk value) => Ok(value);
    public static implicit operator Result<TOk, TError>(TError error) => Error(error);
    public static implicit operator bool(Result<TOk, TError> result) => result._ok;

    public static bool operator true(Result<TOk, TError> result) => result._ok;
    public static bool operator false(Result<TOk, TError> result) => !result._ok;
    public static bool operator !(Result<TOk, TError> result) => !result._ok;
    public static bool operator ~(Result<TOk, TError> _) 
        => throw new NotSupportedException($"Cannot apply ~ to a Result<{typeof(TOk).Name},{typeof(TError).Name}>");

    public static bool operator ==(Result<TOk, TError> left, Result<TOk, TError> right) => left.Equals(right);
    public static bool operator ==(Result<TOk, TError> result, TOk value) => result.Equals(value);
    public static bool operator ==(Result<TOk, TError> result, TError error) => result.Equals(error);
    public static bool operator ==(Result<TOk, TError> result, bool pass) => result.Equals(pass);
    public static bool operator ==(TOk value, Result<TOk, TError> result) => result.Equals(value);
    public static bool operator ==(TError error, Result<TOk, TError> result) => result.Equals(error);
    public static bool operator ==(bool pass, Result<TOk, TError> result) => result.Equals(pass);

    public static bool operator !=(Result<TOk, TError> left, Result<TOk, TError> right) => !left.Equals(right);
    public static bool operator !=(Result<TOk, TError> result, TOk value) => !result.Equals(value);
    public static bool operator !=(Result<TOk, TError> result, TError error) => !result.Equals(error);
    public static bool operator !=(Result<TOk, TError> result, bool pass) => !result.Equals(pass);
    public static bool operator !=(TOk value, Result<TOk, TError> result) => !result.Equals(value);
    public static bool operator !=(TError error, Result<TOk, TError> result) => !result.Equals(error);
    public static bool operator !=(bool pass, Result<TOk, TError> result) => !result.Equals(pass);

    public static bool operator |(Result<TOk, TError> left, Result<TOk, TError> right) => left.IsOk() || right.IsOk();
    public static bool operator |(Result<TOk, TError> result, bool pass) => pass || result.IsOk();
    public static bool operator |(bool pass, Result<TOk, TError> result) => pass || result.IsOk();
    
    public static bool operator &(Result<TOk, TError> left, Result<TOk, TError> right) => left.IsOk() && right.IsOk();
    public static bool operator &(Result<TOk, TError> result, bool pass) => pass && result.IsOk();
    public static bool operator &(bool pass, Result<TOk, TError> result) => pass && result.IsOk();

    public static bool operator ^(Result<TOk, TError> left, Result<TOk, TError> right) => left.IsOk() ^ right.IsOk();
    public static bool operator ^(Result<TOk, TError> result, bool pass) => pass ^ result.IsOk();
    public static bool operator ^(bool pass, Result<TOk, TError> result) => pass ^ result.IsOk();

    /// <summary>
    /// Get an <c>Ok</c> <see cref="Result{V,E}"/> with attached <paramref name="okValue"/>
    /// </summary>
    public static Result<TOk, TError> Ok(TOk okValue)
    {
        return new(true, okValue, default);
    }
    
    /// <summary>
    /// Gets an <c>Error</c> <see cref="Result{V,E}"/> with attached <paramref name="errorValue"/>
    /// </summary>
    /// <param name="errorValue"></param>
    /// <returns></returns>
    public static Result<TOk, TError> Error(TError? errorValue)
    {
        return new(false, default!, errorValue);
    }
    
    
    private readonly bool _ok;
    private readonly TOk _value;
    private readonly TError? _error;

    internal Result(bool ok, TOk value, TError? error)
    {
        _ok = ok;
        _value = value;
        _error = error;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOk() => _ok;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsOk([MaybeNullWhen(false)] out TOk value)
    {
        value = _value;
        return _ok;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsError() => !_ok;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsError(out TError? error)
    {
        error = _error;
        return !_ok;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Match(Action<TOk> onOk, Action<TError?> onError)
    {
        if (_ok)
        {
            onOk(_value);
        }
        else
        {
            onError(_error);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TReturn Match<TReturn>(Func<TOk, TReturn> onOk, Func<TError?, TReturn> onError)
    {
        if (_ok)
        {
            return onOk(_value);
        }
        else
        {
            return onError(_error);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Option<TOk> AsOption()
    {
        return _ok ? Option<TOk>.Some(_value) : Option<TOk>.None;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<TOk> GetEnumerator()
    {
        if (_ok)
        {
            yield return _value;
        }
    }
    
    public bool Equals(Result<TOk, TError> result)
    {
        if (_ok)
        {
            return result.IsOk(out var okValue) && EqualityComparer<TOk>.Default.Equals(_value!, okValue!);
        }
        else
        {
            return result.IsError(out var errorValue) && EqualityComparer<TError>.Default.Equals(_error!, errorValue!);
        }
    }
  
    public bool Equals(TOk? okValue) => _ok && EqualityComparer<TOk>.Default.Equals(_value!, okValue!);
    public bool Equals(TError? errorValue) => !_ok && EqualityComparer<TError>.Default.Equals(_error!, errorValue!);
    public bool Equals(bool isOk) => _ok == isOk;

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            Result<TOk, TError> fullResult => Equals(fullResult),
            TOk value => Equals(value),
            TError ex => Equals(ex),
            bool isOk => Equals(isOk),
            _ => false,
        };
    }

    public override int GetHashCode() => _ok ? Hasher.GetHashCode<TOk>(_value) : Hasher.GetHashCode<TError>(_error);

    public override string ToString()
    {
        return Match(
            ok => $"Ok({typeof(TOk).Name}: {ok})",
            error => $"Error({typeof(TError).Name}: {error})");
    }
}