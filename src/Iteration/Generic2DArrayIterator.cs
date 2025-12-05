namespace ScrubJay.Iteration;

[PublicAPI]
public struct Generic2DArrayIterator<T> : IIterator<T>
{
    private readonly T[] _array;
    private int _index;

    public Generic2DArrayIterator(T[]? array)
    {
        _array = array ?? [];
    }

    public bool TryMoveNext([MaybeNullWhen(false)] out T value)
    {
        if (_index < _array.Length)
        {
            value = _array[_index];
            _index++;
            return true;
        }

        value = default;
        return false;
    }
}