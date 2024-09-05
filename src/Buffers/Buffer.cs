namespace ScrubJay.Buffers;

/// <summary>
/// A delegate that represents access to the available portion of a <see cref="Buffer{T}"/>
/// </summary>
/// <param name="emptyBuffer">
/// The available space in a <see cref="Buffer{T}"/>
/// </param>
/// <returns>
/// The number of items added to <paramref name="emptyBuffer"/>
/// </returns>
public delegate int UseAvailable<T>(Span<T> emptyBuffer);

[PublicAPI]
public static class Buffer
{
    /// <summary>
    /// Create a new <see cref="Buffer{T}"/> over a <c>stackalloc</c> <see cref="Span{T}"/>
    /// </summary>
    /// <param name="span"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [return: MustDisposeResource]
    public static Buffer<T> FromStackAlloc<T>(Span<T> span)
    {
        return new Buffer<T>(span, 0);
    }

    /// <summary>
    /// Create a new <see cref="Buffer{T}"/> over an existing <see cref="Span{T}"/>
    /// </summary>
    /// <param name="span"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [return: MustDisposeResource]
    public static Buffer<T> FromSpan<T>(Span<T> span)
    {
        return new Buffer<T>(span, span.Length);
    }
    
    /// <summary>
    /// Create a new <see cref="Buffer{T}"/> over an existing <see cref="Array">T[]</see>
    /// </summary>
    /// <param name="array"></param>
    /// <param name="takeOwnership"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [return: MustDisposeResource]
    public static Buffer<T> FromArray<T>(T[] array, bool takeOwnership)
    {
        if (takeOwnership)
        {
            return new Buffer<T>(array, array.Length);
        }
        else
        {
            return new Buffer<T>(array.AsSpan(), array.Length);
        }
    }
}