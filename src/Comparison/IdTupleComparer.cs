namespace ScrubJay.Comparison;

[PublicAPI]
public sealed class IdTupleComparer<T> :
    IEqualityComparer<(int Id, T Value)>,
    IComparer<(int Id, T Value)>,
    IHasDefault<IdTupleComparer<T>>
{
    public static IdTupleComparer<T> Default { get; } = new();

    public bool Equals((int Id, T Value) x, (int Id, T Value) y)
    {
        return x.Id == y.Id;
    }

    public int GetHashCode([DisallowNull] (int Id, T Value) obj)
    {
        return obj.Id;
    }

    public int Compare((int Id, T Value) x, (int Id, T Value) y)
    {
        return x.Id.CompareTo(y.Id);
    }
}