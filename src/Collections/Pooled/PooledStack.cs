#pragma warning disable CA1711

namespace ScrubJay.Collections.Pooled;

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

    public T this[Index index]
    {
        get => TryGetItemAt(index).OkOrThrow();
        set => TrySetItemAt(index, value).ThrowIfError();
    }

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _size;
    }

    public PooledStack() : base() { }

    public PooledStack(int minCapacity) : base(minCapacity) { }

    protected override void CopyToNewArray(T[] newArray) => Sequence.CopyTo(_array.AsSpan(0, _size), newArray);

    protected int AdjustOffset(int offset)
    {
        Debug.Assert(offset >= 0 && offset < _size);
        return ((_size - offset) - 1);
    }


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

    public Result<Unit, Exception> TryPushAt(Index index, T item)
    {
        int pos = _size;
        var vr = Validate.InsertIndex(index, pos);
        if (!vr.HasOkOrError(out int offset, out var ex))
            return ex;

        // offset is from the top of the stack
        if (offset == 0)
        {
            Push(item);
            return Unit();
        }
        offset = AdjustOffset(offset);

        int newPos = pos + 1;
        if (newPos >= Capacity)
        {
            GrowTo(newPos);
        }

        Sequence.SelfCopy(_array, offset..pos, (offset + 1)..);
        _array[offset] = item;
        _size = newPos;
        return Unit();
    }

    public Result<Unit, Exception> TryPushManyAt(Index index, ReadOnlySpan<T> items)
    {
        int itemCount = items.Length;

        if (itemCount == 0)
            return Validate.InsertIndex(index, _size).OkSelect(_ => Unit());

        if (itemCount == 1)
            return TryPushAt(index, items[0]);

        var vr = Validate.InsertIndex(index, _size);
        if (!vr.HasOkOrError(out int offset, out var ex))
            return ex;

        // offset is from the top
        if (offset == 0)
        {
            PushMany(items);
            return Unit();
        }
        offset = AdjustOffset(offset);

        int newPos = _size + itemCount;
        if (newPos >= Capacity)
        {
            GrowTo(newPos);
        }

        Sequence.SelfCopy(_array, offset.._size, (offset + itemCount)..);
        Sequence.CopyTo(items, _array.AsSpan(offset, itemCount));
        _size = newPos;
        return Unit();
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

    public Result<T, Exception> TryPeek()
    {
        int topItemIndex = _size - 1;
        if (topItemIndex < 0)
            return new InvalidOperationException("Stack is empty");
        return _array[topItemIndex];
    }

    public Result<T[], Exception> TryPeekMany(int peekCount)
    {
        if (peekCount < 0)
            return new ArgumentOutOfRangeException(nameof(peekCount), peekCount, "Peek count must be zero or greater");

        int size = _size;
        int start = size - peekCount;
        if (start < 0)
            return new InvalidOperationException($"Cannot Peek({peekCount}): There are only {size} items");

        var peeked = _array.Slice(new Range(start, size));
        peeked.Reverse();
        return OkEx(peeked);
    }

    public Result<Unit, Exception> TryPeekManyTo(Span<T> buffer)
    {
        int count = buffer.Length;
        int size = _size;
        int start = size - count;
        if (count < 0 || count > size)
            return new InvalidOperationException($"Cannot Peek to a buffer of size {count}: there are only {size} items");

        var slice = _array.AsSpan(new Range(start, size));
        Debug.Assert(slice.Length == buffer.Length);
        Sequence.CopyTo(slice, buffer);
        buffer.Reverse();
        return Unit();
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
        popped.Reverse();
        _size = 0;
        return popped;
    }

    public Result<T, Exception> TryPop()
    {
        int topItemIndex = _size - 1;
        if (topItemIndex < 0)
            return new InvalidOperationException("Stack is empty");

        _version++;
        _size = topItemIndex;
        return _array[topItemIndex];
    }

    public Result<T[], Exception> TryPopMany(int peekCount)
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
        popped.Reverse();
        return OkEx(popped);
    }

    public Result<Unit,Exception> TryPopManyTo(Span<T> buffer)
    {
        int count = buffer.Length;
        int size = _size;
        int start = size - count;
        if (count < 0 || count > size)
            return new InvalidOperationException($"Cannot Pop to a buffer of size {count}: there are only {size} items");

        var slice = _array.AsSpan(new Range(start, size));
        Debug.Assert(slice.Length == buffer.Length);

        _version++;
        _size = start;
        Sequence.CopyTo(slice, buffer);
        buffer.Reverse();
        return Unit();
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
                Sequence.SelfCopy(_array, (itemIndex + 1).., itemIndex..);
                _size = endIndex;
                return true;
            }
        }
        return false;
    }

    void IList<T>.RemoveAt(int index)
    {
        int offset = Validate.Index(index, _size).OkOrThrow();
        offset = AdjustOffset(offset);
        _version++;
        Sequence.SelfCopy(_array, (offset + 1).., offset..);
        _size--;
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
        return AdjustOffset(_size - index);
    }

    public bool Contains(T item)
    {
        int endIndex = _size - 1;
        return endIndex >= 0 && Array.LastIndexOf<T>(_array, item, endIndex) >= 0;
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

    public Result<T, Exception> TryGetItemAt(Index index)
    {
        int size = _size;
        return Validate.Index(index, size).Select(offset => _array[AdjustOffset(offset)]);
    }

    public Result<T, Exception> TrySetItemAt(Index index, T item)
    {
        int size = _size;
        return Validate.Index(index, size).Select(offset => _array[AdjustOffset(offset)] = item);
    }

    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => TryCopyTo(array.AsSpan(arrayIndex)).ThrowIfError();

    public Result<int, Exception> TryCopyTo(Span<T> span, bool popOrder = true)
    {
        int count = _size;
        var array = _array;

        if (Validate.CanCopyTo(span, count).HasError(out var ex))
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

        while (s < count && (uint)a < (uint)count)
        {
            span[s] = array[a];

            s += 1;
            a += aStep;
        }

        Debug.Assert((s >= count) && (a < 0 || a >= count));

        return count;
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
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator(true);

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

    public override void Dispose()
    {
        _version++;
        _size = 0;
        base.Dispose();
    }
}
