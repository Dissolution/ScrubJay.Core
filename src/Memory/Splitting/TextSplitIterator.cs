namespace ScrubJay.Memory;

[PublicAPI]
public ref struct TextSplitIterator : ISpanSplitIterator<char>
{
    // The span being split
    private readonly ReadOnlySpan<char> _span;

    /// The single-item separator for <see cref="SeparatorKind.Item"/>
    private readonly char _separator;

    /// <see cref="SeparatorKind.Span"/>: A multi-item separator<br/>
    /// <see cref="SeparatorKind.AnySpan"/>: Multiple single-item separators
    private readonly ReadOnlySpan<char> _separators;

    /// How are we storing the separator(s)?
    private readonly SeparatorKind _separatorKind;

    /// Options for Splitting
    private readonly SplitOptions _options;

    private readonly StringComparison _comparison;

    /// Index to start scanning for the next item
    private int _scanStartIndex;

    public SplitOptions Options => _options;

    internal TextSplitIterator(
        ReadOnlySpan<char> span,
        char separator,
        SplitOptions options = SplitOptions.None,
        StringComparison comparison = StringComparison.Ordinal)
    {
        _span = span;
        _separator = separator;
        _separators = default;
        _separatorKind = SeparatorKind.Item;
        _options = options;
        _comparison = comparison;
        _scanStartIndex = 0;
    }

    internal TextSplitIterator(
        ReadOnlySpan<char> span,
        ReadOnlySpan<char> separators,
        SeparatorKind separatorKind,
        SplitOptions options = SplitOptions.None,
        StringComparison comparison = StringComparison.Ordinal)
    {
        _span = span;
        _separator = '\0';
        _separators = separators;
        Debug.Assert(separatorKind != SeparatorKind.None);
        _separatorKind = separators.Length == 0 ? SeparatorKind.Empty : separatorKind;
        _options = options;
        _comparison = comparison;
        _scanStartIndex = 0;
    }

    public bool TryMoveNext(out Segment<char> segment)
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
                index = _span[scan..].IndexOf(_separator.AsSpan(), _comparison);
                skipLength = 1;
                break;
            }
            case SeparatorKind.AnySpan:
            {
                index = _span[scan..].IndexOfAny(_separators, _comparison);
                skipLength = 1;
                break;
            }
            case SeparatorKind.Span:
            {
                index = _span[scan..].IndexOf(_separators, _comparison);
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
        segment = new Segment<char>(range, _span[range]);
        return true;
    }
}