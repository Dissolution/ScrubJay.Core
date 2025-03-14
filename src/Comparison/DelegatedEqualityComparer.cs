namespace ScrubJay.Comparison;

/// <summary>
/// An <see cref="IEqualityComparer{T}"/> and <see cref="IEqualityComparer"/> implementation
/// that uses underlying delegates for equality and hashing
/// </summary>
/// <typeparam name="T"></typeparam>
[PublicAPI]
public sealed class DelegatedEqualityComparer<T> : IEqualityComparer<T>, IEqualityComparer
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    private static readonly Fn<T, int> _throwCannotHash = _ => throw new InvalidOperationException($"{typeof(T).NameOf()} values cannot be hashed");

    private readonly Fn<T?,T?,bool> _equals;
    private readonly Fn<T,int> _getHashCode;

    public DelegatedEqualityComparer(Fn<T?, T?, bool> equals)
    {
        _equals = equals;
        _getHashCode = _throwCannotHash;
    }

    public DelegatedEqualityComparer(Fn<T?, T?, bool> equals, Fn<T, int> getHashCode)
    {
        _equals = equals;
        _getHashCode = getHashCode;
    }

    bool IEqualityComparer.Equals(object? x, object? y)
    {
        if (x is T xT && y is T yT)
            return _equals(xT, yT);
        return false;
    }

    int IEqualityComparer.GetHashCode(object? obj)
    {
        if (obj is null)
            return Hasher.NullHash;
        if (obj is T value)
            return _getHashCode(value);
        throw new ArgumentException($"Object was not a {typeof(T).NameOf()}", nameof(obj));
    }

    public bool Equals(T? x, T? y) => _equals(x, y);

    public int GetHashCode(T? value)
    {
        if (value is null)
            return Hasher.NullHash;
        return _getHashCode(value);
    }
}
