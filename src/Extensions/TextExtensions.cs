namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see> and
/// <see cref="IEnumerable{T}">IEnumerable&lt;char&gt;</see>
/// </summary>
[PublicAPI]
public static class TextExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AsString(this string? str) => str ?? string.Empty;
    
    /// <summary>
    /// Gets this <see cref="text"/> as a <see cref="string"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AsString(this scoped Span<char> text)
    {
#if NETFRAMEWORK || NETSTANDARD2_0
        unsafe
        {
            fixed (char* ptr = text)
            {
                return new string(ptr, 0, text.Length);
            }
        }
#else
        return new string(text);
#endif
    }
    
    /// <summary>
    /// Gets this <see cref="text"/> as a <see cref="string"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AsString(this scoped ReadOnlySpan<char> text)
    {
#if NETFRAMEWORK || NETSTANDARD2_0
        unsafe
        {
            fixed (char* ptr = text)
            {
                return new string(ptr, 0, text.Length);
            }
        }
#else
        return new string(text);
#endif
    }

    public static string AsString(this IEnumerable<char>? characters)
    {
        if (characters is null)
            return string.Empty;
        if (characters is ICollection<char> collection)
        {
            char[] chars = new char[collection.Count];
            collection.CopyTo(chars, 0);
            return new string(chars);
        }

        using var buffer = new Buffer<char>();
        foreach (var character in characters)
        {
            buffer.Add(character);
        }

        return buffer.Written.AsString();
    }
}