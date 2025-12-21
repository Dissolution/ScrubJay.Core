namespace ScrubJay.Memory;

[PublicAPI]
public ref struct SpanSplitEqualityIterator<T> : ISpanSplitIterator<T>
{
    // The span being split
    private readonly ReadOnlySpan<T> _span;

    /// The single-item separator for <see cref="SeparatorKind.Item"/>
    private readonly T _separator;

    /// <see cref="SeparatorKind.Span"/>: A multi-item separator<br/>
    /// <see cref="SeparatorKind.AnySpan"/>: Multiple single-item separators
    private readonly ReadOnlySpan<T> _separators;

    /// How are we storing the separator(s)?
    private readonly SeparatorKind _separatorKind;

    /// Options for Splitting
    private readonly SplitOptions _options;

    private readonly IEqualityComparer<T>? _equalityComparer;

    /// Index to start scanning for the next item
    private int _scanStartIndex;

    public SplitOptions Options => _options;

    internal SpanSplitEqualityIterator(
        ReadOnlySpan<T> span,
        T separator,
        SplitOptions options = SplitOptions.None,
        IEqualityComparer<T>? equalityComparer = null)
    {
        _span = span;
        _separator = separator;
        _separators = default;
        _separatorKind = SeparatorKind.Item;
        _options = options;
        _equalityComparer = equalityComparer;
        _scanStartIndex = 0;
    }

    internal SpanSplitEqualityIterator(
        ReadOnlySpan<T> span,
        ReadOnlySpan<T> separators,
        SeparatorKind separatorKind,
        SplitOptions options = SplitOptions.None,
        IEqualityComparer<T>? equalityComparer = null)
    {
        _span = span;
        _separator = default!;
        _separators = separators;
        Debug.Assert(separatorKind != SeparatorKind.None);
        _separatorKind = separators.Length == 0 ? SeparatorKind.Empty : separatorKind;
        _options = options;
        _equalityComparer = equalityComparer;
        _scanStartIndex = 0;
    }

    public bool TryMoveNext(out Segment<T> segment)
    {
        int scan;
        int index;
        int skipLength;

        TOP:

        scan = _scanStartIndex;
        if (scan >= _span.Length)
        {
            segment = default;
            return false;
        }

        switch (_separatorKind)
        {
            case SeparatorKind.Item:
            {
                index = _span[scan..].IndexOf(_separator, _equalityComparer);
                skipLength = 1;
                break;
            }
            case SeparatorKind.AnySpan:
            {
                index = _span[scan..].IndexOfAny(_separators, _equalityComparer);
                skipLength = 1;
                break;
            }
            case SeparatorKind.Span:
            {
                index = _span[scan..].IndexOf(_separators, _equalityComparer);
                skipLength = _separators.Length;
                break;
            }
            case SeparatorKind.Empty:
            {
                // yield the entier span once
                index = -1;
                skipLength = 1;
                break;
            }
            default:
            {
                throw Ex.UndefinedEnum(_separatorKind);
            }
        }

        var segmentStart = scan;
        int segmentEnd;
        if (index >= 0)
        {
            segmentEnd = segmentStart + index;
            _scanStartIndex = segmentEnd + skipLength;
        }
        else
        {
            segmentEnd = _span.Length;
            _scanStartIndex = segmentEnd;
        }

        if (_options.HasFlags(SplitOptions.IgnoreEmpty))
        {
            int rangeLen = segmentEnd - segmentStart;
            if (rangeLen == 0)
            {
                goto TOP;
            }
        }

        var range = new Range(segmentStart, segmentEnd);
        segment = new Segment<T>(range, _span[range]);
        return true;
    }
}