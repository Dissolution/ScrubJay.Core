#pragma warning disable CA1822

namespace ScrubJay.Comparison;

[PublicAPI]
public sealed class ObjectComparer :
    IEqualityComparer<object>, IEqualityComparer,
    IComparer<object>, IComparer,
    IHasDefault<ObjectComparer>
{
    public static ObjectComparer Default { get; } = new();

    bool IEqualityComparer<object>.Equals(object? x, object? y) => Equate(x, y);

    bool IEqualityComparer.Equals(object? x, object? y) => Equate(x, y);

    public bool Equate(object? left, object? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is not null)
        {
            Type leftType = left.GetType();
            if (leftType == typeof(object)) // prevent recursion
                return false;

            return Comparison.Equate.GetComparer(leftType).Equals(left, right);
        }

        Debug.Assert(right is not null);
        Type rightType = right!.GetType();
        if (rightType == typeof(object)) // prevent recursion
            return false;

        return Comparison.Equate.GetComparer(rightType).Equals(right, left);
    }


    int IEqualityComparer<object>.GetHashCode(object obj) => GetHashCode(obj);

    int IEqualityComparer.GetHashCode(object obj) => GetHashCode(obj);

    public int GetHashCode(object? obj) => Hasher.Hash(obj);

    int IComparer<object>.Compare(object? x, object? y) => Relate(x, y);

    int IComparer.Compare(object? x, object? y) => Relate(x, y);

    public int Relate(object? left, object? right)
    {
        if (ReferenceEquals(left, right))
            return 0;

        // null sorts as first
        if (left is null) return -1;
        if (right is null) return 1;

        Type leftType = left.GetType();

        if (leftType == typeof(object))
        {
            // left is less complex than right
            return -1;
        }

        if (right.GetType() == typeof(object))
        {
            // right is less complex than left
            return 1;
        }

        return Comparison.Relate.GetComparer(leftType).Compare(left, right);
    }
}