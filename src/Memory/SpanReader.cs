﻿using System.ComponentModel;
using ScrubJay.Text;

namespace ScrubJay.Memory;

/// <summary>
/// A <see cref="SpanReader{T}"/> wraps a <see cref="ReadOnlySpan{T}"/>
/// and provides methods to Peek, Skip, and Take <typeparamref name="T"/> item(s) from it
/// in forward-only reads that 'consume' the span
/// </summary>
/// <typeparam name="T">
/// <see cref="Type"/>s of items stored in the <see cref="ReadOnlySpan{T}"/>
/// </typeparam>
public ref struct SpanReader<T>
{
    private readonly ReadOnlySpan<T> _span;
    private int _position;

    public ReadOnlySpan<T> ReadSpan => _span.Slice(0, _position);
    public int ReadCount => _position;
    
    public ReadOnlySpan<T> RemainingSpan => _span.Slice(_position);
    public int RemainingCount => _span.Length - _position;

    public int Position
    {
        get => _position;
        internal set => _position = value;
    }

    /// <summary>
    /// Create a new <see cref="SpanReader{T}"/> that reads from the given <paramref name="span"/>
    /// </summary>
    /// <param name="span"></param>
    public SpanReader(ReadOnlySpan<T> span)
    {
        _span = span;
        _position = 0;
    }

    public SpanReader(ReadOnlySpan<T> span, int position)
    {
        if (position < 0 || position >= span.Length)
            throw new ArgumentOutOfRangeException(nameof(position), position, "Position must be within the span");
        _span = span;
        _position = position;
    }

#region Peek
    /// <summary>
    /// Tries to peek at the next item
    /// </summary>
    /// <returns></returns>
    public Result<T, Exception> TryPeek()
    {
        int pos = _position;
        var span = _span;
        if (pos < span.Length)
        {
            return span[pos];
        }
        return new InvalidOperationException("Cannot Peek: No items remain");
    }

    public Result<int, Exception> TryPeek([MaybeNullWhen(false)] out T item)
    {
        int pos = _position;
        var span = _span;
        if (pos < span.Length)
        {
            item = span[pos];
            return pos;
        }

        item = default!;
        return new InvalidOperationException("Cannot Peek: No items remain");
    }

    public T Peek() => TryPeek().Unwrap();

    /// <summary>
    /// Try to peek at the next <paramref name="count"/> <paramref name="items"/>
    /// </summary>
    /// <param name="count"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public Result<Range, Exception> TryPeek(int count, out ReadOnlySpan<T> items)
    {
        if (count < 0)
        {
            items = default;
            return new ArgumentOutOfRangeException(nameof(count), count, "Count must be zero or greater");
        }
        int pos = _position;
        var span = _span;
        if (pos + count <= span.Length)
        {
            items = span.Slice(pos, count);
            return new Range(pos, pos + count);
        }
        items = default;
        return new InvalidOperationException($"Cannot Peek({count}): Only {RemainingCount} items remain");
    }
    
    public ReadOnlySpan<T> Peek(int count)
    {
        TryPeek(count, out var values).Unwrap();
        return values;
    }
#endregion

#region Skip
    public Result<int, Exception> TrySkip()
    {
        int index = _position;
        if (index < _span.Length)
        {
            _position = index + 1;
            return index;
        }
        return new InvalidOperationException("Cannot Skip: No items remain");
    }
    
    public void Skip() => TrySkip().Unwrap();

    public Result<Range, Exception> TrySkip(int count)
    {
        if (count <= 0)
            return default;

        int index = _position;
        int newIndex = index + count;
        if (newIndex <=  _span.Length)
        {
            _position = newIndex;
            return new Range(index, newIndex);
        }
        return new InvalidOperationException($"Cannot Skip({count}): Only {RemainingCount} items remain");
    }

    public void Skip(int count) => TrySkip(count).Unwrap();

    
    public void SkipWhile(Func<T, bool> itemPredicate)
    {
        var span = _span;
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

    public void SkipAny(params T[] matches) => SkipWhile(item => matches.Contains(item));

    public void SkipAny(IReadOnlyCollection<T> matches) => SkipWhile(item => matches.Contains(item));

    public void SkipAll() => SkipWhile(static _ => true);
#endregion

#region Take

    public Result<T, Exception> TryTake()
    {
        int index = _position;
        var span = _span;
        if (index < span.Length)
        {
            _position = index + 1;
            return span[index];
        }
        return new InvalidOperationException("Cannot Take: No items remain");
    }
    
    
    public Result<int, Exception> TryTake([MaybeNullWhen(false)] out T taken)
    {
        int index = _position;
        var span = _span;
        if (index < span.Length)
        {
            _position = index + 1;
            taken = span[index];
            return index;
        }

        taken = default!;
        return new InvalidOperationException("Cannot Take: No items remain");
    }
    
    public T Take() => TryTake().Unwrap();

    public Result<Range, Exception> TryTake(int count, out ReadOnlySpan<T> taken)
    {
        if (count <= 0)
        {
            taken = default;
            return default;
        }

        int index = _position;
        int newIndex = index + count;
        var span = _span;
        if (newIndex <= span.Length)
        {
            _position = newIndex;
            taken = span.Slice(index, count);
            return new Range(index, newIndex);
        }

        taken = default;
        return new InvalidOperationException($"Cannot Take({count}): Only {RemainingCount} items remain");
    }
    
    public ReadOnlySpan<T> Take(int count)
    {
        TryTake(count, out var values).Unwrap();
        return values;
    }

    public Result<Range, Exception> TryTakeInto(Span<T> buffer)
    {
        var result = TryTake(buffer.Length, out var taken);
        if (!result.IsOk(out var range))
            return result;
        
        taken.CopyTo(buffer);
        return range;
    }

    public void TakeInto(Span<T> buffer) => TryTakeInto(buffer).Unwrap();

    public ReadOnlySpan<T> TakeWhile(Func<T, bool> itemPredicate)
    {
        var span = _span;
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

    public ReadOnlySpan<T> TakeAny(params T[] matches) => TakeWhile(item => matches.Contains(item));

    public ReadOnlySpan<T> TakeAny(IReadOnlyCollection<T> matches) => TakeWhile(item => matches.Contains(item));

    public ReadOnlySpan<T> TakeAll() => TakeWhile(static _ => true);
#endregion

#region IEnumerator Support
    [EditorBrowsable(EditorBrowsableState.Never)]
    public T Current => _span[_position];

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool MoveNext() => TrySkip();
#endregion

    public override string ToString()
    {
        /* We want to show our position in the source span like this:
         * ...,a,b,c ⌖ d,e,f,...
         * For ReadOnlySpan<char>, we have special handling to treat it like text and capture more characters
         */

        string delimiter;
        int capture;
        if (typeof(T) == typeof(char))
        {
            delimiter = string.Empty;
            capture = 16;
        }
        else
        {
            delimiter = ",";
            capture = 4;
        }

        var text = StringBuilderPool.Rent();

        int index = _position;
        var span = _span;

        // Previously read items
        int prevIndex = index - capture;
        // If we have more before this, indicate with ellipsis
        if (prevIndex > 0)
        {
            text.Append('…').Append(delimiter);
        }
        // Otherwise, cap at a min zero
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
            text.Append(span[i]);
        }

        // position indicator
        text.Append(" ⌖ ");

        // items yet to be read
        int nextIndex = index + capture;

        // if we have more after, we're going to end with an ellipsis
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
            text.Append(span[i]);
        }

        if (postpendEllipsis)
        {
            text.Append(delimiter)
                .Append('…');
        }

        return text.ToStringAndReturn();
    }
}