using System.Buffers;

namespace ScrubJay.Buffers;



/// <summary>
/// A Buffer is<br/>
/// - A stack-based collection (like <see cref="Span{T}"/>)<br/>
/// - That can grow as required (like <see cref="IList{T}"/>)<br/>
/// - That uses <see cref="ArrayPool{T}"/> to avoid excess allocation<br/>
/// - And thus must be disposed in order to return that array
/// </summary>
/// <typeparam name="T">The <see cref="Type"/> of items stored in this <see cref="Buffer{T}"/></typeparam>
/// <remarks>
/// Heavily inspired by <see cref="T:System.Collections.Generic.ValueListBuilder{T}"/><br/>
/// <a href="https://github.com/dotnet/runtime/blob/release/8.0/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/ValueListBuilder.cs"/>
/// </remarks>
[PublicAPI]
[MustDisposeResource]
public ref struct Buffer<T>
    /* Roughly implements :
     IList<T>, IReadOnlyList<T>,
     ICollection<T>, IReadOnlyCollection<T>,
     IEnumerable<T>,
     IDisposable
     */
{
    // implicitly let us use Buffer anyplace that wants a Span
    public static implicit operator Span<T>(Buffer<T> buffer) => buffer.Written;


    internal Span<T> _span;
    internal T[]? _array;
    private int _position;

    /// <summary>
    /// Get the <see cref="Span{T}"/> of items in this <see cref="Buffer{T}"/>
    /// </summary>
    public Span<T> Written
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span.Slice(0, _position);
    }

    internal Span<T> Available
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span.Slice(_position);
    }

    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _span[index];
    }

    public ref T this[Index index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _span[index.GetOffset(_position)];
    }

    public Span<T> this[Range range]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            (int offset, int length) = range.GetOffsetAndLength(_position);
            return _span.Slice(offset, length);
        }
    }

    /// <summary>
    /// Gets the number of items in this <see cref="Buffer{T}"/>
    /// </summary>
    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _position;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal set
        {
            Debug.Assert(value >= 0 && value < Capacity);
            _position = value;
        }
    }

    /// <summary>
    /// Gets the current capacity for this <see cref="Buffer{T}"/>, which will be increased as needed
    /// </summary>
    public int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span.Length;
    }

    internal Buffer(T[] initialArray, int initialPosition)
    {
        _span = _array = initialArray;
        Debug.Assert(initialPosition >= 0 && initialPosition <= Capacity);
        _position = initialPosition;
    }
    
    internal Buffer(Span<T> initialSpan, int initialPosition)
    {
        _span = initialSpan;
        _array = null;
        Debug.Assert(initialPosition >= 0 && initialPosition <= Capacity);
        _position = initialPosition;
    }

    /// <summary>
    /// Create a new, empty Buffer that has not allocated anything
    /// </summary>
    public Buffer()
    {
        _span = _array = null;
        _position = 0;
    }

    /// <summary>
    /// Create a new, empty Buffer with at least a starting <see cref="Capacity"/> of <paramref name="minCapacity"/>
    /// </summary>
    /// <param name="minCapacity">
    /// The minimum starting capacity the Buffer may have (but may end up larger)
    /// </param>
    /// <remarks>
    /// If <paramref name="minCapacity"/> is greater than 0, an array will be rented from <see cref="ArrayPool{T}"/>
    /// </remarks>
    public Buffer(int minCapacity)
    {
        if (minCapacity <= 0)
        {
            _span = _array = null;
        }
        else
        {
            _span = _array = ArrayPool.Rent<T>(minCapacity);
        }

        _position = 0;
    }

    /// <summary>
    /// Increases the size of the rented array by at least <paramref name="adding"/> items
    /// </summary>
    /// <param name="adding"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GrowBy(int adding)
    {
        GrowTo((Capacity + adding) * 2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GrowTo(int newCapacity)
    {
        Debug.Assert(newCapacity > Capacity);
        T[] newArray = ArrayPool.Rent<T>(newCapacity);
        Span.Copy(Written, newArray);

        T[]? toReturn = _array;
        _span = _array = newArray;
        ArrayPool.Return(toReturn);
    }

    /// <summary>
    /// Grows the <see cref="Capacity"/> of this <see cref="Buffer{T}"/> to at least twice its current value
    /// </summary>
    /// <remarks>
    /// This method causes a rental from <see cref="ArrayPool{T}"/>
    /// </remarks>
    public void Grow()
    {
        GrowTo(Capacity * 2);
    }
    
    /// <summary>
    /// Grows the <see cref="Capacity"/> of this <see cref="Buffer{T}"/> to at least <paramref name="minCapacity"/>
    /// </summary>
    public void GrowCapacity(int minCapacity)
    {
        if (minCapacity > Capacity)
        {
            GrowTo(minCapacity);
        }
    }

    /// <summary>
    /// Grows this Buffer and then add an item
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void AddGrow(T item)
    {
        int pos = _position;
        Debug.Assert(pos == Capacity);
        GrowBy(1);
        _span[pos] = item;
        _position = pos + 1;
    }

    /// <summary>
    /// Add a new <paramref name="item"/> to this <see cref="Buffer{T}"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        int pos = _position;
        Span<T> span = _span;

        if (pos < span.Length)
        {
            span[pos] = item;
            _position = pos + 1;
        }
        else
        {
            AddGrow(item);
        }
    }

    /// <summary>
    /// Grow this buffer and then add <paramref name="count"/> items from <paramref name="source"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void AddManyGrow(scoped ReadOnlySpan<T> source, int count)
    {
        Debug.Assert(count == source.Length);
        Debug.Assert(count > 0);
        GrowBy(count);
        Span.Copy(source, Available);
        _position += count;
    }

    /// <summary>
    /// Adds the given <paramref name="items"/> to this <see cref="Buffer{T}"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddMany(scoped ReadOnlySpan<T> items)
    {
        int count = items.Length;

        if (count == 0)
        {
            // do nothing
        }
        else if (count == 1)
        {
            Add(items[0]);
        }
        else
        {
            int newPos = _position + count;
            if (newPos <= Capacity)
            {
                Span.Copy(items, Available);
                _position = newPos;
            }
            else
            {
                AddManyGrow(items, count);
            }
        }
    }

    /// <summary>
    /// Adds the given <paramref name="items"/> to this <see cref="Buffer{T}"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddMany(params T[]? items) => AddMany(items.AsSpan());

    /// <summary>
    /// Adds the given <paramref name="items"/> to this <see cref="Buffer{T}"/>
    /// </summary>
    public void AddMany(IEnumerable<T>? items)
    {
        if (items is null)
            return;
        
        if (items.TryGetNonEnumeratedCount(out int count))
        {
            int newPos = _position + count;
            if (newPos > Capacity)
            {
                GrowBy(count);
            }

            var avail = Available;
            int a = 0;
            foreach (var item in items)
            {
                avail[a++] = item;
            }

            Debug.Assert(a == count);
        }
        else
        {
            // slow path
            foreach (T item in items)
            {
                Add(item);
            }
        }
    }

    /// <summary>
    /// Inserts an <paramref name="item"/> into this <see cref="Buffer{T}"/> at <paramref name="index"/>
    /// </summary>
    /// <param name="index">The <see cref="Index"/> to insert the <paramref name="item"/></param>
    /// <param name="item">The item to insert</param>
    public void Insert(Index index, T item)
    {
        int pos = _position;
        int offset = Validate.InsertIndex(index, pos).OkOrThrow();
        if (offset == pos)
        {
            Add(item);
        }
        else
        {
            int newPos = pos + 1;
            if (newPos >= Capacity)
            {
                GrowBy(1);
            }

            Span.SelfCopy(_span, offset..pos, (offset + 1)..);
            _span[offset] = item;
            _position = newPos;
        }
    }

    /// <summary>
    /// Inserts multiple <paramref name="items"/> into this <see cref="Buffer{T}"/> at <paramref name="index"/>
    /// </summary>
    /// <param name="index">The <see cref="Index"/> to insert the <paramref name="items"/></param>
    /// <param name="items">The <see cref="ReadOnlySpan{T}"/> of items to insert</param>
    public void InsertMany(Index index, scoped ReadOnlySpan<T> items)
    {
        int itemCount = items.Length;
        
        if (itemCount == 0)
            return;

        if (itemCount == 1)
        {
            Insert(index, items[0]);
        }
        else
        {
            int offset = Validate.InsertIndex(index, _position).OkOrThrow();
            if (offset == _position)
            {
                AddMany(items);
            }
            else
            {
                int newPos = _position + itemCount;
                if (newPos >= Capacity)
                {
                    GrowBy(itemCount);
                }

                Span.SelfCopy(_span, offset.._position, (offset + itemCount)..);
                Span.Copy(items, _span.Slice(offset, itemCount));
                _position = newPos;
            }
        }
    }

    public void InsertMany(Index index, params T[]? items) => InsertMany(index, items.AsSpan());

    private void InsertManyEnumerable(Index index, IEnumerable<T> items)
    {
        using var buffer = new Buffer<T>();
        foreach (var item in items)
        {
            buffer.Add(item);
        }

        InsertMany(index, buffer.AsSpan());
    }

    public void InsertMany(Index index, IEnumerable<T>? items)
    {
        if (items is null)
            return;

        int pos = _position;

        int offset = Validate.InsertIndex(index, pos).OkOrThrow();
        if (offset == _position)
        {
            AddMany(items);
            return;
        }

        if (items.TryGetNonEnumeratedCount(out int count))
        {
            if (count == 0)
                return;

            int newPos = pos + count;
            if (newPos >= Capacity)
            {
                GrowBy(count);
            }

            var span = _span;

            Span.SelfCopy(span, offset..pos, (offset + count)..);
            int i = offset;
            foreach (T item in items)
            {
                span[i++] = item;
            }

            Debug.Assert(i == (offset + count));
            _position = newPos;
        }
        else
        {
            // Enumerate to a temporary buffer, then insert
            InsertManyEnumerable(index, items);
        }
    }

    /// <summary>
    /// Does this <see cref="Buffer{T}"/> contain any instances of the <paramref name="item"/>?
    /// </summary>
    /// <param name="item">
    /// The item to scan for using <see cref="EqualityComparer{T}"/>.<see cref="EqualityComparer{T}.Default"/>
    /// </param>
    /// <returns>
    /// <c>true</c> if this buffer contains an instance of <paramref name="item"/><br/>
    /// <c>false</c> if it does not
    /// </returns>
    public bool Contains(T item)
    {
        for (var i = 0; i < _position; i++)
        {
            if (EqualityComparer<T>.Default.Equals(item, _span[i]))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Try to find an <paramref name="item"/> in this <see cref="Buffer{T}"/>
    /// </summary>
    /// <param name="item">The item to search for</param>
    /// <param name="firstToLast">
    /// <c>true</c>: Search from low to high indices<br/>
    /// <c>false</c>: Search from high to low indices<br/>
    /// </param>
    /// <param name="offset">
    /// The <see cref="Index"/> offset in this Buffer to start the search from
    /// </param>
    /// <param name="itemComparer">
    /// An optional <see cref="IEqualityComparer{T}"/> to use for <paramref name="item"/> comparison instead of
    /// <see cref="EqualityComparer{T}"/>.<see cref="EqualityComparer{T}.Default"/>
    /// </param>
    /// <returns>
    /// An <see cref="Option{T}"/> that might contain the index of the first matching instance
    /// </returns>
    public Option<int> TryFindIndex(T item, bool firstToLast = true, Index offset = default, IEqualityComparer<T>? itemComparer = null)
    {
        var pos = _position;
        var span = _span;

        var index = Validate.Index(offset, pos).OkOrThrow();
        itemComparer ??= EqualityComparer<T>.Default;
        if (firstToLast)
        {
            for (; index < pos; index++)
            {
                if (itemComparer.Equals(span[index], item))
                {
                    return Some(index);
                }
            }
        }
        else
        {
            for (; index >= 0; index--)
            {
                if (itemComparer.Equals(span[index], item))
                {
                    return Some(index);
                }
            }
        }

        return default;
    }

    public Option<T> TryRemoveAt(Index index)
    {
        if (!Validate.Index(index, _position).IsOk(out var offset))
            return default;
        T item = _span[offset];
        Span.SelfCopy(_span, (offset + 1).., offset..);
        return Some(item);
    }

    public bool RemoveMany(Range range)
    {
        if (!Validate.Range(range, _position).IsOk(out var ol))
            return false;
        (int offset, int length) = ol;
        Span.SelfCopy(this.Written, (offset + length).., offset..);
        return true;
    }
    
    public Option<T[]> TryRemoveMany(Range range)
    {
        if (!Validate.Range(range, _position).IsOk(out var ol))
            return default;
        (int offset, int length) = ol;
        T[] items = _span.Slice(offset, length).ToArray();
        Span.SelfCopy(_span, (offset + length).., offset..);
        return Some(items);
    }

    public Option<T> TryRemoveLast()
    {
        int newPos = _position - 1;
        if (newPos < 0)
            return default;
        _position = newPos;
        return Some(_span[newPos]);
    }

    public void Clear()
    {
        _position = 0;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private Span<T> AllocateGrow(int length)
    {
        int pos = _position;
        GrowBy(length);
        _position = pos + length;
        return _span.Slice(pos, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> Allocate(int length)
    {
        if (length <= 0)
            return Span<T>.Empty;

        int pos = _position;
        Span<T> span = _span;
        if (pos + length <= span.Length)
        {
            _position = pos + length;
            return span.Slice(pos, length);
        }
        else
        {
            return AllocateGrow(length);
        }
    }

    public void Allocate(UseAvailable<T> useAvailable)
    {
        var available = _span.Slice(_position);
        int used = useAvailable(available);
        if (used < 0 || used > available.Length)
            throw new InvalidOperationException("Cannot use an invalid number of available items");
        _position += used;
    }

    /// <summary>
    /// Get the <see cref="Span{T}"/> of written items in this <see cref="Buffer{T}"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan()
    {
        return _span.Slice(0, _position);
    }

    /// <summary>
    /// Copy the items in this <see cref="Buffer{T}"/> to a new <c>T[]</c>
    /// </summary>
    public T[] ToArray()
    {
        int pos = _position;
        T[] array = new T[pos];
        Span.Copy(_span.Slice(0, pos), array);
        return array;
    }
    
    public List<T> ToList()
    {
        List<T> list = new List<T>(Capacity);
        list.AddRange(Written);
        return list;
    }

    /// <summary>
    /// Clears this <see cref="Buffer{T}"/> and returns any rented array back to <see cref="ArrayPool{T}"/>
    /// </summary>
    [HandlesResourceDisposal]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        T[]? toReturn = _array;
        // defensive clear
        _position = 0;
        _span = _array = null;
        ArrayPool.Return(toReturn, true);
    }

    public override bool Equals(object? _) => false;

    public override int GetHashCode() => Hasher.Combine<T>(Written);

    public override string ToString() => Written.ToString();

    public BufferEnumerator GetEnumerator() => new(this);

    public BufferEnumerable ToEnumerable()
    {
        // If we have not yet allocated an array, we have to
        if (_array is null)
        {
            Grow();
        }
        Debug.Assert(_array is not null);
        return new BufferEnumerable(_array!, _position);
    }

    /// <summary>
    /// An <see cref="IEnumerator{T}"/> over the items in a <see cref="Buffer{T}"/>
    /// </summary>
    [PublicAPI]
    public ref struct BufferEnumerator
    {
        private readonly ReadOnlySpan<T> _items;
        private int _index;

        public readonly ref readonly T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _items[_index];
        }

        public readonly int Index
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal BufferEnumerator(Buffer<T> buffer)
        {
            _items = buffer.AsSpan();
            _index = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            int newIndex = _index + 1;
            if (newIndex >= _items.Length)
                return false;
            _index = newIndex;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Option<T> TryMoveNext()
        {
            int newIndex = _index + 1;
            if (newIndex >= _items.Length)
                return default;
            _index = newIndex;
            return Some(_items[newIndex]);
        }
    }


    [PublicAPI]
    [MustDisposeResource(false)]
    public sealed class BufferEnumerable : IEnumerable<T>, IDisposable
    {
        private T[]? _items;
        private int _count;

        internal BufferEnumerable(T[] items, int count)
        {
            _items = items;
            _count = count;
        }

        public void Dispose()
        {
            _items = null;
            _count = 0; // stops enumeration
        }
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        public BufferEnumerableEnumerator GetEnumerator() => new(this);

        [PublicAPI]
        [MustDisposeResource(false)]
        public sealed class BufferEnumerableEnumerator : IEnumerator<T>, IEnumerator, IDisposable
        {
            private T[]? _items;
            private int _count;
            private int _index;

            object? IEnumerator.Current => Current;

            public T Current
            {
                get
                {
                    if (_items is null)
                        throw new ObjectDisposedException(nameof(BufferEnumerableEnumerator));
                    if (_index < 0)
                        throw new InvalidOperationException("Enumeration has not yet started");
                    if (_index >= _count)
                        throw new InvalidOperationException("Enumeration has finished");
                    return _items[_index];
                }
            }
            
            public int Index
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _index;
            }

            internal BufferEnumerableEnumerator(BufferEnumerable enumerable)
            {
                _items = enumerable._items;
                _count = enumerable._count;
                _index = -1;
            }
            
            public bool MoveNext()
            {
                int newIndex = _index + 1;
                if (newIndex >= _count)
                    return false;
                _index = newIndex;
                return true;
            }
            
            public Option<T> TryMoveNext()
            {
                int newIndex = _index + 1;
                if (newIndex >= _count)
                    return default;
                _index = newIndex;
                return Some(_items![newIndex]);
            }

            public void Reset()
            {
                _index = -1;
            }
            
            public void Dispose()
            {
                // Clear my references
                _count = 0; // stops enumeration
                _index = -1;
                _items = null;
            }
        }
    }
}