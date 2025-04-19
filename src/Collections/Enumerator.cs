namespace ScrubJay.Collections;

/// <summary>
/// Helper utility for generating <see cref="IEnumerator{T}"/> instances
/// </summary>
[PublicAPI]
public static class Enumerator
{
    /// <summary>
    /// Gets an <see cref="ArrayEnumerator{T}"/> for the given <paramref name="array"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayEnumerator<T> ForArray<T>(T[] array)
    {
        return new(array);
    }

    /// <summary>
    /// Gets an empty <see cref="IEnumerator{T}"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerator<T> Empty<T>() => new EmptyEnumerable<T>();

    /// <summary>
    /// Gets an <see cref="IEnumerator{T}"/> that enumerates over a single <paramref name="value"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerator<T> One<T>(T value) => new SingleValueEnumerator<T>(value);
}
