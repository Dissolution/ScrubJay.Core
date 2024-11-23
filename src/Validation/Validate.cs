﻿#pragma warning disable S907, S3236, S3247

using ScrubJay.Constraints;
using Unit = ScrubJay.Functional.Unit;

namespace ScrubJay.Validation;

/// <summary>
/// Methods that validate arguments to return a <see cref="Result{TOk,TError}"/>
/// </summary>
/// <remarks>
/// Validation methods all have a similar shape:<br/>
/// <code>
/// Result&lt;T, Exception&gt; METHOD(T+ values, T+ limits, string+ valueNames)
/// </code>
/// Where there are one or more input <i>values</i> being validated,<br/>
/// one or more <i>limits</i> to compare those <i>values</i> against,<br/>
/// and then a series of automatically captured <i>value names</i> used for any thrown <see cref="Exception">Exceptions</see>.<br/>
/// Validation is only performed on <i>values</i>, not on <i>limits</i>.<br/>
/// </remarks>
[PublicAPI]
[StackTraceHidden]
public static class Validate
{
#region Index

    /// <summary>
    /// Validates if an <paramref name="index"/> is valid for a sequence with <paramref name="length"/>
    /// </summary>
    /// <param name="index">
    /// The <c>int</c> index that must be within [0, <paramref name="length"/>)
    /// </param>
    /// <param name="length">
    /// The length of the sequence
    /// </param>
    /// <param name="indexName">The name of the <paramref name="index"/> parameter, captured automatically</param>
    /// <returns>
    /// A <see cref="Result{TOk,TError}">Result&lt;int, Exception&gt;</see> that contains a valid <c>int</c> offset
    /// or an <see cref="Exception"/> describing why the index was invalid
    /// </returns>
    public static Result<int, Exception> Index(
        int index,
        int length,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        if (index >= 0 && index < length)
            return index;
        return new ArgumentOutOfRangeException(indexName, index, $"{indexName} '{index}' must be in [0..{length})");
    }

    /// <summary>
    /// Validates if an <see cref="System.Index"/> is valid for a sequence with <paramref name="length"/>
    /// </summary>
    /// <param name="index">
    /// The <see cref="System.Index"/> that must be within [0, <paramref name="length"/>)
    /// </param>
    /// <param name="length">
    /// The length of the sequence
    /// </param>
    /// <param name="indexName">The name of the <paramref name="index"/> parameter, captured automatically</param>
    /// <returns>
    /// A <see cref="Result{TOk,TError}">Result&lt;int, Exception&gt;</see> that contains a valid <c>int</c> offset
    /// or an <see cref="Exception"/> describing why the index was invalid
    /// </returns>
    public static Result<int, Exception> Index(
        Index index,
        int length,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        int offset = index.GetOffset(length);
        if (offset >= 0 && offset < length)
            return offset;
        return new ArgumentOutOfRangeException(indexName, index, $"{indexName} '{index}' must be in [0..{length})");
    }

    public static Result<int, Exception> InsertIndex(
        int index,
        int length,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        if (index >= 0 && index <= length)
            return index;
        return new ArgumentOutOfRangeException(indexName, index, $"{indexName} '{index}' must be in [0..{length}]");
    }

    public static Result<int, Exception> InsertIndex(
        Index index,
        int length,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        int offset = index.GetOffset(length);
        if (offset >= 0 && offset <= length)
            return offset;
        return new ArgumentOutOfRangeException(indexName, index, $"{indexName} '{index}' must be in [0..{length}]");
    }

#endregion

    /// <summary>
    /// Validates that a Range specified with an <paramref name="index"/> and <paramref name="length"/> fits within an <paramref name="available"/> count
    /// </summary>
    /// <param name="index">Validated<br/>
    /// The inclusive <c>int</c> starting index for the Range
    /// </param>
    /// <param name="length">Validated<br/>
    /// The total number of items covered by the Range
    /// </param>
    /// <param name="available"><b>NOT</b> Validated<br/>
    /// The total number of items available
    /// </param>
    /// <param name="indexName">The name of the <paramref name="index"/> parameter, automatically captured</param>
    /// <param name="lengthName">The name of the <paramref name="length"/> parameter, automatically captured</param>
    /// <returns>
    /// A <see cref="Result{TOk,TError}">Result&lt;(int Offset, int Length), Exception&gt;</see> that contains a valid start Offset and Length<br/>
    /// or an <see cref="Exception"/> that describes the first validation failure
    /// </returns>
    public static Result<(int Offset, int Length), Exception> IndexLength(
        int index,
        int length,
        int available,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null,
        [CallerArgumentExpression(nameof(length))]
        string? lengthName = null)
    {
        if (index < 0 || index > available)
            return new ArgumentOutOfRangeException(indexName, index, $"{indexName} '{index}' must be in [0..{available})");
        if (length < 0 || index + length > available)
            return new ArgumentOutOfRangeException(lengthName, length, $"{indexName} '{index}' + {lengthName} '{length}' must be in [0..{available}]");
        return (index, length);
    }

