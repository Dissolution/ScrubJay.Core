namespace ScrubJay.Collections;

public abstract class ListSlice
{
    public static ListSlice<T> Create<T>(IList<T> list)
    {
        return new ListSlice<T>(list, 0, list.Count);
    }

    public static ListSlice<T> Create<T>(IList<T> list, Range range)
    {
        (int offset, int len) = Validate.Range(range, list.Count).OkOrThrow();
        return new ListSlice<T>(list, offset, len);
    }

    public static ListSlice<T> Create<T>(IList<T> list, Index index, int length)
    {
        (int offset, int len) = Validate.IndexLength(index, length, list.Count).OkOrThrow();
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
        _list = list;
        Debug.Assert(offset >= 0);
        _offset = offset;
        Debug.Assert(length >= 0);
        _length = length;
    }


    void ICollection<T>.Add(T item) => throw Ex.MethodNotSupported();
    void IList<T>.Insert(int index, T item) => throw Ex.MethodNotSupported();

    public bool Contains(T item) => IndexOf(item) >= 0;

    public int IndexOf(T item)
    {
        for (int i = 0; i < _length; i++)
        {
            if (EqualityComparer<T>.Default.Equals(this[i], item))
                return i;
        }

        return -1;
    }

    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
        Validate.CanCopyTo(array, arrayIndex, _length).ThrowIfError();
        for (int i = 0; i < _length; i++)
        {
            array[arrayIndex + i] = this[i];
        }
    }


    bool ICollection<T>.Remove(T item) => throw Ex.MethodNotSupported();


    void IList<T>.RemoveAt(int index) => throw Ex.MethodNotSupported();

    public void Clear()
    {
        for (int i = 0; i < _length; i++)
        {
            this[i] = default!;
        }
    }


    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < _length; i++)
        {
            yield return this[i];
        }
    }
}
