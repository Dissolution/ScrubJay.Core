namespace ScrubJay.Iteration;

[PublicAPI]
public class ArrayIterator<T> : IIndexableIterator<T>
{
    private readonly T[] _array;
    private readonly int _length;
    private int _index;

    public int Count => _length;

    public ArrayIterator(T[] array)
    {
        _array = Validate.IsNotNull(array).OkOrThrow();
        _length = array.Length;
        _index = 0;
    }

    public ArrayIterator(T[] array, int index)
    {
        _array = Validate.IsNotNull(array).OkOrThrow();
        _length = array.Length;
        _index = Validate.Index(index, _length).OkOrThrow();
    }

    public ArrayIterator(T[] array, Index index)
    {
        _array = Validate.IsNotNull(array).OkOrThrow();
        _length = array.Length;
        _index = Validate.Index(index, _length).OkOrThrow();
    }

    public bool MoveBefore(Index valueIndex)
    {
        int offset = valueIndex.GetOffset(_length);
        if (offset < 0 || offset > _length)
            return false;
        _index = offset;
        return true;
    }

    public bool MoveAfter(Index valueIndex)
    {
        int offset = valueIndex.GetOffset(_length) + 1;
        if (offset < 0 || offset > _length)
            return false;
        _index = offset;
        return true;
    }

    public Option<T> Next()
    {
        // _index sits 'to the left' of an item
        int index = _index;
        T[] array = _array;

        if ((uint)index >= (uint)_length)
            return None();

        T item = array[index];

        // increment index to the next gap
        _index = index + 1;

        return Some(item);
    }

    public Option<T> Prev()
    {
        // _index sits 'to the left' of an item
        int index = _index;
        T[] array = _array;

        int prevIndex = index - 1;

        if ((uint)prevIndex >= (uint)_length)
            return None();

        T item = array[index];

        // decrement index to the prev gap
        _index = prevIndex;

        return Some(item);
    }
}