    /// <summary>
    /// Validates that a Range specified with an <paramref name="index"/> and <paramref name="length"/> fits within an <paramref name="available"/> count
    /// </summary>
    /// <param name="index">Validated<br/>
    /// The inclusive starting <see cref="System.Index"/> for the Range
    /// </param>
    /// <param name="length">Validated<br/>
    /// The total number of items covered by the Range
    /// </param>
    /// <param name="available"><b>NOT</b> Validated<br/>
    /// The total number of items available
    /// </param>
    /// <param name="indexName">The name of the <paramref name="index"/> parameter, automatically captured</param>
    /// <param name="lengthName">The name of the <paramref name="length"/> parameter, automatically captured</param>
    /// <returns>
    /// A <see cref="Result{TOk,TError}">Result&lt;(int Offset, int Length), Exception&gt;</see> that contains a valid start Offset and Length<br/>
    /// or an <see cref="Exception"/> that describes the first validation failure
    /// </returns>
    public static Result<(int Offset, int Length), Exception> IndexLength(
        Index index,
        int length,
        int available,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null,
        [CallerArgumentExpression(nameof(length))]
        string? lengthName = null)
    {
        int offset = index.GetOffset(available);
        if (offset < 0 || offset > available)
            return new ArgumentOutOfRangeException(indexName, index, $"{indexName} '{index}' must be in [0..{available}]");
        if (length < 0 || offset + length > available)
            return new ArgumentOutOfRangeException(lengthName, length, $"{indexName} '{index}' + {lengthName} '{length}' must be in [0..{available}]");
        return (offset, length);
    }

    /// <summary>
    /// Validates that a specified <see cref="System.Range"/> fits within an <paramref name="available"/> count
    /// </summary>
    /// <param name="range">Validated<br/>
    /// The <see cref="System.Range"/> to validate against an <paramref name="available"/> count
    /// </param>
    /// <param name="available"><b>NOT</b> Validated<br/>
    /// The total number of items available
    /// </param>
    /// <param name="rangeName">The name of the <paramref name="range"/> parameter, automatically captured</param>
    /// <returns>
    /// A <see cref="Result{TOk,TError}">Result&lt;(int Offset, int Length), Exception&gt;</see> that contains a valid start Offset and Length<br/>
    /// or an <see cref="Exception"/> that describes the first validation failure
    /// </returns>
    public static Result<(int Offset, int Length), Exception> Range(
        Range range,
        int available,
        [CallerArgumentExpression(nameof(range))]
        string? rangeName = null)
    {
        int start = range.Start.GetOffset(available);
        if (start < 0 || start > available)
            return new ArgumentOutOfRangeException(rangeName, range, $"{rangeName} '{range}' must be in [0..{available}]");

        int end = range.End.GetOffset(available);
        if (end < start || end > available)
            return new ArgumentOutOfRangeException(rangeName, range, $"{rangeName} '{range}' must be in [0..{available}]");

        return (start, end - start);
    }


