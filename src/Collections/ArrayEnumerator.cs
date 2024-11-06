namespace ScrubJay.Collections;

/// <summary>
/// A typed <see cref="IEnumerator{T}"/> over an <see cref="Array"/>
/// </summary>
/// <typeparam name="T"></typeparam>
[PublicAPI]
[MustDisposeResource(false)]
public sealed class ArrayEnumerator<T> : IEnumerator<T>, IEnumerator, IDisposable
{
    private readonly T[] _array;

    private readonly int _startIndex;
    private readonly int _endIndex;

    private int _index;

    object? IEnumerator.Current => Current;

    public T Current
    {
        get
        {
            Throw.IfBadEnumerationState(_index < _startIndex, _index > _endIndex);
            return _array[_index];
        }
    }


    public ArrayEnumerator(T[] array)
    {
        Throw.IfNull(array);
        _array = array;
        _startIndex = 0;
        _endIndex = _array.Length - 1;
        _index = -1;
    }

    public ArrayEnumerator(T[] array, Range range)
    {
        Throw.IfNull(array);
        (int offset, int length) = Validate.Range(range, array.Length).OkOrThrow();
        _array = array;
        _startIndex = offset;
        _endIndex = length + offset;
        _index = offset - 1;
    }

    public bool MoveNext()
    {
        int newIndex = _index + 1;
        if (newIndex < _startIndex || newIndex > _endIndex)
            return false;
        _index = newIndex;
        return true;
    }

    public Option<T> TryMoveNext()
    {
        int newIndex = _index + 1;
        if (newIndex < _startIndex || newIndex > _endIndex)
            return None<T>();
        _index = newIndex;
        return Some(_array[newIndex]);
    }

    public void Reset()
    {
        _index = _startIndex - 1;
    }

    void IDisposable.Dispose()
    {
        // do nothing
    }
}