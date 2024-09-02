using System.Buffers;
using ScrubJay.Text;
using ScrubJay.Utilities;
using ScrubJay.Validation;

#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member

namespace ScrubJay.Buffers;

public delegate int UseAvailable<T>(Span<T> emptyBuffer);

/// <summary>
/// 
/// </summary>
/// <typeparam name="T">The <see cref="Type"/> of items stored in this <see cref="Buffer{T}"/></typeparam>
/// <remarks>
/// Heavily inspired by <see cref="T:System.Collections.Generic.ValueListBuilder{T}"/><br/>
/// <a href="https://github.com/dotnet/runtime/blob/release/8.0/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/ValueListBuilder.cs"/>
/// </remarks>
[PublicAPI]
[MustDisposeResource]
public ref struct Buffer<T> // : IDisposable
{
    private Span<T> _span;
    private T[]? _array;
    private int _position;

    internal Span<T> Written
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span.Slice(0, _position);
    }

    internal Span<T> Available
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span.Slice(_position);
    }

    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _span[index];
    }

    public ref T this[Index index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _span[index.GetOffset(_position)];
    }

    public Span<T> this[Range range]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            (int offset, int length) = range.GetOffsetAndLength(_position);
            return _span.Slice(offset, length);
        }
    }

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _position;
    }

    public int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span.Length;
    }


    internal Buffer(Span<T> initialBuffer, int initialPosition)
    {
        _span = initialBuffer;
        _array = null;
        Debug.Assert(initialPosition >= 0 && initialPosition < _span.Length);
        _position = initialPosition;
    }

    public Buffer()
    {
        _span = _array = null;
        _position = 0;
    }

    public Buffer(int minCapacity)
    {
        if (minCapacity <= 0)
        {
            _span = _array = null;
        }
        else
        {
            _span = _array = ArrayPool.Rent<T>(minCapacity);
        }

        _position = 0;
    }

    public Buffer(Span<T> initialBuffer)
    {
        _span = initialBuffer;
        _array = null;
        _position = 0;
    }

    private void GrowBy(int adding)
    {
        T[] newArray = ArrayPool.Rent<T>((_span.Length + adding) * 2);
        _span.Slice(0, _position).CopyTo(newArray);

        T[]? toReturn = _array;
        _span = _array = newArray;
        ArrayPool.Return(toReturn);
    }


    [MethodImpl(MethodImplOptions.NoInlining)]
    private void AppendGrow(T item)
    {
        Debug.Assert(_position == _span.Length);
        int pos = _position;
        GrowBy(1);
        _span[pos] = item;
        _position = pos + 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(T item)
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
            AppendGrow(item);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void AppendManyGrow(scoped ReadOnlySpan<T> source)
    {
        Debug.Assert(source.Length > 0);

        if ((_position + source.Length) > _span.Length)
        {
            GrowBy(source.Length);
        }

        source.CopyTo(_span.Slice(_position));
        _position += source.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendMany(scoped ReadOnlySpan<T> items)
    {
        int pos = _position;
        Span<T> span = _span;

        // Fast check for zero items
        if (items.Length == 0)
            return;

        // Fast check for only one item
        if (items.Length == 1 && pos < span.Length)
        {
            span[pos] = items[0];
            _position = pos + 1;
        }
        else
        {
            AppendManyGrow(items);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendMany(params T[] items) => AppendMany((ReadOnlySpan<T>)items);

    public void AppendMany(IEnumerable<T>? items)
    {
        if (items is null)
            return;
        foreach (T item in items)
        {
            Append(item);
        }
    }

    public void Prepend(T item)
    {
        int pos = _position;

        if (pos >= _span.Length)
        {
            GrowBy(1);
        }

        _span.Slice(0, pos).CopyTo(_span.Slice(1));
        _span[0] = item;
        _position = pos + 1;
    }

    public void PrependMany(scoped ReadOnlySpan<T> items)
    {
        int pos = _position;
        int itemCount = items.Length;

        if ((pos + itemCount) > _span.Length)
        {
            GrowBy(itemCount);
        }

        _span.Slice(0, pos).CopyTo(_span.Slice(itemCount));
        items.CopyTo(_span);
        _position = pos + itemCount;
    }

    public void Insert(Index index, T item)
    {
        int offset = Validate.InsertIndex(index, _position).OkOrThrow();
        if (offset == _position)
        {
            Append(item);
        }
        else
        {
            int newPos = _position + 1;
            if (newPos >= Capacity)
            {
                GrowBy(1);
            }

            _span.Slice(offset, _position - offset).CopyTo(_span.Slice(offset + 1));
            _span[offset] = item;
            _position = newPos;
        }
    }

    public void InsertMany(Index index, scoped ReadOnlySpan<T> items)
    {
        int itemCount = items.Length;
        if (itemCount == 0)
            return;
        int offset = Validate.InsertIndex(index, _position).OkOrThrow();
        if (offset == _position)
        {
            AppendManyGrow(items);
        }
        else
        {
            int newPos = _position + itemCount;
            if (newPos >= Capacity)
            {
                GrowBy(itemCount);
            }

            Span.Copy(_span.Slice(offset, _position - offset), _span.Slice(offset + itemCount));
            Span.Copy(items, _span.Slice(offset, itemCount));
            _position = newPos;
        }
    }

    public void InsertMany(Index index, params T[]? items) => InsertMany(index, items.AsSpan());
    
    public void InsertMany(Index index, IEnumerable<T>? items)
    {
        if (items is null)
            return;

        int offset = Validate.InsertIndex(index, _position).OkOrThrow();
        if (offset == _position)
        {
            AppendMany(items);
            return;
        }
        
        // ReSharper disable PossibleMultipleEnumeration
        if (items.TryGetNonEnumeratedCount(out int count))
        {
            if (count == 0)
                return;
            
            int newPos = _position + count;
            if (newPos >= Capacity)
            {
                GrowBy(count);
            }

            Span.Copy(_span.Slice(offset, _position - offset), _span.Slice(offset + count));
            int i = offset;
            foreach (T item in items)
            {
                _span[i++] = item;
            }

            Debug.Assert(i == (offset + count));
            _position = newPos;
        }
        else
        {
            int i = offset;
            foreach (T item in items)
            {
                Insert(i, item);
                i++;
            }
        }
        // ReSharper enable PossibleMultipleEnumeration
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


    public bool RemoveAt(Index index)
    {
        if (!Validate.Index(index, _position).IsOk(out var offset)) return false;
        Span.SelfCopy(this.Written, (offset+1).., offset..);
        return true;
    }

    public bool RemoveMany(Range range)
    {
        if (!Validate.Range(range, _position).IsOk(out var ol))
            return false;
        (int offset, int length) = ol;
        Span.SelfCopy(this.Written, (offset+length).., offset..);
        return true;
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
        return _span.Slice(pos, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> Allocate(int length)
    {
        if (length <= 0)
            return Span<T>.Empty;

        int pos = _position;
        Span<T> span = _span;
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
        var available = _span.Slice(_position);
        int used = useAvailable(available);
        if (used < 0 || used > available.Length)
            throw new InvalidOperationException("Cannot use an invalid number of available items");
        _position += used;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Pop()
    {
        int newPos = _position - 1;
        if (newPos < 0)
            throw new InvalidOperationException("Cannot Pop: No Items");
        _position = newPos;
        return _span[newPos];
    }


    public bool TryCopyTo(Span<T> destination, out int itemsWritten)
    {
        if (_span.Slice(0, _position).TryCopyTo(destination))
        {
            itemsWritten = _position;
            return true;
        }

        itemsWritten = 0;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan()
    {
        return _span.Slice(0, _position);
    }

    [HandlesResourceDisposal]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        T[]? toReturn = _array;
        if (toReturn != null)
        {
            _array = null;
            ArrayPool<T>.Shared.Return(toReturn);
        }
    }

    [Obsolete("Buffer is a ref struct and not meant to be compared", true)]
    public override bool Equals(object? _) => throw new NotSupportedException();

    [Obsolete("Buffer is a ref struct and not meant to be compared", true)]
    public override int GetHashCode() => throw new NotSupportedException();

    public override string ToString()
    {
        using InterpolatedStringHandler text = new();
        var pos = _position;
        var span = _span;

        text.AppendLiteral('(');
        if (pos > 0)
        {
            text.AppendFormatted<T>(span[0]);
            for (var i = 1; i < pos; i++)
            {
                text.AppendLiteral(", ");
                text.AppendFormatted<T>(span[i]);
            }
        }

        text.AppendLiteral(')');
        return text.ToString();
    }

    public BufferEnumerator GetEnumerator() => new(this);

    public ref struct BufferEnumerator
    {
        private readonly ReadOnlySpan<T> _items;
        private int _index;

        public readonly ref readonly T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _items[_index];
        }

        public int Index
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal BufferEnumerator(Buffer<T> buffer)
        {
            _items = buffer.AsSpan();
            _index = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            int newIndex = _index + 1;
            if (newIndex >= _items.Length)
                return false;
            _index = newIndex;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Option<T> TryMoveNext()
        {
            int newIndex = _index + 1;
            if (newIndex >= _items.Length)
                return None;
            _index = newIndex;
            return Some(_items[newIndex]);
        }
    }
}