namespace ScrubJay.Memory;


#if NET9_0_OR_GREATER

[PublicAPI]
public readonly ref struct Segment<T>
{
    public static implicit operator Range(Segment<T> segment) => segment.Range;

    public static implicit operator ReadOnlySpan<T>(Segment<T> segment) => segment.Span;

    public readonly Range Range;

    public readonly ReadOnlySpan<T> Span;

    public T[] Array => Span.ToArray();

    public Segment(Range range, ReadOnlySpan<T> span)
    {
        this.Range = range;
        this.Span = span;
    }

    public override string ToString()
    {
        return Build($"[{Range}]: {Span:@}");
    }
}

#else

[PublicAPI]
public readonly struct Segment<T>
{
    public static implicit operator Range(Segment<T> segment) => segment.Range;

    public static implicit operator ReadOnlySpan<T>(Segment<T> segment) => segment.Span;

    public readonly Range Range;

    public readonly T[] Array;

    public ReadOnlySpan<T> Span => Array.AsSpan();

    public Segment(Range range, T[] array)
    {
        this.Range = range;
        this.Array = array;
    }

    public Segment(Range range, ReadOnlySpan<T> span)
    {
        this.Range = range;
        this.Array = span.ToArray();
    }


    public override string ToString()
    {
        return Build($"[{Range}]: {Array:@}");
    }
}

#endif