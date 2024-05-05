namespace ScrubJay.Collections;

public class Buffer<T>
{
    private T[] _array;
    private int _count;

    internal int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _array.Length;
    }

    public int Count => _count;

    public Buffer() : this(ArrayPoolHelper.MIN_CAPACITY) { }

    public Buffer(int capacity)
    {
        _array = ArrayPoolHelper.Rent<T>(capacity);
        _count = 0;
    }

    public void Add(T item)
    {
        int count = _count;
        if (count >= Capacity)
        {
            ArrayPoolHelper.ExpandBy<T>(ref _array, count, 1);
        }
        _array[count] = item;
        _count = count + 1;
    }
}