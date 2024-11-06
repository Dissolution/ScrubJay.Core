namespace ScrubJay.Collections;

[PublicAPI]
public static class Enumerator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerator<T> Empty<T>() => (IEnumerator<T>)Enumerable.Empty<T>();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayEnumerator<T> ForArray<T>(T[] array) => new ArrayEnumerator<T>(array);
}