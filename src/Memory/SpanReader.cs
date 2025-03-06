using static ScrubJay.Memory.SpanReadResult;

namespace ScrubJay.Memory;


[PublicAPI]
public ref struct SpanReader<T>
{
    #region delegates
    /// <summary>
    /// A predicate that examines the <see cref="ReadOnlySpan{T}"/> of items after a position
    /// </summary>
    public delegate bool ReadNextPredicate(ReadOnlySpan<T> nextItems);

    /// <summary>
    /// A predicate that examines the <see cref="ReadOnlySpan{T}"/> of items before and after a position
    /// </summary>
    public delegate bool ReadPrevNextPredicate(ReadOnlySpan<T> prevItems, ReadOnlySpan<T> nextItems);

    /// <summary>
    /// Examines the <see cref="RemainingSpan"/> and returns how many of them were read,
    /// which moves the reading position forward
    /// </summary>
    /// <param name="remainingSpan">
    /// The <see cref="RemainingSpan"/>
    /// </param>
    /// <returns>
    /// The number of items in <paramref name="remainingSpan"/> that were read
    /// </returns>
    public delegate int ReadSpan(ReadOnlySpan<T> remainingSpan);
    #endregion

    private readonly ReadOnlySpan<T> _span;
    private readonly int _spanLength;

    private int _position;

    public readonly int Position
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _position;
    }

    public readonly int RemainingCount
    {
        get
        {
            int pos = _position;
            if (pos < 0)
                return _spanLength;
            else if (pos >= _spanLength)
                return 0;
            else
                return _spanLength - pos;
        }
    }

    internal readonly ReadOnlySpan<T> RemainingSpan
    {
        get
        {
            int pos = _position;
            if (pos < 0)
                return _span;
            else if (pos >= _spanLength)
                return [];
            else
                return _span[pos..];
        }
    }


    public SpanReader(ReadOnlySpan<T> span)
    {
        _span = span;
        _spanLength = span.Length;
        _position = 0;
    }

    #region (Try)Peek
    /// <summary>
    /// Try to peek at the next item
    /// </summary>
    /// <returns>
    /// A <see cref="Option{T}.Some"/> containing the next item<br/>
    /// A <see cref="Option{T}.None"/> if there are no remaining items<br/>
    /// </returns>
    public readonly Option<T> TryPeek()
    {
        int pos = _position;
        if ((uint)pos < (uint)_spanLength)
        {
            return Some(_span[pos]);
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
    /// A <see cref="SpanReadResult{T}"/>
    /// </returns>
    public readonly SpanReadResult<T> TryPeek(int count)
    {
        int pos = _position;
        if ((uint)pos + (uint)count <= (uint)_spanLength)
        {
            return Satisified(_span.Slice(pos, count));
        }
        return EndOfSpan<T>();
    }

    /// <summary>
    /// Peek at the next item
    /// </summary>
    /// <returns>
    /// The next item
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if there are no items remaining
    /// </exception>
    public readonly T Peek() => TryPeek().SomeOrThrow($"Cannot {nameof(Peek)}: No items remain");

    /// <summary>
    /// Peek at the next <paramref name="count"/> items
    /// </summary>
    /// <param name="count">
    /// The number of items to peek at
    /// </param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if there are not at least <paramref name="count"/> items remaining
    /// </exception>
    public readonly ReadOnlySpan<T> Peek(int count)
    {
        if (TryPeek(count).HasReason(StopReason.Satisified, out var span))
            return span;
        throw new InvalidOperationException($"Cannot {nameof(Peek)}({count}): Only {RemainingCount} items remain");
    }
    #endregion

    #region (Try)Take
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
        if ((uint)pos < (uint)_spanLength)
        {
            _position = pos + 1;
            return Some(_span[pos]);
        }
        return None();
    }

    public SpanReadResult<T> TryTake(int count)
    {
        int pos = _position;
        if ((uint)pos + (uint)count <= (uint)_spanLength)
        {
            _position = pos + count;
            return Satisified(_span.Slice(pos, count));
        }
        return EndOfSpan<T>();
    }

    public T Take() => TryTake()
        .SomeOrThrow($"Cannot {nameof(Take)}: No items remain");

    public ReadOnlySpan<T> Take(int count)
    {
        if (TryTake(count).HasReason(StopReason.Satisified, out var span))
            return span;
        throw new InvalidOperationException($"Cannot {nameof(Take)}({count}): Only {RemainingCount} items remain");
    }

    public SpanReadResult<T> TryTakeWhile(Func<T, bool> itemPredicate)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;
        StopReason stopReason = StopReason.EndOfSpan;
        while (index < len)
        {
            if (!itemPredicate(span[index]))
            {
                stopReason = StopReason.Predicate;
                break;
            }
            index++;
        }

        _position = index;
        return new SpanReadResult<T>(stopReason, span[start..index]);
    }

    public SpanReadResult<T> TryTakeWhileNext(ReadNextPredicate nextItemsPredicate)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;
        StopReason stopReason = StopReason.EndOfSpan;
        while (index < len)
        {
            if (!nextItemsPredicate(span[index..]))
            {
                stopReason = StopReason.Predicate;
                break;
            }
            index++;
        }

        _position = index;
        return new SpanReadResult<T>(stopReason, span[start..index]);
    }

    public SpanReadResult<T> TryTakeWhilePrevNext(ReadPrevNextPredicate prevNextItemsPredicate)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;
        StopReason stopReason = StopReason.EndOfSpan;
        while (index < len)
        {
            if (!prevNextItemsPredicate(span[start..index], span[index..]))
            {
                stopReason = StopReason.Predicate;
                break;
            }
            index++;
        }

        _position = index;
        return new SpanReadResult<T>(stopReason, span[start..index]);
    }

    public SpanReadResult<T> TryTakeWhileMatches(T match, IEqualityComparer<T>? itemComparer = null)
    {
        if (itemComparer is null)
        {
            return TryTakeWhile(item => EqualityComparer<T>.Default.Equals(item, match));
        }
        return TryTakeWhile(item => itemComparer.Equals(item, match));
    }

    public SpanReadResult<T> TryTakeWhileMatches(ReadOnlySpan<T> match, IEqualityComparer<T>? itemComparer = null)
    {
        int matchLen = match.Length;
        if (matchLen == 0)
            return Satisified<T>();

        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;
        StopReason stopReason = StopReason.EndOfSpan;
        while (index < len)
        {
            if (!Sequence.Equal(span.Slice(index, matchLen), match, itemComparer))
            {
                stopReason = StopReason.Predicate;
                break;
            }
            index += matchLen;
        }

        _position = index;
        return new SpanReadResult<T>(stopReason, span[start..index]);
    }

    public SpanReadResult<T> TryTakeWhileMatchesAny(ICollection<T> matches, IEqualityComparer<T>? itemComparer = null)
    {
        if (itemComparer is null)
        {
            return TryTakeWhile(item => matches.Contains(item));
        }

        return TryTakeWhile(item => matches.Contains(item, itemComparer));
    }


    public SpanReadResult<T> TryTakeUntil(Func<T, bool> itemPredicate) => TryTakeWhile(item => !itemPredicate(item));
    public SpanReadResult<T> TryTakeUntilNext(ReadNextPredicate nextItemsPredicate) => TryTakeWhileNext(items => !nextItemsPredicate(items));
    public SpanReadResult<T> TryTakeUntilPrevNext(ReadPrevNextPredicate prevNextItemsPredicate) => TryTakeWhilePrevNext((prev, next) => !prevNextItemsPredicate(prev, next));
    public SpanReadResult<T> TryTakeUntilMatches(T match, IEqualityComparer<T>? itemComparer = null)
    {
        if (itemComparer is null)
        {
            return TryTakeWhile(item => !EqualityComparer<T>.Default.Equals(item, match));
        }
        return TryTakeWhile(item => !itemComparer.Equals(item, match));
    }

    public SpanReadResult<T> TryTakeUntilMatches(ReadOnlySpan<T> match, IEqualityComparer<T>? itemComparer = null)
    {
        int matchLen = match.Length;
        if (matchLen == 0)
            return Satisified<T>();

        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;
        StopReason stopReason = StopReason.EndOfSpan;
        while (index < len)
        {
            if (Sequence.Equal(span.Slice(index, matchLen), match, itemComparer))
            {
                stopReason = StopReason.Predicate;
                break;
            }
            index += matchLen;
        }

        _position = index;
        return new SpanReadResult<T>(stopReason, span[start..index]);
    }

    public SpanReadResult<T> TryTakeUntilMatchesAny(ICollection<T> matches, IEqualityComparer<T>? itemComparer = null)
    {
        if (itemComparer is null)
        {
            return TryTakeWhile(item => !matches.Contains(item));
        }

        return TryTakeWhile(item => !matches.Contains(item, itemComparer));
    }


    public ReadOnlySpan<T> TakeAll() => TryTakeWhile(static _ => true).Span;


    #endregion

    #region (Try)Skip
    /// <summary>
    /// Try to skip the next item
    /// </summary>
    public bool TrySkip()
    {
        int pos = _position;
        if ((uint)pos < (uint)_spanLength)
        {
            _position = pos + 1;
            return true;
        }
        return false;
    }

    public bool TrySkip(int count)
    {
        int pos = _position;
        if ((uint)pos + (uint)count <= (uint)_spanLength)
        {
            _position = pos + count;
            return true;
        }
        return false;
    }

    public void Skip()
    {
        if (!TrySkip())
            throw new InvalidOperationException($"Cannot {nameof(Skip)}: No items remain");
    }

    public void Skip(int count)
    {
        if (!TrySkip(count))
            throw new InvalidOperationException($"Cannot {nameof(Skip)}({count}): Only {RemainingCount} items remain");
    }

    public StopReason TrySkipWhile(Func<T, bool> itemPredicate)
    {
        var span = _span;
        int index = _position;
        int len = _spanLength;
        StopReason stopReason = StopReason.EndOfSpan;
        while (index < len)
        {
            if (!itemPredicate(span[index]))
            {
                stopReason = StopReason.Predicate;
                break;
            }
            index++;
        }

        _position = index;
        return stopReason;
    }

    public StopReason TrySkipWhileNext(ReadNextPredicate nextItemsPredicate)
    {
        var span = _span;
        int index = _position;
        int len = _spanLength;
        StopReason stopReason = StopReason.EndOfSpan;
        while (index < len)
        {
            if (!nextItemsPredicate(span[index..]))
            {
                stopReason = StopReason.Predicate;
                break;
            }
            index++;
        }

        _position = index;
        return stopReason;
    }

    public StopReason TrySkipWhilePrevNext(ReadPrevNextPredicate prevNextItemsPredicate)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;
        StopReason stopReason = StopReason.EndOfSpan;
        while (index < len)
        {
            if (!prevNextItemsPredicate(span[start..index], span[index..]))
            {
                stopReason = StopReason.Predicate;
                break;
            }
            index++;
        }

        _position = index;
        return stopReason;
    }

    public StopReason TrySkipWhileMatches(T match, IEqualityComparer<T>? itemComparer = null)
    {
        if (itemComparer is null)
        {
            return TrySkipWhile(item => EqualityComparer<T>.Default.Equals(item, match));
        }
        return TrySkipWhile(item => itemComparer.Equals(item, match));
    }

    public StopReason TrySkipWhileMatches(ReadOnlySpan<T> match, IEqualityComparer<T>? itemComparer = null)
    {
        int matchLen = match.Length;
        if (matchLen == 0)
            return StopReason.Satisified;

        var span = _span;
        int index = _position;
        int len = _spanLength;
        StopReason stopReason = StopReason.EndOfSpan;
        while (index < len)
        {
            if (!Sequence.Equal(span.Slice(index, matchLen), match, itemComparer))
            {
                stopReason = StopReason.Predicate;
                break;
            }
            index += matchLen;
        }

        _position = index;
        return stopReason;
    }

    public StopReason TrySkipWhileMatchesAny(ICollection<T> matches, IEqualityComparer<T>? itemComparer = null)
    {
        if (itemComparer is null)
        {
            return TrySkipWhile(item => matches.Contains(item));
        }

        return TrySkipWhile(item => matches.Contains(item, itemComparer));
    }

    public StopReason TrySkipUntil(Func<T, bool> itemPredicate) => TrySkipWhile(item => !itemPredicate(item));
    public StopReason TrySkipUntilNext(ReadNextPredicate nextItemsPredicate) => TrySkipWhileNext(items => !nextItemsPredicate(items));
    public StopReason TrySkipUntilPrevNext(ReadPrevNextPredicate prevNextItemsPredicate) => TrySkipWhilePrevNext((prev, next) => !prevNextItemsPredicate(prev, next));
    public StopReason TrySkipUntilMatches(T match, IEqualityComparer<T>? itemComparer = null)
    {
        if (itemComparer is null)
        {
            return TrySkipWhile(item => !EqualityComparer<T>.Default.Equals(item, match));
        }
        return TrySkipWhile(item => !itemComparer.Equals(item, match));
    }

    public StopReason TrySkipUntilMatches(ReadOnlySpan<T> match, IEqualityComparer<T>? itemComparer = null)
    {
        int matchLen = match.Length;
        if (matchLen == 0)
            return StopReason.Satisified;

        var span = _span;
        int index = _position;
        int len = _spanLength;
        StopReason stopReason = StopReason.EndOfSpan;
        while (index < len)
        {
            if (Sequence.Equal(span.Slice(index, matchLen), match, itemComparer))
            {
                stopReason = StopReason.Predicate;
                break;
            }
            index += matchLen;
        }

        _position = index;
        return stopReason;
    }

    public StopReason TrySkipUntilMatchesAny(ICollection<T> matches, IEqualityComparer<T>? itemComparer = null)
    {
        if (itemComparer is null)
        {
            return TrySkipWhile(item => !matches.Contains(item));
        }

        return TrySkipWhile(item => !matches.Contains(item, itemComparer));
    }


    public void SkipAll() => TrySkipWhile(static _ => true);


    #endregion



    public void Consume(ReadSpan consumeRemainingSpan)
    {
        int consumed = consumeRemainingSpan(RemainingSpan);
        if (consumed < 0 || consumed > RemainingCount)
            throw new InvalidOperationException($"Cannot consume {consumed} items");
        _position += consumed;
    }

    /// <summary>
    /// Resets this <see cref="SpanReader{T}"/> back to its beginning position
    /// </summary>
    public void Reset() => _position = 0;



    private readonly string ToStringFromCharSpan()
    {
        var text = new Buffer<char>();

        int index = _position;
        text chars = Notsafe.As<T, char>(_span);

        const int CAPTURE_COUNT = 32;

        // Previously read items
        int startIndex = index - CAPTURE_COUNT;
        // If we have more before this, indicate with ellipsis
        if (startIndex > 0)
        {
            text.Write('…');
            text.Write(chars[startIndex..index]);
        }
        // Otherwise, cap at a min zero
        else
        {
            text.Write(chars[..index]);
        }

        // position indicator
        text.Write('×');

        // items yet to be read
        int endIndex = index + CAPTURE_COUNT;
        // Indicate further characters with ellipsis
        if (endIndex < chars.Length)
        {
            text.Write(chars[index..endIndex]);
            text.Write("…");
        }
        else
        {
            text.Write(chars[index..]);
        }

        return text.ToStringAndDispose();
    }


    public override readonly string ToString()
    {
        // For debugging purposes, we want to show our position in the source span

        // Special handling for reading ReadOnlySpan<char>
        if (typeof(T) == typeof(char))
            return ToStringFromCharSpan();


        const string DELIMITER = ", ";
        const int CAPTURE_COUNT = 8;

        var text = new Buffer<char>();

        int index = _position;
        var span = _span;

        // Previously read items
        int startIndex = index - CAPTURE_COUNT;
        // If we have more before this, indicate with ellipsis
        if (startIndex > 0)
        {
            text.Write('…');
            text.Write(DELIMITER);
        }
        // Otherwise, cap at a min zero
        else
        {
            startIndex = 0;
        }

        for (int i = startIndex; i < index; i++)
        {
            if (i > startIndex)
            {
                text.Write(DELIMITER);
            }

            text.Write<T>(span[i]);
        }

        // position indicator
        text.Write('×');

        // items yet to be read
        int nextIndex = index + CAPTURE_COUNT;

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

        for (int i = index; i < nextIndex; i++)
        {
            if (i > index)
            {
                text.Write(DELIMITER);
            }

            text.Write<T>(span[i]);
        }

        if (postpendEllipsis)
        {
            text.Write(DELIMITER);
            text.Write("…");
        }

        return text.ToStringAndDispose();
    }
}
/*

public static class SpanReaderExtensions
{

    public static bool TryTakeUntil<T>(
        this scoped ref SpanReader<T> spanReader,
        T match,
        out ReadOnlySpan<T> taken,
        bool inclusive = false,
        bool skipFirst = false)
    {
        // Operate on the remaining span
        var span = spanReader.RemainingSpan;
        int len = span.Length;

        int i = skipFirst ? 1 : 0;
        while (i < len)
        {
            if (EqualityComparer<T>.Default.Equals(match, span[i]))
            {
                if (inclusive)
                {
                    i += 1;
                }
                taken = span[..i];
                spanReader.Skip(count: i);
                return true;
            }
            i++;
        }
        // Did not find the ending match
        taken = default;
        return false;
    }
}
*/
