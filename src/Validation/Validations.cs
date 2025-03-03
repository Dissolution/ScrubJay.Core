using ScrubJay.Pooling;
using ScrubJay.Text;

namespace ScrubJay.Validation;

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
            Add(ex);
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
            Add(ex);
        }
    }

    public Option<Exception> HasException()
    {
        var exceptions = _errors;
        return exceptions.Count switch
        {
            0 => None(),
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

    public Result<TOk, Exception> ToResult<TOk>(TOk okValue)
    {
        if (HasException(out var ex))
            return ex;
        return okValue;
    }

    public Result<TOk, Exception> ToResult<TOk>(Func<TOk> getOk)
    {
        if (HasException(out var ex))
            return ex;
        return getOk();
    }
}

/// <summary>
/// A collection of validations
/// </summary>
/// <remarks>
/// Supports <a href="https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/object-and-collection-initializers#collection-initializers">collection initialization</a>
/// and <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/collection-expressions">collection expressions</a>
/// </remarks>
public class Validations<TError> : IReadOnlyCollection<TError>
{
    protected readonly List<TError> _errors = [];

    public int Count => _errors.Count;

    public void Add(TError? error)
    {
        if (error is not null)
        {
            _errors.Add(error);
        }
    }

    public void Add(Result<Unit, TError> result)
    {
        if (result.HasError(out var error))
            Add(error);
    }

    public void Add<T>(Result<T, TError> result)
    {
        if (result.HasError(out var error))
            Add(error);
    }

    public void Add(Func<Result<Unit, TError>> getResult)
         => Add(getResult());

    public void Add<T>(Func<Result<T, TError>> getResult)
        => Add(getResult());

    public virtual void ThrowIfErrors()
    {
        if (_errors.Count > 0)
        {
            var msg = new Buffer<char>();
            msg.Write(_errors.Count);
            msg.Write(" Validations Failed:");
            foreach (var error in _errors)
            {
                msg.Write(Environment.NewLine);
                msg.Write(error);
            }
            string message = msg.ToStringAndDispose();
            throw new InvalidOperationException(message);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public IEnumerator<TError> GetEnumerator() => _errors.GetEnumerator();
}
