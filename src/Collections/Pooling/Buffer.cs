// Member can be made readonly
#pragma warning disable IDE0251

namespace ScrubJay.Collections.Pooling;

/// <summary>
/// A Buffer is a stack-based <see cref="IList{T}"/>-like collection <i>(grows as required)</i>,
/// that uses <see cref="ArrayInstancePool{T}"/> to avoid allocation,
/// and thus must be <see cref="Dispose">Disposed</see> after use
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of items stored in this <see cref="Buffer{T}"/>
/// </typeparam>
/// <remarks>
/// Heavily inspired by <c>System.Collections.Generic.ValueListBuilder&lt;T&gt;</c><br/>
/// <a href="https://github.com/dotnet/runtime/blob/release/8.0/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/ValueListBuilder.cs"/>
/// </remarks>
[PublicAPI]
[MustDisposeResource(true)]
public ref struct Buffer<T>
/* Roughly implements :
 IList<T>, IReadOnlyList<T>,
 ICollection<T>, IReadOnlyCollection<T>,
 IEnumerable<T>,
 IDisposable
 */
{
    /// <summary>
    /// Implicitly use the <see cref="Written"/> portion of a <see cref="Buffer{T}"/> as a <see cref="Span{T}"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Span<T>(Buffer<T> spanBuffer) => spanBuffer.Written;

    /// <summary>
    /// Implicitly use the <see cref="Written"/> portion of a <see cref="Buffer{T}"/> as a <see cref="ReadOnlySpan{T}"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<T>(Buffer<T> spanBuffer) => spanBuffer.Written;

    /// <summary>
    /// Implicitly convert a <see cref="Span{T}"/> into a <see cref="Buffer{T}"/> that starts filling it<br/>
    /// This is useful with <c>stackalloc</c>:<br/>
    /// <c>using Buffer&lt;byte&gt; buffer = stackalloc byte[8];</c>
    /// </summary>
    /// <param name="initialBuffer">
    /// The initial <see cref="Span{T}"/> buffer the returned <see cref="Buffer{T}"/> will start to fill<br/>
    /// If items are added beyond this <see cref="Span{T}"/>'s <see cref="Span{T}.Length"/>,
    /// a new <see cref="Array">T[]</see> will be rented and this <see cref="Span{T}"/> will no longer be used
    /// </param>
    /// <returns>
    /// A <see cref="Buffer{T}"/> filling the <paramref name="initialBuffer"/>
    /// </returns>
    [MustDisposeResource]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Buffer<T>(Span<T> initialBuffer) => new(initialBuffer, 0);

    // writeable span, likely points to _array
    internal Span<T> _span;

    // _array, likely borrowed from ArrayPool
    internal T[]? _array;

    // the position in _span that we're writing to
    private int _position;

    /// <summary>
    /// Get a <see cref="Span{T}"/> over items in this <see cref="Buffer{T}"/>
    /// </summary>
    public Span<T> Written
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span.Slice(0, _position);
    }

    /// <summary>
    /// Gets a <see cref="Span{T}"/> over the unwritten, available portion of this <see cref="Buffer{T}"/>
    /// </summary>
    internal Span<T> Available
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span.Slice(_position);
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
        readonly get => _position;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal set
        {
            Debug.Assert((value >= 0) && (value < Capacity));
            _position = value;
        }
    }

    /// <summary>
    /// Gets the current capacity for this <see cref="Buffer{T}"/>, which will be increased as needed
    /// </summary>
    public readonly int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span.Length;
    }

    /// <summary>
    /// Creates a new <see cref="Buffer{T}"/> that starts with an <paramref name="initialArray"/> and <paramref name="initialPosition"/>
    /// </summary>
    /// <param name="initialArray">
    /// The initial <c>T[]</c> that this <see cref="Buffer{T}"/> will own and return to <see cref="ArrayInstancePool{T}"/> when growing and disposal
    /// </param>
    /// <param name="initialPosition">
    /// The initial position to begin writing
    /// </param>
    internal Buffer(T[] initialArray, int initialPosition)
    {
        _span = _array = initialArray;
        Debug.Assert((initialPosition >= 0) && (initialPosition <= Capacity));
        _position = initialPosition;
    }

    /// <summary>
    /// Creates a new <see cref="Buffer{T}"/> that starts with an <paramref name="initialSpan"/> and <paramref name="initialPosition"/>
    /// </summary>
    /// <param name="initialSpan">
    /// The initial <see cref="Span{T}"/> that this <see cref="Buffer{T}"/> will use
    /// </param>
    /// <param name="initialPosition">
    /// The initial position to begin writing
    /// </param>
    internal Buffer(Span<T> initialSpan, int initialPosition)
    {
        _span = initialSpan;
        _array = null;
        Debug.Assert((initialPosition >= 0) && (initialPosition <= Capacity));
        _position = initialPosition;
    }

    /// <summary>
    /// Create a new, empty <see cref="Buffer{T}"/> that has not allocated
    /// </summary>
    public Buffer()
    {
        _span = _array = null;
        _position = 0;
    }

    /// <summary>
    /// Create a new, empty <see cref="Buffer{T}"/> with at least a starting <see cref="Capacity"/> of <paramref name="minCapacity"/>
    /// </summary>
    /// <param name="minCapacity">
    /// The minimum starting <see cref="Capacity"/> this <see cref="Buffer{T}"/> will have
    /// </param>
    /// <remarks>
    /// If <paramref name="minCapacity"/> is greater than 0, an array will be rented from <see cref="ArrayInstancePool{T}"/>
    /// </remarks>
    public Buffer(int minCapacity)
    {
        if (minCapacity <= 0)
        {
            _span = _array = null;
        }
        else
        {
            _span = _array = ArrayInstancePool<T>.Shared.Rent(minCapacity);
        }

        _position = 0;
    }

    #region nonpublic methods

    /// <summary>
    /// Increases the size of the rented array by at least <paramref name="adding"/> items
    /// </summary>
    /// <param name="adding"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GrowBy(int adding) => GrowTo((Capacity + adding) * 2);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GrowTo(int newCapacity)
    {
        Debug.Assert(newCapacity >= Capacity);
        T[] newArray = ArrayInstancePool<T>.Shared.Rent(newCapacity);
        Sequence.CopyTo(Written, newArray);

        T[]? toReturn = _array;
        _span = _array = newArray;
        ArrayInstancePool<T>.Shared.Return(toReturn);
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

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void InsertManyEnumerable(int index, IEnumerable<T> items)
    {
        // Slow path, fill another buffer and then insert known
        using var buffer = new Buffer<T>();
        foreach (var item in items)
        {
            buffer.Add(item);
        }

        _ = TryInsertMany(index, buffer).OkOrThrow();
    }

    #endregion nonpublic methods

    /// <summary>
    /// Grows the <see cref="Capacity"/> of this <see cref="Buffer{T}"/> to at least twice its current value
    /// </summary>
    /// <remarks>
    /// This method causes a rental from <see cref="ArrayInstancePool{T}"/>
    /// </remarks>
    public void Grow() => GrowBy(1);

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
    /// Grows this PooledList and then add an item
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
    public void AddMany(params T[]? items) => AddMany(new ReadOnlySpan<T>(items));

    /// <summary>
    /// Adds the given <paramref name="items"/> to this <see cref="Buffer{T}"/>
    /// </summary>
    public void AddMany(IEnumerable<T>? items)
    {
        if (items is null)
        {
            return;
        }

        int itemCount;
        if (items is ICollection<T> collection)
        {
            itemCount = collection.Count;
            if (itemCount == 0)
            {
                return;
            }

            int pos = _position;
            int newPos = pos + itemCount;
            if ((newPos > Capacity) || _array is null)
            {
                GrowBy(itemCount);
            }

            collection.CopyTo(_array!, pos);
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

    /// <summary>
    /// Try to insert an <paramref name="item"/> into this <see cref="Buffer{T}"/> at <paramref name="index"/>
    /// </summary>
    /// <param name="index">The <see cref="Index"/> to insert the <paramref name="item"/></param>
    /// <param name="item">The item to insert</param>
    /// <returns>
    /// A <see cref="Result{O,E}"/> that contains the <c>int</c> offset the item was inserted at or an <see cref="Exception"/> that describes why insertion failed
    /// </returns>
    public Result<int> TryInsert(Index index, T item)
    {
        int pos = _position;
        var vr = Validate.InsertIndex(index, pos);
        if (!vr.IsOk(out int offset))
        {
            return vr;
        }

        if (offset == pos)
        {
            Add(item);
            return Ok(offset);
        }

        int newPos = pos + 1;
        if (newPos >= Capacity)
        {
            GrowBy(1);
        }

        Sequence.SelfCopy(_span, offset..pos, (offset + 1)..);
        _span[offset] = item;
        _position = newPos;
        return Ok(offset);
    }

    /// <summary>
    /// Try to insert multiple <paramref name="items"/> into this <see cref="Buffer{T}"/> at <paramref name="index"/>
    /// </summary>
    /// <param name="index">The <see cref="Index"/> to insert the <paramref name="items"/></param>
    /// <param name="items">The <see cref="ReadOnlySpan{T}"/> of items to insert</param>
    /// <returns>
    /// A <see cref="Result{O,E}"/> that contains the <c>int</c> offset the items were inserted at or an <see cref="Exception"/> that describes why insertion failed
    /// </returns>
    public Result<int> TryInsertMany(Index index, scoped ReadOnlySpan<T> items)
    {
        int itemCount = items.Length;

        if (itemCount == 0)
        {
            return Validate.InsertIndex(index, _position);
        }

        if (itemCount == 1)
        {
            return TryInsert(index, items[0]);
        }

        var vr = Validate.InsertIndex(index, _position);
        if (!vr.IsOk(out int offset))
        {
            return vr;
        }

        if (offset == _position)
        {
            AddMany(items);
            return Ok(offset);
        }

        int newPos = _position + itemCount;
        if (newPos >= Capacity)
        {
            GrowBy(itemCount);
        }

        Sequence.SelfCopy(_span, offset.._position, (offset + itemCount)..);
        Sequence.CopyTo(items, _span.Slice(offset, itemCount));
        _position = newPos;
        return Ok(offset);
    }

    /// <summary>
    /// Try to insert multiple <paramref name="items"/> into this <see cref="Buffer{T}"/> at <paramref name="index"/>
    /// </summary>
    /// <param name="index">The <see cref="Index"/> to insert the <paramref name="items"/></param>
    /// <param name="items">The <see cref="Array">T[]</see> of items to insert</param>
    /// <returns>
    /// A <see cref="Result{O,E}"/> that contains the <c>int</c> offset the items were inserted at or an <see cref="Exception"/> that describes why insertion failed
    /// </returns>
    public void TryInsertMany(Index index, params T[]? items) => TryInsertMany(index, new ReadOnlySpan<T>(items));

    /// <summary>
    /// Try to insert multiple <paramref name="items"/> into this <see cref="Buffer{T}"/> at <paramref name="index"/>
    /// </summary>
    /// <param name="index">The <see cref="Index"/> to insert the <paramref name="items"/></param>
    /// <param name="items">The <see cref="IEnumerable{T}"/> of items to insert</param>
    /// <returns>
    /// A <see cref="Result{O,E}"/> that contains the <c>int</c> offset the items were inserted at or an <see cref="Exception"/> that describes why insertion failed
    /// </returns>
    public Result<int> TryInsertMany(Index index, IEnumerable<T>? items)
    {
        if (items is null)
        {
            return Validate.InsertIndex(index, _position);
        }

        int pos = _position;

        var vr = Validate.InsertIndex(index, pos);
        if (!vr.IsOk(out int offset))
        {
            return vr;
        }

        if (offset == _position)
        {
            AddMany(items);
            return Ok(offset);
        }

        int itemCount;
        if (items is ICollection<T> collection)
        {
            itemCount = collection.Count;
            if (itemCount == 0)
            {
                return Ok(offset);
            }

            int newPos = pos + itemCount;
            if ((newPos > Capacity) || _array is null)
            {
                GrowBy(itemCount);
            }

            Sequence.SelfCopy(_span, offset.._position, (offset + itemCount)..);
            collection.CopyTo(_array!, offset);
            _position = newPos;
            return Ok(offset);
        }

        // Enumerate to a temporary PooledList, then insert
        InsertManyEnumerable(offset, items);
        return Ok(offset);
    }

    /// <summary>
    /// Sorts the items in this <see cref="Buffer{T}"/> using an optional <see cref="IComparer{T}"/>
    /// </summary>
    /// <param name="itemComparer">
    /// An optional <see cref="IComparer{T}"/> used to sort the items, defaults to <see cref="Comparer{T}"/>.<see cref="Comparer{T}.Default"/>
    /// </param>
    public void Sort(IComparer<T>? itemComparer = default)
    {
        if (_array is null)
        {
            if (_position <= 0)
            {
                return; // nothing to sort
            }

            // Force us to allocate an array
            Grow();
        }

        Array.Sort(_array!, 0, _position, itemComparer);
    }

    /// <summary>
    /// Sorts the items in this <see cref="Buffer{T}"/> using a <see cref="Comparison{T}"/> delegate
    /// </summary>
    /// <param name="itemComparison">
    /// The <see cref="Comparison{T}"/> delegate used to sort the items
    /// </param>
    public void Sort(Comparison<T> itemComparison)
    {
        if (_array is null)
        {
            if (_position <= 0)
            {
                return; // nothing to sort
            }

            // Force us to allocate an array
            Grow();
        }

        Array.Sort(_array!, 0, _position, Compare.CreateComparer<T>(itemComparison));
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
    public readonly bool Contains(T item)
    {
        for (int i = 0; i < _position; i++)
        {
            if (EqualityComparer<T>.Default.Equals(item, _span[i]))
            {
                return true;
            }
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
    /// The <see cref="Index"/> offset in this <see cref="Buffer{T}"/> to start the search from, defaults the start or end of this buffer depending on <paramref name="firstToLast"/>
    /// </param>
    /// <param name="itemComparer">
    /// An optional <see cref="IEqualityComparer{T}"/> to use for <paramref name="item"/> comparison instead of
    /// <see cref="EqualityComparer{T}"/>.<see cref="EqualityComparer{T}.Default"/>
    /// </param>
    /// <returns>
    /// An <see cref="Option{T}"/> that might contain the index of the first matching instance
    /// </returns>
    public readonly Option<int> TryFindIndex(T item, bool firstToLast = true, Index? offset = default, IEqualityComparer<T>? itemComparer = null)
    {
        int pos = _position;
        var span = _span;

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
                {
                    return None();
                }
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
                {
                    return None();
                }
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

    /// <summary>
    /// Try to find a sequence of <paramref name="items"/> in this <see cref="Buffer{T}"/>
    /// </summary>
    /// <param name="items">The <see cref="ReadOnlySpan{T}"/> to search for</param>
    /// <param name="firstToLast">
    /// <c>true</c>: Search from low to high indices<br/>
    /// <c>false</c>: Search from high to low indices<br/>
    /// </param>
    /// <param name="offset">
    /// The <see cref="Index"/> offset in this <see cref="Buffer{T}"/> to start the search from
    /// </param>
    /// <param name="itemComparer">
    /// An optional <see cref="IEqualityComparer{T}"/> to use for item comparison instead of
    /// <see cref="EqualityComparer{T}"/>.<see cref="EqualityComparer{T}.Default"/>
    /// </param>
    /// <returns>
    /// An <see cref="Option{T}"/> that might contain the index of the first matching sequence
    /// </returns>
    public readonly Option<int> TryFindIndex(
        ReadOnlySpan<T> items,
        bool firstToLast = true,
        Index? offset = default,
        IEqualityComparer<T>? itemComparer = null)
    {
        int itemCount = items.Length;
        int pos = _position;
        var span = _span;

        if ((itemCount == 0) || (itemCount > pos))
        {
            return None();
        }

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
                {
                    return None();
                }
            }
            else
            {
                // No offset, we start at the first item
                index = 0;
            }

            for (; index <= end; index++)
            {
                if (SequenceEqual(itemComparer, span.Slice(index), items, itemCount))
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
                {
                    return None();
                }

                // No point in scanning until the last valid index
                if (index > end)
                {
                    index = end;
                }
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
                {
                    return Some(index);
                }
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
    public readonly Option<(int Index, T Item)> TryFindItemIndex(
        Func<T, bool>? itemPredicate,
        bool firstToLast = true,
        Index? offset = default)
    {
        if (itemPredicate is null)
        {
            return None();
        }

        int pos = _position;
        var span = _span;

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
                {
                    return None();
                }
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
                {
                    return None();
                }
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
        if (!Validate.Index(index, _position).IsOk(out int offset))
        {
            return None<T>();
        }

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
        if (!Validate.Index(index, _position).IsOk(out int offset))
        {
            return None<T>();
        }

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
        {
            return false;
        }

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
    #pragma warning disable IDE0251
    public Option<T[]> TryRemoveAndGetMany(Range range)
    {
        if (!Validate.Range(range, _position).IsOk(out var ol))
        {
            return None<T[]>();
        }

        (int offset, int length) = ol;
        T[] items = _span.Slice(offset, length).ToArray();
        Sequence.SelfCopy(_span, (offset + length).., offset..);
        return Some(items);
    }
    #pragma warning restore IDE0251

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
        {
            return 0;
        }

        int freeIndex = 0; // the first free slot in items array
        int pos = _position;
        Span<T> span = _span;

        // Find the first item which needs to be removed.
        while ((freeIndex < pos) && !itemPredicate(span[freeIndex]))
        {
            freeIndex++;
        }

        if (freeIndex >= pos)
        {
            return 0;
        }

        int current = freeIndex + 1;
        while (current < pos)
        {
            // Find the first item which needs to be kept.
            while ((current < pos) && itemPredicate(span[current]))
            {
                current++;
            }

            if (current < pos)
            {
                // copy item to the free slot.
                span[freeIndex++] = span[current++];
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
    public void Clear() => _position = 0;

    [MethodImpl(MethodImplOptions.NoInlining)]
    private Span<T> AllocateGrow(int length)
    {
        int pos = _position;
        GrowBy(length);
        _position = pos + length;
        return _span.Slice(pos, length);
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
        {
            return [];
        }

        int pos = _position;
        Span<T> span = _span;
        if ((pos + length) <= span.Length)
        {
            _position = pos + length;
            return span.Slice(pos, length);
        }
        else
        {
            return AllocateGrow(length);
        }
    }

    /// <summary>
    /// Try to use the available capacity of this <see cref="Buffer{T}"/> using a <see cref="SpanDelegates.FuncS{T1,R}"/> delegate
    /// </summary>
    /// <param name="useAvailable">
    /// <see cref="SpanDelegates.FuncS{T1,R}"/> to apply to any currently available space
    /// </param>
    /// <returns>
    /// <c>true</c> if the <see cref="SpanDelegates.FuncS{T1,R}"/> operation succeeded<br/>
    /// <c>false</c> if it did not
    /// </returns>
    public bool TryUseAvailable(SpanDelegates.FuncS<T, int> useAvailable)
    {
        int used = useAvailable(Available);
        if ((used < 0) || (used > Available.Length))
        {
            return false;
        }

        _position += used;
        return true;
    }

    /// <summary>
    /// Performs a <see cref="ActionRef{T}"/> operation on each item in this <see cref="Buffer{T}"/>
    /// </summary>
    /// <param name="perItem">
    /// The <see cref="ActionRef{T}"/> delegate that can mutate items
    /// </param>
    public void ForEach(ActionRef<T>? perItem)
    {
        if (perItem is null)
        {
            return;
        }

        int pos = _position;
        var span = _span;
        for (int i = 0; i < pos; i++)
        {
            perItem(ref span[i]);
        }
    }

    /// <summary>
    /// Try to copy the items in this <see cref="Buffer{T}"/> to a <see cref="Span{T}"/>
    /// </summary>
    public bool TryCopyTo(Span<T> span) => Written.TryCopyTo(span);

    /// <summary>
    /// Get the <see cref="Span{T}"/> of written items in this <see cref="Buffer{T}"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan() => _span.Slice(0, _position);

    /// <summary>
    /// Gets a <see cref="Span{T}"/> slice of written items starting at the given <paramref name="index"/>
    /// </summary>
    public Span<T> Slice(int index)
    {
        Validate.Index(index, _position).ThrowIfError();
        return _array.AsSpan(index.._position);
    }

    /// <summary>
    /// Gets a <see cref="Span{T}"/> slice of written items starting at the given <see cref="Index"/>
    /// </summary>
    public Span<T> Slice(Index index)
    {
        int offset = Validate.Index(index, _position).OkOrThrow();
        return _array.AsSpan(offset.._position);
    }

    /// <summary>
    /// Gets a <see cref="Span{T}"/> slice of written items from <paramref name="index"/> for <paramref name="count"/>
    /// </summary>
    public Span<T> Slice(int index, int count)
    {
        Validate.IndexLength(index, count, _position).ThrowIfError();
        return _array.AsSpan(index, count);
    }

    /// <summary>
    /// Gets a <see cref="Span{T}"/> slice of written items from <paramref name="index"/> for <paramref name="count"/>
    /// </summary>
    public Span<T> Slice(Index index, int count)
    {
        (int offset, int len) = Validate.IndexLength(index, count, _position).OkOrThrow();
        return _array.AsSpan(offset, len);
    }

    /// <summary>
    /// Gets a <see cref="Span{T}"/> slice of written items for a <see cref="Range"/>
    /// </summary>
    public Span<T> Slice(Range range)
    {
        (int offset, int len) = Validate.Range(range, _position).OkOrThrow();
        return _array.AsSpan(offset, len);
    }

#pragma warning disable CA1002
    /// <summary>
    /// Copy the items in this <see cref="Buffer{T}"/> to a new <c>T[]</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T[] ToArray() => Written.ToArray();

    /// <summary>
    /// Convert this <see cref="Buffer{T}"/> to a <see cref="List{T}"/> containing the same items
    /// </summary>
    // CA1002: Do not expose generic lists
    public List<T> ToList()
    {
        List<T> list = new List<T>(Capacity);
        list.AddRange(Written);
        return list;
    }
#pragma warning restore CA1002

    /// <summary>
    /// Clears this <see cref="Buffer{T}"/> and returns any rented array back to <see cref="ArrayInstancePool{T}"/>
    /// </summary>
    [HandlesResourceDisposal]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        T[]? toReturn = _array;
        // defensive clear
        _position = 0;
        _span = _array = null;
        ArrayInstancePool<T>.Shared.Return(toReturn);
    }

    /// <summary>
    /// <see cref="Buffer{T}"/> is a <c>ref struct</c> and should not be used for comparison
    /// </summary>
    public override bool Equals(object? obj) => false;

    /// <summary>
    /// <see cref="Buffer{T}"/> is a <c>ref struct</c> and should not be used for comparison
    /// </summary>
    public override int GetHashCode() => 0;

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
            for (int i = 1; i < written.Length; i++)
            {
                text.AppendLiteral(", ");
                text.AppendFormatted<T>(written[i]);
            }
        }

        text.AppendLiteral("]");
        return text.ToStringAndClear();
    }

    /// <summary>
    /// Gets an enumerator over the items in this <see cref="Buffer{T}"/>
    /// </summary>
    public Span<T>.Enumerator GetEnumerator() => Written.GetEnumerator();
}
