#pragma warning disable CA1711, CA1815

namespace ScrubJay.Collections.Pooling;

[PublicAPI]
[MustDisposeResource(true)]
[DebuggerDisplay("Count = {Count}")]
public class PooledStack<T> : PooledArray<T>,
    IList<T>, IReadOnlyList<T>,
    ICollection<T>, IReadOnlyCollection<T>,
    IEnumerable<T>,
    IDisposable
{
    private int _size;

    bool ICollection<T>.IsReadOnly => false;

    T IList<T>.this[int index]
    {
        get => this[index];
        set => this[index] = value;
    }

    T IReadOnlyList<T>.this[int index] => this[index];

    #pragma warning disable CA1043 // Use Integral Or String Argument For indexers
    public T this[StackIndex index]
    {
        get => TryGetItemAt(index).OkOrThrow();
        set => TrySetItemAt(index, value).ThrowIfError();
    }
    #pragma warning restore CA1043

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _size;
    }

    public PooledStack() : base() { }

    public PooledStack(int minCapacity) : base(minCapacity) { }

    protected override void CopyToNewArray(T[] newArray) => Sequence.CopyTo(_array.AsSpan(0, _size), newArray);

    void ICollection<T>.Add(T item) => Push(item);

    /// <summary>
    /// Pushes an item to the top of the stack
    /// </summary>
    public void Push(T item)
    {
        _version++;
        int pos = _size;
        int newSize = pos + 1;
        if (newSize > Capacity)
        {
            GrowTo(newSize);
        }
        _array[pos] = item;
        _size = newSize;
    }

    public void PushMany(ReadOnlySpan<T> items)
    {
        _version++;
        int pos = _size;
        int newSize = pos + items.Length;
        if (newSize > _array.Length)
        {
            GrowTo(newSize);
        }
        Sequence.CopyTo(items, _array.AsSpan(pos));
        _size = newSize;
    }

    public void PushMany(params T[] items) => PushMany(new ReadOnlySpan<T>(items));

    public void PushMany(IEnumerable<T> items)
    {
        int pos = _size;
        if (items is ICollection<T> collection)
        {
            _version++;
            int newSize = pos + collection.Count;
            if (newSize > _array.Length)
            {
                GrowTo(newSize);
            }
            collection.CopyTo(_array, pos);
            _size = newSize;
        }
        else if (items is IReadOnlyCollection<T> readOnlyCollection)
        {
            _version++;
            int newSize = pos + readOnlyCollection.Count;
            if (newSize > _array.Length)
            {
                GrowTo(newSize);
            }
            var array = _array; // proper size now
            using var e = readOnlyCollection.GetEnumerator();
            while (e.MoveNext())
            {
                array[pos++] = e.Current;
            }
            Debug.Assert(pos == newSize);
            _size = newSize;
        }
        else
        {
            foreach (var item in items)
            {
                Push(item);
            }
        }
    }


    void IList<T>.Insert(int index, T item) => TryPushAt(index, item).ThrowIfError();

    public Result<Unit> TryPushAt(StackIndex index, T item)
    {
        int end = _size;
        int offset = index.GetOffset(end);

        if ((offset < 0) || (offset >= end))
            return new ArgumentOutOfRangeException(nameof(index));

        // If we indicated the front, push
        if (offset == (end - 1))
        {
            Push(item);
            return Ok(Unit());
        }

        int newSize = end + 1;
        if (newSize >= Capacity)
        {
            GrowTo(newSize);
        }

        _version++;
        Sequence.SelfCopy(_array, offset..end, (offset + 1)..);
        _array[offset] = item;
        _size = newSize;
        return Ok(Unit());
    }

    public Result<Unit> TryPushManyAt(StackIndex index, ReadOnlySpan<T> items)
    {
        int end = _size;
        int offset = index.GetOffset(end);

        if ((offset < 0) || (offset >= end))
            return new ArgumentOutOfRangeException(nameof(index));

        int itemCount = items.Length;
        if (itemCount == 0)
            return Ok(Unit());

        // If we indicated the front, push
        if (offset == (end - 1))
        {
            PushMany(items);
            return Ok(Unit());
        }

        int newSize = end + itemCount;
        if (newSize >= Capacity)
        {
            GrowTo(newSize);
        }

        _version++;
        Sequence.SelfCopy(_array, offset..end, (offset + itemCount)..);
        Sequence.CopyTo(items, _array.AsSpan(offset, itemCount));
        _size = newSize;
        return Ok(Unit());
    }

    /// <summary>
    /// Returns the top object on the stack without removing it.  If the stack is empty, Peek throws an InvalidOperationException.
    /// </summary>
    public T Peek() => TryPeek().OkOrThrow();

    public T PeekOr(T defaultValue) => TryPeek().OkOr(defaultValue);

    public T PeekOrPush(T pushValue)
    {
        int endIndex = _size - 1;
        if (endIndex < 0)
        {
            Push(pushValue);
            return pushValue;
        }
        return _array[endIndex];
    }

    public Result<T> TryPeek()
    {
        int topItemIndex = _size - 1;
        if (topItemIndex < 0)
            return new InvalidOperationException("Stack is empty");
        return Ok(_array[topItemIndex]);
    }

    public Result<T[]> TryPeekMany(int peekCount)
    {
        if (peekCount < 0)
            return new ArgumentOutOfRangeException(nameof(peekCount), peekCount, "Peek count must be zero or greater");

        int size = _size;
        int start = size - peekCount;
        if (start < 0)
            return new InvalidOperationException($"Cannot Peek({peekCount}): There are only {size} items");

        var peeked = _array.Slice(new Range(start, size));
        Array.Reverse(peeked);
        return Ok(peeked);
    }

    public Result<Unit> TryPeekManyTo(Span<T> buffer)
    {
        int count = buffer.Length;
        int size = _size;
        int start = size - count;
        if ((count < 0) || (count > size))
            return new InvalidOperationException($"Cannot Peek to a buffer of size {count}: there are only {size} items");

        var slice = _array.AsSpan(new Range(start, size));
        Debug.Assert(slice.Length == buffer.Length);
        Sequence.CopyTo(slice, buffer);
        buffer.Reverse();
        return Ok(Unit());
    }

    /// <summary>
    /// Pops an item from the top of the stack.  If the stack is empty, Pop throws an InvalidOperationException.
    /// </summary>
    public T Pop() => TryPop().OkOrThrow();

    public T PopOr(T defaultValue) => TryPop().OkOr(defaultValue);

    public T[] PopAll()
    {
        _version++;
        T[] popped = _array.Slice(0, _size);
        Array.Reverse(popped);
        _size = 0;
        return popped;
    }

    public Result<T> TryPop()
    {
        int topItemIndex = _size - 1;
        if (topItemIndex < 0)
            return new InvalidOperationException("Stack is empty");

        _version++;
        _size = topItemIndex;
        return Ok(_array[topItemIndex]);
    }

    public Result<T[]> TryPopMany(int peekCount)
    {
        if (peekCount < 0)
            return new ArgumentOutOfRangeException(nameof(peekCount), peekCount, "Pop count must be zero or greater");

        int size = _size;
        int start = size - peekCount;
        if (start < 0)
            return new InvalidOperationException($"Cannot Pop({peekCount}): There are only {size} items");

        _version++;
        _size = start;
        var popped = _array.Slice(new Range(start, size));
        Array.Reverse(popped);
        return Ok(popped);
    }

    public Result<Unit> TryPopManyTo(Span<T> buffer)
    {
        int count = buffer.Length;
        int size = _size;
        int start = size - count;
        if ((count < 0) || (count > size))
            return new InvalidOperationException($"Cannot Pop to a buffer of size {count}: there are only {size} items");

        var slice = _array.AsSpan(new Range(start, size));
        Debug.Assert(slice.Length == buffer.Length);

        _version++;
        _size = start;
        Sequence.CopyTo(slice, buffer);
        buffer.Reverse();
        return Ok(Unit());
    }

    bool ICollection<T>.Remove(T item)
    {
        int endIndex = _size - 1;
        if (endIndex >= 0)
        {
            int itemIndex = Array.LastIndexOf<T>(_array, item, endIndex);
            if (itemIndex >= 0)
            {
                _version++;
                _size = endIndex;
                Sequence.SelfCopy(_array, (itemIndex + 1).., itemIndex..);
                return true;
            }
        }
        return false;
    }

    void IList<T>.RemoveAt(int index) => TryPopAt(index).OkOrThrow();

    public Result<T> TryPopAt(StackIndex index)
    {
        int end = _size;
        int offset = index.GetOffset(end);

        if ((offset < 0) || (offset >= end))
            return new ArgumentOutOfRangeException(nameof(index));

        int newSize = end - 1;

        _version++;
        _size = newSize;
        T item = _array[offset];
        if (offset != newSize)
        {
            Sequence.SelfCopy(_array, (offset + 1)..end, offset..);
        }
        return Ok(item);
    }

    public void Clear()
    {
        _version++;
        _size = 0;
        // We only clear the array items when we are disposed
    }

    int IList<T>.IndexOf(T item)
    {
        int endIndex = _size - 1;
        if (endIndex < 0)
            return -1;
        int index = Array.LastIndexOf<T>(_array, item, endIndex);
        if (index < 0)
            return -1;
        return (endIndex - index);
    }

    public bool Contains(T item)
    {
        int endIndex = _size - 1;
        return (endIndex >= 0) && (Array.LastIndexOf<T>(_array, item, endIndex) >= 0);
    }

    public bool Contains(T item, IEqualityComparer<T>? itemComparer)
    {
        if (itemComparer is null)
            return Contains(item);
        for (int i = _size - 1; i >= 0; i--)
        {
            if (itemComparer.Equals(_array[i], item))
                return true;
        }
        return false;
    }

