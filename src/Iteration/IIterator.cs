namespace ScrubJay.Iteration;

[PublicAPI]
public interface IIterator<T>
{
    bool TryMoveNext([MaybeNullWhen(false)] out T value);
}