namespace ScrubJay.Iteration;

[PublicAPI]
public struct ArrayIterator : IIterator<object?>
{
    private readonly Array _array;
    private int _index;

    public ArrayIterator(Array? array)
    {
        _array = array ?? Array.Empty<object?>();
    }

    public bool TryMoveNext(out object? value)
    {
        if (_index < _array.Length)
        {
            value = _array.GetValue(_index);
            _index++;
            return true;
        }

        value = null;
        return false;
    }
}

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