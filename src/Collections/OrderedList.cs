namespace ScrubJay.Collections;

[PublicAPI]
public class OrderedList<T> : IList<T>
{
    private T[] _items = [];
    private int _size = 0;

    bool ICollection<T>.IsReadOnly => false;

    public int Count => _size;

    public IComparer<T> Comparer { get; }

    public bool NewestFirst { get; }

    T IList<T>.this[int index]
    {
        get => this[index];
        set => throw Ex.Invalid($"Cannot set an {GetType():@)} by index");
    }

    public T this[int index]
    {
        get => _items[index];
    }

    public OrderedList(IComparer<T>? comparer = null, bool newestFirst = false)
    {
        Comparer = comparer ?? Comparer<T>.Default;
        NewestFirst = newestFirst;
    }

#region NonPublic Methods

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Adding(int count)
    {
        if (_size + count > _items.Length)
            GrowBy(count);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowBy(int count)
    {
        int newCapacity = Math.Min((_items.Length + count) * 2, 16);
        var newItems = new T[newCapacity];
        if (_size > 0)
        {
            Array.Copy(_items, 0, newItems, 0, _size);
        }

        _items = newItems;
    }

    private int FindFirstIndex(T item, int left, int right)
    {
        int firstIndex = -1;

        while (left <= right)
        {
            int mid = left + ((right - left) >> 1);
            int comparison = Comparer.Compare(item, _items[mid]);

            if (comparison == 0)
            {
                firstIndex = mid;
                right = mid - 1; // Continue searching left
            }
            else if (comparison < 0)
            {
                right = mid - 1;
            }
            else
            {
                left = mid + 1;
            }
        }

        return firstIndex;
    }

    private int FindLastIndex(T item, int left, int right)
    {
        int lastIndex = -1;

        while (left <= right)
        {
            int mid = left + ((right - left) >> 1);
            int comparison = Comparer.Compare(item, _items[mid]);

            if (comparison == 0)
            {
                lastIndex = mid;
                left = mid + 1; // Continue searching right
            }
            else if (comparison < 0)
            {
                right = mid - 1;
            }
            else
            {
                left = mid + 1;
            }
        }

        return lastIndex;
    }

    private int FindInsertionIndex(T item)
    {
        if (_size == 0)
            return 0;

        int left = 0;
        int right = _size - 1;

        while (left <= right)
        {
            int mid = left + ((right - left) >> 1);
            int comparison = Comparer.Compare(item, _items[mid]);

            if (comparison < 0)
            {
                right = mid - 1;
            }
            else if (comparison > 0)
            {
                left = mid + 1;
            }
            else
            {
                // Keys are equal - handle NewestFirst logic
                return NewestFirst ? FindFirstIndex(item, left, mid) : FindLastIndex(item, mid, right) + 1;
            }
        }

        return left;
    }

    void IList<T>.Insert(int index, T item)
    {
        throw Ex.Invalid($"Cannot insert into an {GetType():@)}");
    }

    private void InsertAt(int index, T item)
    {
        int count = _size;

        int i = Throw.IfBadInsertIndex(index, count);

        Adding(1);

        if (index < count)
        {
            int chunk = count - index;
            _items.AsSpan(index, chunk).CopyTo(_items.AsSpan(index + 1, chunk));
        }

        _items[index] = item;
        _size = count + 1;
    }

#endregion


    public void Add(T item)
    {
        int insertIndex = FindInsertionIndex(item);
        InsertAt(insertIndex, item);
    }

    public void AddMany(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            Add(item);
        }
    }

    public bool Contains(T item)
        => FindFirstIndex(item) >= 0;

    int IList<T>.IndexOf(T item) => FindFirstIndex(item);

    public int FindFirstIndex(T item)
        => FindFirstIndex(item, 0, _size - 1);

    public int FindLastIndex(T item)
        => FindLastIndex(item, 0, _size - 1);

    public void RemoveAt(int index)
    {
        int count = _size;

        Throw.IfBadIndex(index, count);

        count--;
        if (index < count)
        {
            var chunk = count - index;
            _items.AsSpan(index + 1, chunk).CopyTo(_items.AsSpan(index, chunk));
        }

        _items[count] = default!;
        _size = count;
    }

    bool ICollection<T>.Remove(T item) => RemoveFirst(item);

    public bool RemoveFirst(T item)
    {
        int index = FindFirstIndex(item);
        if (index >= 0)
        {
            RemoveAt(index);
            return true;
        }

        return false;
    }

    public bool RemoveLast(T item)
    {
        int index = FindLastIndex(item);
        if (index >= 0)
        {
            RemoveAt(index);
            return true;
        }

        return false;
    }

    public int RemoveAll(T item)
    {
        int end = FindLastIndex(item);
        if (end == -1)
            return 0;

        int i = end - 1;
        while (i >= 0)
        {
            if (Comparer.Compare(_items[i], item) != 0)
            {
                break;
            }

            i--;
        }

        if (i < 0)
            i = 0;

        int removed = end - i;
        Array.Copy(_items, end + 1, _items, i, _size - (end-1));
        return removed;
    }

    public void Clear()
    {
        if (_size > 0)
        {
            Array.Clear(_items, 0, _size);
            _size = 0;
        }
    }

    public void CopyTo(T[] array, int arrayIndex = 0)
    {
        Validate.CanCopyTo(array, arrayIndex, _size).ThrowIfError();
        _items.AsSpan(0, _size).CopyTo(array.AsSpan(arrayIndex));
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<T> GetEnumerator()
    {
        return new ArrayEnumerator<T>(_items, 0, _size - 1, 1);
    }
}