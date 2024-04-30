namespace ScrubJay.Extensions;

public static class ArrayExtensions
{
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
}