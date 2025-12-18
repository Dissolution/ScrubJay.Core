namespace ScrubJay.Validation;

partial class Ex
{
    public static IndexOutOfRangeException Index(
        int index,
        int available,
        InterpolatedTextBuilder info = default,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        string message = TextBuilder
            .New
            .Append($"An {indexName ?? "Index"} of `{index}` does not fit in [0..{available})")
            .IfNotEmpty(info, static (tb, n) => tb.Append(": ").Append(ref n))
            .ToStringAndDispose();
        return new IndexOutOfRangeException(message, innerException);
    }

    public static IndexOutOfRangeException Index(
        Index index,
        int available,
        InterpolatedTextBuilder info = default,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        string message = TextBuilder
            .New
            .Append($"An {indexName ?? "Index"} of `{index}` does not fit in [0..{available})")
            .IfNotEmpty(info, static (tb, n) => tb.Append(": ").Append(ref n))
            .ToStringAndDispose();
        return new IndexOutOfRangeException(message, innerException);
    }

    public static IndexOutOfRangeException Index(
        StackIndex index,
        int available,
        InterpolatedTextBuilder info = default,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        string message = TextBuilder
            .New
            .Append($"An {indexName ?? "Index"} of `{index}` does not fit in [0..{available})")
            .IfNotEmpty(info, static (tb, n) => tb.Append(": ").Append(ref n))
            .ToStringAndDispose();
        return new IndexOutOfRangeException(message, innerException);
    }
}