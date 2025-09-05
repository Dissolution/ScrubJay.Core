// Exception to Identifiers Require Correct Suffix

#pragma warning disable CA1710

namespace ScrubJay.Validation;

[PublicAPI]
public class Validations : Validations<Exception>
{
    public void Add(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            base.Add(ex);
        }
    }

    public void Add<T>(Func<T> func)
    {
        try
        {
            _ = func();
        }
        catch (Exception ex)
        {
            base.Add(ex);
        }
    }

    public void Add<T>(Fn<T> func)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        try
        {
            _ = func();
        }
        catch (Exception ex)
        {
            base.Add(ex);
        }
    }

    public void Add(Result<Unit> result)
    {
        if (result.IsError(out var error))
        {
            base.Add(error);
        }
    }

    public void Add<T>(Result<T> result)
    {
        if (result.IsError(out var error))
        {
            base.Add(error);
        }
    }

    public void Add(Func<Result<Unit>> getResult)
        => Add(getResult());

    public void Add<T>(Func<Result<T>> getResult)
        => Add(getResult());

    public Option<Exception> HasException()
    {
        var exceptions = _errors;
        return exceptions.Count switch
        {
            0 => None,
            1 => Some(exceptions[0]),
            _ => Some<Exception>(new AggregateException($"{exceptions.Count} Validations Failed", exceptions)),
        };
    }

    public bool HasException([NotNullWhen(true)] out Exception? exception)
    {
        var exceptions = _errors;
        int count = exceptions.Count;
        if (count == 0)
        {
            exception = null;
            return false;
        }

        if (count == 1)
        {
            exception = exceptions[0];
            return true;
        }

        exception = new AggregateException($"{exceptions.Count} Validations Failed", exceptions);
        return true;
    }

    public override void ThrowIfErrors()
    {
        if (HasException(out var ex))
            throw ex;
    }

    public Result<T> ToResult<T>(T okValue)
    {
        if (HasException(out var ex))
            return ex;
        return Ok(okValue);
    }

    public Result<T> ToResult<T>(Func<T> getOk)
    {
        if (HasException(out var ex))
            return ex;
        return Ok(getOk());
    }
}

/// <summary>
/// A collection of validations
/// </summary>
/// <remarks>
/// Supports <a href="https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/object-and-collection-initializers#collection-initializers">collection initialization</a>
/// and <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/collection-expressions">collection expressions</a>
/// </remarks>
[PublicAPI]
public class Validations<E> : IReadOnlyCollection<E>
{
    protected readonly List<E> _errors = [];

    public int Count => _errors.Count;

    public void Add(E? error)
    {
        if (error is not null)
        {
            _errors.Add(error);
        }
    }

    public void Add(Result<Unit, E> result)
    {
        if (result.IsError(out var error))
            Add(error);
    }

    public void Add<T>(Result<T, E> result)
    {
        if (result.IsError(out var error))
            Add(error);
    }

    public void Add(Func<Result<Unit, E>> getResult)
        => Add(getResult());

    public void Add<T>(Func<Result<T, E>> getResult)
        => Add(getResult());

    public virtual void ThrowIfErrors()
    {
        if (_errors.Count <= 0)
            return;

        string message = TextBuilder.New
            .Format(_errors.Count)
            .Append(" validations failed:")
            .NewLine()
            .EnumerateFormatAndDelimitLines(_errors)
            .ToStringAndDispose();
        throw new InvalidOperationException(message);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<E> GetEnumerator() => _errors.GetEnumerator();
}