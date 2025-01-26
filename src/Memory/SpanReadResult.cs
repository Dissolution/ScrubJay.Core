namespace ScrubJay.Memory;

[PublicAPI]
public static class SpanReadResult
{
    public static SpanReadResult<T> EndOfSpan<T>() => new(StopReason.EndOfSpan, []);
    public static SpanReadResult<T> Satisified<T>() => new(StopReason.Satisified, []);
    public static SpanReadResult<T> Predicate<T>() => new(StopReason.Predicate, []);

    public static SpanReadResult<T> EndOfSpan<T>(ReadOnlySpan<T> span) => new(StopReason.EndOfSpan, span);
    public static SpanReadResult<T> Satisified<T>(ReadOnlySpan<T> span) => new(StopReason.Satisified, span);
    public static SpanReadResult<T> Predicate<T>(ReadOnlySpan<T> span) => new(StopReason.Predicate, span);
}

[PublicAPI]
public readonly ref struct SpanReadResult<T>
{
    /// <summary>
    /// The reason that capturing <see cref="Span"/> stopped
    /// </summary>
    public readonly StopReason StopReason;

    /// <summary>
    /// The <see cref="ReadOnlySpan{T}"/> read from a <see cref="SpanReader{T}"/>
    /// </summary>
    public readonly ReadOnlySpan<T> Span;


    public SpanReadResult(StopReason stopReason, ReadOnlySpan<T> span)
    {
        this.StopReason = stopReason;
        this.Span = span;
    }

    public void Deconstruct(out StopReason stopReason, out ReadOnlySpan<T> span)
    {
        stopReason = this.StopReason;
        span = this.Span;
    }

    public bool HasReason(StopReason stopReason, out ReadOnlySpan<T> span)
    {
        if (stopReason == this.StopReason)
        {
            span = this.Span;
            return true;
        }
        span = [];
        return false;
    }
}
