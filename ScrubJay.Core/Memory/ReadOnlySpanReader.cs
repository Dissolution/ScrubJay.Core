using ScrubJay.Text;

namespace ScrubJay.Memory;

/// <summary>
/// A <see cref="ReadOnlySpanReader{T}"/> reads <typeparamref name="T"/> items from a source <see cref="ReadOnlySpan{T}"/>
/// with <c>LINQ</c>-like <see cref="Peek()"/>, <see cref="Skip()"/>, and <see cref="Take()"/> operations
/// </summary>
/// <typeparam name="T">
/// The <see cref="Type"/> of items stored in the <see cref="ReadOnlySpan{T}"/>
/// </typeparam>
public ref struct ReadOnlySpanReader<T>
{
    private readonly ReadOnlySpan<T> _span;
    private int _position;

    /// <summary>
    /// Gets the number of items remaining to be read
    /// </summary>
    public int RemainingCount => _span.Length - _position;

    /// <summary>
    /// Gets a <see cref="ReadOnlySpan{T}"/> of the items remaining to be read
    /// </summary>
    public ReadOnlySpan<T> RemainingSpan => _span[_position..];

    public ReadOnlySpanReader(ReadOnlySpan<T> span)
    {
        _span = span;
        _position = 0;
    }

#region Peek

    /// <summary>
    /// Try to peek at the next <paramref name="item"/> to be read and return whether it exists
    /// </summary>
    /// <param name="item">
    /// If there is another item to be read: That <typeparamref name="T"/><br/>
    /// If not, <c>default(</c><typeparamref name="T"/><c>)</c>
    /// </param>
    /// <returns>
    /// <c>true</c> if there is another item to be read<br/>
    /// <c>false</c> if there is not
    /// </returns>
    public Result TryPeek([MaybeNullWhen(false)] out T item)
    {
        int pos = _position;
        var span = _span;
        if (pos < span.Length)
        {
            item = span[pos];
            return Ok();
        }

        item = default;
        return new InvalidOperationException("Cannot Peek for an item: Remaining Count is 0");
    }

    public T Peek()
    {
        TryPeek(out var item).ThrowIfError();
        return item!;
    }

    public Result TryPeek(int count, out ReadOnlySpan<T> items)
    {
        if (count < 0)
        {
            items = default;
            return new ArgumentOutOfRangeException(nameof(count), count, $"{nameof(count)} must be 0 or greater");
        }

        int pos = _position;
        var span = _span;
        if (pos + count <= span.Length)
        {
            items = span.Slice(pos, count);
            return Ok();
        }

        items = default;
        return new InvalidOperationException($"Cannot Peek for {count} items: Remaining Count is {RemainingCount}");
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
        if (index < _span.Length)
        {
            _position = index + 1;
            return Ok();
        }

        return new InvalidOperationException("Cannot Skip an item: Remaining Count is 0");
    }

    public void Skip() => TrySkip().ThrowIfError();

    public Result TrySkip(int count)
    {
        if (count <= 0)
            return Ok();

        int pos = _position;
        int newPos = pos + count;
        if (newPos <= _span.Length)
        {
            _position = newPos;
            return Ok();
        }

        return new InvalidOperationException($"Cannot Skip {count} items: Remaining Count is {RemainingCount}");
    }

    public void Skip(int count) => TrySkip(count).ThrowIfError();

    // ------------------------------------------------------------------------------------

    public void SkipUntil(Func<T, bool> itemPredicate)
    {
        var span = _span;
        int index = _position;
        int len = span.Length;
        while (index < len && !itemPredicate(span[index]))
        {
            index += 1;
        }

        _position = index;
    }


    public void SkipUntil(T match) => SkipUntil(item => EqualityComparer<T>.Default.Equals(item, match));


    // ------------------------------------------------------------------------------------

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

    public void SkipWhile(ReadOnlySpanFunc<T, int> slicePredicate)
    {
        var span = _span;
        int index = _position;
        int len = span.Length;
        int consumed;
        while (index < len && (consumed = slicePredicate(span[index..])) > 0)
        {
            index += consumed;
        }

        _position = index;
    }

    public void SkipWhile(T match) => SkipWhile(item => EqualityComparer<T>.Default.Equals(item, match));

    public void SkipWhile(ReadOnlySpan<T> match)
    {
        var span = _span;
        int index = _position;
        int len = span.Length;
        while (index < len && span[index..].StartsWith(match))
        {
            index += match.Length;
        }

        _position = index;
    }

    public void SkipWhileAny(params T[] matches) => SkipWhile(item => matches.Contains(item));

    public void SkipWhileAny(ReadOnlySpan<T> matches)
    {
        var span = _span;
        int index = _position;
        int len = span.Length;
        while (index < len && matches.Contains(span[index]))
        {
            index += 1;
        }

        _position = index;
    }

    public void SkipWhileAny(ICollection<T> matches) => SkipWhile(item => matches.Contains(item));

    /// <summary>
    /// Skip all the remaining items to be read
    /// </summary>
    public void SkipAll() => SkipWhile(static _ => true);

#endregion

#region Take

    public Result TryTake([MaybeNullWhen(false)] out T taken)
    {
        int index = _position;
        var span = _span;
        if (index < span.Length)
        {
            _position = index + 1;
            taken = span[index];
            return Ok();
        }

        taken = default;
        return new InvalidOperationException("Cannot Take an item: Remaining Count is 0");
    }

    public Result<T> TryTake()
    {
        int index = _position;
        var span = _span;
        if (index < span.Length)
        {
            _position = index + 1;
            return Ok(span[index]);
        }

        return new InvalidOperationException("Cannot Take an item: Remaining Count is 0");
    }

    public T Take() => TryTake().OkValueOrThrowError();

    public Result TryTake(int count, out ReadOnlySpan<T> taken)
    {
        if (count <= 0)
        {
            taken = default;
            return Ok();
        }

        int pos = _position;
        int newPos = pos + count;
        var span = _span;
        if (newPos <= span.Length)
        {
            _position = newPos;
            taken = span.Slice(pos, count);
            return Ok();
        }

        taken = default;
        return new InvalidOperationException($"Cannot Take {count} items: Remaining Count is {RemainingCount}");
    }

    public ReadOnlySpan<T> Take(int count)
    {
        TryTake(count, out var values).ThrowIfError();
        return values;
    }

    public Result TryTakeInto(Span<T> buffer)
    {
        if (TryTake(buffer.Length, out var taken))
        {
            taken.CopyTo(buffer);
            return Ok();
        }

        return new InvalidOperationException($"Cannot Take {buffer.Length} items: Remaining Count is {RemainingCount}");
    }

    public void TakeInto(Span<T> buffer) => TryTakeInto(buffer).ThrowIfError();

    // ------------------------------------------------------------------------------------

    public ReadOnlySpan<T> TakeUntil(Func<T, bool> itemPredicate)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = span.Length;
        while (index < len && !itemPredicate(span[index]))
        {
            index += 1;
        }

        _position = index;
        return span[start..index];
    }

    public ReadOnlySpan<T> TakeUntil(T match) => TakeUntil(item => EqualityComparer<T>.Default.Equals(item, match));

    // ------------------------------------------------------------------------------------

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
        return span[start..index];
    }

    public ReadOnlySpan<T> TakeWhile(ReadOnlySpanFunc<T, int> slicePredicate)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = span.Length;
        int consumed;
        while (index < len && (consumed = slicePredicate(span[index..])) > 0)
        {
            index += consumed;
        }

        _position = index;
        return span[start..index];
    }

    public ReadOnlySpan<T> TakeWhile(T match) => TakeWhile(item => EqualityComparer<T>.Default.Equals(item, match));

    public ReadOnlySpan<T> TakeWhile(ReadOnlySpan<T> match)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = span.Length;
        while (index < len && span[index..].StartsWith(match))
        {
            index += match.Length;
        }

        _position = index;
        return span[start..index];
    }

    // ------------------------------------------------------------------------------------

    public ReadOnlySpan<T> TakeWhileAny(params T[] matches) => TakeWhile(item => matches.Contains(item));

    public ReadOnlySpan<T> TakeWhileAny(ReadOnlySpan<T> matches)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = span.Length;
        while (index < len && matches.Contains(span[index]))
        {
            index += 1;
        }

        _position = index;
        return span[start..index];
    }

    public ReadOnlySpan<T> TakeWhileAny(ICollection<T> matches) => TakeWhile(item => matches.Contains(item));

    // ------------------------------------------------------------------------------------

    public ReadOnlySpan<T> TakeAll() => TakeWhile(static _ => true);

#endregion

#region Match

    /// <summary>
    /// <see cref="Take()">Takes</see> the next item and compares it to <paramref name="match"/><br/>
    /// If they are not equal, a <see cref="ArgumentOutOfRangeException"/> is thrown
    /// </summary>
    /// <param name="match">The item to match against</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="match"/> is not equal to the read item
    /// </exception>
    public void Match(T match)
    {
        T item = Take();
        if (EqualityComparer<T>.Default.Equals(item, match)) return;
        throw new ArgumentOutOfRangeException(nameof(match), match, $"Match failed against {item}");
    }

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
        var span = _span;

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