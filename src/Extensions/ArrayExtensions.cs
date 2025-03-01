// extract assignment from expression

#pragma warning disable S1121

namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on 2D Arrays
/// </summary>
[PublicAPI]
public static class ArrayExtensions
{
#if NETFRAMEWORK || NETSTANDARD2_0
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
    public static TOutput[] ConvertAll<TInput, TOutput>(
        this TInput[] array,
        Converter<TInput, TOutput> converter)
        => Array.ConvertAll<TInput, TOutput>(array, converter);

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
        => array is null || array.Length == 0;

#if NETFRAMEWORK || NETSTANDARD2_0
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Slice<T>(this T[] array, int start) => array.AsSpan(start).ToArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Slice<T>(this T[] array, int start, int length) => array.AsSpan(start, length).ToArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Slice<T>(this T[] array, Range range) => array.AsSpan(range).ToArray();

    public static void Reverse<T>(this T[] array)
    {
        int end = array.Length - 1;

        T temp1;
        T temp2;
        for (int i = 0; i < end; --end)
        {
            temp1 = array[i];
            temp2 = array[end];
            array[i] = temp2;
            array[end] = temp1;
            ++i;
        }
    }
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Slice<T>(this T[] array, int start) => array[start..];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Slice<T>(this T[] array, int start, int length) => array[new Range(start, start + length)];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] Slice<T>(this T[] array, Range range) => array[range];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Reverse<T>(this T[] array) => Array.Reverse<T>(array);
#endif


}
