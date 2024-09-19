#pragma warning disable S4136, CA1002

using System.Buffers;
using ScrubJay.Comparison;

namespace ScrubJay.Collections;

[PublicAPI]
[MustDisposeResource]
public sealed class Vector<T> :
     IList<T>, IReadOnlyList<T>,
     ICollection<T>, IReadOnlyCollection<T>,
     IEnumerable<T>,
     IDisposable
{
    // implicitly let us use Vector anyplace that wants a Span
    public static implicit operator Span<T>(Vector<T> vector) => vector.Written;

    private T[] _array;
    private int _position;

    /// <summary>
    /// Get the <see cref="Span{T}"/> of items in this <see cref="Vector{T}"/>
    /// </summary>
    public Span<T> Written
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _array.AsSpan(0, _position);
    }

    internal Span<T> Available
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _array.AsSpan(_position);
    }

    bool ICollection<T>.IsReadOnly => false;

    /// <inheritdoc cref="IReadOnlyList{T}.this"/>
    T IReadOnlyList<T>.this[int index]
    {
        get => this[index];
    }
    
    /// <inheritdoc cref="IList{T}.this"/>
    T IList<T>.this[int index]
    {
        get => this[index];
        set => this[index] = value;
    }

    /// <summary>
    /// Returns a reference to the item in this vector at the given <paramref name="index"/>
    /// </summary>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when <paramref name="index"/> is invalid
    /// </exception>
    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _array[index];
    }

    /// <summary>
    /// Returns a reference to the item in this vector at the given <see cref="Index"/>
    /// </summary>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when <paramref name="index"/> is invalid
    /// </exception>
    public ref T this[Index index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _array[index];
    }

    /// <summary>
    /// Returns a <see cref="Span{T}"/> reference to the items at the given <see cref="Range"/>
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="range"/> is invalid
    /// </exception>
    public Span<T> this[Range range]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _array.AsSpan(range);
    }

    /// <summary>
    /// Gets the number of items in this <see cref="Vector{T}"/>
    /// </summary>
    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _position;
    }

    /// <summary>
    /// Gets the current capacity for this <see cref="Vector{T}"/>, which will be increased as needed
    /// </summary>
    public int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _array.Length;
    }

    internal Vector(T[] initialArray, int initialPosition)
    {
       _array = initialArray;
        Debug.Assert(initialPosition >= 0 && initialPosition <= Capacity);
        _position = initialPosition;
    }
    
    /// <summary>
    /// Create a new, empty Vector that has not allocated anything
    /// </summary>
    public Vector()
    {
        _array = [];
        _position = 0;
    }

    /// <summary>
    /// Create a new, empty Vector with at least a starting <see cref="Capacity"/> of <paramref name="minCapacity"/>
    /// </summary>
    /// <param name="minCapacity">
    /// The minimum starting capacity the Vector may have (but may end up larger)
    /// </param>
    /// <remarks>
    /// If <paramref name="minCapacity"/> is greater than 0, an array will be rented from <see cref="ArrayPool{T}"/>
    /// </remarks>
    public Vector(int minCapacity)
    {
        if (minCapacity <= 0)
        {
            _array = [];
        }
        else
        {
            _array = ArrayPool.Rent<T>(minCapacity);
        }

        _position = 0;
    }

    public Vector(T item)
    {
        _array = [item];
        _position = 1;
    }
    
    public Vector(scoped ReadOnlySpan<T> items)
    {
        _array = ArrayPool.Rent<T>(items.Length);
        Sequence.CopyTo(items, _array);
        _position = 1;
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
    /// Grows the <see cref="Capacity"/> of this <see cref="Vector{T}"/> to at least <paramref name="minCapacity"/>
    /// </summary>
    /// <param name="minCapacity">
    /// The minimum value <see cref="Capacity"/> must be<br/>
    /// If set to the <c>default</c> of <c>-1</c>, the <see cref="Capacity"/> will be at least doubled
    /// </param>
    public void GrowCapacity(int minCapacity = -1)
    {
        if (minCapacity == -1)
        {
            GrowTo(Capacity * 2);
        }
        else if (minCapacity > Capacity)
        {
            GrowTo(minCapacity);
        }
    }

    /// <summary>
    /// Grows this Vector and then add an item
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
    /// Add a new <paramref name="item"/> to this <see cref="Vector{T}"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        int pos = _position;
        Span<T> span = _array;

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
    /// Grow this vector and then add <paramref name="count"/> items from <paramref name="source"/>
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
    /// Adds the given <paramref name="items"/> to this <see cref="Vector{T}"/>
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
    /// Adds the given <paramref name="items"/> to this <see cref="Vector{T}"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddMany(params T[]? items) => AddMany(items.AsSpan());

    /// <summary>
    /// Adds the given <paramref name="items"/> to this <see cref="Vector{T}"/>
    /// </summary>
    public void AddMany(IEnumerable<T>? items)
    {
        if (items is null)
            return;

        int itemCount;
        if (items is ICollection<T> collection)
        {
            itemCount = collection.Count;

            int pos = _position;
            int newPos = pos + itemCount;
            if (newPos > Capacity)
            {
                GrowBy(itemCount);
            }

            collection.CopyTo(_array, pos);
            _position = newPos;
        }
        // ReSharper disable PossibleMultipleEnumeration
        else if (items.TryGetNonEnumeratedCount(out itemCount))
        {
            int newPos = _position + itemCount;
            if (newPos > Capacity)
            {
                GrowBy(itemCount);
            }

            var avail = Available;
            int a = 0;
            foreach (var item in items)
            {
                avail[a++] = item;
            }

            Debug.Assert(a == itemCount);
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
        // ReSharper restore PossibleMultipleEnumeration
    }

    void IList<T>.Insert(int index, T item) => Insert(index, item);

    /// <summary>
    /// Inserts an <paramref name="item"/> into this <see cref="Vector{T}"/> at <paramref name="index"/>
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
    /// Inserts multiple <paramref name="items"/> into this <see cref="Vector{T}"/> at <paramref name="index"/>
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

                Sequence.SelfCopy(_array.AsSpan(), offset.._position, (offset + itemCount)..);
                Sequence.CopyTo(items, _array.AsSpan(offset, itemCount));
                _position = newPos;
            }
        }
    }

    public void InsertMany(Index index, params T[]? items) => InsertMany(index, items.AsSpan());

    private void InsertManyEnumerable(Index index, IEnumerable<T> items)
    {
        // Copy everything to a temporary vector, then insert
        using var vector = new Buffer<T>();
        vector.AddMany(items);
        InsertMany(index, vector.AsSpan());
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
        
        int itemCount;
        if (items is ICollection<T> collection)
        {
            itemCount = collection.Count;
            if (itemCount == 0)
                return;
            
            int newPos = pos + itemCount;
            if (newPos > Capacity)
            {
                GrowBy(itemCount);
            }
            
            Sequence.SelfCopy(_array, offset..pos, (offset + itemCount)..);
            _position = newPos;
        }
        else
        {
            // Enumerate to a temporary vector, then insert
            InsertManyEnumerable(index, items);
        }
        // ReSharper restore PossibleMultipleEnumeration
    }

    public void Sort(IComparer<T>? itemComparer = default)
    {
        Array.Sort(_array, 0, _position, itemComparer);
    }
    
    public void Sort(Comparison<T> itemComparison)
    {
        Array.Sort(_array, 0, _position, Compare.CreateComparer<T>(itemComparison));
    }
    
    

    /// <summary>
    /// Does this <see cref="Vector{T}"/> contain any instances of the <paramref name="item"/>?
    /// </summary>
    /// <param name="item">
    /// The item to scan for using <see cref="EqualityComparer{T}"/>.<see cref="EqualityComparer{T}.Default"/>
    /// </param>
    /// <returns>
    /// <c>true</c> if this vector contains an instance of <paramref name="item"/><br/>
    /// <c>false</c> if it does not
    /// </returns>
    public bool Contains(T item)
    {
        for (var i = 0; i < _position; i++)
        {
            if (EqualityComparer<T>.Default.Equals(item, _array[i]))
                return true;
        }

        return false;
    }

    public bool Contains(Func<T, bool> itemPredicate) => TryFindIndex(itemPredicate).IsSome();

    int IList<T>.IndexOf(T item) => TryFindIndex(item).SomeOr(-1);

    /// <summary>
    /// Try to find an <paramref name="item"/> in this <see cref="Vector{T}"/>
    /// </summary>
    /// <param name="item">The item to search for</param>
    /// <param name="firstToLast">
    /// <c>true</c>: Search from low to high indices<br/>
    /// <c>false</c>: Search from high to low indices<br/>
    /// </param>
    /// <param name="offset">
    /// The <see cref="Index"/> offset in this Vector to start the search from
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
        Span<T> span = _array;

        if (!Validate.Index(offset, pos).IsOk(out var index))
            return None<int>();
       
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

        return None<int>();
    }

    
    public Option<int> TryFindIndex(
        Func<T, bool>? itemPredicate, 
        bool firstToLast = true,
        Index offset = default)
    {
        if (itemPredicate is null)
            return None<int>();
        
        var pos = _position;
        Span<T> span = _array;

        if (!Validate.Index(offset, pos).IsOk(out var index))
            return None<int>();
       
        if (firstToLast)
        {
            for (; index < pos; index++)
            {
                if (itemPredicate(span[index]))
                {
                    return Some(index);
                }
            }
        }
        else
        {
            for (; index >= 0; index--)
            {
                if (itemPredicate(span[index]))
                {
                    return Some(index);
                }
            }
        }

        return None<int>();
    }

    public Option<T> TryFind(
        Func<T, bool>? itemPredicate, 
        bool firstToLast = true,
        Index offset = default)
    {
        if (itemPredicate is null)
            return None<T>();
        
        var pos = _position;
        Span<T> span = _array;

        if (!Validate.Index(offset, pos).IsOk(out var index))
            return None<T>();

        T item;
        if (firstToLast)
        {
            for (; index < pos; index++)
            {
                item = span[index];
                if (itemPredicate(item))
                {
                    return Some(item);
                }
            }
        }
        else
        {
            for (; index >= 0; index--)
            {
                item = span[index];
                if (itemPredicate(item))
                {
                    return Some(item);
                }
            }
        }

        return None<T>();
    }
    
    void IList<T>.RemoveAt(int index) => TryRemoveAt(index);

    bool ICollection<T>.Remove(T item) => TryFindIndex(item).Map(i => TryRemoveAt(i));

    public Option<T> TryRemoveAt(Index index)
    {
        if (!Validate.Index(index, _position).IsOk(out var offset))
            return None<T>();
        T item = _array[offset];
        Sequence.SelfCopy(_array, (offset + 1).., offset..);
        return Some(item);
    }

    public bool RemoveMany(Range range)
    {
        if (!Validate.Range(range, _position).IsOk(out var ol))
            return false;
        (int offset, int length) = ol;
        Sequence.SelfCopy(this.Written, (offset + length).., offset..);
        return true;
    }

    public Option<T[]> TryRemoveMany(Range range)
    {
        if (!Validate.Range(range, _position).IsOk(out var ol))
            return None<T[]>();
        (int offset, int length) = ol;
        T[] items = _array.AsSpan(offset, length).ToArray();
        Sequence.SelfCopy(_array.AsSpan(), (offset + length).., offset..);
        return Some(items);
    }

    public int RemoveWhere(Func<T, bool>? itemPredicate)
    {
        if (itemPredicate is null)
            return 0;
        
        int freeIndex = 0;   // the first free slot in items array
        int pos = _position;
        Span<T> array = _array;
        
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
    

    public Option<T> TryRemoveLast()
    {
        int newPos = _position - 1;
        if (newPos < 0)
            return None<T>();
        _position = newPos;
        return Some(_array[newPos]);
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
        return _array.AsSpan(pos, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> Allocate(int length)
    {
        if (length <= 0)
            return Span<T>.Empty;

        int pos = _position;
        Span<T> span = _array;
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
        var available = _array.AsSpan(_position);
        int used = useAvailable(available);
        if (used < 0 || used > available.Length)
            throw new InvalidOperationException("Cannot use an invalid number of available items");
        _position += used;
    }

    void ICollection<T>.CopyTo(T[] array, int arrayIndex)
    {
        Validate.CopyTo(_position, array, arrayIndex).OkOrThrow();
        Written.CopyTo(array.AsSpan(arrayIndex));
    }
    
    public bool TryCopyTo(Span<T> span) => Written.TryCopyTo(span);
    
    
    /// <summary>
    /// Get the <see cref="Span{T}"/> of written items in this <see cref="Vector{T}"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan()
    {
        return _array.AsSpan(0, _position);
    }

    /// <summary>
    /// Copy the items in this <see cref="Vector{T}"/> to a new <c>T[]</c>
    /// </summary>
    public T[] ToArray()
    {
        int pos = _position;
        T[] array = new T[pos];
        Sequence.CopyTo(_array.AsSpan(0, pos), array);
        return array;
    }

#pragma warning disable MA0016
    public List<T> ToList()
    {
        List<T> list = new List<T>(Capacity);
        list.AddRange(Written);
        return list;
    }
#pragma warning restore MA0016

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < _position; i++)
        {
            yield return _array[i];
        }
    }

    public IEnumerable<T> Where(Func<T, bool>? itemPredicate)
    {
        if (itemPredicate is null)
            yield break;
        int pos = _position;
        var array = _array;
        T item;
        for (int i = 0; i < pos; i++)
        {
            item = array[i];
            if (itemPredicate(item))
                yield return item;
        }
    }

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

    /// <summary>
    /// Clears this <see cref="Vector{T}"/> and returns any rented array back to <see cref="ArrayPool{T}"/>
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

    public override bool Equals(object? _) => false;

    public override int GetHashCode() => Hasher.Combine<T>(Written);

    public override string ToString() => Written.ToString();
}