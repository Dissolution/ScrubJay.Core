namespace ScrubJay.Iteration;

[PublicAPI]
public readonly ref struct Iterable<R, V>
    where R : struct, IIterator<V>
#if NET9_0_OR_GREATER
    , allows ref struct
#endif
{
    public readonly R Iterator;

    public Iterable(R iterator)
    {
        this.Iterator = iterator;
    }
}

public static class Iterable
{
    public static Iterable<EmptyIterator<T>, T> Empty<T>() => new(EmptyIterator<T>.Default);
}