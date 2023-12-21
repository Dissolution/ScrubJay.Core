namespace ScrubJay.Memory;

/// <summary>
/// Extensions on <see cref="SpanEnumerator{T}"/>
/// </summary>
/// <remarks>
/// These methods are not on <see cref="SpanEnumerator{T}"/> itself so that we can constrain
/// the generic type further
/// </remarks>
public static class SpanReaderExtensions
{
    public static void SkipWhile<T>(
        this ref SpanEnumerator<T> spanEnumerator,
        ReadOnlySpan<T> match)
        where T : IEquatable<T>
    {
        int readCount = spanEnumerator.EnumeratedCount;
        Span<T> span = spanEnumerator.Span[readCount..];
        int i = 0;
        int capacity = span.Length;
        while (i < capacity && span[i..].StartsWith(match))
        {
            i += match.Length;
        }
        spanEnumerator.EnumeratedCount = readCount + i;
    }
    
    public static Span<T> TakeWhile<T>(
        this ref SpanEnumerator<T> spanEnumerator,
        ReadOnlySpan<T> match)
        where T : IEquatable<T>
    {
        int readCount = spanEnumerator.EnumeratedCount;
        Span<T> span = spanEnumerator.Span[readCount..];
        int i = 0;
        int capacity = span.Length;
        while (i < capacity && span[i..].StartsWith(match))
        {
            i += match.Length;
        }

        spanEnumerator.EnumeratedCount = readCount + i;
        return span[..i];
    }
    
    public static void SkipUntil<T>(
        this ref SpanEnumerator<T> spanEnumerator,
        ReadOnlySpan<T> match)
        where T : IEquatable<T>
    {
        int readCount = spanEnumerator.EnumeratedCount;
        Span<T> span = spanEnumerator.Span[readCount..];
        int i = 0;
        int capacity = span.Length;
        while (i < capacity && !span[i..].StartsWith(match))
        {
            i += match.Length;
        }
        spanEnumerator.EnumeratedCount = readCount + i;
    }
    
    public static Span<T> TakeUntil<T>(
        this ref SpanEnumerator<T> spanEnumerator,
        ReadOnlySpan<T> match)
        where T : IEquatable<T>
    {
        int readCount = spanEnumerator.EnumeratedCount;
        Span<T> span = spanEnumerator.Span[readCount..];
        int i = 0;
        int capacity = span.Length;
        while (i < capacity && !span[i..].StartsWith(match))
        {
            i += match.Length;
        }

        spanEnumerator.EnumeratedCount = readCount + i;
        return span[..i];
    }
}