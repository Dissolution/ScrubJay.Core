namespace ScrubJay.Extensions;

[PublicAPI]
public static class ArrayExtensions
{
#if NET48_OR_GREATER || NETSTANDARD2_0
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpan<T>(this T[]? array, Range range)
    {
        if (array is null)
            return [];
        (int start, int length) = range.GetOffsetAndLength(array.Length);
        return new Span<T>(array, start, length);
    }
#endif


    /// <inheritdoc cref="Array.ConvertAll{TInput,TOutput}"/>
    public static TOutput[] ConvertAll<TInput, TOutput>(this TInput[] array, Converter<TInput, TOutput> converter)
    {
        return Array.ConvertAll<TInput, TOutput>(array, converter);
    }

    public static Result<T, Exception> TryGet<T>(
        this T[]? array,
        Index index,
        [CallerArgumentExpression(nameof(array))]
        string? arrayName = null,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        return
            from arr in Validate.IsNotNull(array, arrayName)
            from idx in Validate.Index(index, arr.Length, indexName)
            select arr[idx];
    }

    public static Result<T, Exception> TrySet<T>(
        this T[]? array,
        Index index,
        T item,
        [CallerArgumentExpression(nameof(array))]
        string? arrayName = null,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        return
            from arr in Validate.IsNotNull(array, arrayName)
            from idx in Validate.Index(index, arr.Length, indexName)
            select (arr[idx] = item);
    }

    /// <summary>
    /// Returns <c>true</c> if <paramref name="array"/> is <c>null</c> or has a Length of 0
    /// </summary>
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this T[]? array)
    {
        return array is null || array.Length == 0;
    }
}