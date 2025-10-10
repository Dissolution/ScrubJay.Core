#pragma warning disable IDE0044

namespace ScrubJay.Memory;

[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public ref struct SpanWriter<T>
{
    private Span<T> _span;
    private int _position;

    public readonly ref T this[Index index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _span[.._position][index];
    }

    public Span<T> this[Range range]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span[.._position][range];
    }

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

    public Span<T> Written
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span.Slice(0, _position);
    }

    public Span<T> Available
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span.Slice(_position);
    }

    public int RemainingCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span.Length - _position;
    }

    public SpanWriter(Span<T> span)
    {
        _span = span;
        _position = 0;
    }

    public SpanWriter(Span<T> span, int position)
    {
        Throw.IfBadIndex(position, span.Length);
        _span = span;
        _position = position;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void WriteUnsafe(T item)
    {
        _span[_position] = item;
        _position++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void WriteUnsafe(scoped ReadOnlySpan<T> items)
    {
        items.CopyTo(_span[_position..]);
        _position += items.Length;
    }

    public void UseAvailable(FnSpan<T, int> useAvailable)
    {
        int used = useAvailable(_span.Slice(_position));
        _position += used;
    }

    public void Write(T item)
    {
        int pos = _position;
        int newPos = pos + 1;
        if (newPos <= Capacity)
        {
            _span[pos] = item;
            _position = newPos;
            return;
        }

        throw Ex.Invalid($"Could not write item '{item}': No capacity remaining");
    }

    public Result<Unit> TryWrite(T item)
    {
        if (_position < Capacity)
        {
            _span[_position] = item;
            _position++;
            return Unit();
        }

        return new InvalidOperationException($"Could not write item '{item}': No capacity remaining");
    }

    public void WriteMany(params ReadOnlySpan<T> items)
    {
        if (_position + items.Length <= Capacity)
        {
            items.CopyTo(Available);
            _position += items.Length;
        }
        else
        {
            throw Ex.Arg(items, $"Could not write {items.Length} items: Only {RemainingCount} capacity remains");
        }
    }


    public Result<int> TryWriteMany(params ReadOnlySpan<T> items)
    {
        if (_position + items.Length <= Capacity)
        {
            items.CopyTo(Available);
            _position += items.Length;
            return Ok(items.Length);
        }

        return Ex.Invalid($"Could not write {items.Length} items: Only {RemainingCount} capacity remaining");
    }

    public void WriteMany(T[] items) => WriteMany(items.AsSpan());

    public Result<int> TryWriteMany(T[]? items)
        => TryWriteMany(items.AsSpan());

    public Result<int> TryWriteMany(IEnumerable<T>? items)
    {
        if (items is null)
            return Ok(0);

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
                return Ok(count);
            }

            return new InvalidOperationException(
                $"Could not write {list.Count} items: Only {RemainingCount} capacity remaining");
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
                return Ok(count);
            }

            return new InvalidOperationException(
                $"Could not write {collection.Count} items: Only {RemainingCount} capacity remaining");
        }
        else
        {
            int start = pos;
            foreach (var item in items)
            {
                if (pos >= Capacity)
                {
                    _span[start..].Clear();
                    return new InvalidOperationException(
                        $"Could not write {pos} or more items: Only {RemainingCount} capacity remaining");
                }

                _span[pos++] = item;
            }

            _position = pos;
            return Ok(pos - start);
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