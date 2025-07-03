#pragma warning disable CA1716, CA1710

namespace ScrubJay.Iteration;

[PublicAPI]
public interface IIterator<T>
{
    Option<T> Next();

#if !NETFRAMEWORK && !NETSTANDARD2_0
    bool Next(out T? item) => Next().IsSome(out item);
#endif
}

[PublicAPI]
public interface IReverseIterator<T>
{
    Option<T> Prev();

#if !NETFRAMEWORK && !NETSTANDARD2_0
    bool Prev(out T? item) => Prev().IsSome(out item);
#endif
}

[PublicAPI]
public interface IIndexableIterator<T> : IIterator<T>, IReverseIterator<T>
{
    int Count { get; }

    bool MoveBefore(Index valueIndex);

    bool MoveAfter(Index valueIndex);
}