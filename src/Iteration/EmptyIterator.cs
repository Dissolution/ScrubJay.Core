namespace ScrubJay.Iteration;

[PublicAPI]
public sealed class EmptyIterator<T> : IIndexableIterator<T>, IHasDefault<EmptyIterator<T>>
{
    public static EmptyIterator<T> Default { get; } = new EmptyIterator<T>();

    public int Count => 0;

    public Option<T> Next() => None<T>();

    public Option<T> Prev() => None<T>();

    public bool MoveBefore(Index valueIndex) => true;

    public bool MoveAfter(Index valueIndex) => true;
}