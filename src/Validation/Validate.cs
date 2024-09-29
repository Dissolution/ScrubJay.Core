#pragma warning disable S907, S3236

using ScrubJay.Collections;

namespace ScrubJay.Validation;

/// <summary>
/// Validation methods that return <see cref="Result{O,E}">Result</see>&lt;T, <see cref="Exception"/>&gt;
/// </summary>
public static partial class Validate
{
#region Index

    public static Result<int, Exception> Index(int index, int length, [CallerArgumentExpression(nameof(index))] string? indexName = null, [CallerArgumentExpression(nameof(length))] string? lengthName = null)
    {
        return new Validations()
        {
            IsGreaterOrEqualThan(length, 0, valueName: lengthName),
            InBounds(index, Bounds.ForLength(length), indexName),
        }.GetResult(index);
    }

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

#endregion

    public static Result<(int Offset, int Length), Exception> Range(int offset, int length, 
        int available,
        [CallerArgumentExpression(nameof(offset))] string? offsetName = null, 
        [CallerArgumentExpression(nameof(length))] string? lengthName = null, 
        [CallerArgumentExpression(nameof(available))] string? availableName = null)
    {
        return new Validations
        {
            IsGreaterOrEqualThan(available, 0, valueName: availableName),
            InInclusiveLength(offset, available, valueName: offsetName),
            IsGreaterOrEqualThan(length, 0, valueName: lengthName),
            InInclusiveLength(offset + length, available, valueName: "Offset + Length"),
        }.GetResult((offset, length));
    }

    public static Result<(int Offset, int Length), Exception> Range(Index index, int length, int available, [CallerArgumentExpression(nameof(index))] string? indexName = null, [CallerArgumentExpression(nameof(length))] string? lengthName = null, [CallerArgumentExpression(nameof(available))] string? availableName = null)
    {
        if (available < 0)
            return new ArgumentOutOfRangeException(availableName, available, "Available must be zero or greater");
        int offset = index.GetOffset(length);
        if (offset < 0 || offset >= available)
            return new ArgumentOutOfRangeException(indexName, index, $"Index must be in the range [0, {available})");
        if (length < 0 || offset + length > available)
            return new ArgumentOutOfRangeException(lengthName, length, $"Offset + Length must be in [0, {available}]");
        return (offset, length);
    }

    public static Result<(int Offset, int Length), Exception> Range(Range range, int length, [CallerArgumentExpression(nameof(range))] string? rangeName = null, [CallerArgumentExpression(nameof(length))] string? lengthName = null)
    {
        if (length < 0)
            return new ArgumentOutOfRangeException(lengthName, length, "Length must be zero or greater");
        var (offset, count) = range.GetOffsetAndLength(length);
        if (offset >= 0 && offset < length)
            return (offset, count);
        return new ArgumentOutOfRangeException(rangeName, range, $"Range must be in [0, {length})");
    }

    public static Result<(int Offset, int Length), Exception> Range<T>(Range range, ReadOnlySpan<T> span, [CallerArgumentExpression(nameof(range))] string? rangeName = null, [CallerArgumentExpression(nameof(span))] string? spanName = null)
    {
        int spanLen = span.Length;
        var (offset, count) = range.GetOffsetAndLength(spanLen);
        if (offset >= 0 && offset < spanLen)
            return (offset, count);
        return new ArgumentOutOfRangeException(rangeName, range, $"Range must be in [0, {spanLen})");
    }

    public static Result<(int Offset, int Length), Exception> Range<T>(Range range, T[]? array, [CallerArgumentExpression(nameof(range))] string? rangeName = null, [CallerArgumentExpression(nameof(array))] string? arrayName = null)
    {
        if (array is null)
            return new ArgumentNullException(arrayName);
        int arrayLen = array.Length;
        var (offset, count) = range.GetOffsetAndLength(arrayLen);
        if (offset >= 0 && offset < arrayLen)
            return (offset, count);
        return new ArgumentOutOfRangeException(rangeName, range, $"Range must be in [0, {arrayLen})");
    }

