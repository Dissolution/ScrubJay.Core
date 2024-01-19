using System.ComponentModel;
using ScrubJay.Text;

namespace ScrubJay.Memory;

/// <summary>
/// A <see cref="ReadOnlySpanEnumerator{T}"/> wraps a <see cref="ReadOnlySpan{T}"/>,
/// enumerating it with <c>LINQ</c>-like <see cref="Peek()"/>, <see cref="Skip()"/>, and <see cref="Take()"/> operations
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of items stored in the <see cref="ReadOnlySpan{T}"/>
/// </typeparam>
public ref struct ReadOnlySpanEnumerator<T> // : IEnumerator<T>
{
    private readonly ReadOnlySpan<T> _readOnlySpan;
    private int _position;

    /// <summary>
    /// Gets a <c>ref readonly</c> to the item at the given <paramref name="index"/>
    /// </summary>
    public ref readonly T this[int index] => ref _readOnlySpan[index];
    /// <summary>
    /// Gets a <c>ref readonly</c> to the item at the given <paramref name="index"/>
    /// </summary>
    public ref readonly T this[Index index] => ref _readOnlySpan[index];
    /// <summary>
    /// Gets a <see cref="ReadOnlySpan{T}"/> of the items at the given <paramref name="range"/>
    /// </summary>
    public ReadOnlySpan<T> this[Range range] => _readOnlySpan[range];

    /// <summary>
    /// The wrapped <see cref="ReadOnlySpan{T}"/>
    /// </summary>
    public ReadOnlySpan<T> ReadOnlySpan
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _readOnlySpan;
    }

    /// <summary>
    /// Gets the total number of values that have been read
    /// </summary>
    public int EnumeratedCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _position;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal set => _position = value;
    }

    /// <summary>
    /// Gets the <see cref="ReadOnlySpan{T}"/> of items that have already been read
    /// </summary>
    public ReadOnlySpan<T> EnumeratedItems
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _readOnlySpan[.._position];
    }

    /// <summary>
    /// Gets the total number of values that can yet be read
    /// </summary>
    public int UnenumeratedCount
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _readOnlySpan.Length - _position;
    }

    /// <summary>
    /// Gets the <see cref="ReadOnlySpan{T}"/> of items that have not yet been read
    /// </summary>
    public ReadOnlySpan<T> UnenumeratedItems
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _readOnlySpan[_position..];
    }

    
    /// <summary>
    /// Create a new <see cref="ReadOnlySpanEnumerator{T}"/> that enumerates the given <see cref="ReadOnlySpan{T}"/>
    /// </summary>
    public ReadOnlySpanEnumerator(ReadOnlySpan<T> readOnlySpan)
    {
        _readOnlySpan = readOnlySpan;
        _position = 0;
    }

#region Peek
    /// <summary>
    /// Tries to peek at the next <paramref name="item"/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public Result TryPeek([MaybeNullWhen(false)] out T item)
    {
        int pos = _position;
        var span = _readOnlySpan;
        if (pos < span.Length)
        {
            item = span[pos];
            return Ok();
        }
        item = default;
        return new InvalidOperationException("Cannot peek an item: Zero items remain");
    }

    public T Peek()
    {
        TryPeek(out var item).ThrowIfError();
        return item!;
    }
    
    /// <summary>
    /// Try to peek at the next <paramref name="count"/> <paramref name="items"/>
    /// </summary>
    /// <param name="count"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public Result TryPeek(int count, out ReadOnlySpan<T> items)
    {
        if (count < 0)
        {
            items = default;
            return new ArgumentOutOfRangeException(nameof(count), count, "Count must be zero or greater");
        }
        int pos = _position;
        var span = _readOnlySpan;
        if (pos + count <= span.Length)
        {
            items = span.Slice(pos, count);
            return Ok();
        }
        items = default;
        return new InvalidOperationException($"Cannot peek {count} items: Only {UnenumeratedCount} items remain");
    }
    
    public ReadOnlySpan<T> Peek(int count)
    {
        TryPeek(count, out var values).ThrowIfError();
        return values;
    }
#endregion

#region Skip
    public Result TrySkip()
    {
        int index = _position;
        if (index < _readOnlySpan.Length)
        {
            _position = index + 1;
            return Ok();
        }
        return new InvalidOperationException("Cannot skip an item: No items remain");
    }
    
    public void Skip() => TrySkip().ThrowIfError();

    public Result TrySkip(int count)
    {
        if (count <= 0)
            return Ok();

        int index = _position;
        int newIndex = index + count;
        if (newIndex <=  _readOnlySpan.Length)
        {
            _position = newIndex;
            return Ok();
        }
        return new InvalidOperationException($"Cannot skip {count} items: Only {UnenumeratedCount} items remain");
    }
    
    public void Skip(int count) => TrySkip(count).ThrowIfError();

    
    public void SkipWhile(Func<T, bool> itemPredicate)
    {
        var span = _readOnlySpan;
        int start = _position;
        int index = start;
        int len = span.Length;
        while (index < len && itemPredicate(span[index]))
        {
            index += 1;
        }
        _position = index;
    }

    public void SkipWhile(T match) => SkipWhile(item => EqualityComparer<T>.Default.Equals(item, match));

    public void SkipUntil(Func<T, bool> itemPredicate) => SkipWhile(item => !itemPredicate(item));

    public void SkipUntil(T match) => SkipWhile(item => !EqualityComparer<T>.Default.Equals(item, match));

    public void SkipAny(params T[] matches) => SkipWhile(item => Enumerable.Contains(matches, item));

    public void SkipAny(IReadOnlyCollection<T> matches) => SkipWhile(item => matches.Contains(item));

    public void SkipAll() => SkipWhile(static _ => true);
