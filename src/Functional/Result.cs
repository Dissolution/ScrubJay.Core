// Prefix generic type parameter with T
// Do not declare static methods on generic types
// Do not catch Exception

#pragma warning disable CA1715, CA1000, CA1031


namespace ScrubJay.Functional;

[PublicAPI]
[AsyncMethodBuilder(typeof(ResultAsyncMethodBuilder<>))]
[StructLayout(LayoutKind.Auto)]
public readonly partial struct Result :
#if NET7_0_OR_GREATER
    IEqualityOperators<Result, Result, bool>,
#endif
#if NET6_0_OR_GREATER
    ISpanFormattable,
#endif
    IEquatable<Result>,
    IFormattable
{
#region Operators

    public static implicit operator bool(Result result) => result._error is null;
    public static implicit operator Result(bool success) => success ? Ok : Error(Ex.Invalid());
    public static implicit operator Result(Exception ex) => Error(ex);
    public static implicit operator Result(IMPL.Error<Exception> error) => Error(error.Value);

    public static bool operator ==(Result left, Result right) => left.Equals(right);
    public static bool operator !=(Result left, Result right) => !left.Equals(right);

#endregion


    public static readonly Result Ok = new Result(null);
    public static Result Error(Exception ex) => new Result(ex);

    // Unlike Result<T> and Result<T,E>, default(Result) == true
    private readonly Exception? _error;

    private Result(Exception? error)
    {
        _error = error;
    }


    public bool IsOk() => _error is null;



#region Error

    public bool IsError() => _error is not null;

    public bool IsError([MaybeNullWhen(false)] out Exception error)
    {
        error = _error;
        return error is not null;
    }

    /// <summary>
    /// Returns <c>true</c> if this Result is Error and the value inside of it matches a predicate
    /// </summary>
    /// <param name="errorPredicate"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/result/enum.Result.html#method.is_err_and"/>
    public bool IsErrorAnd(Func<Exception, bool> errorPredicate) => _error is not null && errorPredicate(_error!);

    public Exception ErrorOr(Exception fallback)
    {
        if (_error is not null)
            return _error;
        return fallback;
    }

    public Exception ErrorOr(Func<Exception> getFallback)
    {
        if (_error is not null)
            return _error;
        return getFallback();
    }

    [StackTraceHidden]
    public void ThrowIfError()
    {
        if (_error is not null)
        {
            throw _error;
        }
    }

#endregion

#region Match

    public void Match(Action onOk, Action<Exception> onError)
    {
        if (_error is null)
        {
            onOk();
        }
        else
        {
            onError(_error!);
        }
    }


    public R Match<R>(Func<R> onOk, Func<Exception, R> onError)
    {
        if (_error is null)
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
        if (_error is null)
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
        return EqualityComparer<Exception>.Default.Equals(_error!, other._error!);
    }

    public bool Equals(Exception? error)
    {
        return EqualityComparer<Exception>.Default.Equals(_error!, error!);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj switch
        {
            Result result => Equals(result),
            Exception ex => Equals(ex),
            bool isOk => isOk == _error is null,
            _ => false,
        };


    public override int GetHashCode()
    {
        return Hasher.Hash<Exception>(_error);
    }

#endregion

#region ToString / TryFormat

    public string ToString(string? format, IFormatProvider? provider = null)
    {
        if (_error is null)
        {
            return "Result.Ok";
        }

        return Build($"Result.Error({_error:@})");
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
    }

    public override string ToString()
    {
        if (_error is null)
        {
            return "Result.Ok";
        }

        return Build($"Result.Error({_error:@})");
    }

#endregion
}