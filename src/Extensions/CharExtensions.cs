namespace ScrubJay.Extensions;

[PublicAPI]
public static class CharExtensions
{
    /// <summary>
    /// Returns <see langword="true"/> if <paramref name="ch"/> is an ASCII
    /// character ([ U+0000..U+007F ]).
    /// </summary>
    /// <remarks>
    /// Per http://www.unicode.org/glossary/#ASCII, ASCII is only U+0000..U+007F.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAscii(this char ch) => (uint)ch <= '\x007F';
}
