namespace ScrubJay.Iteration;

public readonly struct EmptyIterator<T> : IIterator<T>, IHasDefault<EmptyIterator<T>>
{
    public static EmptyIterator<T> Default { get; } = new();

    public bool TryMoveNext([MaybeNullWhen(false)] out T value)
    {
        value = default;
        return false;
    }
}