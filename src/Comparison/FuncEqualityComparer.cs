namespace ScrubJay.Comparison;

/// <summary>
/// An <see cref="IEqualityComparer{T}"/> and <see cref="IEqualityComparer"/> implementation
/// that uses underlying delegates for equality and hashing
/// </summary>
/// <typeparam name="T"></typeparam>
[PublicAPI]
public sealed class FuncEqualityComparer<T> : IEqualityComparer<T>, IEqualityComparer
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    private static readonly Fn<T?, int> _throwCannotHash =
        _ => throw Ex.Invalid(Build($"{typeof(T):@} values cannot be hashed"));

    private readonly Fn<T?, T?, bool> _equals;
    private readonly Fn<T?, int> _getHashCode;

    public FuncEqualityComparer(Fn<T?, T?, bool> equals)
    {
        _equals = equals;
        _getHashCode = _throwCannotHash;
    }

    public FuncEqualityComparer(Fn<T?, T?, bool> equals, Fn<T?, int> getHashCode)
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
        if (obj is T value)
            return _getHashCode(value);

        if (obj is null)
        {
            if (typeof(T).CanContainNull())
                return _getHashCode(default!);
            else
                return Hasher.NullHash;
        }

        throw Ex.Arg(obj, $"Object was not a {typeof(T):@}");
    }

    public bool Equals(T? x, T? y) => _equals(x, y);

    public int GetHashCode(T? value) => _getHashCode(value);
}