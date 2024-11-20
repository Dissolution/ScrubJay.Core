namespace ScrubJay.Collections;

[PublicAPI]
public static class Enumerator
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ArrayEnumerator<T> ForArray<T>(T[] array) => new(array);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerator<T> Empty<T>() => new SingleValueEnumerator<T>();


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable CA1720 // Identifier contains type name
    public static IEnumerator<T> Single<T>(T value) => new SingleValueEnumerator<T>(value);
}