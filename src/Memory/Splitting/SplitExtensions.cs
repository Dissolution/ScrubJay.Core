namespace ScrubJay.Memory;

public static class SplitExtensions
{
    extension(text text)
    {
        public TextSplitIterator Split(char separator) => new(text, separator);

        public TextSplitIterator Split(
            char separator,
            StringComparison comparison,
            SplitOptions options = SplitOptions.None)
            => new(text, separator, options, comparison);

        public TextSplitIterator Split(
            char separator,
            SplitOptions options,
            StringComparison comparison = StringComparison.Ordinal)
            => new(text, separator, options, comparison);

        public TextSplitIterator Split(text separator) => new(text, separator, SeparatorKind.Span);

        public TextSplitIterator Split(
            text separator,
            StringComparison comparison,
            SplitOptions options = SplitOptions.None)
            => new(text, separator, SeparatorKind.Span, options, comparison);

        public TextSplitIterator Split(
            text separator,
            SplitOptions options,
            StringComparison comparison = StringComparison.Ordinal)
            => new(text, separator, SeparatorKind.Span, options, comparison);

        public TextSplitIterator SplitAny(text separators) => new(text, separators, SeparatorKind.AnySpan);

        public TextSplitIterator SplitAny(
            text separators,
            StringComparison comparison,
            SplitOptions options = SplitOptions.None)
            => new(text, separators, SeparatorKind.AnySpan, options, comparison);

        public TextSplitIterator SplitAny(
            text separators,
            SplitOptions options,
            StringComparison comparison = StringComparison.Ordinal)
            => new(text, separators, SeparatorKind.AnySpan, options, comparison);
    }

    extension<T>(ReadOnlySpan<T> span)
        where T : IEquatable<T>
    {
        public SpanSplitIterator<T> Split(
            T separator,
            SplitOptions options = SplitOptions.None)
            => new(span, separator, options);

        public SpanSplitIterator<T> Split(
            ReadOnlySpan<T> separator,
            SplitOptions options = SplitOptions.None)
            => new(span, separator, SeparatorKind.Span, options);

        public SpanSplitIterator<T> SplitAny(
            ReadOnlySpan<T> separators,
            SplitOptions options = SplitOptions.None)
            => new(span, separators, SeparatorKind.AnySpan, options);
    }

    extension<T>(ReadOnlySpan<T> span)
    {
        public SpanSplitEqualityIterator<T> Split(
            T separator,
            IEqualityComparer<T>? comparer = null,
            SplitOptions options = SplitOptions.None)
            => new(span, separator, options, comparer);

        public SpanSplitEqualityIterator<T> Split(
            ReadOnlySpan<T> separator,
            IEqualityComparer<T>? comparer = null,
            SplitOptions options = SplitOptions.None)
            => new(span, separator, SeparatorKind.Span, options, comparer);

        public SpanSplitEqualityIterator<T> SplitAny(
            ReadOnlySpan<T> separators,
            IEqualityComparer<T>? comparer = null,
            SplitOptions options = SplitOptions.None)
            => new(span, separators, SeparatorKind.AnySpan,options, comparer);
    }
}