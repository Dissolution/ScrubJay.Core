using ScrubJay.Collections;

namespace ScrubJay.Validation;

/// <summary>
/// A collection of sequential validations
/// </summary>
public sealed class Validations : IEnumerable<Unit>, IEnumerable
{
    public static Validations New => new();
    
    private Option<Exception> _hasException;
    
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

    public Result<Unit, Exception> GetResult()
    {
        if (_hasException.IsSome(out var ex))
            return ex;
        return Unit.Default;
    }

    public Result<TOk, Exception> GetResult<TOk>(TOk okValue)
    {
        if (_hasException.IsSome(out var ex))
            return ex;
        return okValue;
    }
    
    IEnumerator IEnumerable.GetEnumerator() => EmptyEnumerator<Unit>.Instance;
    IEnumerator<Unit> IEnumerable<Unit>.GetEnumerator() => EmptyEnumerator<Unit>.Instance;
}