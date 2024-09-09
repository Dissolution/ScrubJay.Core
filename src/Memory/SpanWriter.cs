namespace ScrubJay.Memory;

public delegate bool SpanWriterTryWrite<T>(ref SpanWriter<T> writer);

[StructLayout(LayoutKind.Auto)]
public ref struct SpanWriter<T>
{
    private readonly Span<T> _span;
    private int _position;

    public ref T this[Index index] => ref AsSpan()[index];
    
    public Span<T> this[Range range] => AsSpan()[range];
    
    private int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span.Length;
    }

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _position;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal set => _position = value.Clamp(0, Capacity);
    }

    internal Span<T> Written => _span.Slice(0, _position);
    internal Span<T> Available => _span.Slice(_position);
    
    public SpanWriter(Span<T> span)
    {
        _span = span;
        _position = 0;
    }

    public SpanWriter(Span<T> span, int position)
    {
        if (position < 0 || position >= span.Length)
            throw new ArgumentOutOfRangeException(nameof(position), position, "Position must be inside of span");
        _span = span;
        _position = position;
    }

    public bool Chain(params SpanWriterTryWrite<T>[] writes)
    {
        for (int i = 0; i < writes.Length; i++)
        {
            if (!writes[i](ref this))
                return false;
        }
        return true;
    }

    public void UseAvailable(UseAvailable<T> useAvailable)
    {
        int used = useAvailable(_span.Slice(_position));
        _position += used;
    }
    
    public bool TryWrite(T item)
    {
        int pos = _position;
        int newPos = pos + 1;
        if (newPos <= Capacity)
        {
            _span[pos] = item;
            _position = newPos;
            return true;
        }

        return false;
    }

    public bool TryWriteMany(scoped ReadOnlySpan<T> items)
    {
        int pos = _position;
        int newPos = pos + items.Length;
        if (newPos <= Capacity)
        {
            items.CopyTo(_span[pos..]);
            _position = newPos;
            return true;
        }

        return false;
    }
    
    public bool TryWriteMany(params T[]? items)
    {
        if (items is null)
            return true;
        int pos = _position;
        int newPos = pos + items.Length;
        if (newPos <= Capacity)
        {
            items.CopyTo(_span[pos..]);
            _position = newPos;
            return true;
        }

        return false;
    }

    public bool TryWriteMany(ICollection<T>? items)
    {
        if (items is null)
            return true;
        int pos = _position;
        int newPos = pos + items.Count;
        if (newPos <= Capacity)
        {
            foreach (T item in items)
            {
                _span[pos] = item;
                pos += 1;
            }
            Debug.Assert(pos == newPos);
            _position = newPos;
            return true;
        }

        return false;
    }
    
    public bool TryWriteMany(IReadOnlyCollection<T>? items)
    {
        if (items is null)
            return true;
        int pos = _position;
        int newPos = pos + items.Count;
        if (newPos <= Capacity)
        {
            foreach (T item in items)
            {
                _span[pos] = item;
                pos += 1;
            }
            Debug.Assert(pos == newPos);
            _position = newPos;
            return true;
        }

        return false;
    }
    
    public bool TryWriteMany(IList<T>? items)
    {
        if (items is null)
            return true;
        int pos = _position;
        int newPos = pos + items.Count;
        if (newPos <= Capacity)
        {
            for (int i = 0; i < items.Count; i++)
            {
                _span[pos + i] = items[i];
            }
            _position = newPos;
            return true;
        }

        return false;
    }
    
    public bool TryWriteMany(IReadOnlyList<T>? items)
    {
        if (items is null)
            return true;
        int pos = _position;
        int newPos = pos + items.Count;
        if (newPos <= Capacity)
        {
            for (int i = 0; i < items.Count; i++)
            {
                _span[pos + i] = items[i];
            }
            _position = newPos;
            return true;
        }

        return false;
    }
    
    public Result<int, Exception> TryWriteMany(IEnumerable<T>? items)
    {
        if (items is null)
            return 0;

        if (items.TryGetNonEnumeratedCount(out int count))
        {
            int pos = _position;
            int newPos = pos + count;
            if (newPos <= Capacity)
            {
                foreach (T item in items)
                {
                    _span[pos] = item;
                    pos += 1;
                }
                Debug.Assert(pos == newPos);
                _position = newPos;
                return count;
            }
            return new ArgumentException($"Cannot write {count} items", nameof(items));
        }
        return new ArgumentException("Cannot write an uncountable collection", nameof(items));
    }

    public void Clear()
    {
        _span.Slice(0, _position).Clear();
    }
    
    
    public bool TryCopyTo(Span<T> span) => AsSpan().TryCopyTo(span);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan() => _span.Slice(0, _position);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T[] ToArray() => _span.Slice(0, _position).ToArray();

    public Span<T>.Enumerator GetEnumerator() => AsSpan().GetEnumerator();
}