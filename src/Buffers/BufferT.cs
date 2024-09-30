#pragma warning disable S4136, CA1002

using System.Buffers;
using ScrubJay.Comparison;

namespace ScrubJay.Buffers;

/// <summary>
/// A Buffer is similar to a <see cref="SpanBuffer{T}"/> except:<br/>
/// it is a <c>class</c> that implements <see cref="IList{T}"/> and derivative interfaces<br/>
/// it always has a rented <see cref="Array">T[]</see><br/>
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of items stored in this <see cref="Buffer{T}"/>
/// </typeparam>
/// <remarks>
/// This is a near-copy of <see cref="SpanBuffer{T}"/> aside from necessary differences and slight tweaks for performance
/// </remarks>
[PublicAPI]
[MustDisposeResource]
public class Buffer<T> :
    IList<T>,
    IReadOnlyList<T>,
    ICollection<T>,
    IReadOnlyCollection<T>,
    IEnumerable<T>,
    IDisposable
{
    /// <summary>
    /// Implicitly use the <see cref="Written"/> portion of a <see cref="Buffer{T}"/> as a <see cref="Span{T}"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Span<T>(Buffer<T> spanBuffer) => spanBuffer.Written;

    // _array borrowed from ArrayPool
    internal T[] _array;

    // the position in _array that we're writing to
    private int _position;

    bool ICollection<T>.IsReadOnly => false;

    /// <summary>
    /// Get a <see cref="Span{T}"/> over items in this <see cref="Buffer{T}"/>
    /// </summary>
    public Span<T> Written
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _array.AsSpan(0, _position);
    }

    /// <summary>
    /// Gets a <see cref="Span{T}"/> over the unwritten, available portion of this <see cref="Buffer{T}"/>
    /// </summary>
    internal Span<T> Available
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _array.AsSpan(_position);
    }

    T IReadOnlyList<T>.this[int index] => this[index];

    T IList<T>.this[int index]
    {
        get => this[index];
        set => this[index] = value;
    }

    /// <summary>
    /// Returns a reference to the item in this <see cref="Buffer{T}"/> at the given <paramref name="index"/>
    /// </summary>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when <paramref name="index"/> is invalid
    /// </exception>
    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Written[index];
    }

    /// <summary>
    /// Returns a reference to the item in this <see cref="Buffer{T}"/> at the given <see cref="Index"/>
    /// </summary>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when <paramref name="index"/> is invalid
    /// </exception>
    public ref T this[Index index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Written[index];
    }

    /// <summary>
    /// Gets a <see cref="Span{T}"/> over the given <see cref="Range"/> of items in this <see cref="Buffer{T}"/>
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="range"/> is invalid
    /// </exception>
    public Span<T> this[Range range]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Written[range];
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
        get => _array.Length;
    }


    /// <summary>
    /// Create a new, empty <see cref="Buffer{T}"/> that has not allocated
    /// </summary>
    public Buffer()
    {
        _array = ArrayPool.Rent<T>();
        _position = 0;
    }

    /// <summary>
    /// Create a new, empty <see cref="Buffer{T}"/> with at least a starting <see cref="Capacity"/> of <paramref name="minCapacity"/>
    /// </summary>
    /// <param name="minCapacity">
    /// The minimum starting <see cref="Capacity"/> this <see cref="Buffer{T}"/> will have
    /// </param>
    /// <remarks>
    /// If <paramref name="minCapacity"/> is greater than 0, an array will be rented from <see cref="ArrayPool{T}"/>
    /// </remarks>
    public Buffer(int minCapacity)
    {
        _array = ArrayPool.Rent<T>(minCapacity);
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
        Sequence.CopyTo(Written, newArray);

        T[] toReturn = _array;
        _array = newArray;
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
        _array[pos] = item;
        _position = pos + 1;
    }

    /// <summary>
    /// Add a new <paramref name="item"/> to this <see cref="Buffer{T}"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        int pos = _position;
        var array = _array;

        if (pos < array.Length)
        {
            array[pos] = item;
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
        Sequence.CopyTo(source, Available);
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
                Sequence.CopyTo(items, Available);
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

        int itemCount;
        if (items is ICollection<T> collection)
        {
            itemCount = collection.Count;
            if (itemCount == 0)
                return;

            int pos = _position;
            int newPos = pos + itemCount;
            if (newPos > Capacity)
            {
                GrowBy(itemCount);
            }

            collection.CopyTo(_array, pos);
            _position = newPos;
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

    void IList<T>.Insert(int index, T item) => Insert(index, item);

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

            Sequence.SelfCopy(_array, offset..pos, (offset + 1)..);
            _array[offset] = item;
            _position = newPos;
        }
    }

    /// <summary>
    /// Try to insert an <paramref name="item"/> into this <see cref="SpanBuffer{T}"/> at <paramref name="index"/>
    /// </summary>
    /// <param name="index">The <see cref="Index"/> to insert the <paramref name="item"/></param>
    /// <param name="item">The item to insert</param>
    /// <returns>
    /// A <see cref="Result{O,E}"/> that contains the <c>int</c> offset the item was inserted at or an <see cref="Exception"/> that describes why insertion failed
    /// </returns>
    public Result<int, Exception> TryInsert(Index index, T item)
    {
        int pos = _position;
        var vr = Validate.InsertIndex(index, pos);
        if (!vr.IsOk(out var offset))
            return vr;
        
        if (offset == pos)
        {
            Add(item);
            return offset;
        }

        int newPos = pos + 1;
        if (newPos >= Capacity)
        {
            GrowBy(1);
        }

        Sequence.SelfCopy(_array, offset..pos, (offset + 1)..);
        _array[offset] = item;
        _position = newPos;
        return offset;
    }

    /// <summary>
    /// Try to insert multiple <paramref name="items"/> into this <see cref="SpanBuffer{T}"/> at <paramref name="index"/>
    /// </summary>
    /// <param name="index">The <see cref="Index"/> to insert the <paramref name="items"/></param>
    /// <param name="items">The <see cref="ReadOnlySpan{T}"/> of items to insert</param>
    /// <returns>
    /// A <see cref="Result{O,E}"/> that contains the <c>int</c> offset the items were inserted at or an <see cref="Exception"/> that describes why insertion failed
    /// </returns>
    public Result<int, Exception> TryInsertMany(Index index, scoped ReadOnlySpan<T> items)
    {
        int itemCount = items.Length;

        if (itemCount == 0)
            return Validate.InsertIndex(index, _position);

        if (itemCount == 1)
            return TryInsert(index, items[0]);

        var vr = Validate.InsertIndex(index, _position);
        if (!vr.IsOk(out var offset))
            return vr;
        
        if (offset == _position)
        {
            AddMany(items);
            return offset;
        }

        int newPos = _position + itemCount;
        if (newPos >= Capacity)
        {
            GrowBy(itemCount);
        }

        Sequence.SelfCopy(_array, offset.._position, (offset + itemCount)..);
        Sequence.CopyTo(items, _array.AsSpan(offset, itemCount));
        _position = newPos;
        return offset;
    }

    /// <summary>
    /// Try to insert multiple <paramref name="items"/> into this <see cref="SpanBuffer{T}"/> at <paramref name="index"/>
    /// </summary>
    /// <param name="index">The <see cref="Index"/> to insert the <paramref name="items"/></param>
    /// <param name="items">The <see cref="Array">T[]</see> of items to insert</param>
    /// <returns>
    /// A <see cref="Result{O,E}"/> that contains the <c>int</c> offset the items were inserted at or an <see cref="Exception"/> that describes why insertion failed
    /// </returns>
    public void TryInsertMany(Index index, params T[]? items) => TryInsertMany(index, new ReadOnlySpan<T>(items));

    
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void InsertManyEnumerable(int index, IEnumerable<T> items)
    {
        // Slow path, fill another buffer and then insert known
        using var buffer = new SpanBuffer<T>();
        foreach (var item in items)
        {
            buffer.Add(item);
        }
        TryInsertMany(index, buffer);
    }

    /// <summary>
    /// Try to insert multiple <paramref name="items"/> into this <see cref="SpanBuffer{T}"/> at <paramref name="index"/>
    /// </summary>
    /// <param name="index">The <see cref="Index"/> to insert the <paramref name="items"/></param>
    /// <param name="items">The <see cref="IEnumerable{T}"/> of items to insert</param>
    /// <returns>
    /// A <see cref="Result{O,E}"/> that contains the <c>int</c> offset the items were inserted at or an <see cref="Exception"/> that describes why insertion failed
    /// </returns>
    public Result<int, Exception> TryInsertMany(Index index, IEnumerable<T>? items)
    {
        if (items is null)
            return Validate.InsertIndex(index, _position);

        int pos = _position;

        var vr = Validate.InsertIndex(index, pos);
        if (!vr.IsOk(out var offset))
            return vr;
        
        if (offset == _position)
        {
            AddMany(items);
            return offset;
        }
        
        int itemCount;
        if (items is ICollection<T> collection)
        {
            itemCount = collection.Count;
            if (itemCount == 0)
                return offset;
            
            int newPos = pos + itemCount;
            if (newPos > Capacity)
            {
                GrowBy(itemCount);
            }

            Sequence.SelfCopy(_array, offset.._position, (offset + itemCount)..);
            collection.CopyTo(_array, offset);
            _position = newPos;
            return offset;
        }

        // Enumerate to a temporary Buffer, then insert
        InsertManyEnumerable(offset, items);
        return offset;
    }

    /// <summary>
    /// Sorts the items in this <see cref="Buffer{T}"/> using an optional <see cref="IComparer{T}"/>
    /// </summary>
    /// <param name="itemComparer">
    /// An optional <see cref="IComparer{T}"/> used to sort the items, defaults to <see cref="Comparer{T}"/>.<see cref="Comparer{T}.Default"/>
    /// </param>
    public void Sort(IComparer<T>? itemComparer = default)
    {
        Array.Sort(_array, 0, _position, itemComparer);
    }

    /// <summary>
    /// Sorts the items in this <see cref="Buffer{T}"/> using a <see cref="Comparison{T}"/> delegate
    /// </summary>
    /// <param name="itemComparison">
    /// The <see cref="Comparison{T}"/> delegate used to sort the items
    /// </param>
    public void Sort(Comparison<T> itemComparison)
    {
        Array.Sort(_array, 0, _position, Compare.CreateComparer<T>(itemComparison));
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
        var array = _array;
        for (var i = 0; i < _position; i++)
        {
            if (EqualityComparer<T>.Default.Equals(item, array[i]))
                return true;
        }

        return false;
    }

    int IList<T>.IndexOf(T item) => TryFindIndex(item).SomeOr(-1);

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
    public Option<int> TryFindIndex(T item, bool firstToLast = true, Index? offset = default, IEqualityComparer<T>? itemComparer = null)
    {
        var pos = _position;
        var span = new Span<T>(_array);

        // get a valid item comparer
        itemComparer ??= EqualityComparer<T>.Default;
        
        if (firstToLast)
        {
            int index;
            // Check for a starting offset
            if (offset.TryGetValue(out Index offsetIndex))
            {
                // Validate that offset
                var validIndex = Validate.Index(offsetIndex, pos);
                if (!validIndex.IsOk(out index))
                    return None();
            }
            else
            {
                // No offset, we start at the first item
                index = 0;
            }

            // we can scan until the last item
            for (; index < pos; index++)
            {
                if (itemComparer.Equals(span[index], item))
                {
                    return Some(index);
                }
            }
        }
        else // lastToFirst
        {
            int index;
            // Check for a starting offset
            if (offset.TryGetValue(out Index offsetIndex))
            {
                // Validate that offset
                var validIndex = Validate.Index(offsetIndex, pos);
                if (!validIndex.IsOk(out index))
                    return None();
            }
            else
            {
                // No offset, we start at the last item
                index = pos - 1;
            }

            // we can scan until the first item
            for (; index >= 0; index--)
            {
                if (itemComparer.Equals(span[index], item))
                {
                    return Some(index);
                }
            }
        }

        return None();
    }

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool SequenceEqual(IEqualityComparer<T> itemComparer, Span<T> left, ReadOnlySpan<T> right, int count)
    {
        Debug.Assert(left.Length >= count);
        Debug.Assert(right.Length >= count);
        for (int i = 0; i < count; i++)
        {
            if (!itemComparer.Equals(left[i], right[i]))
            {
                return false;
            }
        }
        return true;
    }
    
    /// <summary>
    /// Try to find a sequence of <paramref name="items"/> in this <see cref="SpanBuffer{T}"/>
    /// </summary>
    /// <param name="items">The <see cref="ReadOnlySpan{T}"/> to search for</param>
    /// <param name="firstToLast">
    /// <c>true</c>: Search from low to high indices<br/>
    /// <c>false</c>: Search from high to low indices<br/>
    /// </param>
    /// <param name="offset">
    /// The <see cref="Index"/> offset in this <see cref="SpanBuffer{T}"/> to start the search from
    /// </param>
    /// <param name="itemComparer">
    /// An optional <see cref="IEqualityComparer{T}"/> to use for item comparison instead of
    /// <see cref="EqualityComparer{T}"/>.<see cref="EqualityComparer{T}.Default"/>
    /// </param>
    /// <returns>
    /// An <see cref="Option{T}"/> that might contain the index of the first matching sequence
    /// </returns>
    public Option<int> TryFindIndex(
        ReadOnlySpan<T> items, 
        bool firstToLast = true, 
        Index? offset = default, 
        IEqualityComparer<T>? itemComparer = null)
    {
        int itemCount = items.Length;
        var pos = _position;
        var span = new Span<T>(_array);

        if (itemCount == 0 || itemCount > pos)
            return None();
    
        // we can only scan until an end item (past that there wouldn't be enough items to match)
        int end = pos - itemCount;
        
        // get a valid item comparer
        itemComparer ??= EqualityComparer<T>.Default;
        
        if (firstToLast)
        {
            int index;
            // Check for a starting offset
            if (offset.TryGetValue(out Index offsetIndex))
            {
                // Validate that offset
                var validIndex = Validate.Index(offsetIndex, pos);
                if (!validIndex.IsOk(out index))
                    return None();
            }
            else
            {
                // No offset, we start at the first item
                index = 0;
            }
            
            for (; index <= end; index++)
            {
                if (SequenceEqual(itemComparer, span.Slice(index), items, itemCount))
                    return Some(index);
            }
        }
        else // lastToFirst
        {
            int index;
            // Check for a starting offset
            if (offset.TryGetValue(out Index offsetIndex))
            {
                // Validate that offset
                var validIndex = Validate.Index(offsetIndex, pos);
                if (!validIndex.IsOk(out index))
                    return None();
                
                // No point in scanning until the last valid index
                if (index > end)
                    index = end;
            }
            else
            {
                // No offset, we start at the last valid item
                index = end;
            }

            // we can scan until the first item
            for (; index >= 0; index--)
            {
                if (SequenceEqual(itemComparer, span.Slice(index), items, itemCount))
                    return Some(index);
            }
        }

        return None();
    }
    
    /// <summary>
    /// Try to find the Index and Item that match an <paramref name="itemPredicate"/>
    /// </summary>
    /// <param name="itemPredicate">
    /// The <see cref="Predicate{T}"/> used to determine if a valid item has been found
    /// </param>
    /// <param name="firstToLast"><b>default: true</b><br/>
    /// <c>true</c>: Search from low to high indices<br/>
    /// <c>false</c>: Search from high to low indices
    /// </param>
    /// <param name="offset">
    /// The <see cref="Index"/> offset to start the search from
    /// </param>
    /// <returns>
    /// An <see cref="Option{T}"/> that might contain the first matching Index + Item
    /// </returns>
    public Option<(int Index, T Item)> TryFindItemIndex(
        Func<T, bool>? itemPredicate,
        bool firstToLast = true,
        Index? offset = default)
    {
        if (itemPredicate is null)
            return None();
        
        var pos = _position;
        var span = new Span<T>(_array);

        int index;
        T item;
      
        if (firstToLast)
        {
            // Check for a starting offset
            if (offset.TryGetValue(out Index offsetIndex))
            {
                // Validate that offset
                var validIndex = Validate.Index(offsetIndex, pos);
                if (!validIndex.IsOk(out index))
                    return None();
            }
            else
            {
                // No offset, we start at the first item
                index = 0;
            }

            // we can scan until the last item
            for (; index < pos; index++)
            {
                item = span[index];
                if (itemPredicate(item))
                {
                    return Some((index, item));
                }
            }
        }
        else // lastToFirst
        {
            // Check for a starting offset
            if (offset.TryGetValue(out Index offsetIndex))
            {
                // Validate that offset
                var validIndex = Validate.Index(offsetIndex, pos);
                if (!validIndex.IsOk(out index))
                    return None();
            }
            else
            {
                // No offset, we start at the last item
                index = pos - 1;
            }

            // we can scan until the first item
            for (; index >= 0; index--)
            {
                item = span[index];
                if (itemPredicate(span[index]))
                {
                    return Some((index, item));
                }
            }
        }

        return None();
    }

    void IList<T>.RemoveAt(int index) => TryRemoveAt(index);

    /// <summary>
    /// Try to remove the item at the given <see cref="Index"/>
    /// </summary>
    /// <param name="index">
    /// The <see cref="Index"/> of the item to remove
    /// </param>
    /// <returns>
    /// <c>true</c> if the item was removed<br/>
    /// <c>false</c> if it was not
    /// </returns>
    public bool TryRemoveAt(Index index)
    {
        if (!Validate.Index(index, _position).IsOk(out var offset))
            return None<T>();
        Sequence.SelfCopy(Written, (offset + 1).., offset..);
        return true;
    }


    /// <summary>
    /// Try to remove and return the item at the given <see cref="Index"/>
    /// </summary>
    /// <param name="index">
    /// The <see cref="Index"/> of the item to remove
    /// </param>
    /// <returns>
    /// An <see cref="Option{T}"/> that contains the removed value
    /// </returns>
    public Option<T> TryRemoveAndGetAt(Index index)
    {
        if (!Validate.Index(index, _position).IsOk(out var offset))
            return None<T>();
        T item = Written[offset];
        Sequence.SelfCopy(Written, (offset + 1).., offset..);
        return Some(item);
    }

    /// <summary>
    /// Try to remove the items at the given <see cref="Range"/>
    /// </summary>
    /// <param name="range">
    /// The <see cref="Range"/> of items to remove
    /// </param>
    /// <returns>
    /// <c>true</c> if the range of items was removed<br/>
    /// <c>false</c> if they were not
    /// </returns>
    public bool TryRemoveMany(Range range)
    {
        if (!Validate.Range(range, _position).IsOk(out var ol))
            return false;
        (int offset, int length) = ol;
        Sequence.SelfCopy(Written, (offset + length).., offset..);
        return true;
    }

    /// <summary>
    /// Try to remove and return the items at the given <see cref="Range"/>
    /// </summary>
    /// <param name="range">
    /// The <see cref="Range"/> of items to remove
    /// </param>
    /// <returns>
    /// An <see cref="Option{T}"/> containing an <see cref="Array">T[]</see> of removed items
    /// </returns>
    public Option<T[]> TryRemoveAndGetMany(Range range)
    {
        if (!Validate.Range(range, _position).IsOk(out var ol))
            return None<T[]>();
        (int offset, int length) = ol;
        T[] items = _array.AsSpan(offset, length).ToArray();
        Sequence.SelfCopy(_array, (offset + length).., offset..);
        return Some(items);
    }

    bool ICollection<T>.Remove(T item)
    {
        return TryFindIndex(item).IsSome(out int index) && TryRemoveAt(index);
    }

    /// <summary>
    /// Remove all the items in this <see cref="Buffer{T}"/> that match an <paramref name="itemPredicate"/>
    /// </summary>
    /// <param name="itemPredicate">
    /// The <see cref="Predicate{T}"/> to determine if an item is to be removed
    /// </param>
    /// <returns>
    /// The total number of items removed
    /// </returns>
    public int RemoveWhere(Func<T, bool>? itemPredicate)
    {
        if (itemPredicate is null)
            return 0;

        int freeIndex = 0; // the first free slot in items array
        int pos = _position;
        var array = _array;

        // Find the first item which needs to be removed.
        while (freeIndex < pos && !itemPredicate(array[freeIndex]))
            freeIndex++;

        if (freeIndex >= pos)
            return 0;

        int current = freeIndex + 1;
        while (current < pos)
        {
            // Find the first item which needs to be kept.
            while (current < pos && itemPredicate(array[current]))
                current++;

            if (current < pos)
            {
                // copy item to the free slot.
                array[freeIndex++] = array[current++];
            }
        }

        int removedCount = pos - freeIndex;
        _position = freeIndex;
        return removedCount;
    }

    /// <summary>
    /// Removes all items in this <see cref="Buffer{T}"/>, setting its <see cref="Count"/> to zero
    /// </summary>
    /// <remarks>
    /// This does not release references to any items that had been added, use <see cref="Dispose"/> to ensure proper cleanup
    /// </remarks>
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
        return _array.AsSpan(pos, length);
    }

    /// <summary>
    /// Allocates a <see cref="Span{T}"/> of the given <paramref name="length"/>
    /// </summary>
    /// <param name="length">
    /// The total number of items to allocate a <see cref="Span{T}"/> for
    /// </param>
    /// <returns>
    /// A <see cref="Span{T}"/> over the allocated items
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> Allocate(int length)
    {
        if (length <= 0)
            return Span<T>.Empty;

        int pos = _position;
        var array = _array;
        if (pos + length <= array.Length)
        {
            _position = pos + length;
            return array.AsSpan(pos, length);
        }
        else
        {
            return AllocateGrow(length);
        }
    }

    /// <summary>
    /// Try to use the available capacity of this <see cref="Buffer{T}"/> using a <see cref="UseAvailable{T}"/> delegate
    /// </summary>
    /// <param name="useAvailable">
    /// <see cref="UseAvailable{T}"/> to apply to any currently available space
    /// </param>
    /// <returns>
    /// <c>true</c> if the <see cref="UseAvailable{T}"/> operation succeeded<br/>
    /// <c>false</c> if it did not
    /// </returns>
    public bool TryUseAvailable(UseAvailable<T> useAvailable)
    {
        int used = useAvailable(Available);
        if (used < 0 || used > Available.Length)
            return false;
        _position += used;
        return true;
    }

    /// <summary>
    /// Performs a <see cref="RefItem{T}"/> operation on each item in this <see cref="Buffer{T}"/>
    /// </summary>
    /// <param name="perItem">
    /// The <see cref="RefItem{T}"/> delegate that can mutate items
    /// </param>
    public void ForEach(RefItem<T>? perItem)
    {
        if (perItem is null)
            return;
        int pos = _position;
        var array = _array;
        for (int i = 0; i < pos; i++)
        {
            perItem(ref array[i]);
        }
    }

    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
        Validate.CopyTo(_position, array, arrayIndex).OkOrThrow();
        Written.CopyTo(array.AsSpan(arrayIndex));
    }

    /// <summary>
    /// Try to copy the items in this <see cref="Buffer{T}"/> to a <see cref="Span{T}"/>
    /// </summary>
    public bool TryCopyTo(Span<T> span) => Written.TryCopyTo(span);

    /// <summary>
    /// Get the <see cref="Span{T}"/> of written items in this <see cref="Buffer{T}"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan() => _array.AsSpan(0, _position);

    /// <summary>
    /// Copy the items in this <see cref="Buffer{T}"/> to a new <c>T[]</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T[] ToArray() => Written.ToArray();

    /// <summary>
    /// Convert this <see cref="Buffer{T}"/> to a <see cref="List{T}"/> containing the same items
    /// </summary>
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
        T[] toReturn = _array;
        // defensive clear
        _position = 0;
        _array = [];
        ArrayPool.Return(toReturn, true);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => ReferenceEquals(this, obj);

    /// <inheritdoc/>
    public override int GetHashCode() => Hasher.Combine<T>(Written);

    /// <summary>
    /// Gets a <see cref="string"/> representation of the <see cref="Written"/> items
    /// </summary>
    public override string ToString()
    {
        var written = Written;
        // Special handling for textual types
        if (typeof(T) == typeof(char))
        {
            return written.ToString(); // will convert directly to a string
        }

        DefaultInterpolatedStringHandler text = new(Count * 2, Count);
        text.AppendLiteral("[");
        if (written.Length > 0)
        {
            text.AppendFormatted<T>(written[0]);
            for (var i = 1; i < written.Length; i++)
            {
                text.AppendLiteral(", ");
                text.AppendFormatted<T>(written[i]);
            }
        }

        text.AppendLiteral("]");
        return text.ToStringAndClear();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < _position; i++)
        {
            yield return _array[i];
        }
    }
}