namespace ScrubJay.Scratch;

using System.Diagnostics;

public readonly struct Result<TOk, TError> :
    #if NET7_0_OR_GREATER
    IEqualityOperators<Result<TOk, TError>, Result<TOk, TError>, bool>,
    IEqualityOperators<Result<TOk, TError>, TOk, bool>,
    // IEqualityOperators<Result<TOk, TError>, TError, bool>,
    IEqualityOperators<Result<TOk, TError>, bool, bool>,
    //IBitwiseOperators<Result<TOk, TError>, Result<TOk, TError>, bool>,
    IBitwiseOperators<Result<TOk, TError>, bool, bool>,
#endif
    IEquatable<Result<TOk, TError>>,
    IEquatable<TOk>,
    // IEquatable<TError>,
    IEquatable<bool>,
    IEnumerable<TOk>,
    IFormattable
where TOk : notnull
where TError : notnull
{
    public static implicit operator Result<TOk, TError>([DisallowNull] TOk ok) => Ok(ok);
    public static implicit operator Result<TOk, TError>([DisallowNull] TError error) => Error(error);
    public static implicit operator bool(Result<TOk, TError> result) => result.IsOk();

    public static bool operator true(Result<TOk, TError> result) => result.IsOk();
    public static bool operator false(Result<TOk, TError> result) => result.IsError();
    public static bool operator !(Result<TOk, TError> result) => result.IsError();

    [Obsolete("Do not use ~ with Result<TOk, TError>", true)]
    public static bool operator ~(Result<TOk, TError> _)
        => throw new NotSupportedException($"Cannot apply ~ to a Result<{typeof(TOk).Name}, {typeof(TError).Name}>");

    public static bool operator ==(Result<TOk, TError> left, Result<TOk, TError> right) => left.Equals(right);
    public static bool operator ==(Result<TOk, TError> result, TOk? ok) => result.Equals(ok);
    public static bool operator ==(Result<TOk, TError> result, TError? error) => result.Equals(error);
    public static bool operator ==(Result<TOk, TError> result, bool isOk) => result.Equals(isOk);

    public static bool operator !=(Result<TOk, TError> left, Result<TOk, TError> right) => !left.Equals(right);
    public static bool operator !=(Result<TOk, TError> result, TOk? ok) => !result.Equals(ok);
    public static bool operator !=(Result<TOk, TError> result, TError? error) => !result.Equals(error);
    public static bool operator !=(Result<TOk, TError> result, bool isOk) => !result.Equals(isOk);

    public static bool operator |(Result<TOk, TError> left, Result<TOk, TError> right) => left.IsOk() || right.IsOk();
    public static bool operator |(Result<TOk, TError> result, Result r) => result.IsOk() || r.IsOk();
    public static bool operator |(Result<TOk, TError> result, bool isOk) => isOk || result.IsOk();

    public static bool operator &(Result<TOk, TError> left, Result<TOk, TError> right) => left.IsOk() && right.IsOk();
    public static bool operator &(Result<TOk, TError> result, Result r) => result.IsOk() && r.IsOk();
    public static bool operator &(Result<TOk, TError> result, bool isOk) => isOk && result.IsOk();

    public static bool operator ^(Result<TOk, TError> left, Result<TOk, TError> right) => left.IsOk() ^ right.IsOk();
    public static bool operator ^(Result<TOk, TError> result, Result r) => result.IsOk() ^ r.IsOk();
    public static bool operator ^(Result<TOk, TError> result, bool isOk) => isOk ^ result.IsOk();

    public static Result<TOk, TError> Ok([DisallowNull] TOk ok)
    {
        Throw.IfNull(ok);
        return new Result<TOk, TError>(ok);
    }
    public static Result<TOk, TError> Error([DisallowNull] TError error)
    {
        Throw.IfNull(error);
        return new Result<TOk, TError>(error);
    }

    private readonly object? _obj;

    private Result([DisallowNull] TOk ok)
    {
        Debug.Assert(ok is not null);
        _obj = (object)ok;
    }
    private Result([DisallowNull] TError error)
    {
        Debug.Assert(error is not null);
        _obj = (object)error;
    }

    public bool IsOk() => _obj is TOk;

    public bool IsOk([NotNullWhen(true), MaybeNullWhen(false)] out TOk ok)
    {
        return _obj.Is(out ok);
    }

    public bool IsError() => _obj is TError;

    public bool IsError([NotNullWhen(true), MaybeNullWhen(false)] out TError error)
    {
        return _obj.Is(out error);
    }

    public bool IsOk(
        [NotNullWhen(true), MaybeNullWhen(false)]
        out TOk ok,
        [NotNullWhen(false), MaybeNullWhen(true)]
        out TError error)
    {
        if (_obj is TOk)
        {
            ok = (TOk)_obj;
            error = default;
            return true;
        }
        else
        {
            ok = default;
            Debug.Assert(_obj is TError);
            error = (TError)_obj;
            return false;
        }
    }

    public void Match(Action<TOk> isOk, Action<TError> isError)
    {
        if (_obj is TOk ok)
        {
            isOk(ok);
        }
        else
        {
            Debug.Assert(_obj is TError);
            isError((TError)_obj);
        }
    }

    public TReturn Match<TReturn>(Func<TOk, TReturn> isOk, Func<TError, TReturn> isError)
    {
        if (_obj is TOk ok)
        {
            return isOk(ok);
        }
        else
        {
            Debug.Assert(_obj is TError);
            return isError((TError)_obj);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<TOk> GetEnumerator()
    {
        if (this.IsOk(out var ok))
            yield return ok;
    }

    public bool Equals(Result<TOk, TError> result)
    {
        if (this.IsOk(out var thisOk, out var thisError))
        {
            return result.IsOk(out var thatOk) && EqualityComparer<TOk>.Default.Equals(thisOk, thatOk);
        }
        else
        {
            return result.IsError(out var thatError) && EqualityComparer<TError>.Default.Equals(thisError, thatError);
        }
    }


    public bool Equals(TOk? thatOk) => this.IsOk(out var thisOk) && EqualityComparer<TOk>.Default.Equals(thisOk, thatOk);
    public bool Equals(TError? thatError) => this.IsError(out var thisError) && EqualityComparer<TError>.Default.Equals(thisError, thatError);
    public bool Equals(bool isOk) => isOk ? this.IsOk() : this.IsError();
    public override bool Equals(object? obj) => obj switch
    {
        Result<TOk, TError> result => Equals(result),
        TOk ok => Equals(ok),
        TError error => Equals(error),
        bool isOk => Equals(isOk),
        _ => false,
    };
    public override int GetHashCode() => _obj?.GetHashCode() ?? 0;

    public string ToString(string? format, IFormatProvider? provider = default)
    {
        string? str;
        if (_obj is IFormattable formattable)
        {
            str = formattable.ToString(format, provider);
        }
        else
        {
            str = _obj?.ToString();
        }
        if (_obj is TOk)
        {
            return $"Result.Ok<{typeof(TOk).Name}>({str})";
        }
        return $"Result.Error<{typeof(TError).Name}>({str})";

    }

    public override string ToString() => Match(static ok => $"Ok({ok})", static error => $"Error({error})");
}