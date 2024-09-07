namespace ScrubJay.Collections;

/// <summary>
/// Utility methods for creating <see cref="Bounds{T}"/>
/// </summary>
[PublicAPI]
public static class Bounds
{
    /// <summary>
    /// Get <see cref="Bounds{T}"/> that match any value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Bounds<T> Any<T>() => new(None(), false, None(), false);

    /// <summary>
    /// Get <see cref="Bounds{T}"/> that starts at a <paramref name="minimum"/> value and has no upper bound
    /// </summary>
    /// <param name="minimum"></param>
    /// <param name="minIsInclusive"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Bounds<T> StartAt<T>(T minimum, bool minIsInclusive = true) => new Bounds<T>(minimum, minIsInclusive, None(), default);

    /// <summary>
    /// Get <see cref="Bounds{T}"/> that ends at a <paramref name="maximum"/> value and has no lower bound
    /// </summary>
    /// <param name="maximum"></param>
    /// <param name="maxIsInclusive"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Bounds<T> EndAt<T>(T maximum, bool maxIsInclusive = true) => new Bounds<T>(None(), false, maximum, maxIsInclusive);

    public static Bounds<T> Create<T>(Option<T> minimum, bool minIsInclusive, Option<T> maximum, bool maxIsInclusive) => new Bounds<T>(minimum, minIsInclusive, maximum, maxIsInclusive);

    public static Bounds<T> IncMinMax<T>(T inclusiveMinimum, T inclusiveMaximum) => new Bounds<T>(inclusiveMinimum, true, inclusiveMaximum, true);

    
    public static Bounds<int> FromRange(Range range)
    {
        if (range.Start.IsFromEnd || range.End.IsFromEnd)
            throw new ArgumentOutOfRangeException(nameof(range), range, "Range Start and End must be specified from 0");
        return Create<int>(range.Start.Value, true, range.End.Value, false);
    }

    public static Bounds<int> From<T>(ReadOnlySpan<T> span) => new Bounds<int>(0, true, span.Length, false);
    
    public static Bounds<int> For<T>(T[] array) => new Bounds<int>(0, true, array.Length, false);
    
    public static Bounds<int> From<T>(ICollection<T> collection)
    {
        return new(0, true, collection.Count, false);
    }
    
    public static Bounds<int> From<T>(IReadOnlyCollection<T> collection)
    {
        return new(0, true, collection.Count, false);
    }
}