using ScrubJay.Collections;

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


    public static Result<T, Exception> IsNotNull<T>([NotNullWhen(true)] T? value, [CallerArgumentExpression(nameof(value))] string? valueName = null)
        where T : notnull
    {
        if (value is not null)
            return value;
        return new ArgumentNullException(valueName);
    }


    public static Result<T, Exception> InBounds<T>(T value, Bounds<T> bounds, [CallerArgumentExpression(nameof(value))] string? valueName = null)
    {
        if (bounds.Contains(value))
            return value;
        return new ArgumentOutOfRangeException(valueName, value, $"{valueName} '{value}' was not in {bounds}");
    }

    public static Result<T, Exception> IsGreaterOrEqualThan<T>(T value, T minInclusive, IComparer<T>? valueComparer = null, [CallerArgumentExpression(nameof(value))] string? valueName = null)
    {
        int c = (valueComparer ?? Comparer<T>.Default).Compare(value, minInclusive);
        if (c >= 0)
            return value;
        return new ArgumentOutOfRangeException(valueName, value, $"{valueName} '{value}' must be >= {minInclusive}");
    }

    public static Result<Unit, Exception> Args(Validations validations) => validations.Result;


    public static Result<Unit, Exception> CopyTo<T>(int count, T[]? array, int arrayIndex = 0, [CallerArgumentExpression(nameof(count))] string? countName = null, [CallerArgumentExpression(nameof(array))] string? arrayName = null, [CallerArgumentExpression(nameof(arrayIndex))] string? arrayIndexName = null)
    {
        return new Validations
        {
            IsGreaterOrEqualThan(count, 0, null, countName),
            IsNotNull(array, arrayName),
            InBounds(arrayIndex, Bounds.ForLength(array!.Length)),
            () =>
            {
                if (count + arrayIndex <= array!.Length)
                    return Unit();
                return new ArgumentOutOfRangeException(arrayName, array, $"Cannot fit {count} items into [{array.Length}]{(arrayIndex == 0 ? "" : $"[{arrayIndex}..]")}");
            },
        }.Result;
    }
}