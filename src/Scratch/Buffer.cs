#pragma warning disable IDE0251


using System.Buffers;

[PublicAPI]
public ref struct Buffer<T> : IDisposable
{
    [MustDisposeResource(false)]
    public static implicit operator Buffer<T>(Span<T> span) => new(span, false);

    private readonly bool _canGrow;

    // The writeable array, usually rented from an ArrayPool
    // may be null if started with a span
    internal T[]? _array;

    // The writable span, usually pointing to _array, but possibly from an initial Span<T>
    internal Span<T> _span;

    // the position in _span that we're writing to
    private int _position;

    /// <summary>
    /// Gets a <see cref="Span{T}"/> over the written items in this Buffer
    /// </summary>
    internal Span<T> Written
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span[.._position];
    }

    /// <summary>
    /// Gets a <see cref="Span{T}"/> over the unwritten|available portion of this Buffer
    /// </summary>
    internal Span<T> Available
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
        internal set => _position = value.Clamp(0, Capacity);
    }

    /// <summary>
    /// Gets the current capacity for this <see cref="Buffer{T}"/>, which will be increased as required
    /// </summary>
    public readonly int Capacity
    {
        get => _span.Length;
    }

    internal Buffer(T[] rentedArray, int startPosition, bool canGrow)
    {
        Debug.Assert(rentedArray is not null);
        Debug.Assert(startPosition >= 0 && startPosition < rentedArray!.Length);

        _canGrow = canGrow;
        _span = _array = rentedArray;
        _position = startPosition;
    }

    [MustDisposeResource(false)]
    public Buffer()
    {
        _canGrow = true;
        _span = _array = null;
        _position = 0;
    }

    [MustDisposeResource(true)]
    public Buffer(int minCapacity)
    {
        _canGrow = true;
        _span = _array = ArrayNest.Rent<T>(minCapacity);
        _position = 0;
    }

    [MustDisposeResource(false)]
    public Buffer(Span<T> buffer, bool canGrow = false)
    {
        _canGrow = canGrow;
        _array = null;
        _span = buffer;
        _position = 0;
    }

    [MustDisposeResource(false)]
    public Buffer(Span<T> buffer, int startPosition, bool canGrow = false)
    {
        _canGrow = canGrow;
        _array = null;
        _span = buffer;
        _position = Throw.IfNotBetween(startPosition, 0, buffer.Length);
    }

    private void GrowBy(int adding)
    {
        Debug.Assert(adding > 0);
        if (_canGrow)
        {
            int newCapacity = (_span.Length + adding) * 2;
            T[] newArray = ArrayNest.Rent<T>(newCapacity);
            if (_position > 0)
            {
                _span[.._position].CopyTo(newArray);
            }

            var toReturn = Reference.Exchange(ref _array, newArray);
            ArrayNest.Return(toReturn);
        }
        else
        {
            throw Ex.Invalid($"This Buffer<{typeof(T):@} cannot Grow beyond its starting Capacity of {Capacity}");
        }
    }

    private Result<Unit> TryGrowBy(int adding)
    {
        Debug.Assert(adding > 0);
        if (_canGrow)
        {
            int newCapacity = (_span.Length + adding) * 2;
            T[] newArray = ArrayNest.Rent<T>(newCapacity);
            if (_position > 0)
            {
                _span[.._position].CopyTo(newArray);
            }

            var toReturn = Reference.Exchange(ref _array, newArray);
            ArrayNest.Return(toReturn);
            return Unit();
        }
        else
        {
            return Ex.Invalid($"This Buffer<{typeof(T):@} cannot Grow beyond its starting Capacity of {Capacity}");
        }
    }

    public void Add(T item)
    {
        if (_position >= Capacity)
        {
            GrowBy(1);
        }

        _span[_position++] = item;
    }

    public void AddMany(params ReadOnlySpan<T> items)
    {
        if (_position + items.Length > Capacity)
        {
            GrowBy(items.Length);
        }

        items.CopyTo(Available);
    }

    public void AddMany(T[]? items) => AddMany(items.AsSpan());

    public void AddMany(IEnumerable<T>? items)
    {
        if (items is null)
        {
        }
#if !NETFRAMEWORK && !NETSTANDARD
        else if (items is List<T> list)
        {
            var listSpan = CollectionsMarshal.AsSpan(list);
            AddMany(listSpan);
        }
#endif
        else if (items is ICollection<T> collection)
        {
            if (_position + collection.Count > Capacity)
            {
                GrowBy(collection.Count);
            }

            if (_array is not null)
            {
                collection.CopyTo(_array, _position);
            }
            else
            {
                int i = _position;
                foreach (var item in collection)
                {
                    _span[i] = item;
                }
            }
        }
        // ReSharper disable once PossibleMultipleEnumeration
        else if (items.TryGetNonEnumeratedCount(out var count))
        {
            if (_position + count > Capacity)
            {
                GrowBy(count);
            }

            int i = _position;
            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var item in items)
            {
                _span[i] = item;
            }
        }
        else
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }
    }

    public void AddMany(Buffer<T> buffer)
    {
        AddMany(buffer.Written);
    }

    public Result<int> TryAdd(T item)
    {
        if (_position >= Capacity)
        {
            if (TryGrowBy(1).IsError(out var err))
                return err;
        }

        _span[_position++] = item;
        return Ok(1);
    }

    public Result<int> TryAddMany(params ReadOnlySpan<T> items)
    {
        if (_position + items.Length > Capacity)
        {
            if (TryGrowBy(items.Length).IsError(out var err))
                return err;
        }

        items.CopyTo(Available);
        return Ok(items.Length);
    }

    public Result<int> TryAddMany(T[]? items) => TryAddMany(items.AsSpan());

    public Result<int> TryAddMany(IEnumerable<T>? items)
    {
        if (items is null)
        {
            return Ok(0);
        }
#if !NETFRAMEWORK && !NETSTANDARD
        else if (items is List<T> list)
        {
            var listSpan = CollectionsMarshal.AsSpan(list);
            return TryAddMany(listSpan);
        }
#endif
        else if (items is ICollection<T> collection)
        {
            if (_position + collection.Count > Capacity)
            {
                if (TryGrowBy(collection.Count).IsError(out var err))
                    return err;
            }

            if (_array is not null)
            {
                collection.CopyTo(_array, _position);
            }
            else
            {
                int i = _position;
                foreach (var item in collection)
                {
                    _span[i] = item;
                }
            }
            return Ok(collection.Count);
        }
        // ReSharper disable once PossibleMultipleEnumeration
        else if (items.TryGetNonEnumeratedCount(out var count))
        {
            if (_position + count > Capacity)
            {
                if (TryGrowBy(count).IsError(out var err))
                    return err;
            }

            int i = _position;
            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var item in items)
            {
                _span[i] = item;
            }

            return Ok(count);
        }
        else
        {
            int added = 0;
            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var item in items)
            {
                if (TryAdd(item).IsError(out var err))
                    return err;
                added++;
            }

            return Ok(added);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan() => _span[.._position];

    public T[] ToArray() => AsSpan().ToArray();

    public Span<T>.Enumerator GetEnumerator() => AsSpan().GetEnumerator();

    [HandlesResourceDisposal]
    public void Dispose()
    {
        T[]? toReturn = Reference.Exchange(ref _array, null);
        this = default; // hard clear
        ArrayNest.Return(toReturn);
    }

    public override bool Equals([NotNullWhen(true)] object? obj) => false;

    public override int GetHashCode() => Hasher.HashMany<T>(AsSpan());

    public override string ToString() => AsSpan().ToString();
}