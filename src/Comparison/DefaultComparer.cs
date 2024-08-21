namespace ScrubJay.Comparison;

public sealed class DefaultComparer : IComparer
{
    public static DefaultComparer Instance { get; } = new();

    private DefaultComparer()
    {
    }

    public int Compare(object? x, object? y) => Comparison.Compare.Objects(x, y);
}

public sealed class DefaultComparer<T> : IComparer<T>, IComparer
{
    public static DefaultComparer<T> Instance { get; } = new();

    private DefaultComparer()
    {
    }

    public int Compare(T? x, T? y) => Comparison.Compare.Values<T>(x, y);

    public int Compare(object? x, object? y) => Comparison.Compare.Objects(x, y);
}