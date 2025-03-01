namespace ScrubJay.Pooling;

[PublicAPI]
public interface IArrayInstancePool<T> : IInstancePool<T[]>
{
    /// <summary>
    /// Rent a <c>T[]</c> from this pool with a minimum <see cref="Array.Length"/>
    /// </summary>
    /// <param name="minimumLength">
    /// The minimum length of the returned <c>T[]</c>, but can be larger
    /// </param>
    T[] Rent(int minimumLength);
}