    public static Result<(int Offset, int Length), Exception> Range<T>(Range range, ICollection<T>? collection, [CallerArgumentExpression(nameof(range))] string? rangeName = null, [CallerArgumentExpression(nameof(collection))] string? collectionName = null)
    {
        if (collection is null)
            return new ArgumentNullException(collectionName);
        int collectionCount = collection.Count;
        var (offset, count) = range.GetOffsetAndLength(collectionCount);
        if (offset >= 0 && offset < collectionCount)
            return (offset, count);
        return new ArgumentOutOfRangeException(rangeName, range, $"Range must be in [0, {collectionCount})");
    }


    public static Result<T, Exception> IsNotNull<T>([NotNullWhen(true)] T? value, [CallerArgumentExpression(nameof(value))] string? valueName = null)
        where T : notnull
    {
        if (value is not null)
            return value;
        return new ArgumentNullException(valueName);
    }
    
    public static Result<T[], Exception> IsNotEmpty<T>(
        [NotNullWhen(true)] T[]? array, 
        [CallerArgumentExpression(nameof(array))] string? arrayName = null)
    {
        if (array is null)
            return new ArgumentNullException(arrayName);
        if (array.Length == 0)
            return new ArgumentException("Array cannot be empty", arrayName);
        return array;
    }
    
    // todo: eventual support for Result<ReadOnlySpan<>,>
    public static Result<Unit, Exception> IsNotEmpty<T>(
        ReadOnlySpan<T> span,
        [CallerArgumentExpression(nameof(span))] string? spanName = null)
    {
        if (span.Length == 0)
            return new ArgumentException("Span cannot be empty", spanName);
        return Unit.Default;
    }
    
    // todo: eventual support for Result<ReadOnlySpan<>,>
    public static Result<ICollection<T>, Exception> IsNotEmpty<T>(
        ICollection<T>? collection,
        [CallerArgumentExpression(nameof(collection))] string? collectionName = null)
    {
        if (collection is null)
            return new ArgumentNullException(collectionName);
        if (collection.Count == 0)
            return new ArgumentException("Collection cannot be empty", collectionName);
        return Ok(collection);
    }


    public static Result<T, Exception> InBounds<T>(T value, Bounds<T> bounds, [CallerArgumentExpression(nameof(value))] string? valueName = null)
    {
        if (bounds.Contains(value))
            return value;
        return new ArgumentOutOfRangeException(valueName, value, $"{valueName} '{value}' was not in {bounds}");
    }

    public static Result<T, Exception> InInclusiveLength<T>(
        T value,
        T inclusiveMaximum,
        IComparer<T>? valueComparer = null,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        valueComparer ??= Comparer<T>.Default;

        int c = valueComparer.Compare(value, default(T)!);
        if (c < 0)
            goto FAIL;
        c = valueComparer.Compare(value, inclusiveMaximum);
        if (c > 0)
            goto FAIL;
        return value;
    FAIL:
        return new ArgumentOutOfRangeException(valueName, value, $"{valueName} '{value}' must be in [0, {inclusiveMaximum}]");
    }

    public static Result<T, Exception> InExclusiveLength<T>(
        T value,
        T exclusiveMaximum,
        IComparer<T>? valueComparer = null,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        valueComparer ??= Comparer<T>.Default;

        int c = valueComparer.Compare(value, default(T)!);
        if (c < 0)
            return new ArgumentOutOfRangeException(valueName, value, $"{valueName} '{value}' must be in [0, {exclusiveMaximum})");
        c = valueComparer.Compare(value, exclusiveMaximum);
        if (c >= 0)
            return new ArgumentOutOfRangeException(valueName, value, $"{valueName} '{value}' must be in [0, {exclusiveMaximum})");
        return value;
    }


    public static Result<T, Exception> IsGreaterOrEqualThan<T>(T value, T minInclusive, IComparer<T>? valueComparer = null, [CallerArgumentExpression(nameof(value))] string? valueName = null)
    {
        int c = (valueComparer ?? Comparer<T>.Default).Compare(value, minInclusive);
        if (c >= 0)
            return value;
        return new ArgumentOutOfRangeException(valueName, value, $"{valueName} '{value}' must be >= {minInclusive}");
    }


    public static Result<Unit, Exception> Args(Validations validations) => validations.GetResult();


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
        }.GetResult();
    }
}