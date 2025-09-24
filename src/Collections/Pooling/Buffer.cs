// Member can be made `readonly` -- Incorrect on members that return Span<T>, which may change the underlying data

#pragma warning disable IDE0251
// Rename collections to end in a suffix
#pragma warning disable CA1710


using System.Buffers;
using System.Text;
using ScrubJay.Exceptions;

namespace ScrubJay.Collections.Pooling;

/// <summary>
/// A Buffer is a stack-based, <see cref="IList{T}"/>-like collection <i>(grows as required)</i>,
/// that uses <see cref="ArrayPool{T}"/>.<see cref="ArrayPool{T}.Shared"/> to minimalize allocations.<br/>
/// It must be <see cref="Dispose">Disposed</see> after use or there is little benefit to using one.
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
public ref struct Buffer<T> :
    // IList<T>,
    // IReadOnlyList<T>,
    // ICollection<T>,
    // IReadOnlyCollection<T>,
    // IEnumerable<T>,
    IDisposable
{
    /// <summary>
    /// Implicitly wrap a <see cref="Span{T}"/> with a <see cref="Buffer{T}"/><br/>
    /// This is useful with <c>stackalloc</c>:<br/>
    /// <c>using Buffer&lt;byte&gt; buffer = stackalloc byte[8];</c>
    /// </summary>
    /// <param name="span">
    /// The initial <see cref="Span{T}"/> the returned <see cref="Buffer{T}"/> will wrap<br/>
    /// If items are added beyond the <see cref="Span{T}"/>'s <see cref="Span{T}.Length"/>,
    /// a new underlying <see cref="Array">T[]</see> will be rented,
    /// the <see cref="Span{T}"/> will be <see cref="Span{T}.Clear">Cleared</see>,
    /// and then remain unused.
    /// </param>
    /// <returns>
    /// A <see cref="Buffer{T}"/> wrapping the <paramref name="span"/>
    /// </returns>
    [MustDisposeResource(false)]
    public static implicit operator Buffer<T>(Span<T> span) => new(span, 0);

    /// <summary>
    /// The minimum capacity a buffer can start with
    /// </summary>
    private static readonly int _minCapacity = typeof(T).IsValueType ? 1024 : 64;

    public static Buffer<T> Empty
    {
        [MustDisposeResource]
        get => new();
    }

    [MustDisposeResource]
    public static Buffer<T> New() => new();

    [MustDisposeResource]
    public static Buffer<T> New(int minCapacity) => new(minCapacity);


    // The writable span, usually pointing to _array, but possibly from an initial Span<T>
    internal Span<T> _span;

    // The writeable array, usually rented from an ArrayPool
    // may be null if started with a span
    internal T[]? _array;

    // the position in _span that we're writing to
    private int _position;

    /// <summary>
    /// Gets a <see cref="Span{T}"/> over the written items in this Buffer
    /// </summary>
    public Span<T> Written
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span[.._position];
    }

    /// <summary>
    /// Gets a <see cref="Span{T}"/> over the unwritten|available portion of this Buffer
    /// </summary>
    public Span<T> Available
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span[_position..];
    }

    /// <summary>
    /// Returns a reference to the item in this <see cref="Buffer{T}"/> at the given <paramref name="index"/>
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="index"/> is not in [0..Count)
    /// </exception>
    public ref T this[int index]
    {
        get
        {
            int offset = Throw.IfBadIndex(index, _position);
            return ref _span[offset];
        }
    }

    /// <summary>
    /// Returns a reference to the item in this <see cref="Buffer{T}"/> at the given <see cref="Index"/>
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="index"/> is not in [0..Count)
    /// </exception>
    public ref T this[Index index]
    {
        get
        {
            int offset = Throw.IfBadIndex(index, _position);
            return ref _span[offset];
        }
    }

    /// <summary>
    /// Gets a <see cref="Span{T}"/> over the given <see cref="Range"/> of written items in this <see cref="Buffer{T}"/>
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="range"/> is invalid
    /// </exception>
    public Span<T> this[Range range]
    {
        get
        {
            (int offset, int len) = Throw.IfBadRange(range, _position);
            return _span.Slice(offset, len);
        }
    }

    /// <summary>
    /// Gets the number of items in this <see cref="Buffer{T}"/>
    /// </summary>
    public int Count
    {
        readonly get => _position;
        set
        {
            Throw.IfNotBetween(value, 0, Capacity);
            _position = value;
        }
    }

    /// <summary>
    /// Gets the current capacity for this <see cref="Buffer{T}"/>, which will be increased as required
    /// </summary>
    public int Capacity
    {
        readonly get => _span.Length;
        set
        {
            if (value < _position)
            {
                throw Ex.ArgRange(value, $"Capacity cannot be less that Count of {Count}");
            }
            else if (value > _span.Length)
            {
                GrowTo(value);
            }
        }
    }

