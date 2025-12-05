namespace ScrubJay.Iteration;

[PublicAPI]
public struct TupleIterator<T> : IIterator<object?>
    where T : ITuple
{
    private readonly T _tuple;
    private int _index;

    public TupleIterator(T tuple)
    {
        _tuple = tuple;
    }

    public bool TryMoveNext(out object? value)
    {
        if (_index < _tuple.Length)
        {
            value = _tuple[_index];
            _index++;
            return true;
        }

        value = null;
        return false;
    }
}