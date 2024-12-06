namespace ScrubJay.Comparison;

/// <summary>
/// The default comparer for two objects so they can be compared by any containing value
/// </summary>
internal sealed class ObjectComparer :
    IEqualityComparer<object>, IEqualityComparer,
    IComparer<object>, IComparer
{
    public static readonly ObjectComparer Default = new();

    bool IEqualityComparer.Equals(object? x, object? y) => Equals(x, y);
    bool IEqualityComparer<object>.Equals(object? x, object? y) => Equals(x, y);
    public new static bool Equals(object? x, object? y)
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

    int IEqualityComparer.GetHashCode(object? obj) => GetHashCode(obj);
    int IEqualityComparer<object>.GetHashCode(object? obj) => GetHashCode(obj);
    public static int GetHashCode(object? obj) => Hasher.GetHashCode(obj);

    int IComparer<object>.Compare(object? x, object? y) => Compare(x, y);
    int IComparer.Compare(object? x, object? y) => Compare(x, y);
    public static int Compare(object? x, object? y)
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