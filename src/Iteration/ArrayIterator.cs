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