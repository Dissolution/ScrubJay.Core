using System.Diagnostics;

namespace ScrubJay.Memory;

public ref struct Buffer<T>
{
    public static implicit operator Buffer<T>(Span<T> span) => new Buffer<T>(span);

    private readonly Span<T> _span;
    private int _position;

    public ref T this[Index index] => ref AsSpan()[index];
    public Span<T> this[Range range] => AsSpan()[range];
    
    public int Count => _position;
    public int Capacity => _span.Length;

    public Buffer(Span<T> span)
    {
        _span = span;
        _position = 0;
    }

    public bool TryAdd(T item)
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

    public bool TryAddMany(ReadOnlySpan<T> items)
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
    
    public bool TryAddMany(params T[]? items)
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
    
    public bool TryAddMany(IList<T>? items)
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
    
    public bool TryAddMany(IReadOnlyList<T>? items)
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
    
    public bool TryAddMany(ICollection<T>? items)
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
    
    public bool TryAddMany(IReadOnlyCollection<T>? items)
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

    public bool TryCopyTo(Span<T> span) => AsSpan().TryCopyTo(span);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan() => _span.Slice(0, _position);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T[] ToArray() => _span.Slice(0, _position).ToArray();

    public Span<T>.Enumerator GetEnumerator() => AsSpan().GetEnumerator();
}