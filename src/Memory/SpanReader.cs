namespace ScrubJay.Memory;

[PublicAPI]
public ref struct SpanReader<T>
{
#region delegates

    public delegate bool ScanNext(ReadOnlySpan<T> nextItems);

    public delegate bool ScanPrevNext(ReadOnlySpan<T> prevItems, ReadOnlySpan<T> nextItems);

    public delegate int ReadSpan(ReadOnlySpan<T> remainingSpan);

#endregion

    private readonly ReadOnlySpan<T> _span;
    private readonly int _spanLength;

    private int _position;

    public int Position
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        readonly get => _position;
        set
        {
            Throw.IfNotBetween(value, 0, _spanLength);
            _position = value;
        }
    }

    public readonly bool IsCompleted => _position >= _spanLength;

    public readonly ReadOnlySpan<T> Previous => _span[.._position];

    public readonly ReadOnlySpan<T> Next => _span[_position..];


    public SpanReader(ReadOnlySpan<T> span)
    {
        _span = span;
        _spanLength = span.Length;
        _position = 0;
    }

    private readonly InvalidOperationException GetEx(
        string? info = null,
        [CallerMemberName] string? methodName = null)
    {
        string message = TextBuilder.New
            .Append("Cannot ")
            .Append(methodName)
            .Append(", ")
            .If(Next.Length, static len => len == 0,
                static (tb, _) => tb.Append("no items remain"),
                static (tb, len) => tb.Append($"only {len} items remain"))
            .IfNotNull(info, static (tb, n) => tb.Append($": {n}"))
            .ToStringAndDispose();
        return new InvalidOperationException(message);
    }

#region Take

#region (Try)Take

    public Option<T> TryTake()
    {
        if (_position < _spanLength)
        {
            T value = _span[_position];
            _position++;
            return Some(value);
        }

        return None;
    }

    public T Take()
    {
        if (_position < _spanLength)
        {
            T value = _span[_position];
            _position++;
            return value;
        }

        throw GetEx();
    }

#if NET9_0_OR_GREATER
    public RefOption<ReadOnlySpan<T>> TryTake(int count)
    {
        if (count <= 0)
            return RefOption<ReadOnlySpan<T>>.Some(ReadOnlySpan<T>.Empty);

        if (_position + count <= _spanLength)
        {
            var slice = _span.Slice(_position, count);
            _position += count;
            return RefOption<ReadOnlySpan<T>>.Some(slice);
        }

        return None;
    }
#endif

    public Option<T[]> TryTakeToArray(int count)
    {
        if (count <= 0)
            return Some<T[]>([]);

        if (_position + count <= _spanLength)
        {
            var taken = _span.Slice(_position, count).ToArray();
            _position += count;
            return Some(taken);
        }

        return None;
    }

    public T[] TakeToArray(int count)
    {
        Throw.IfLessThan(count, 0);
        if (_position + count <= _spanLength)
        {
            var taken = _span.Slice(_position, count).ToArray();
            _position += count;
            return taken;
        }

        throw GetEx();
    }

    public ReadOnlySpan<T> Take(int count)
    {
        Throw.IfLessThan(count, 0);
        if (_position + count <= _spanLength)
        {
            var slice = _span.Slice(_position, count);
            _position += count;
            return slice;
        }

        throw GetEx();
    }

#endregion

#region TakeInto

    public void TakeInto(scoped Span<T> buffer)
    {
        if (_position + buffer.Length <= _spanLength)
        {
            _span.Slice(_position, buffer.Length).CopyTo(buffer);
            _position += buffer.Length;
            return;
        }

        throw GetEx();
    }

    public bool TryTakeInto(Span<T> buffer)
    {
        if (_position + buffer.Length <= _spanLength)
        {
            _span.Slice(_position, buffer.Length).CopyTo(buffer);
            _position += buffer.Length;
            return true;
        }

        return false;
    }

#endregion

#region TakeWhile

    public ReadOnlySpan<T> TakeWhile(Func<T, bool> itemPredicate)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;

        while (index < len && itemPredicate(span[index]))
            index++;
        _position = index;
        return span[start..index];
    }

    public ReadOnlySpan<T> TakeWhile(ScanNext scanNext)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;

        while (index < len && scanNext(span[index..]))
            index++;
        _position = index;
        return span[start..index];
    }

    public ReadOnlySpan<T> TakeWhile(ScanPrevNext scanPrevNext)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;

        while (index < len && scanPrevNext(span[..index], span[index..]))
            index++;
        _position = index;
        return span[start..index];
    }

#endregion

