namespace ScrubJay.Comparison;

[PublicAPI]
public abstract class Relater<T> :
    IEqualityComparer<T>, IEqualityComparer,
    IComparer<T>, IComparer
{
    private sealed class DefaultRelater : Relater<T>
    {
        public override bool Equate(T? left, T? right)
        {
            return EqualityComparer<T>.Default.Equals(left!, right!);
        }

        public override int GetHashCode(T? value)
        {
            if (value is null)
                return 0;
            return EqualityComparer<T>.Default.GetHashCode(value);
        }

        public override int Compare(T? left, T? right)
        {
            return Comparer<T>.Default.Compare(left!, right!);
        }
    }

    bool IEqualityComparer.Equals(object? x, object? y) => Equate(x.ThrowIfNot<T>(), y.ThrowIfNot<T>());
    bool IEqualityComparer<T>.Equals(T? x, T? y) => Equate(x, y);

    int IEqualityComparer.GetHashCode(object? obj) => GetHashCode(obj.ThrowIfNot<T>());
    int IEqualityComparer<T>.GetHashCode(T obj) => GetHashCode(obj);

    int IComparer.Compare(object? x, object? y) => Compare(x.ThrowIfNot<T>(), y.ThrowIfNot<T>());
    int IComparer<T>.Compare(T? x, T? y) => Compare(x, y);

    public abstract bool Equate(T? left, T? right);
    public abstract int GetHashCode(T? value);
    public abstract int Compare(T? left, T? right);
}