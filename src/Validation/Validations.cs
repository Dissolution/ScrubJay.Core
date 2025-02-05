#pragma warning disable CA1710, IDE0028, CA1031, CA1010

namespace ScrubJay.Validation;

/// <summary>
/// A collection of validations
/// </summary>
/// <remarks>
/// Supports <a href="https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/object-and-collection-initializers#collection-initializers">collection initialization</a>
/// and <a href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/collection-expressions">collection expressions</a>
/// </remarks>
public sealed class Validations : IEnumerable
{
    private readonly List<Exception> _exceptions = [];

    public void Add(Result<Unit, Exception> result)
    {
        if (result.HasError(out var exception))
        {
            _exceptions.Add(exception);
        }
    }

    public void Add<T>(Result<T, Exception> result)
    {
        if (result.HasError(out var exception))
        {
            _exceptions.Add(exception);
        }
    }

    public void Add(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            _exceptions.Add(ex);
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
            _exceptions.Add(ex);
        }
    }

    public void Add(Func<Result<Unit, Exception>> getResult)
         => Add(getResult());

    public void Add<T>(Func<Result<T, Exception>> getResult)
        => Add(getResult());

    /// <summary>
    /// If any <see cref="Exception">Exceptions</see> were generated during any <c>Add</c> method,
    /// throw a single exception or an <see cref="AggregateException"/>
    /// </summary>
    public void ThrowErrors()
    {
        var exceptions = _exceptions;
        int count = exceptions.Count;
        if (count == 0)
            return;
        if (count == 1)
            throw exceptions[0];
        throw new AggregateException($"{count} Validations Failed", exceptions);
    }

    IEnumerator IEnumerable.GetEnumerator() => Enumerator.Empty<Unit>();
}
