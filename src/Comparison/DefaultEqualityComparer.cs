namespace ScrubJay.Comparison;

#pragma warning disable CS8604
public sealed class DefaultEqualityComparer : IEqualityComparer
{
    public static DefaultEqualityComparer Instance { get; } = new();

    private DefaultEqualityComparer() { }

    bool IEqualityComparer.Equals(object? x, object? y) => Equate.Objects(x, y);
    public new bool Equals(object x, object y) => Equate.Objects(x, y);
    public int GetHashCode(object? obj) => obj?.GetHashCode() ?? 0;
}

public sealed class DefaultEqualityComparer<T> : IEqualityComparer<T>, IEqualityComparer
{
    public static DefaultEqualityComparer<T> Instance { get; } = new();

    private DefaultEqualityComparer()
    {
    }

    public bool Equals(T? x, T? y) => Equate.Values<T>(x, y);
    public int GetHashCode(T? value) => value?.GetHashCode() ?? 0;
    
    bool IEqualityComparer.Equals(object? x, object? y) => Equate.Objects(x, y);
    int IEqualityComparer.GetHashCode(object? obj) => obj?.GetHashCode() ?? 0;
}