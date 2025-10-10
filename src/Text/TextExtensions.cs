using ScrubJay.Text.Comparison;
#pragma warning disable IDE0022

namespace ScrubJay.Text;

/// <summary>
/// Extensions on <see cref="char"/>, <see cref="string"/>, <see cref="text">ReadOnlySpan&lt;char&gt;</see>, and other textual types
/// </summary>
[PublicAPI]
public static class TextExtensions
{
#if NETFRAMEWORK || NETSTANDARD
    /// <summary>
    /// Gets a pinned <c>ref readonly</c> to this <see cref="string"/>
    /// </summary>
    public static ref readonly char GetPinnableReference(this string str)
    {
        unsafe
        {
            fixed (char* strPtr = str)
            {
                return ref Notsafe.PtrAsRef<char>(strPtr);
            }
        }
    }
#endif

    /// <summary>
    /// Converts this <see cref="char"/> into a <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see> containing it
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static text AsSpan(this in char ch)
    {
#if NET7_0_OR_GREATER
        return new text(in ch);
#else
        unsafe
        {
            fixed (char* pointer = &ch)
            {
                return new text(pointer, 1);
            }
        }
#endif
    }

#region AsString()

    /// <summary>
    /// Gets this <see cref="char"/> as a <see cref="string"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AsString(this char ch) => new string(ch, 1);

    /// <summary>
    /// Gets this <see cref="Span{T}">Span&lt;char&gt;</see> as a <see cref="string"/>
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
    /// Efficiently convert a <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see> to a <see cref="string"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string AsString(this scoped text text)
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
    /// Efficiently convert a <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;char&gt;</see> to a <see cref="string"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(characters))]
    public static string? AsString(this char[]? characters)
    {
        if (characters is null)
            return null;
        return new(characters);
    }

    [return: NotNullIfNotNull(nameof(list))]
    public static string? AsString(this IList<char>? list)
    {
        if (list is null)
            return null;

#if NETFRAMEWORK || NETSTANDARD2_0
        Span<char> span = stackalloc char[list.Count];
        for (int i = list.Count - 1; i >= 0; i--)
        {
            span[i] = list[i];
        }
        return span.AsString();
#else
        return string.Create(list.Count, list, static (span, chars) =>
        {
            for (int i = chars.Count - 1; i >= 0; i--)
            {
                span[i] = chars[i];
            }
        });
#endif
    }

    [return: NotNullIfNotNull(nameof(characters))]
    public static string? AsString(this IEnumerable<char>? characters)
    {
        if (characters is null)
            return null;

        if (characters is ICollection<char> collection)
        {
            char[] chars = new char[collection.Count];
            collection.CopyTo(chars, 0);
            return new string(chars);
        }

        using var builder = new TextBuilder();
        foreach (char ch in characters)
        {
            builder.Append(ch);
        }

        return builder.ToString();
    }

#endregion

#region Matches

    public static bool Matches(this char ch, char other, StringMatch match)
        => Matches(ch.AsSpan(), other.AsSpan(), match);

    public static bool Matches(this string? str, string? value, StringMatch match)
        => Matches(str.AsSpan(), value.AsSpan(), match);

    public static bool Matches(this text text, string? value, StringMatch match)
        => Matches(text, value.AsSpan(), match);

    public static bool Matches(this text text, text value, StringMatch match)
    {
        var comparison = match.StringComparison;

        // Always match on exact
        if (text.Equate(value, comparison))
            return true;
        if (match.Contains && TextHelper.Contains(text, value, comparison))
            return true;
        if (match.StartsWith && TextHelper.StartsWith(text, value, comparison))
            return true;
        if (match.EndsWith && TextHelper.EndsWith(text, value, comparison))
            return true;

        // does not meet specifications
        return false;
    }

#endregion
}