#region constructors

    internal Buffer(T[] initialArray, int initialPosition)
    {
        Debug.Assert(initialArray is not null);
        _span = _array = initialArray;
        Debug.Assert((initialPosition >= 0) && (initialPosition <= Capacity));
        _position = initialPosition;
    }

    internal Buffer(Span<T> initialSpan, int initialPosition)
    {
        _span = initialSpan;
        _array = null;
        Debug.Assert((initialPosition >= 0) && (initialPosition <= Capacity));
        _position = initialPosition;
    }

    /// <summary>
    /// Create an empty <see cref="Buffer{T}"/> that has not allocated anything
    /// </summary>
    public Buffer()
    {
        _span = _array = null;
        _position = 0;
    }

    /// <summary>
    /// Create an empty <see cref="Buffer{T}"/> with at least a starting <see cref="Capacity"/> of <paramref name="minCapacity"/>
    /// </summary>
    /// <param name="minCapacity">
    /// The minimum starting <see cref="Capacity"/> the <see cref="Buffer{T}"/> will have
    /// </param>
    /// <remarks>
    /// If <paramref name="minCapacity"/> is greater than 0, an underlying array will be rented
    /// </remarks>
    public Buffer(int minCapacity)
    {
        if (minCapacity <= 0)
        {
            _span = _array = null;
        }
        else
        {
            _span = _array = ArrayPool<T>.Shared.Rent(Math.Max(_minCapacity, minCapacity));
        }

        _position = 0;
    }

#endregion

#region Grow

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GrowBy(int adding)
    {
        Debug.Assert(adding > 0);
        GrowTo(Capacity + (adding * 16));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowTo(int minCapacity)
    {
        Debug.Assert(minCapacity >= Capacity);
        T[] array = ArrayPool<T>.Shared.Rent(Math.Max(minCapacity * 2, _minCapacity));
        if (_span.Length > 0)
        {
            Written.CopyTo(array);
        }

        if (_array is not null)
        {
            ArrayPool<T>.Shared.Return(_array, true);
        }

        _span = _array = array;
    }

    /// <summary>
    /// Grows the <see cref="Capacity"/> of this <see cref="Buffer{T}"/> to at least twice its current value
    /// </summary>
    /// <remarks>
    /// This method causes a <see cref="Array">T[]</see> rental
    /// from <see cref="ArrayPool{T}"/>.<see cref="ArrayPool{T}.Shared"/>
    /// </remarks>
    public void Grow() => GrowTo(Capacity * 2);

#endregion

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

        InsertMany(index, buffer.Written);
    }


#region Add

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAdd(T item)
    {
        Debug.Assert(_position == Capacity);
        GrowBy(1);
        _span[_position] = item;
        _position++;
    }

    /// <summary>
    /// Add a new <paramref name="item"/> to this <see cref="Buffer{T}"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        if (_position < _span.Length)
        {
            _span[_position] = item;
            _position++;
        }
        else
        {
            GrowAndAdd(item);
        }
    }

#endregion

