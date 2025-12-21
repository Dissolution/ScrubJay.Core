namespace ScrubJay.Iteration;

[PublicAPI]
public readonly ref struct Iterable<TIterator, TValue>
    where TIterator : struct, IIterator<TValue>
#if NET9_0_OR_GREATER
    , allows ref struct
    where TValue : allows ref struct
#endif
{
    public readonly TIterator Iterator;

    public Iterable(TIterator iterator)
    {
        this.Iterator = iterator;
    }
}

[PublicAPI]
public static class Iterable
{
    public static Iterable<EmptyIterator<T>, T> Empty<T>() => new(EmptyIterator<T>.Default);
}