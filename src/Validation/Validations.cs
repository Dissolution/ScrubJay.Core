using ScrubJay.Collections;

namespace ScrubJay.Validation;

/// <summary>
/// A collection of sequential validations
/// </summary>
public class Validations : IEnumerable
{
    protected Option<Exception> _hasException = default;

    public Result<Unit, Exception> Result
    {
        get
        {
            if (_hasException.IsSome(out var ex))
                return Result<Unit, Exception>.Error(ex);
            return Result<Unit, Exception>.Ok(default);
        }
    }

    public void Add(Result<Unit, Exception> result) => Add<Unit>(result);
    
    public void Add<T>(Result<T, Exception> result)
    {
        if (_hasException)
            return; // we've already failed

        if (result.IsError(out var ex))
        {
            _hasException = Some(ex);
        }
    }

    public void Add(Func<Result<Unit, Exception>> getResult) => Add<Unit>(getResult());

    public void Add<T>(Func<Result<T, Exception>> getResult) => Add<T>(getResult());

    public IEnumerator GetEnumerator() => EmptyEnumerator<Unit>.Instance;
}