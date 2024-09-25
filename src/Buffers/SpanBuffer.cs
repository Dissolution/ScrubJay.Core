namespace ScrubJay.Buffers;

/// <summary>
/// Methods for creating <see cref="SpanBuffer{T}"/> instances
/// </summary>
[PublicAPI]
public static class SpanBuffer
{
    /// <summary>
    /// Create a new <see cref="SpanBuffer{T}"/> over a <c>stackalloc</c> <see cref="Span{T}"/>
    /// </summary>
    /// <param name="span"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [return: MustDisposeResource]
    public static SpanBuffer<T> FromStackAlloc<T>(Span<T> span)
    {
        return new SpanBuffer<T>(span, 0);
    }

    /// <summary>
    /// Create a new <see cref="SpanBuffer{T}"/> over an existing <see cref="Span{T}"/>
    /// </summary>
    /// <param name="span"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [return: MustDisposeResource]
    public static SpanBuffer<T> FromSpan<T>(Span<T> span)
    {
        return new SpanBuffer<T>(span, span.Length);
    }
    
    /// <summary>
    /// Create a new <see cref="SpanBuffer{T}"/> over an existing <see cref="Array">T[]</see>
    /// </summary>
    /// <param name="array"></param>
    /// <param name="takeOwnership"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [return: MustDisposeResource]
    public static SpanBuffer<T> FromArray<T>(T[] array, bool takeOwnership)
    {
        if (takeOwnership)
        {
            return new SpanBuffer<T>(array, array.Length);
        }
        else
        {
            return new SpanBuffer<T>(array.AsSpan(), array.Length);
        }
    }
}