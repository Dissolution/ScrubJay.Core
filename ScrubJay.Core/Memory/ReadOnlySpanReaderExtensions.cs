namespace ScrubJay.Memory;

/// <summary>
/// Extensions on <see cref="ReadOnlySpanEnumerator{T}"/>
/// </summary>
/// <remarks>
/// These methods are not on <see cref="ReadOnlySpanEnumerator{T}"/> itself so that we can constrain
/// the generic type further
/// </remarks>
public static class ReadOnlySpanEnumeratorExtensions
{
    public static void SkipWhile<T>(
        this ref ReadOnlySpanEnumerator<T> readOnlySpanEnumerator,
        ReadOnlySpan<T> match)
        where T : IEquatable<T>
    {
        int readCount = readOnlySpanEnumerator.EnumeratedCount;
        ReadOnlySpan<T> span = readOnlySpanEnumerator.ReadOnlySpan[readCount..];
        int i = 0;
        int capacity = span.Length;
        while (i < capacity && MemoryExtensions.StartsWith(span[i..], match))
        {
            i += match.Length;
        }
        readOnlySpanEnumerator.EnumeratedCount = readCount + i;
    }
    
    public static ReadOnlySpan<T> TakeWhile<T>(
        this ref ReadOnlySpanEnumerator<T> readOnlySpanEnumerator,
        ReadOnlySpan<T> match)
        where T : IEquatable<T>
    {
        int readCount = readOnlySpanEnumerator.EnumeratedCount;
        ReadOnlySpan<T> span = readOnlySpanEnumerator.ReadOnlySpan[readCount..];
        int i = 0;
        int capacity = span.Length;
        while (i < capacity && MemoryExtensions.StartsWith(span[i..], match))
        {
            i += match.Length;
        }

        readOnlySpanEnumerator.EnumeratedCount = readCount + i;
        return span[..i];
    }
    
    public static void SkipUntil<T>(
        this ref ReadOnlySpanEnumerator<T> readOnlySpanEnumerator,
        ReadOnlySpan<T> match)
        where T : IEquatable<T>
    {
        int readCount = readOnlySpanEnumerator.EnumeratedCount;
        ReadOnlySpan<T> span = readOnlySpanEnumerator.ReadOnlySpan[readCount..];
        int i = 0;
        int capacity = span.Length;
        while (i < capacity && !MemoryExtensions.StartsWith(span[i..], match))
        {
            i += match.Length;
        }
        readOnlySpanEnumerator.EnumeratedCount = readCount + i;
    }
    
    public static ReadOnlySpan<T> TakeUntil<T>(
        this ref ReadOnlySpanEnumerator<T> readOnlySpanEnumerator,
        ReadOnlySpan<T> match)
        where T : IEquatable<T>
    {
        int readCount = readOnlySpanEnumerator.EnumeratedCount;
        ReadOnlySpan<T> span = readOnlySpanEnumerator.ReadOnlySpan[readCount..];
        int i = 0;
        int capacity = span.Length;
        while (i < capacity && !MemoryExtensions.StartsWith(span[i..], match))
        {
            i += match.Length;
        }

        readOnlySpanEnumerator.EnumeratedCount = readCount + i;
        return span[..i];
    }
}