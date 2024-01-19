namespace ScrubJay.Comparison;

#pragma warning disable CS8604
public sealed class DefaultEqualityComparer : IEqualityComparer
{
    public static DefaultEqualityComparer Instance { get; } = new();

    private DefaultEqualityComparer() { }

    bool IEqualityComparer.Equals(object? x, object? y) => Relate.Equal.Objects(x, y);
    public new bool Equals(object x, object y) => Relate.Equal.Objects(x, y);
    public int GetHashCode(object? obj) => Relate.Hash.Value<object?>(obj);
}

public sealed class DefaultEqualityComparer<T> : IEqualityComparer<T>, IEqualityComparer
{
    public static DefaultEqualityComparer<T> Instance { get; } = new();

    private DefaultEqualityComparer()
    {
    }

    public bool Equals(T? x, T? y) => Relate.Equal.Values<T>(x, y);
    public int GetHashCode(T? value) => Relate.Hash.Value<T>(value);
    bool IEqualityComparer.Equals(object? x, object? y) => Relate.Equal.Objects(x, y);
    public new bool Equals(object? x, object? y) => Relate.Equal.Objects(x, y);
    public int GetHashCode(object? obj) => Relate.Hash.Value<object?>(obj);
}