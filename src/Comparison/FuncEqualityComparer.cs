namespace ScrubJay.Comparison;

/// <summary>
/// An <see cref="IEqualityComparer"/> that works via passed in functions
/// </summary>
[PublicAPI]
public sealed class FuncEqualityComparer : IEqualityComparer
{
    private readonly Func<object?, object?, bool> _equals;
    private readonly Func<object?, int> _getHashCode;

    public FuncEqualityComparer(Func<object?, object?, bool> equals, Func<object?, int> getHashCode)
    {
        _equals = equals;
        _getHashCode = getHashCode;
    }

    bool IEqualityComparer.Equals(object? x, object? y) => _equals(x, y);
    public new bool Equals(object? x, object? y) => _equals(x, y);
    public int GetHashCode(object? obj) => _getHashCode(obj);
}

/// <summary>
/// An <see cref="IEqualityComparer{T}"/> that works via passed in functions
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of values that can be equated
/// </typeparam>
public sealed class FuncEqualityComparer<T> : EqualityComparer<T>
{
    private readonly Func<T?, T?, bool> _equals;
    private readonly Func<T?, int> _getHashCode;

    public FuncEqualityComparer(Func<T?, T?, bool> equals, Func<T?, int> getHashCode)
    {
        _equals = equals;
        _getHashCode = getHashCode;
    }

    public override bool Equals(T? x, T? y) => _equals(x, y);

    public override int GetHashCode(T? obj) => _getHashCode(obj);
}