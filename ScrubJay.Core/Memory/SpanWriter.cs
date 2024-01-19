using ScrubJay.Text;

namespace ScrubJay.Memory;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public ref struct SpanWriter<T>
{
    public static implicit operator SpanWriter<T>(Span<T> span) => new(span);
    
    private readonly Span<T> _span;
    private int _position;

    public int Position
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _position;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _position = value.Clamp(0, Capacity);
    }
    
    public int Capacity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span.Length;
    }
    
    public Span<T> WrittenItems
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span[.._position];
    }

    public Span<T> AvailableItems
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span[_position..];
    }

    public SpanWriter(Span<T> span)
    {
        _span = span;
        _position = 0;
    }

    public Result<int> TryWrite(T item)
    {
        int index = _position;
        var span = _span;
        if (index < span.Length)
        {
            span[index] = item;
            _position = index + 1;
            return Ok(1);
        }
        return new InvalidOperationException("Cannot write an item: No remaining capacity");
    }
    
    public Result<int> TryWriteMany(params T[]? items)
    {
        if (items is null) 
            return new ArgumentNullException(nameof(items));

        var avail = this.AvailableItems;
        var count = items.Length;
        if (count <= avail.Length)
        {
            items.CopyTo(avail);
            _position += count;
            return Ok(count);
        }
        return new InvalidOperationException($"Cannot write {count} items: Only a capacity of {avail.Length} remains");
    }
    
    public Result<int> TryWriteMany(scoped ReadOnlySpan<T> items)
    {
        var avail = this.AvailableItems;
        var count = items.Length;
        if (count <= avail.Length)
        {
            items.CopyTo(avail);
            _position += count;
            return Ok(count);
        }
        return new InvalidOperationException($"Cannot write {count} items: Only a capacity of {avail.Length} remains");
    }
    
    public Result<int> TryWriteMany(IEnumerable<T> items)
    {
        var avail = AvailableItems;
        int i = 0;
        foreach (var item in items)
        {
            if (i >= avail.Length)
            {
                avail.Clear();
                return new InvalidOperationException($"Cannot write {i + 1} items: Only a capacity of {avail.Length} remains");
            }
            avail[i] = item;
            i++;
        }

        _position += i;
        return Ok(i);
    }
    
    public void Write(T item) => TryWrite(item).ThrowIfError();

    public void WriteMany(params T[]? items) => TryWriteMany(items).ThrowIfError();
    
    public void WriteMany(scoped ReadOnlySpan<T> items) => TryWriteMany(items).ThrowIfError();
    
    public Result TryAllocate(int count, out Span<T> allocated)
    {
        var remaining = this.AvailableItems;
        if ((uint)count <= remaining.Length)
        {
            allocated = remaining[..count];
            _position += count;
            return Ok();
        }
        allocated = default;
        return new InvalidOperationException($"Cannot allocate {count} items: Only a capacity of {remaining.Length} remains");
    }

    public override string ToString()
    {
        var written = this.WrittenItems;
        var writtenCount = written.Length;
        if (writtenCount == 0)
            return string.Empty;
        
        var text = StringBuilderPool.Rent();
        
        // We do not want to delimit Span<char>
        var delimiter = typeof(T) == typeof(char) ? "" : ",";

        // our length is > 0
        text.Append(written[0]);
        for (var i = 1; i < writtenCount; i++)
        {
            text.Append(delimiter)
                .Append(written[i]);
        }

        return text.ToStringAndReturn();
    }
}