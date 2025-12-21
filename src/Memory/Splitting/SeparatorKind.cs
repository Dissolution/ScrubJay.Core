namespace ScrubJay.Memory;

/// <summary>
/// What kind of separator is being used in a <see cref="SpanSplitIterator{T}"/>
/// </summary>
internal enum SeparatorKind
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