#region TakeWhileMatching

    public ReadOnlySpan<T> TakeWhileMatching(
        T match,
        IEqualityComparer<T>? comparer = null)
    {
        if (comparer is null)
            return TakeWhile(item => EqualityComparer<T>.Default.Equals(item, match));
        return TakeWhile(item => comparer.Equals(item, match));
    }

    public ReadOnlySpan<T> TakeWhileMatching(
        scoped ReadOnlySpan<T> match,
        IEqualityComparer<T>? comparer = null)
    {
        int matchLen = match.Length;
        if (matchLen == 0)
            return [];

        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;

        while (index < len && Sequence.Equal(span.Slice(index, matchLen), match, comparer))
        {
            index += matchLen;
        }

        _position = index;
        return span[start..index];
    }

    public ReadOnlySpan<T> TakeWhileMatchingAny(
        ICollection<T> matches,
        IEqualityComparer<T>? comparer = null)
    {
        if (comparer is null)
        {
            return TakeWhile(item => matches.Contains(item));
        }

        return TakeWhile(item => matches.Contains(item, comparer));
    }

#endregion

#region TakeUntil

    public ReadOnlySpan<T> TakeUntil(Func<T, bool> itemPredicate)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;

        while (index < len && !itemPredicate(span[index]))
            index++;
        _position = index;
        return span[start..index];
    }

    public ReadOnlySpan<T> TakeUntil(ScanNext scanNext)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;

        while (index < len && !scanNext(span[index..]))
            index++;
        _position = index;
        return span[start..index];
    }

    public ReadOnlySpan<T> TakeUntil(ScanPrevNext scanPrevNext)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;

        while (index < len && !scanPrevNext(span[..index], span[index..]))
            index++;
        _position = index;
        return span[start..index];
    }

#endregion

#region TakeUntilMatching

    public ReadOnlySpan<T> TakeUntilMatching(
        T match,
        IEqualityComparer<T>? comparer = null)
    {
        if (comparer is null)
            return TakeUntil(item => !EqualityComparer<T>.Default.Equals(item, match));
        return TakeUntil(item => !comparer.Equals(item, match));
    }

    public ReadOnlySpan<T> TakeUntilMatching(
        scoped ReadOnlySpan<T> match,
        IEqualityComparer<T>? comparer = null,
        bool chunk = false)
    {
        int matchLen = match.Length;
        if (matchLen == 0)
            return [];

        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;

        while (index < len && !Sequence.Equal(span.Slice(index, matchLen), match, comparer))
        {
            if (chunk)
                index += matchLen;
            else
                index++;
        }

        _position = index;
        return span[start..index];
    }

    public ReadOnlySpan<T> TakeUntilMatchingAny(
        ICollection<T> matches,
        IEqualityComparer<T>? comparer = null)
    {
        if (comparer is null)
        {
            return TakeUntil(item => !matches.Contains(item));
        }

        return TakeUntil(item => !matches.Contains(item, comparer));
    }

#endregion

#endregion
#region Peek!


#region Peek / Try / Into

    public readonly Option<T> TryPeek()
    {
        if (_position < _spanLength)
            return Some(_span[_position]);
        return None;
    }

    public readonly T Peek()
    {
        if (_position < _spanLength)
            return _span[_position];
        throw GetEx();
    }

#if NET9_0_OR_GREATER
    public readonly RefOption<ReadOnlySpan<T>> TryPeek(int count)
    {
        if (count <= 0)
            return RefOption<ReadOnlySpan<T>>.Some(ReadOnlySpan<T>.Empty);

        if (_position + count <= _spanLength)
            return RefOption<ReadOnlySpan<T>>.Some(_span.Slice(_position, count));

        return None;
    }
#endif

    public readonly ReadOnlySpan<T> Peek(int count)
    {
        Throw.IfLessThan(count, 0);
        if (_position + count <= _spanLength)
            return _span.Slice(_position, count);

        throw GetEx();
    }


    public readonly Option<T[]> TryPeekToArray(int count)
    {
        if (count <= 0)
            return Some<T[]>([]);

        if (_position + count <= _spanLength)
            return Some(_span.Slice(_position, count).ToArray());

        return None;
    }

    public readonly T[] PeekToArray(int count)
    {
        Throw.IfLessThan(count, 0);
        if (_position + count <= _spanLength)
            return _span.Slice(_position, count).ToArray();

        throw GetEx();
    }


    public readonly bool TryPeekInto(Span<T> destination)
    {
        int count = destination.Length;
        if (_position + count <= _spanLength)
        {
            _span.Slice(_position, count).CopyTo(destination);
            return true;
        }

        return false;
    }

    public readonly void PeekInto(Span<T> destination)
    {
        int count = destination.Length;
        if (_position + count <= _spanLength)
        {
            _span.Slice(_position, count).CopyTo(destination);
            return;
        }

        throw GetEx();
    }


