using System.Buffers;
using System.Diagnostics;
using JetBrains.Annotations;
using ScrubJay.Text;

namespace ScrubJay.Memory;

[MustDisposeResource]
public ref struct StackList<T> // : IList<T>, IDisposable
{
    private T[]? _array;
    private Span<T> _span;
    private int _position;

    public ref T this[Index index] => ref Written[index];

    public Span<T> this[Range range] => Written[range];

    public int Count => _position;

    private int Capacity => _span.Length;
    private Span<T> Written => _span.Slice(0, _position);
    private Span<T> Available => _span.Slice(_position);

    public StackList()
    {
        this = default;
    }
    
    public StackList(int minCapacity)
    {
        _span = _array = ArrayPool<T>.Shared.Rent(minCapacity);
        _position = 0;
    }

    public StackList(Span<T> initialBuffer)
    {
        _array = null;
        _span = initialBuffer;
        _position = 0;
    }

    public StackList(Span<T> initialBuffer, int position)
    {
        _array = null;
        _span = initialBuffer;
        if (position < 0 || position >= initialBuffer.Length)
            throw new ArgumentOutOfRangeException(nameof(position), position, "Position must be within initialBuffer");
        _position = position;
    }
    
    private void Grow()
    {
        int newCapacity = Capacity * 2;
        T[] newArray = ArrayPool<T>.Shared.Rent(newCapacity);
        _span[.._position].CopyTo(newArray);

        T[]? toReturn = _array;
        _span = _array = newArray;
        if (toReturn is not null && toReturn.Length > 0)
        {
            ArrayPool<T>.Shared.Return(toReturn, true);
        }
    }

    public void Add(T item)
    {
        int pos = _position;
        if (pos >= Capacity)
        {
            Grow();
        }

        _span[pos] = item;
        _position = pos + 1;
    }

    public void AddMany(ReadOnlySpan<T> items)
    {
        int pos = _position;
        int newPos = pos + items.Length;
        if (newPos >= Capacity)
        {
            Grow();
        }

        items.CopyTo(_span[pos..]);
        _position = newPos;
    }

    public void AddMany(params T[]? items)
    {
        if (items is null)
            return;
        int pos = _position;
        int newPos = pos + items.Length;
        if (newPos >= Capacity)
        {
            Grow();
        }

        items.CopyTo(_span[pos..]);
        _position = newPos;
    }

    public void AddMany(IEnumerable<T>? items)
    {
        if (items is null)
            return;
        foreach (T item in items)
        {
            Add(item);
        }
    }

    public void Insert(Index index, T item)
    {
        int offset = index.GetOffset(_position);
        if ((uint)offset > (uint)_position)
            throw new ArgumentOutOfRangeException(nameof(index), index, $"Index must be between 0 and Count ({Count})");
        if (offset == _position)
        {
            Add(item);
        }
        else
        {
            int newPos = _position + 1;
            if (newPos >= Capacity)
            {
                Grow();
            }

            _span.Slice(offset, _position - offset).CopyTo(_span.Slice(offset + 1));
            _span[offset] = item;
            _position = newPos;
        }
    }

    public void InsertMany(Index index, ReadOnlySpan<T> items)
    {
        int itemCount = items.Length;
        if (itemCount == 0)
            return;
        int offset = index.GetOffset(_position);
        if ((uint)offset > (uint)_position)
            throw new ArgumentOutOfRangeException(nameof(index), index, $"Index must be between 0 and Count ({Count})");
        if (offset == _position)
        {
            AddMany(items);
        }
        else
        {
            int newPos = _position + itemCount;
            if (newPos >= Capacity)
            {
                Grow();
            }

            _span.Slice(offset, _position - offset).CopyTo(_span.Slice(offset + itemCount));
            items.CopyTo(_span.Slice(offset, itemCount));
            _position = newPos;
        }
    }

    public void InsertMany(Index index, params T[]? items)
    {
        if (items is null)
            return;
        int itemCount = items.Length;
        if (itemCount == 0)
            return;
        int offset = index.GetOffset(_position);
        if ((uint)offset > (uint)_position)
            throw new ArgumentOutOfRangeException(nameof(index), index, $"Index must be between 0 and Count ({Count})");
        if (offset == _position)
        {
            AddMany(items);
        }
        else
        {
            int newPos = _position + itemCount;
            if (newPos >= Capacity)
            {
                Grow();
            }

            _span.Slice(offset, _position - offset).CopyTo(_span.Slice(offset + itemCount));
            items.CopyTo(_span.Slice(offset, itemCount));
            _position = newPos;
        }
    }

    public void InsertMany(Index index, ICollection<T>? items)
    {
        if (items is null)
            return;
        int itemCount = items.Count;
        if (itemCount == 0)
            return;
        int offset = index.GetOffset(_position);
        if ((uint)offset > (uint)_position)
            throw new ArgumentOutOfRangeException(nameof(index), index, $"Index must be between 0 and Count ({Count})");
        if (offset == _position)
        {
            AddMany(items);
        }
        else
        {
            int newPos = _position + itemCount;
            if (newPos >= Capacity)
            {
                Grow();
            }

            _span.Slice(offset, _position - offset).CopyTo(_span.Slice(offset + itemCount));
            int i = offset;
            foreach (T item in items)
            {
                _span[i++] = item;
            }

            Debug.Assert(i == (offset + itemCount));
            _position = newPos;
        }
    }

    public bool Contains(T item) => FirstIndexOf(item) >= 0;

    public int FirstIndexOf(T item)
    {
        for (var i = 0; i < _position; i++)
        {
            if (EqualityComparer<T>.Default.Equals(item, _span[i]))
                return i;
        }

        return -1;
    }

    public int FirstIndexOf(T item, int offset, IEqualityComparer<T>? itemComparer = null)
    {
        if (offset < 0 || offset >= _position)
            return -1;
        itemComparer ??= EqualityComparer<T>.Default;

        for (var i = offset; i < _position; i++)
        {
            if (itemComparer.Equals(item, _span[i]))
                return (i - offset);
        }

        return -1;
    }

    // public bool Remove(T item)
    // {
    //     throw new NotImplementedException();
    // }

    public void Clear()
    {
        _position = 0;
    }

    public void Dispose()
    {
        T[]? toReturn = _array;
        this = default; // defensive clear
        if (toReturn is not null && toReturn.Length > 0)
        {
            ArrayPool<T>.Shared.Return(toReturn, true);
        }
    }
    
    public bool TryCopyTo(Span<T> span) => Written.TryCopyTo(span);

    public Span<T> AsSpan() => Written;
    public T[] ToArray() => Written.ToArray();
    
    public Span<T>.Enumerator GetEnumerator()
    {
        return Written.GetEnumerator();
    }

    public override string ToString()
    {
        // Special handling for char
        if (typeof(T) == typeof(char))
        {
            return Written.ToString();
        }

        return StringBuilderPool.Rent().AppendJoin<T>(", ", Written).ToStringAndReturn();
    }
}