    public static Result<T, Exception> IsNotNull<T>(
        [NotNullWhen(true)] T? value,
        string? message = null,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
        where T : notnull
    {
        if (value is not null)
            return OkEx(value); // do not implicitly cast in case value is Exception
        return new ArgumentNullException(valueName, message);
    }

    public static Result<T, Exception> IsNotNull<T>(
        // ReSharper disable once ConvertNullableToShortForm
        [NotNullWhen(true)] Nullable<T> value,
        string? message = null,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
        where T : struct
    {
        if (value.HasValue)
            return value.GetValueOrDefault();
        return new ArgumentNullException(valueName, message);
    }

    public static Result<T[], Exception> IsNotEmpty<T>(
        [NotNullWhen(true)] T[]? array,
        [CallerArgumentExpression(nameof(array))]
        string? arrayName = null)
    {
        if (array is null)
            return new ArgumentNullException(arrayName);
        if (array.Length == 0)
            return new ArgumentException("Array cannot be empty", arrayName);
        return array;
    }

    public static Result<Unit, Exception> IsNotEmpty<T>(
        Span<T> span,
        [CallerArgumentExpression(nameof(span))]
        string? spanName = null)
    {
        if (span.Length == 0)
            return new ArgumentException("Span cannot be empty", spanName);
        return Unit.Default;
    }

    public static Result<Unit, Exception> IsNotEmpty<T>(
        ReadOnlySpan<T> span,
        [CallerArgumentExpression(nameof(span))]
        string? spanName = null)
    {
        if (span.Length == 0)
            return new ArgumentException("ReadOnlySpan cannot be empty", spanName);
        return Unit.Default;
    }

    public static Result<ICollection<T>, Exception> IsNotEmpty<T>(
        ICollection<T>? collection,
        [CallerArgumentExpression(nameof(collection))]
        string? collectionName = null)
    {
        if (collection is null)
            return new ArgumentNullException(collectionName);
        if (collection.Count == 0)
            return new ArgumentException("Collection cannot be empty", collectionName);
        return OkEx(collection);
    }


    public static Result<T, Exception> Is<T>(
        [NotNullWhen(true)] object? obj,
        [CallerArgumentExpression(nameof(obj))]
        string? objectName = null)
    {
        if (obj is null)
            return new ArgumentNullException(objectName);
        if (obj is T)
            return OkEx((T)obj);
        return new ArgumentException($"{objectName} '{obj}' is not a {typeof(T)}", objectName);
    }

    public static Result<T, Exception> CanBe<T>(
        object? obj,
        [CallerArgumentExpression(nameof(obj))]
        string? objectName = null)
    {
        if (obj is T)
        {
            return OkEx((T)obj);
        }

        // the only value that can be null is Nullable<>
        // but any non valueTypes (class, interface) can be null
        if (obj is null && (!typeof(T).IsValueType || Nullable.GetUnderlyingType(typeof(T)) is not null))
            return OkEx(default(T)!);

        return new ArgumentException($"{objectName} '{obj}' does not contain a {typeof(T)}", objectName);
    }


    public static Result<int, Exception> InLength(
        int value,
        int exclusiveLength,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        if (value >= 0 && value < exclusiveLength)
            return value;
        return new ArgumentOutOfRangeException(valueName, value, $"{valueName} '{value}' must be in [0, {exclusiveLength})");
    }

    public static Result<int, Exception> InRange(
        int value,
        Range range,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        if (!range.Start.IsFromEnd)
        {
            int lower = range.Start.Value;
            if (value < lower)
                goto FAIL;
        }
        else
        {
            Debugger.Break();
            throw new NotImplementedException();
        }

        if (!range.End.IsFromEnd)
        {
            int upper = range.End.Value;
            if (value >= upper)
                goto FAIL;
        }
        else
        {
            Debugger.Break();
            throw new NotImplementedException();
        }

        return value;

        FAIL:
        return new ArgumentOutOfRangeException(valueName, value, $"{valueName} '{value}' was not in {range}");
    }


    public static Result<T, Exception> InBounds<T>(T value, Bounds<T> bounds, [CallerArgumentExpression(nameof(value))] string? valueName = null)
    {
        if (bounds.Contains(value))
            return value;
        return new ArgumentOutOfRangeException(valueName, value, $"{valueName} '{value}' was not in {bounds}");
    }

    public static Result<T, Exception> InBounds<T>(
        T value,
        Bound<T> lowerBound,
        Bound<T> upperBound,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
        => InBounds<T>(value, Bounds.Create(lowerBound, upperBound), valueName);

    public static Result<T, Exception> InInclusiveLength<T>(
        T value,
        T inclusiveMaximum,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        int c = Comparer<T>.Default.Compare(value, default!);
        if (c < 0)
            goto FAIL;
        c = Comparer<T>.Default.Compare(value, inclusiveMaximum);
        if (c > 0)
            goto FAIL;
        return value;
    FAIL:
        return new ArgumentOutOfRangeException(valueName, value, $"{valueName} '{value}' must be in [0..{inclusiveMaximum}]");
    }

    public static Result<T, Exception> InInclusiveLength<T>(
        T value,
        T inclusiveMaximum,
        IComparer<T>? valueComparer,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        valueComparer ??= Comparer<T>.Default;

        int c = valueComparer.Compare(value, default!);
        if (c < 0)
            goto FAIL;
        c = valueComparer.Compare(value, inclusiveMaximum);
        if (c > 0)
            goto FAIL;
        return value;
    FAIL:
        return new ArgumentOutOfRangeException(valueName, value, $"{valueName} '{value}' must be in [0..{inclusiveMaximum}]");
    }

    public static Result<T, Exception> InExclusiveLength<T>(
        T value,
        T exclusiveMaximum,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        int c = Comparer<T>.Default.Compare(value, default!);
        if (c < 0)
            goto FAIL;
        c = Comparer<T>.Default.Compare(value, exclusiveMaximum);
        if (c >= 0)
            goto FAIL;
        return value;
    FAIL:
        return new ArgumentOutOfRangeException(valueName, value, $"{valueName} '{value}' must be in [0..{exclusiveMaximum})");
    }

    public static Result<T, Exception> InExclusiveLength<T>(
        T value,
        T exclusiveMaximum,
        IComparer<T>? valueComparer,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        valueComparer ??= Comparer<T>.Default;

        int c = valueComparer.Compare(value, default!);
        if (c < 0)
            goto FAIL;
        c = valueComparer.Compare(value, exclusiveMaximum);
        if (c >= 0)
            goto FAIL;
        return value;
    FAIL:
        return new ArgumentOutOfRangeException(valueName, value, $"{valueName} '{value}' must be in [0..{exclusiveMaximum})");
    }


    public static Result<T, Exception> IsGreaterOrEqualThan<T>(T value, T minInclusive, IComparer<T>? valueComparer = null, [CallerArgumentExpression(nameof(value))] string? valueName = null)
    {
        int c = (valueComparer ?? Comparer<T>.Default).Compare(value, minInclusive);
        if (c >= 0)
            return value;
        return new ArgumentOutOfRangeException(valueName, value, $"{valueName} '{value}' must be >= {minInclusive}");
    }


    [Obsolete("Old Validate Pattern", true)]
    public static Result<Unit, Exception> CopyTo<T>(
        int count,
        T[]? array,
        int arrayIndex = 0,
        [CallerArgumentExpression(nameof(count))]
        string? countName = null,
        [CallerArgumentExpression(nameof(array))]
        string? arrayName = null,
        [CallerArgumentExpression(nameof(arrayIndex))]
        string? arrayIndexName = null)
    {
        return new Validations
        {
            IsGreaterOrEqualThan(count, 0, null, countName),
            IsNotNull(array, arrayName),
            InBounds(arrayIndex, Bounds.ForLength(array!.Length), arrayIndexName),
            () =>
            {
                if (count + arrayIndex <= array!.Length)
                    return Unit();
                return new ArgumentOutOfRangeException(arrayName, array, $"Cannot fit {count} items into [{array.Length}]{(arrayIndex == 0 ? "" : $"[{arrayIndex}..]")}");
            },
        }.GetResult();
    }

    public static Result<Unit, Exception> CopyTo<T>(
        T[]? array,
        int arrayIndex,
        int count,
        [CallerArgumentExpression(nameof(array))]
        string? arrayName = null,
        [CallerArgumentExpression(nameof(arrayIndex))]
        string? arrayIndexName = null)
    {
        return
            from arr in IsNotNull(array, arrayName)
            from arrIndex in InRange(arrayIndex, ..arr.Length, arrayIndexName)
            from _ in ErrorIf(
                count + arrIndex > arr.Length,
                () => new ArgumentOutOfRangeException(
                    arrayName, arr,
                    $"Cannot fit {count} items into [{arr.Length}]{(arrayIndex == 0 ? "" : $"[{arrayIndex}..]")}"))
            select Unit.Default;
    }

    public static Result<Unit, Exception> ErrorIf(
        bool predicate,
        Func<Exception> exception)
    {
        if (predicate)
            return exception();
        return Unit();
    }

    public static Result<T, Exception> ErrorIf<T>(
        T value,
        Func<T, bool> predicate,
        Func<Exception> exception)
    {
        if (predicate(value))
            return exception();
        return OkEx(value);
    }
}