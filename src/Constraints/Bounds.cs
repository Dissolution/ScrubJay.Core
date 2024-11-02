namespace ScrubJay.Constraints;

[PublicAPI]
public static class Bounds
{
    public static Bounds<T> Any<T>() => new Bounds<T>(Option.None(), Option.None());
    public static Bounds<T> None<T>() => new Bounds<T>(Some(Bound<T>.Exclusive(default!)), Some(Bound<T>.Exclusive(default!)));
    public static Bounds<T> StartAt<T>(T minimum, bool inclusive = true) => new Bounds<T>(Some(new Bound<T>(minimum, inclusive)), Option.None());
    public static Bounds<T> EndAt<T>(T maximum, bool inclusive = false) => new Bounds<T>(Option.None(), Some(new Bound<T>(maximum, inclusive)));
    public static Bounds<T> Create<T>(Bound<T> minimum, Bound<T> maximum) => new(Some(minimum), Some(maximum));

    public static Option<Bounds<int>> FromRange(Range range)
    {
        if (range.Start.IsFromEnd || range.End.IsFromEnd)
            return Option.None();
        return Some(Create(Bound<int>.Inclusive(range.Start.Value), Bound<int>.Exclusive(range.End.Value)));
    }

    public static Bounds<int> ForLength(int length) => Create<int>(Bound<int>.Inclusive(0), Bound<int>.Exclusive(length));

    public static Bounds<int> For<T>(ReadOnlySpan<T> span) => ForLength(span.Length);

    public static Option<Bounds<int>> For<T>(T[]? array)
    {
        if (array is null)
            return Option.None();
        return Some(ForLength(array.Length));
    }

    public static Option<Bounds<int>> For<T>(IEnumerable<T>? enumerable)
    {
        if (enumerable is null)
            return Option.None();
        if (enumerable.TryGetNonEnumeratedCount(out int count))
            return Some(ForLength(count));
        return Option.None();
    }
}