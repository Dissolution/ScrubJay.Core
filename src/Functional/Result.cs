// Prefix generic type parameter with T
// Do not declare static methods on generic types
// Do not catch Exception

#pragma warning disable CA1715, CA1000, CA1031


namespace ScrubJay.Functional;

[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly struct Result :
#if NET7_0_OR_GREATER
    IEqualityOperators<Result, Result, bool>,
#endif
    IEquatable<Result>
{
#region Operators

    public static implicit operator bool(Result result) => result._isOk;
    public static implicit operator Result(bool success) => success ? Ok : Error(null);
    public static implicit operator Result(Exception ex) => Error(ex);
    public static implicit operator Result(IMPL.Error<Exception> error) => Error(error.Value);

    public static bool operator ==(Result left, Result right) => left.Equals(right);
    public static bool operator !=(Result left, Result right) => !left.Equals(right);

#endregion


    public static readonly Result Ok = new Result(true, null);

    public static Result Error(Exception? ex) => new Result(false, ex ?? new InvalidOperationException());

    private readonly bool _isOk;
    private readonly Exception? _error;

    private Result(bool isOk, Exception? error)
    {
        _isOk = isOk;
        _error = error;
        Debug.Assert((isOk && error is null) || (!isOk && error is not null));
    }


    public bool IsOk() => _isOk;


#region Error

    public bool IsError() => !_isOk;

    public bool IsError([MaybeNullWhen(false)] out Exception error)
    {
        error = _error;
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

    [StackTraceHidden]
    public void ThrowIfError()
    {
        if (!_isOk)
        {
            throw _error!;
        }
    }

#endregion

#region Match

    public void Match(Action onOk, Action<Exception> onError)
    {
        if (_isOk)
        {
            onOk();
        }
        else
        {
            onError(_error!);
        }
    }


    public R Match<R>(Func<R> onOk, Func<Exception, R> onError)
#if NET9_0_OR_GREATER
    where R : allows ref struct
#endif
    {
        if (_isOk)
        {
            return onOk();
        }
        else
        {
            return onError(_error!);
        }
    }

#endregion

    public Option<Unit> AsOption()
    {
        if (_isOk)
        {
            return Some(default(Unit));
        }
        else
        {
            return None;
        }
    }

#region Equality

    public bool Equals(Result other)
    {
        if (_isOk)
            return other._isOk;
        return !other._isOk && EqualityComparer<Exception>.Default.Equals(_error!, other._error!);
    }

    public bool Equals(Exception? error)
    {
        return !_isOk && EqualityComparer<Exception>.Default.Equals(_error!, error!);
    }

    public bool Equals(bool success)
    {
        return _isOk == success;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj switch
        {
            Result result => Equals(result),
            Exception ex => Equals(ex),
            bool isOk => Equals(isOk),
            _ => false,
        };


    public override int GetHashCode()
    {
#if NETFRAMEWORK || NETSTANDARD2_0
        if (_isOk)
        {
            return typeof(Unit).GetHashCode();
        }
        else
        {
            if (_error is not null)
                return _error.GetHashCode();
            
            return typeof(Exception).GetHashCode();
        }
#else
        return HashCode.Combine(_isOk, _error);
#endif
    }

#endregion

#region Formatting

    public override string ToString()
    {
        if (_isOk)
        {
            return "Ok";
        }
        else
        {
            return $"Error({_error})";
        }
    }

#endregion
}