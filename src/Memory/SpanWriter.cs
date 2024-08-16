using System.Diagnostics;

namespace ScrubJay.Memory;

public ref struct SpanWriter<T>
{
    private readonly Span<T> _span;
    private int _position;

    private int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span.Length;
    }

    public int Count => _position;
    
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

    public bool TryWriteMany(ReadOnlySpan<T> items)
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
    
    public bool TryCopyTo(Span<T> span) => AsSpan().TryCopyTo(span);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan() => _span.Slice(0, _position);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T[] ToArray() => _span.Slice(0, _position).ToArray();

    public Span<T>.Enumerator GetEnumerator() => AsSpan().GetEnumerator();
}