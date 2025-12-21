namespace ScrubJay.Iteration;

[PublicAPI]
public ref struct ReadOnlySpanIterator<T>: IIterator<T>
{
    private readonly ReadOnlySpan<T> _span;
    private int _index;

    public ReadOnlySpanIterator(ReadOnlySpan<T> span)
    {
        _span = span;
        _index = 0;
    }

    public bool TryMoveNext([MaybeNullWhen(false)] out T value)
    {
        if (_index < _span.Length)
        {
            value = _span[_index];
            _index++;
            return true;
        }

        value = default;
        return false;
    }
}