#pragma warning disable IDE0044

namespace ScrubJay.Memory;

[StructLayout(LayoutKind.Auto)]
public ref struct SpanWriter<T>
{
    private Span<T> _span;
    private int _position;

    public ref T this[Index index] => ref AsSpan()[index];

    public Span<T> this[Range range] => AsSpan()[range];

    private readonly int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span.Length;
    }

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get => _position;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal set => _position = value.Clamp(0, Capacity);
    }

    internal Span<T> Written => _span.Slice(0, _position);
    internal Span<T> Available => _span.Slice(_position);

    public int RemainingCount => Capacity - _position;

    public SpanWriter(Span<T> span)
    {
        _span = span;
        _position = 0;
    }

    public SpanWriter(Span<T> span, int position)
    {
        if ((position < 0) || (position >= span.Length))
            throw new ArgumentOutOfRangeException(nameof(position), position, "Position must be inside of span");
        _span = span;
        _position = position;
    }

    public void UseAvailable(UseAvailable<T> useAvailable)
    {
        int used = useAvailable(_span.Slice(_position));
        _position += used;
    }

    public Result<Unit, Exception> TryWrite(T item)
    {
        int pos = _position;
        int newPos = pos + 1;
        if (newPos <= Capacity)
        {
            _span[pos] = item;
            _position = newPos;
            return Ok(Unit());
        }

        return new InvalidOperationException($"Could not write item '{item}': No capacity remaining");
    }

    public Result<Unit, Exception> TryWriteMany(scoped ReadOnlySpan<T> items)
    {
        int pos = _position;
        int newPos = pos + items.Length;
        if (newPos <= Capacity)
        {
            items.CopyTo(_span[pos..]);
            _position = newPos;
            return Ok(Unit());
        }

        return new InvalidOperationException($"Could not write {items.Length} items: Only {RemainingCount} capacity remaining");
    }

    public Result<Unit, Exception> TryWriteMany(params T[]? items)
    {
        if (items is null)
            return Ok(Unit());
        int pos = _position;
        int newPos = pos + items.Length;
        if (newPos <= Capacity)
        {
            items.CopyTo(_span[pos..]);
            _position = newPos;
            return Ok(Unit());
        }

        return new InvalidOperationException($"Could not write {items.Length} items: Only {RemainingCount} capacity remaining");
    }

    public Result<Unit, Exception> TryWriteMany(IEnumerable<T>? items)
    {
        if (items is null)
            return Ok(Unit());

        int pos = _position;

        if (items is IList<T> list)
        {
            int count = list.Count;
            int newPos = pos + count;
            if (newPos <= Capacity)
            {
                for (int i = 0; i < count; i++)
                {
                    _span[pos++] = list[i];
                }
                Debug.Assert(pos == newPos);
                _position = newPos;
                return Unit();
            }

            return new InvalidOperationException($"Could not write {list.Count} items: Only {RemainingCount} capacity remaining");
        }
        else if (items is ICollection<T> collection)
        {
            int count = collection.Count;
            int newPos = pos + count;
            if (newPos <= Capacity)
            {
                foreach (T item in items)
                {
                    _span[pos++] = item;
                }
                Debug.Assert(pos == newPos);
                _position = newPos;
                return Unit();
            }

            return new InvalidOperationException($"Could not write {collection.Count} items: Only {RemainingCount} capacity remaining");
        }
        else
        {
            int start = pos;
            foreach (var item in items)
            {
                if (pos >= Capacity)
                {
                    _span[start..].Clear();
                    return new InvalidOperationException($"Could not write {pos} or more items: Only {RemainingCount} capacity remaining");
                }
                _span[pos++] = item;
            }

            _position = pos;
            return Unit();
        }
    }

    public void Clear() => _span.Slice(0, _position).Clear();

    public bool TryCopyTo(Span<T> span) => AsSpan().TryCopyTo(span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan() => _span.Slice(0, _position);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T[] ToArray() => _span.Slice(0, _position).ToArray();

    public Span<T>.Enumerator GetEnumerator() => AsSpan().GetEnumerator();
}
