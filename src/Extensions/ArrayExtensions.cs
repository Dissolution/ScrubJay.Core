namespace ScrubJay.Extensions;

public static class ArrayExtensions
{
#region Array
    [return: NotNullIfNotNull(nameof(array))]
    public static Type? GetElementType(this Array? array)
    {
        if (array is null) return null;
        return array.GetType().GetElementType()!;
    }
#endregion

#region non-generic array
    /// <summary>
    /// Converts an array of <see cref="object"/>s to an array of their <see cref="Type"/>s.
    /// </summary>
    public static Type?[] ToTypeArray(this object?[]? objectArray, Type? nullType = null)
    {
        if (objectArray is null) return Type.EmptyTypes;
        int len = objectArray.Length;
        if (len == 0) return Type.EmptyTypes;
        var types = new Type?[len];
        for (var i = 0; i < len; i++)
        {
            types[i] = objectArray[i]?.GetType() ?? nullType;
        }
        return types;
    }
#endregion

#region T[]
    public static Type GetElementType<T>(this T[] array) => typeof(T);

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

    [return: NotNullIfNotNull(nameof(@default))]
    public static T? GetOrDefault<T>(this T[]? array, int index, T? @default = default)
    {
        if (array is null)
            return @default;
        if ((uint)index >= (uint)array.Length)
            return @default;
        return array[index];
    }

    public static Result TrySet<T>(this T[]? array,
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
        return Ok();
    }

#if NET481 || NETSTANDARD2_0
    public static Span<T> AsSpan<T>(this T[]? array, Range range)
    {
        (int offset, int length) = range.GetOffsetAndLength(array?.Length ?? 0);
        return array.AsSpan(offset, length);
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this T[]? array)
    {
        return array is null || array.Length == 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this T[]? array, out int length)
    {
        if (array is not null)
        {
            length = array.Length;
            return length > 0;
        }

        length = 0;
        return false;
    }

    public static bool Any<T>(this T[] array, Func<T, bool> predicate)
    {
        for (var i = 0; i < array.Length; i++)
        {
            if (predicate(array[i]))
                return true;
        }
        return false;
    }

    public static bool All<T>(this T[] array, Func<T, bool> predicate)
    {
        for (var i = 0; i < array.Length; i++)
        {
            if (!predicate(array[i]))
                return false;
        }
        return true;
    }

    public static bool Contains<T>(this T[]? array, T item)
    {
        if (array is null) return false;
        for (var i = 0; i < array.Length; i++)
        {
            if (EqualityComparer<T>.Default.Equals(array[i], item))
                return true;
        }
        return false;
    }

    public static bool Contains<T>(this T[]? array, T item, IEqualityComparer<T>? itemComparer)
    {
        if (array is null) return false;
        if (itemComparer is null) return array.Contains(item);
        for (var i = 0; i < array.Length; i++)
        {
            if (itemComparer.Equals(array[i], item))
                return true;
        }
        return false;
    }

    public static int FirstIndexOf<T>(this T[] array, T value)
    {
        for (var i = 0; i < array.Length; i++)
        {
            if (EqualityComparer<T>.Default.Equals(array[i], value))
                return i;
        }
        return -1;
    }

    /// <summary>
    /// Gets every possible pair of values in this array
    /// </summary>
    /// <param name="array"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<T[]> GetPossiblePairs<T>(this T[] array)
    {
        int len = array.Length;
        if (len < 2)
            yield break;

        for (var s = 0; s < len; s++)
        {
            for (int e = s + 1; e < len; e++)
            {
                yield return new T[2] { array[s], array[e] };
            }
        }
    }

    /// <summary>
    /// Initializes each element of this <paramref name="array"/> to the given <paramref name="value"/>
    /// </summary>
    public static void Initialize<T>(this T[] array, T value)
    {
        int len = array.Length;
        for (var i = 0; i < len; i++)
        {
            array[i] = value;
        }
    }

    public static IEnumerable<T> Reversed<T>(this T[] array)
    {
        for (int i = array.Length - 1; i >= 0; i--)
        {
            yield return array[i];
        }
    }

    public static void CopyTo<T>(this T[] array, T[] dest)
        => array.AsSpan().CopyTo(dest.AsSpan());

    public static ref T GetPinnableReference<T>(this T[] array)
    {
#if NET6_0_OR_GREATER
        return ref MemoryMarshal.GetArrayDataReference<T>(array);
#else
        return ref array[0];
#endif
    }
#endregion
}