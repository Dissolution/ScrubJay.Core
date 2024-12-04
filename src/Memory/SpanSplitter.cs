namespace ScrubJay.Memory;

/// <summary>
/// Enables enumerating each split within a <see cref="ReadOnlySpan{T}"/> that has been divided using one or more separators.
/// </summary>
/// <remarks>
/// <code>
/// SpanSplitter&lt;T&gt; : IEnumerator&lt;ReadOnlySpan&lt;T&gt;&gt;
/// </code>
/// </remarks>
public ref struct SpanSplitter<T>
    where T : IEquatable<T>
{
    public static SpanSplitter<T> Split(ReadOnlySpan<T> span, T separator)
        => new(span, separator);

    public static SpanSplitter<T> Split(ReadOnlySpan<T> span, ReadOnlySpan<T> separator)
        => new(span, separator, SpanSplitterSeparatorKind.Span);

    public static SpanSplitter<T> SplitAny(ReadOnlySpan<T> span, ReadOnlySpan<T> separators)
        => new(span, separators, SpanSplitterSeparatorKind.AnySpan);


    // The span being split
    private readonly ReadOnlySpan<T> _span;

    /// The single-item separator for <see cref="SpanSplitterSeparatorKind.Item"/>
    private readonly T _separator = default!;


    /// <see cref="SpanSplitterSeparatorKind.Span"/>: A multi-item separator<br/>
    /// <see cref="SpanSplitterSeparatorKind.AnySpan"/>: Multiple single-item separators
    private readonly ReadOnlySpan<T> _separators = default!;

    /// How are we storing the separator(s)?
    private SpanSplitterSeparatorKind _separatorKind;

    /// Inclusive starting index for Current
    private int _currentRangeStart = 0;

    /// Exclusive ending index for Current
    private int _currentRangeEnd = 0;

    /// Index to start scanning for the next item
    private int _nextStart = 0;


    /// <summary>
    /// Gets the current segment of <see cref="ReadOnlySpan{T}"/> items
    /// </summary>
    public ReadOnlySpan<T> Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _span[CurrentRange];
    }

    /// <summary>
    /// Gets the <see cref="Range"/> in the original <see cref="ReadOnlySpan{T}"/> that <see cref="Current"/> covers
    /// </summary>
    public Range CurrentRange
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(_currentRangeStart, _currentRangeEnd);
    }

    private SpanSplitter(ReadOnlySpan<T> span, T separator)
    {
        _span = span;
        _separator = separator;
        _separatorKind = SpanSplitterSeparatorKind.Item;
    }

    private SpanSplitter(ReadOnlySpan<T> span, ReadOnlySpan<T> separators, SpanSplitterSeparatorKind separatorKind)
    {
        _span = span;
        _separators = separators;
        _separatorKind = separators.Length == 0 ? SpanSplitterSeparatorKind.Empty : separatorKind;
    }

    /// <summary>
    /// Try to find another segment
    /// </summary>
    public bool MoveNext()
    {
        int index;
        int length;
        switch (_separatorKind)
        {
            case SpanSplitterSeparatorKind.Item:
            {
                index = _span.Slice(_nextStart).IndexOf(_separator);
                length = 1;
                break;
            }
            case SpanSplitterSeparatorKind.AnySpan:
            {
                index = _span.Slice(_nextStart).IndexOfAny(_separators);
                length = 1;
                break;
            }
            case SpanSplitterSeparatorKind.Span:
            {
                index = _span.Slice(_nextStart).IndexOf(_separators);
                length = _separators.Length;
                break;
            }
            case SpanSplitterSeparatorKind.Empty:
            {
                // same as enumeration ended, will yield the entire span once
                index = -1;
                length = 1;
                break;
            }
            case SpanSplitterSeparatorKind.None:
            default:
                return false;
        }

        _currentRangeStart = _nextStart;
        if (index >= 0)
        {
            _currentRangeEnd = _currentRangeStart + index;
            _nextStart = _currentRangeEnd + length;
        }
        else
        {
            _currentRangeEnd = _span.Length;
            _nextStart = _currentRangeEnd;

            // stops further enumeration
            _separatorKind = SpanSplitterSeparatorKind.None;
        }

        return true;
    }

    /// <summary>
    /// What kind of separator is being used in a <see cref="SpanSplitter{T}"/>
    /// </summary>
    private enum SpanSplitterSeparatorKind
    {
        None = 0,

        /// <summary>
        /// A single-item separator
        /// </summary>
        Item,

        /// <summary>
        /// A multi-item separator
        /// </summary>
        Span,

        /// <summary>
        /// A sequence of separators treated independently
        /// </summary>
        AnySpan,

        /// <summary>
        /// An empty sequence
        /// </summary>
        Empty,
    }
}