#endregion

#region PeekWhile

    public readonly ReadOnlySpan<T> PeekWhile(Func<T, bool> itemPredicate)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;

        while (index < len && itemPredicate(span[index]))
            index++;
        return span[start..index];
    }

    public readonly ReadOnlySpan<T> PeekWhile(ScanNext scanNext)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;

        while (index < len && scanNext(span[index..]))
            index++;
        return span[start..index];
    }

    public readonly ReadOnlySpan<T> PeekWhile(ScanPrevNext scanPrevNext)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;

        while (index < len && scanPrevNext(span[..index], span[index..]))
            index++;
        return span[start..index];
    }

#endregion

#region PeekWhileMatching

    public readonly ReadOnlySpan<T> PeekWhileMatching(
        T match,
        IEqualityComparer<T>? comparer = null)
    {
        if (comparer is null)
            return PeekWhile(item => EqualityComparer<T>.Default.Equals(item, match));
        return PeekWhile(item => comparer.Equals(item, match));
    }

    public readonly ReadOnlySpan<T> PeekWhileMatching(
        scoped ReadOnlySpan<T> match,
        IEqualityComparer<T>? comparer = null)
    {
        int matchLen = match.Length;
        if (matchLen == 0)
            return [];

        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;

        while (index < len && Sequence.Equal(span.Slice(index, matchLen), match, comparer))
        {
            index += matchLen;
        }

        return span[start..index];
    }

    public readonly ReadOnlySpan<T> PeekWhileMatchingAny(
        ICollection<T> matches,
        IEqualityComparer<T>? comparer = null)
    {
        if (comparer is null)
        {
            return PeekWhile(item => matches.Contains(item));
        }

        return PeekWhile(item => matches.Contains(item, comparer));
    }

#endregion

#region PeekUntil

    public readonly ReadOnlySpan<T> PeekUntil(Func<T, bool> itemPredicate)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;

        while (index < len && !itemPredicate(span[index]))
            index++;
        return span[start..index];
    }

    public readonly ReadOnlySpan<T> PeekUntil(ScanNext scanNext)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;

        while (index < len && !scanNext(span[index..]))
            index++;
        return span[start..index];
    }

    public readonly ReadOnlySpan<T> PeekUntil(ScanPrevNext scanPrevNext)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;

        while (index < len && !scanPrevNext(span[..index], span[index..]))
            index++;
        return span[start..index];
    }

#endregion

#region PeekUntilMatching

    public readonly ReadOnlySpan<T> PeekUntilMatching(
        T match,
        IEqualityComparer<T>? comparer = null)
    {
        if (comparer is null)
            return PeekUntil(item => !EqualityComparer<T>.Default.Equals(item, match));
        return PeekUntil(item => !comparer.Equals(item, match));
    }

    public readonly ReadOnlySpan<T> PeekUntilMatching(
        scoped ReadOnlySpan<T> match,
        IEqualityComparer<T>? comparer = null)
    {
        int matchLen = match.Length;
        if (matchLen == 0)
            return [];

        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;

        while (index < len && !Sequence.Equal(span.Slice(index, matchLen), match, comparer))
        {
            index++;
        }

        return span[start..index];
    }

    public readonly ReadOnlySpan<T> PeekUntilMatchingAny(
        ICollection<T> matches,
        IEqualityComparer<T>? comparer = null)
    {
        if (comparer is null)
        {
            return PeekUntil(item => !matches.Contains(item));
        }

        return PeekUntil(item => !matches.Contains(item, comparer));
    }

#endregion

#endregion /Peek!

#region Skip

#region (Try)Skip

    public bool TrySkip()
    {
        if (_position < _spanLength)
        {
            _position++;
            return true;
        }

        return false;
    }

    public void Skip()
    {
        if (_position < _spanLength)
        {
            _position++;
            return;
        }

        throw GetEx();
    }

    public bool TrySkip(int count)
    {
        if (count <= 0)
            return true;

        if (_position + count <= _spanLength)
        {
            _position += count;
            return true;
        }

        return false;
    }


    public void Skip(int count)
    {
        Throw.IfLessThan(count, 0);

        if (_position + count <= _spanLength)
        {
            _position += count;
            return;
        }

        throw GetEx();
    }