#region AddMany

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAddMany(scoped ReadOnlySpan<T> items)
    {
        GrowBy(items.Length);
        Sequence.CopyTo(items, _span[_position..]);
        _position += items.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddMany(params ReadOnlySpan<T> items)
    {
        if (_position + items.Length <= _span.Length)
        {
            Sequence.CopyTo(items, _span[_position..]);
            _position += items.Length;
        }
        else
        {
            GrowAndAddMany(items);
        }
    }

    /// <summary>
    /// Adds the given <paramref name="items"/> to this <see cref="Buffer{T}"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddMany(T[]? items)
    {
        if (items is not null)
        {
            AddMany(new ReadOnlySpan<T>(items));
        }
    }

    /// <summary>
    /// Adds the given <paramref name="items"/> to this <see cref="Buffer{T}"/>
    /// </summary>
    public void AddMany(IEnumerable<T>? items)
    {
        if (items is null)
        {
            return;
        }

        if (items is ICollection<T> collection)
        {
            int len = collection.Count;
            if (len == 0)
            {
                return;
            }

            if ((_position + len > Capacity) || _array is null)
            {
                GrowBy(len);
            }

            collection.CopyTo(_array!, _position);
            _position += len;
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

#endregion

#region Insert

    public void Insert(Index index, T item)
    {
        int offset = Throw.IfBadInsertIndex(index, _position);
        if (offset == _position)
        {
            Add(item);
            return;
        }

        if (_position + 1 >= _span.Length)
        {
            GrowBy(1);
        }

        Sequence.SelfCopy(_span, offset.._position, (offset + 1)..);
        _span[offset] = item;
        _position++;
    }

#endregion

    #region InsertMany

    public void InsertMany(Index index, params ReadOnlySpan<T> items)
    {
        int offset = Throw.IfBadInsertIndex(index, _position);
        if (offset == _position)
        {
            AddMany(items);
            return;
        }

        int len = items.Length;

        if (_position + len >= _span.Length)
        {
            GrowBy(len);
        }

        Sequence.SelfCopy(_span, offset.._position, (offset + len)..);
        Sequence.CopyTo(items, _span.Slice(offset, len));
        _position += len;
    }


    public void InsertMany(Index index, T[]? items)
    {
        if (items is not null)
        {
            InsertMany(index, new ReadOnlySpan<T>(items));
        }
    }


    public void InsertMany(Index index, IEnumerable<T>? items)
    {
        if (items is null) return;
        int offset = Throw.IfBadInsertIndex(index, _position);
        if (offset == _position)
        {
            AddMany(items);
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

            int newPos = _position + itemCount;
            if ((newPos > Capacity) || _array is null)
            {
                GrowBy(itemCount);
            }

            Sequence.SelfCopy(_span, offset.._position, (offset + itemCount)..);
            collection.CopyTo(_array!, offset);
            _position = newPos;
            return;
        }

        // Enumerate to a temporary PooledList, then insert
        InsertManyEnumerable(offset, items);
    }

    #endregion

    /// <summary>
    /// Sorts the items in this <see cref="Buffer{T}"/> using an optional <see cref="IComparer{T}"/>
    /// </summary>
    /// <param name="itemComparer">
    /// An optional <see cref="IComparer{T}"/> used to sort the items, defaults to <see cref="Comparer{T}"/>.<see cref="Comparer{T}.Default"/>
    /// </param>
    public void Sort(IComparer<T>? itemComparer = null)
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

        Array.Sort(_array!, 0, _position, Relate.CreateComparer<T>(itemComparison));
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
    public readonly Option<int> TryFindIndex(T item, bool firstToLast = true, Index? offset = null,
        IEqualityComparer<T>? itemComparer = null)
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
                    return None;
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
                    return None;
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

        return None;
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
        Index? offset = null,
        IEqualityComparer<T>? itemComparer = null)
    {
        int itemCount = items.Length;
        int pos = _position;
        var span = _span;

        if ((itemCount == 0) || (itemCount > pos))
        {
            return None;
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
                    return None;
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
                    return None;
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

        return None;
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
        Index? offset = null)
    {
        if (itemPredicate is null)
        {
            return None;
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
                    return None;
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
                    return None;
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

        return None;
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
            return None;
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
            return None;
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
            return None;
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
    /// Try to use the available capacity of this <see cref="Buffer{T}"/> using a <see cref="FnSpan{T1,R}"/> delegate
    /// </summary>
    /// <param name="useAvailable">
    /// <see cref="FnSpan{T1,R}"/> to apply to any currently available space
    /// </param>
    /// <returns>
    /// <c>true</c> if the <see cref="FnSpan{T1,R}"/> operation succeeded<br/>
    /// <c>false</c> if it did not
    /// </returns>
    public bool TryUseAvailable(FnSpan<T, int> useAvailable)
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
    /// Performs a <see cref="ActRef{T}"/> operation on each item in this <see cref="Buffer{T}"/>
    /// </summary>
    /// <param name="perItem">
    /// The <see cref="ActRef{T}"/> delegate that can mutate items
    /// </param>
    public void ForEach(ActRef<T>? perItem)
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
#if NETSTANDARD2_1
        foreach (var item in Written)
        {
            list.Add(item);
        }
#else
        list.AddRange((ReadOnlySpan<T>)Written);
#endif
        return list;
    }
#pragma warning restore CA1002

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
        if (toReturn is not null)
        {
            ArrayPool<T>.Shared.Return(toReturn, true);
        }
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
        Span<T> written = Written;
        // Special handling for textual types
        if (typeof(T) == typeof(char))
        {
            return written.ToString(); // will convert directly to a string
        }

        StringBuilder text = new(Count * 2, Count);
        text.Append('[');
        if (written.Length > 0)
        {
            text.Append(written[0]);
            for (int i = 1; i < written.Length; i++)
            {
                text.Append(", ").Append(written[i]);
            }
        }

        text.Append(']');
        return text.ToString();
    }

    /// <summary>
    /// Gets an enumerator over the items in this <see cref="Buffer{T}"/>
    /// </summary>
    public Span<T>.Enumerator GetEnumerator() => Written.GetEnumerator();
}