#pragma warning disable IDE0060

    public Result<T> TryGetItemAt(StackIndex index)
    {
        int end = _size;
        int offset = index.GetOffset(end);
        if ((offset < 0) || (offset >= end))
            return new ArgumentOutOfRangeException(nameof(index));
        return Ok(_array[offset]);
    }

    public Result<T> TrySetItemAt(StackIndex index, T item)
    {
        int end = _size;
        int offset = index.GetOffset(end);
        if ((offset < 0) || (offset >= end))
            return new ArgumentOutOfRangeException(nameof(index));
        _array[offset] = item;
        return Ok(item);
    }

    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => TryCopyTo(array.AsSpan(arrayIndex)).ThrowIfError();

    public Result<int> TryCopyTo(Span<T> span, bool popOrder = true)
    {
        int count = _size;
        var array = _array;

        if (Validate.CanCopyTo(span, count).IsError(out var ex))
            return ex;

        int s = 0; // index into span
        int a; // index into our array
        int aStep; // which order we're reading array

        if (popOrder)
        {
            // from end of array to beginning
            a = count - 1;
            aStep = -1;
        }
        else
        {
            // from beginning of array to end
            a = 0;
            aStep = +1;
        }

        while ((s < count) && ((uint)a < (uint)count))
        {
            span[s] = array[a];

            s += 1;
            a += aStep;
        }

        Debug.Assert((s >= count) && ((a < 0) || (a >= count)));

        return Ok(count);
    }

    /// <summary>
    /// Copies the Stack to an array, in the same order Pop would return the items.
    /// </summary>
    public T[] ToArray(bool popOrder = true)
    {
        T[] newArray = new T[_size];
        TryCopyTo(newArray, popOrder).ThrowIfError();
        return newArray;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    [MustDisposeResource(false)]
    public IEnumerator<T> GetEnumerator(bool popOrder = true)
    {
        if (_size == 0)
            return Enumerator.Empty<T>();

        return new ArrayEnumerator<T>(
            _array,
            0, _size - 1,
            popOrder ? -1 : +1,
            () => _version);
    }

    protected override void OnDisposing()
    {
        _version++;
        _size = 0;
    }
}
