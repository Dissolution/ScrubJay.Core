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
[StructLayout(LayoutKind.Auto)]
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
    public Option<T> TryPeek()
    {
        if (_position < _span.Length)
            return Some(_span[_position]);
        return None<T>();
    }
    
    /// <summary>
    /// Try to peek at the next <paramref name="count"/> items
    /// </summary>
    /// <param name="count"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public OptionReadOnlySpan<T> TryPeek(int count)
    {
        if (count < 0)
            return default;

        int pos = _position;
        var span = _span;
        if (pos + count <= span.Length)
        {
            return OptionReadOnlySpan<T>.Some(span.Slice(pos, count));
        }

        return default;
    }

    public T Peek() => TryPeek().SomeOrThrow($"There was not an item to Peek");
    
    public ReadOnlySpan<T> Peek(int count) => TryPeek(count).SomeOrThrow($"There were not {count} items to Peek");

#endregion

#region Take

    public Option<T> TryTake()
    {
        int index = _position;
        var span = _span;
        if (index < span.Length)
        {
            _position = index + 1;
            return Some(span[index]);
        }

        return None<T>();
    }
    
    public OptionReadOnlySpan<T> TryTake(int count)
    {
        if (count < 0)
            return default;

        int pos = _position;
        int newPos = pos + count;
        var span = _span;
        if (newPos <= span.Length)
        {
            var taken = span.Slice(pos, count);
            _position = newPos;
            return OptionReadOnlySpan<T>.Some(taken);
        }

        return default;
    }
    
    public T Take() => TryTake().SomeOrThrow($"There was not an item to Take");
    
    public ReadOnlySpan<T> Take(int count) => TryTake(count).SomeOrThrow($"There were not {count} items to Take");
    

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

    public ReadOnlySpan<T> TakeWhileAny(params T[] matches) => TakeWhile(item => matches.Contains(item));

    public ReadOnlySpan<T> TakeWhileAny(IReadOnlyCollection<T> matches) => TakeWhile(item => matches.Contains(item));

    public ReadOnlySpan<T> TakeAll() => TakeWhile(static _ => true);

#endregion


#region Skip

    public bool TrySkip() => TryTake();
    
    public bool TrySkip(int count) => TryTake(count);

    public void Skip() => TryTake().SomeOrThrow($"There was not an item to Skip");
    
    public void Skip(int count) => TryTake(count).SomeOrThrow($"There were not ${count} items to Skip");

    public void SkipWhile(Func<T, bool> itemPredicate)
    {
        var span = _span;
        int index = _position;
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
            text.Append(delimiter).Append('…');
        }

        return text.ToStringAndReturn();
    }
}