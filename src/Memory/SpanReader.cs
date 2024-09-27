namespace ScrubJay.Memory;

/// <summary>
/// A <see cref="SpanReader{T}"/> wraps a <see cref="ReadOnlySpan{T}"/>
/// and provides methods to Peek, Skip, and Take item(s) from it
/// in forward-only reads that 'consume' the span
/// </summary>
/// <typeparam name="T">
/// <see cref="Type"/>s of items stored in the <see cref="ReadOnlySpan{T}"/>
/// </typeparam>
[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public ref struct SpanReader<T>
{
    /// <summary>
    /// Returns a <see cref="SpanReader{T}"/> intended for use as an <see cref="IEnumerator{T}"/> proxy over the given <see cref="ReadOnlySpan{T}"/>
    /// </summary>
    /// <param name="span"></param>
    /// <returns></returns>
    public static SpanReader<T> AsEnumerator(ReadOnlySpan<T> span)
    {
        return new SpanReader<T>(span, -1);
    }
    
    
    private readonly ReadOnlySpan<T> _span;
    private int _position;


    /// <summary>
    /// Gets a <see cref="ReadOnlySpan{T}"/> over the already read items
    /// </summary>
    public ReadOnlySpan<T> ReadSpan => _span.Slice(0, _position);

    /// <summary>
    /// Gets the total number of items that have been read
    /// </summary>
    public int ReadCount => _position;

    /// <summary>
    /// Gets a <see cref="ReadOnlySpan{T}"/> over the items remaining to be read
    /// </summary>
    public ReadOnlySpan<T> RemainingSpan => _span.Slice(_position);

    /// <summary>
    /// Gets the total number of items remaining to be read
    /// </summary>
    public int RemainingCount => _span.Length - _position;

    /// <summary>
    /// Gets the current read position
    /// </summary>
    public int Position
    {
        get => _position;
        // Support for Extensions
        internal set => _position = value;
    }

    private SpanReader(ReadOnlySpan<T> span, int position)
    {
        Debug.Assert(position == -1); // remove if we ever have another use than as an Enumerator proxy
        _span = span;
        _position = position;
    }

    /// <summary>
    /// Create a new <see cref="SpanReader{T}"/> that reads from the given <paramref name="span"/>
    /// </summary>
    /// <param name="span">
    /// The <see cref="ReadOnlySpan{T}"/> to read from
    /// </param>
    public SpanReader(ReadOnlySpan<T> span)
    {
        _span = span;
        _position = 0;
    }

    /// <summary>
    /// Create a new <see cref="SpanReader{T}"/> that reads from the given <paramref name="span"/>
    /// </summary>
    /// <param name="span">
    /// The <see cref="ReadOnlySpan{T}"/> to read from
    /// </param>
    /// <param name="offset">
    /// The <see cref="Index"/> offset to start reading from
    /// </param>
    public SpanReader(ReadOnlySpan<T> span, Index offset)
    {
        _span = span;
        _position = Validate.Index(offset, span.Length).OkOrThrow();
    }

#region Peek

    /// <summary>
    /// Try to peek at the next item
    /// </summary>
    /// <returns>
    /// A <see cref="Option{T}.Some"/> containing the next item<br/>
    /// A <see cref="Option{T}.None"/> if there are no more items
    /// </returns>
    public Option<T> TryPeek()
    {
        int pos = _position;
        var span = _span;
        if ((uint)pos < span.Length)
        {
            return Option<T>.Some(span[pos]);
        }

        return None();
    }

    /// <summary>
    /// Try to peek at the next <paramref name="count"/> items
    /// </summary>
    /// <param name="count">
    /// The number of items to peek at
    /// </param>
    /// <returns>
    /// A <see cref="OptionReadOnlySpan{T}.Some"/> containing the next items<br/>
    /// A <see cref="OptionReadOnlySpan{T}.None"/> if there are not at least <paramref name="count"/> items
    /// </returns>
    public OptionReadOnlySpan<T> TryPeek(int count)
    {
        int pos = _position;
        var span = _span;
        if ((uint)pos + (uint)count <= span.Length)
        {
            return OptionReadOnlySpan<T>.Some(span.Slice(pos, count));
        }
        return OptionReadOnlySpan<T>.None();
    }

    /// <summary>
    /// Peek at the next item
    /// </summary>
    /// <returns>
    /// The next item
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if there are no items left
    /// </exception>
    public T Peek() => TryPeek().SomeOrThrow("Cannot Peek(): No items remain");

    /// <summary>
    /// Peek at the next <paramref name="count"/> items
    /// </summary>
    /// <param name="count">
    /// The number of items to peek at
    /// </param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if there are not at least <paramref name="count"/> items
    /// </exception>
    public ReadOnlySpan<T> Peek(int count) => TryPeek(count).SomeOrThrow($"Cannot Peek({count}): Not enough items remain");

#endregion

#region Take

    /// <summary>
    /// Try to take the next item
    /// </summary>
    /// <returns>
    /// A <see cref="Option{T}.Some"/> containing the next item<br/>
    /// A <see cref="Option{T}.None"/> if there are no more items
    /// </returns>
    public Option<T> TryTake()
    {
        int pos = _position;
        if (pos < _span.Length)
        {
            _position = pos + 1;
            return Some(_span[pos]);
        }

        return None();
    }

    public OptionReadOnlySpan<T> TryTake(int count)
    {
        int pos = _position;
        var span = _span;
        if ((uint)pos + (uint)count <= span.Length)
        {
            _position = pos + count;
            return OptionReadOnlySpan<T>.Some(span.Slice(pos, count));
        }
        return OptionReadOnlySpan<T>.None();
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


    public override string ToString()
    {
        /* We want to show our position in the source span like this:
         * ...,a,b,c ⌖ d,e,f,...
         * For ReadOnlySpan<char>, we have special handling to treat it like text and capture more characters
         */

        string delimiter;
        int captureCount;
        if (typeof(T) == typeof(char))
        {
            delimiter = string.Empty;
            captureCount = 20;
        }
        else
        {
            delimiter = ", ";
            captureCount = 5;
        }

        var text = new DefaultInterpolatedStringHandler(delimiter.Length * captureCount, captureCount);

        int index = _position;
        var span = _span;

        // Previously read items
        int prevIndex = index - captureCount;
        // If we have more before this, indicate with ellipsis
        if (prevIndex > 0)
        {
            text.AppendLiteral("…");
            text.AppendFormatted(delimiter);
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
                text.AppendFormatted(delimiter);
            }

            text.AppendFormatted<T>(span[i]);
        }

        // position indicator
        text.AppendFormatted(" ^ ");

        // items yet to be read
        int nextIndex = index + captureCount;

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
                text.AppendFormatted(delimiter);
            }

            text.AppendFormatted<T>(span[i]);
        }

        if (postpendEllipsis)
        {
            text.AppendFormatted(delimiter);
            text.AppendLiteral("…");
        }

        return text.ToStringAndClear();
    }
}