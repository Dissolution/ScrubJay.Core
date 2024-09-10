namespace ScrubJay.Extensions;

[PublicAPI]
public static class ArrayExtensions
{
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