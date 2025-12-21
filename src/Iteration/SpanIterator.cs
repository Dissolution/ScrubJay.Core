namespace ScrubJay.Iteration;

[PublicAPI]
public ref struct SpanIterator<T>: IIterator<T>
{
    private readonly Span<T> _span;
    private int _index;

    public SpanIterator(Span<T> span)
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