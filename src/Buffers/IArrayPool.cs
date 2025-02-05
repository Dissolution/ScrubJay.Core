namespace ScrubJay.Buffers;

[PublicAPI]
public interface IArrayPool<T> : IObjectPool<T[]>
{
    /// <summary>
    /// Rent a <c>T[]</c> from this pool with a minimum size
    /// </summary>
    /// <param name="minimumLength">
    /// The minimum size of the returned array
    /// </param>
    /// <remarks>
    /// <see cref="IObjectPool{T}.Return"/> the array to this pool when you are finished with it
    /// </remarks>
    T[] Rent(int minimumLength);
}
