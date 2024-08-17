namespace ScrubJay.Extensions;

public static class ArrayExtensions
{
    public static TOutput[] ConvertAll<TInput, TOutput>([NotNull] this TInput[] array, [NotNull] Converter<TInput, TOutput> converter)
    {
        return Array.ConvertAll<TInput, TOutput>(array, converter);
    }
    
    public static Result<T, Exception> TryGet<T>(this T[]? array, 
        int index,
        [CallerArgumentExpression(nameof(array))]
        string? arrayName = null,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        if (array is null)
            return new ArgumentNullException(arrayName);
        if (array.Length == 0)
            return new ArgumentException($"{arrayName} is empty", arrayName);
        if (index < 0 || index >= array.Length)
            return new ArgumentOutOfRangeException(indexName, index, $"{indexName} must be between 0 and {array.Length - 1}");
        return array[index];
    }
    
    public static bool TryGet<T>(this T[]? array, 
        int index,
        [MaybeNullWhen(false)] out T item,
        [CallerArgumentExpression(nameof(array))]
        string? arrayName = null,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        if (array is null || array.Length == 0)
        {
            item = default;
            return false;
        }
        if (index < 0 || index >= array.Length)
        {
            item = default;
            return false;
        }
        item = array[index];
        return true;
    }
    
    public static OkExResult TrySet<T>(this T[]? array, 
        int index,
        T item,
        [CallerArgumentExpression(nameof(array))]
        string? arrayName = null,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        if (array is null)
            return new ArgumentNullException(arrayName);
        if (array.Length == 0)
            return new ArgumentException($"{arrayName} is empty", arrayName);
        if (index < 0 || index >= array.Length)
            return new ArgumentOutOfRangeException(indexName, index, $"{indexName} must be between 0 and {array.Length - 1}");
        array[index] = item;
        return Ok;
    }
}