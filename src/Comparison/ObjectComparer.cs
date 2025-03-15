namespace ScrubJay.Comparison;

/// <summary>
/// A combination <see cref="IEqualityComparer{T}"/> and <see cref="IComparer{T}"/> for <see cref="object"/>
/// that uses underlying type equality
/// </summary>
public sealed class ObjectComparer :
    IEqualityComparer<object>, IEqualityComparer,
    IComparer<object>, IComparer
{
    public static readonly ObjectComparer Default = new();

    public new bool Equals(object? x, object? y)
    {
        if (ReferenceEquals(x, y))
            return true;

        if (x is not null)
        {
            Type xType = x.GetType();
            if (xType == typeof(object))
                return false; // reference failed
            return Equate.GetEqualityComparer(xType).Equals(x, y);
        }

        Debug.Assert(y is not null);
        Type yType = y!.GetType();
        if (yType == typeof(object))
            return false; // reference failed
        return Equate.GetEqualityComparer(yType).Equals(x, y);
    }

    public int GetHashCode(object? obj) => Hasher.GetHashCode<object>(obj);

    public int Compare(object? x, object? y)
    {
        if (ReferenceEquals(x, y))
            return 0;

        // nulls sort before all other values
        if (x is null)
            return -1;
        if (y is null)
            return 1;

        Type xType = x.GetType();
        if (xType == typeof(object))
            return -1; // both objects should have been caught in RefEquals, so x is 'less complex' than y
        return Comparison.Compare.GetComparer(xType).Compare(x, y);
    }
}
