namespace ScrubJay.Comparison;

[PublicAPI]
public class RelativeIndexComparer<T> : IComparer<T>
{
    private readonly T[] _orderedItems;

    public RelativeIndexComparer(T[] orderedItems)
    {
        if (orderedItems.Distinct().Count() != orderedItems.Length)
            throw new ArgumentException(null, nameof(orderedItems));
        _orderedItems = orderedItems;
    }

    public int Compare(T? x, T? y)
    {
        var xIndex = Array.IndexOf<T>(_orderedItems, x!);
        var yIndex = Array.IndexOf<T>(_orderedItems, y!);

        return xIndex.CompareTo(yIndex);
    }
}
