#pragma warning disable CA1000 // Do not declare static members on generic types

using ScrubJay.Enums;

namespace ScrubJay.Memory;

[PublicAPI]
[Flags]
public enum SpanSplitterOptions
{
    None = 0,
    /// <summary>
    /// Do not return any Length == 0 segments
    /// </summary>
    IgnoreEmpty = 1,
}

/// <summary>
/// Enables enumerating each split within a <see cref="ReadOnlySpan{T}"/> that has been divided using one or more separators.
/// </summary>
/// <remarks>
/// <code>
/// SpanSplitter&lt;T&gt; : IEnumerator&lt;ReadOnlySpan&lt;T&gt;&gt;
/// </code>
/// </remarks>
[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public ref struct SpanSplitter<T> // : IEnumerable<ReadOnlySpan<T>>
    where T : IEquatable<T> // IndexOf requires this
{
    public static SpanSplitter<T> Split(ReadOnlySpan<T> span, T separator, SpanSplitterOptions options = SpanSplitterOptions.None)
        => new(span, separator, options);

    public static SpanSplitter<T> Split(ReadOnlySpan<T> span, ReadOnlySpan<T> separator, SpanSplitterOptions options = SpanSplitterOptions.None)
        => new(span, separator, SpanSplitterSeparatorKind.Span, options);

    public static SpanSplitter<T> SplitAny(ReadOnlySpan<T> span, ReadOnlySpan<T> separators, SpanSplitterOptions options = SpanSplitterOptions.None)
        => new(span, separators, SpanSplitterSeparatorKind.AnySpan, options);


    // The span being split
    private readonly ReadOnlySpan<T> _span;
    /// The single-item separator for <see cref="SpanSplitterSeparatorKind.Item"/>
    private readonly T _separator = default!;
    /// <see cref="SpanSplitterSeparatorKind.Span"/>: A multi-item separator<br/>
    /// <see cref="SpanSplitterSeparatorKind.AnySpan"/>: Multiple single-item separators
    private readonly ReadOnlySpan<T> _separators = default!;
    /// How are we storing the separator(s)?
    private readonly SpanSplitterSeparatorKind _separatorKind;
    /// Options for Splitting
    private readonly SpanSplitterOptions _options;


    /// Inclusive starting index for Current
    private int _currentRangeStart = 0;
    /// Exclusive ending index for Current
    private int _currentRangeEnd = 0;
    /// Index to start scanning for the next item
    private int _nextStart = 0;

    private bool _finished = false;

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

    private SpanSplitter(ReadOnlySpan<T> span, T separator, SpanSplitterOptions options)
    {
        _span = span;
        _separator = separator;
        _separatorKind = SpanSplitterSeparatorKind.Item;
        _options = options;
    }

    private SpanSplitter(ReadOnlySpan<T> span, ReadOnlySpan<T> separators, SpanSplitterSeparatorKind separatorKind, SpanSplitterOptions options)
    {
        _span = span;
        _separators = separators;
        _separatorKind = separators.Length == 0 ? SpanSplitterSeparatorKind.Empty : separatorKind;
        _options = options;
    }

    /// <summary>
    /// Try to find another segment
    /// </summary>
    public bool MoveNext()
    {
        if (_finished)
            return false;

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
                _finished = true;
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

            // stop further enumeration
            _finished = true;
        }

        if (_options.HasFlags(SpanSplitterOptions.IgnoreEmpty))
        {
            int rangeLen = _currentRangeEnd - _currentRangeStart;
            if (rangeLen == 0)
                return MoveNext();
        }

        return true;
    }

    public void Reset()
    {
        _currentRangeStart = 0;
        _currentRangeEnd = 0;
        _nextStart = 0;
        _finished = false;
    }

    public IReadOnlyList<IReadOnlyList<T>> ToLists()
    {
        // we always start consumption here, they can Reset if they wish
        var segments = new List<IReadOnlyList<T>>();
        while (MoveNext())
        {
            segments.Add(Current.ToArray());
        }
        return segments;

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
