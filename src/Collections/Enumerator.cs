namespace ScrubJay.Collections;

public static class Enumerator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerator<T> Empty<T>() => (IEnumerator<T>)Enumerable.Empty<T>();
}