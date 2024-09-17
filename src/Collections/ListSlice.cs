namespace ScrubJay.Collections;

public abstract class ListSlice
{
    public static ListSlice<T> Of<T>(IList<T> list, Range range)
    {
        (int offset, int len) = Validate.Range(range, list.Count).OkOrThrow();
        return new ListSlice<T>(list, offset, len);
    }

    public static ListSlice<T> Of<T>(IList<T> list, Index index, int length)
    {
        (int offset, int len) = Validate.Range(index, length, list.Count).OkOrThrow();
        return new ListSlice<T>(list, offset, len);
    }
}


public sealed class ListSlice<T> : ListSlice, IList<T>, IReadOnlyList<T>
{
    private readonly IList<T> _list;
    private readonly int _offset;
    private readonly int _length;

    public int Count => _length;

    public T this[int index]
    {
        get => _list[index + _offset];
        set => _list[index + _offset] = value;
    }

    T IReadOnlyList<T>.this[int index] => this[index];

    bool ICollection<T>.IsReadOnly => false;


    internal ListSlice(IList<T> list, int offset, int length)
    {
        Debug.Assert(list is not null);
        _list = list!;
        Debug.Assert(offset >= 0);
        _offset = offset;
        Debug.Assert(length >= 0);
        _length = length;
    }


    void ICollection<T>.Add(T item) => throw new NotSupportedException();
    void IList<T>.Insert(int index, T item) => throw new NotSupportedException();

    public bool Contains(T item) => IndexOf(item) >= 0;

    public int IndexOf(T item)
    {
        for (var i = 0; i < _length; i++)
        {
            if (EqualityComparer<T>.Default.Equals(this[i], item))
                return i;
        }

        return -1;
    }

    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
        Validate.CopyTo(_length, array, arrayIndex).OkOrThrow();
        for (var i = 0; i < _length; i++)
        {
            array[arrayIndex + i] = this[i];
        }
    }


    bool ICollection<T>.Remove(T item) => throw new NotSupportedException();


    void IList<T>.RemoveAt(int index) => throw new NotSupportedException();

    public void Clear()
    {
        for (var i = 0; i < _length; i++)
        {
            this[i] = default!;
        }
    }


    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < _length; i++)
        {
            yield return this[i];
        }
    }
}