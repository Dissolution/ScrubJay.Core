using System.Buffers;
using ScrubJay.Text;

namespace ScrubJay.Buffers;

public delegate int UseAvailable<T>(Span<T> emptyBuffer);

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// Heavily inspired by <see cref="T:System.Collections.Generic.ValueListBuilder{T}"/><br/>
/// <a href="https://github.com/dotnet/runtime/blob/a9ed4168626c14b4d74db0d8c205c69e56fc45ed/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/ValueListBuilder.cs"/>
/// </remarks> 
[MustDisposeResource]
public ref struct Buffer<T> // : IDisposable
{
    private Span<T> _span;
    private T[]? _array;
    private int _position;

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
    
    public ref struct BufferEnumerator // : IEnumerator<T>, IEnumerator, IDisposable
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
    }
}