#endregion

#region SkipWhile

    public void SkipWhile(Func<T, bool> itemPredicate)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;

        while (index < len && itemPredicate(span[index]))
            index++;
        _position = index;
    }

    public void SkipWhile(ScanNext scanNext)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;

        while (index < len && scanNext(span[index..]))
            index++;
        _position = index;
    }

    public void SkipWhile(ScanPrevNext scanPrevNext)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;

        while (index < len && scanPrevNext(span[..index], span[index..]))
            index++;
        _position = index;
    }

#endregion

#region SkipWhileMatching

    public void SkipWhileMatching(
        T match,
        IEqualityComparer<T>? comparer = null)
    {
        if (comparer is null)
        {
            SkipWhile(item => EqualityComparer<T>.Default.Equals(item, match));
        }
        else
        {
            SkipWhile(item => comparer.Equals(item, match));
        }
    }

    public void SkipWhileMatching(
        scoped ReadOnlySpan<T> match,
        IEqualityComparer<T>? comparer = null)
    {
        int matchLen = match.Length;
        if (matchLen == 0)
            return;

        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;

        while (index < len && Sequence.Equal(span.Slice(index, matchLen), match, comparer))
        {
            index += matchLen;
        }

        _position = index;
    }

    public void SkipWhileMatchingAny(
        ICollection<T> matches,
        IEqualityComparer<T>? comparer = null)
    {
        if (comparer is null)
        {
            SkipWhile(item => matches.Contains(item));
        }
        else
        {
            SkipWhile(item => matches.Contains(item, comparer));
        }
    }

#endregion

#region SkipUntil

    public void SkipUntil(Func<T, bool> itemPredicate)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;

        while (index < len && !itemPredicate(span[index]))
            index++;
        _position = index;
    }

    public void SkipUntil(ScanNext scanNext)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;

        while (index < len && !scanNext(span[index..]))
            index++;
        _position = index;
    }

    public void SkipUntil(ScanPrevNext scanPrevNext)
    {
        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;

        while (index < len && !scanPrevNext(span[..index], span[index..]))
            index++;
        _position = index;
    }

#endregion

#region SkipUntilMatching

    public void SkipUntilMatching(
        T match,
        IEqualityComparer<T>? comparer = null)
    {
        if (comparer is null)
        {
            SkipUntil(item => !EqualityComparer<T>.Default.Equals(item, match));
        }
        else
        {
            SkipUntil(item => !comparer.Equals(item, match));
        }
    }

    public void SkipUntilMatching(
        scoped ReadOnlySpan<T> match,
        IEqualityComparer<T>? comparer = null)
    {
        int matchLen = match.Length;
        if (matchLen == 0)
            return;

        var span = _span;
        int start = _position;
        int index = start;
        int len = _spanLength;

        while (index < len && !Sequence.Equal(span.Slice(index, matchLen), match, comparer))
        {
            index++;
        }

        _position = index;
    }

    public void SkipUntilMatchingAny(
        ICollection<T> matches,
        IEqualityComparer<T>? comparer = null)
    {
        if (comparer is null)
        {
            SkipUntil(item => !matches.Contains(item));
        }
        else
        {
            SkipUntil(item => !matches.Contains(item, comparer));
        }
    }

#endregion

#endregion


    /// <summary>
    /// Resets this <see cref="SpanReader{T}"/> back to its beginning position
    /// </summary>
    public void Reset() => _position = 0;

    public readonly override string ToString()
    {
        // For debugging purposes, we want to show our position in the source span

        string delimiter;
        int captureCount;
        string pointer;

        if (typeof(T) == typeof(char))
        {
            delimiter = string.Empty;
            captureCount = 13;
            pointer = " ⌖ ";
        }
        else
        {
            delimiter = ", ";
            captureCount = 3;
            pointer = "⌖ ";
        }

        using var builder = new TextBuilder();

        // Previously read items
        var prev = Previous;
        int len = prev.Length;
        if (len > captureCount)
        {
            builder.Append('…').Append(delimiter);
        }
        else if (len < captureCount)
        {
            captureCount = len;
        }

        // Append them
        builder.Delimit(delimiter, prev[^captureCount..]);

        // position indicator
        builder.Append(pointer);

        // Next items
        var next = Next;
        len = next.Length;

        if (len < captureCount)
        {
            captureCount = len;
        }

        // Append them
        builder.Delimit(delimiter, next[..captureCount]);

        if (len > captureCount)
        {
            builder.Append(delimiter).Append('…');
        }

        return builder.ToString();
    }
}