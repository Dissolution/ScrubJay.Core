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
    
    public static Result<T, Exception> TryGet<T>(this T[]? array, Index index, [CallerArgumentExpression(nameof(array))] string? arrayName = null, [CallerArgumentExpression(nameof(index))] string? indexName = null)
    {
        return Validate
            .IsNotNull(array, arrayName)
            .MapOk(arr => Validate.Index(index, arr.Length, indexName))
            .MapOk(offset => array![offset]);
    }
    
    public static Result<T, Exception> TrySet<T>(this T[]? array, 
        Index index,
        T item,
        [CallerArgumentExpression(nameof(array))]
        string? arrayName = null,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        return Validate
            .IsNotNull(array, arrayName)
            .MapOk(arr => Validate.Index(index, arr.Length, indexName))
            .MapOk(offset => array![offset] = item);
    }
}