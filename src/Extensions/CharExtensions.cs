#pragma warning disable CA1822 // Mark members as static

namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="char"/>
/// </summary>
[PublicAPI]
public static class CharExtensions
{
    extension(char)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAsciiHexDigitLower(char ch)
        {
#if NET7_0_OR_GREATER
            return char.IsAsciiHexDigitLower(ch);
#else
            return (uint)(ch - '0') <= 9 || (uint)(ch - 'a') <= ('f' - 'a');
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAsciiHexDigitUpper(char ch)
        {
#if NET7_0_OR_GREATER
            return char.IsAsciiHexDigitUpper(ch);
#else
            return (uint)(ch - '0') <= 9 || (uint)(ch - 'A') <= ('F' - 'A');
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAsciiHexDigit(char ch)
        {
#if NET7_0_OR_GREATER
            return char.IsAsciiHexDigit(ch);
#else
            return (uint)(ch - '0') <= 9 || (uint)(ch - 'A') <= ('F' - 'A') || (uint)(ch - 'a') <= ('f' - 'a');
#endif
        }
    }

    extension(char ch)
    {
        /// <summary>
        /// Is this <see cref="char"/> valid <c>ASCII</c>?<br/>
        /// </summary>
        /// <remarks>
        /// Per <see href="http://www.unicode.org/glossary/#ASCII"/>, ASCII is the range U+0000..=U+007F
        /// </remarks>
        public bool IsAscii
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (uint)ch <= '\x007F';
        }
    }
}