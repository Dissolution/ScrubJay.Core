using static ScrubJay.Memory.SpanReadResult;

namespace ScrubJay.Memory;

[PublicAPI]
public ref struct SpanReader<T>
{
#region delegates

    /// <summary>
    /// A predicate that examines the <see cref="ReadOnlySpan{T}"/> of items after a position
    /// </summary>
    public delegate bool ScanNext(ReadOnlySpan<T> nextItems);

    /// <summary>
    /// A predicate that examines the <see cref="ReadOnlySpan{T}"/> of items before and after a position
    /// </summary>
    public delegate bool ScanPrevNext(ReadOnlySpan<T> prevItems, ReadOnlySpan<T> nextItems);

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

        return None<T>();
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

    public OptionROSpan<T> TryTake(int count)
    {
        if (count <= 0)
            return ReadOnlySpan<T>.Empty;

        if (_position + count <= _spanLength)
        {
            var slice = _span.Slice(_position, count);
            _position += count;
            return slice;
        }

        return None();
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

#region Peek

#region (Try)Peek

    public readonly Option<T> TryPeek()
    {
        if (_position < _spanLength)
            return Some(_span[_position]);
        return None<T>();
    }

    public readonly T Peek()
    {
        if (_position < _spanLength)
            return _span[_position];
        throw GetEx();
    }

    public readonly OptionROSpan<T> TryPeek(int count)
    {
        if (count <= 0)
            return ReadOnlySpan<T>.Empty;

        if (_position + count <= _spanLength)
            return _span.Slice(_position, count);

        return None();
    }


    public readonly ReadOnlySpan<T> Peek(int count)
    {
        Throw.IfLessThan(count, 0);
        if (_position + count <= _spanLength)
            return _span.Slice(_position, count);

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

#endregion

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
        var prev = this.Previous;
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
        builder.EnumerateFormatAndDelimit(prev[^captureCount..], delimiter);

        // position indicator
        builder.Append(pointer);

        // Next items
        var next = this.Next;
        len = next.Length;

        if (len < captureCount)
        {
            captureCount = len;
        }

        // Append them
        builder.EnumerateFormatAndDelimit(next[..captureCount], delimiter);

        if (len > captureCount)
        {
            builder.Append(delimiter).Append('…');
        }

        return builder.ToString();
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