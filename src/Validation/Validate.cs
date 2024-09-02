namespace ScrubJay.Validation;

/// <summary>
/// Validation methods that return <see cref="Result{O,E}">Result</see>&lt;T, <see cref="Exception"/>&gt;
/// </summary>
public static partial class Validate
{
    public static Result<int, Exception> Index(Index index, int length, [CallerArgumentExpression(nameof(index))] string? indexName = null, [CallerArgumentExpression(nameof(length))] string? lengthName = null)
    {
        if (length < 0)
            return new ArgumentOutOfRangeException(lengthName, length, "Length must be zero or greater");
        int offset = index.GetOffset(length);
        if (offset < 0 || offset >= length)
            return new ArgumentOutOfRangeException(indexName, index, $"Index must be in the range [0, {length})");
        return offset;
    }

    public static Result<int, Exception> InsertIndex(Index index, int length, [CallerArgumentExpression(nameof(index))] string? indexName = null, [CallerArgumentExpression(nameof(length))] string? lengthName = null)
    {
        if (length < 0)
            return new ArgumentOutOfRangeException(lengthName, length, "Length must be zero or greater");
        int offset = index.GetOffset(length);
        if (offset < 0 || offset > length)
            return new ArgumentOutOfRangeException(indexName, index, $"Insert Index must be in the range [0, {length}]");
        return offset;
    }

    public static Result<(int Offset, int Length), Exception> Range(Range range, int length, [CallerArgumentExpression(nameof(range))] string? rangeName = null, [CallerArgumentExpression(nameof(length))] string? lengthName = null)
    {
        if (length < 0)
            return new ArgumentOutOfRangeException(lengthName, length, "Length must be zero or greater");
        var (offset, count) = range.GetOffsetAndLength(length);
        if (offset < 0 || offset >= length)
            return new ArgumentOutOfRangeException(rangeName, range, $"Range must be in [0, {length})");
        return (offset, count);
    }


    public static Result<T, Exception> IsNotNull<T>([AllowNull] T? value, [CallerArgumentExpression(nameof(value))] string? valueName = null)
        where T : notnull
    {
        if (value is not null)
            return value;
        return new ArgumentNullException(valueName);
    }
}