#endregion

#region Take
    public Result TryTake([MaybeNullWhen(false)] out T taken)
    {
        int index = _position;
        var span = _readOnlySpan;
        if (index < span.Length)
        {
            _position = index + 1;
            taken = span[index];
            return Ok();
        }

        taken = default;
        return new InvalidOperationException("Cannot take an item: No items remain");
    }

    public T Take()
    {
        TryTake(out var item).ThrowIfError();
        return item!;
    }

    public Result TryTake(int count, out ReadOnlySpan<T> taken)
    {
        if (count <= 0)
        {
            taken = default;
            return Ok();
        }

        int index = _position;
        int newIndex = index + count;
        var span = _readOnlySpan;
        if (newIndex <= span.Length)
        {
            _position = newIndex;
            taken = span.Slice(index, count);
            return Ok();
        }

        taken = default;
        return new InvalidOperationException($"Cannot take {count} items: Only {UnenumeratedCount} items remain");
    }
    
    public ReadOnlySpan<T> Take(int count)
    {
        var result = TryTake(count, out var values);
        result.ThrowIfError();
        return values;
    }

    public Result TryTakeInto(Span<T> buffer)
    {
        if (TryTake(buffer.Length, out var taken))
        {
            taken.CopyTo(buffer);
            return Ok();
        }
        return Error(null);
    }

    public void TakeInto(Span<T> buffer) => TryTakeInto(buffer).ThrowIfError();

    public ReadOnlySpan<T> TakeWhile(Func<T, bool> itemPredicate)
    {
        var span = _readOnlySpan;
        int start = _position;
        int index = start;
        int len = span.Length;
        while (index < len && itemPredicate(span[index]))
        {
            index += 1;
        }
        _position = index;
        return span.Slice(start, index - start);
    }

    public ReadOnlySpan<T> TakeWhile(T match) => TakeWhile(item => EqualityComparer<T>.Default.Equals(item, match));

    public ReadOnlySpan<T> TakeUntil(Func<T, bool> itemPredicate) => TakeWhile(item => !itemPredicate(item));

    public ReadOnlySpan<T> TakeUntil(T match) => TakeWhile(item => !EqualityComparer<T>.Default.Equals(item, match));

    public ReadOnlySpan<T> TakeAny(T[] matches) => TakeWhile(item => Enumerable.Contains(matches, item));

    public ReadOnlySpan<T> TakeAny(IReadOnlyCollection<T> matches) => TakeWhile(item => matches.Contains(item));

    public ReadOnlySpan<T> TakeAll() => TakeWhile(static _ => true);
#endregion

#region IEnumerator Support
    /// <summary>
    /// Gets the current index of enumeration
    /// </summary>
    public int Index => _position;
    
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ref readonly T Current => ref _readOnlySpan[_position];

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool MoveNext() => TrySkip();
#endregion

    public override string ToString()
    {
        /* We want to show our position in the source span like this:
         * (...,)a,b,c_⌖_d,e,f(,...)
         */

        string delimiter;
        int capture;
        if (typeof(T) == typeof(char))
        {
            // For ReadOnlySpan<char> (text), we do not include a delimiter and we capture more characters on either side
            delimiter = string.Empty;
            capture = 16;
        }
        else
        {
            // Otherwise, comma delimit and grab 4
            delimiter = ",";
            capture = 4;
        }

        var text = StringBuilderPool.Rent();

        int index = _position;
        var span = _readOnlySpan;

        // Previously read items
        int prevIndex = index - capture;
        // If we have more before this, indicate with (...,)
        if (prevIndex > 0)
        {
            text.Append('…').Append(delimiter);
        }
        // Otherwise, limit to start index 0
        else
        {
            prevIndex = 0;
        }
        
        for (var i = prevIndex; i < index; i++)
        {
            if (i > prevIndex)
            {
                text.Append(delimiter);
            }
            text.Append<T>(span[i]);
        }

        // position indicator
        text.Append(" ⌖ ");

        // items yet to be read
        int nextIndex = index + capture;

        // if we have more after, we're going to end with an (,...)
        bool postpendEllipsis;
        // but we also need to cap at capacity
        if (nextIndex < span.Length)
        {
            postpendEllipsis = true;
        }
        else
        {
            postpendEllipsis = false;
            nextIndex = span.Length;
        }

        for (var i = index; i < nextIndex; i++)
        {
            if (i > index)
            {
                text.Append(delimiter);
            }
            text.Append<T>(span[i]);
        }

        if (postpendEllipsis)
        {
            text.Append(delimiter)
                .Append('…');
        }

        return text.ToStringAndReturn();
    }
}