#pragma warning disable CA1034, CA1045, CA1062, CA1200
#pragma warning disable IDE0060, IDE0022
// ReSharper disable EntityNameCapturedOnly.Global
// ReSharper disable ArrangeMethodOrOperatorBody
// ReSharper disable InvokeAsExtensionMethod

using static InlineIL.IL;

namespace ScrubJay.Text;

/// <summary>
/// Methods for efficiently working on textual types (<see cref="char"/>, <see cref="string"/>, <see cref="text">ReadOnlySpan&lt;char&gt;</see>
/// </summary>
[PublicAPI]
public static class TextHelper
{
#region Unsafe
    /// <summary>
    /// <c>unsafe</c> methods on textual types
    /// </summary>
    /// <remarks>
    /// <b>WARNING</b>: All methods in <see cref="Notsafe"/> lack bounds checking and can cause undefined behavior
    /// </remarks>
    public static class Notsafe
    {
        /// <summary>
        /// Efficiently copy <see cref="char">characters</see> from a
        /// <c>in char</c> <paramref name="source"/> pointer
        /// to a <c>ref char</c> <paramref name="destination"/> pointer
        /// <br/>
        /// <b>Warning:</b> behavior is undefined if <paramref name="source"/> and <paramref name="destination"/> overlap
        /// </summary>
        /// <param name="source">
        /// A readonly reference to the starting <see cref="char"/> in the source text
        /// </param>
        /// <param name="destination">
        /// A reference to the starting <see cref="char"/> in the destination buffer
        /// </param>
        /// <param name="count">
        /// The number of <see cref="char"/>s to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        /// <remarks>
        /// <see cref="M:System.Runtime.CompilerServices.Unsafe.CopyBlock"/> has limitations:<br/>
        /// - It requires either <c>ref byte</c><br/>
        /// --- which requires a conversion from <see cref="char"/> to <see cref="byte"/><br/>
        /// --- and a conversion from <c>in byte</c> to <c>ref byte</c> (as a source <see cref="string"/> will only return <c>readonly</c> references)<br/>
        /// - Or <c>void*</c><br/>
        /// --- which requires <c>unsafe</c> and <c>fixed</c> to manipulate the pointers<br/>
        /// <br/>
        /// This version of CopyBlock restricts <paramref name="source"/> and <paramref name="destination"/> to <see cref="char"/><br/>
        /// and uses the <c>Cpblk</c> instruction directly<br/>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CpBlk(in char source, ref char destination, int count)
        {
            Emit.Ldarg(nameof(destination)); // dest ptr
            Emit.Ldarg(nameof(source)); // src ptr
            // Total byte count == (sizeof(char) * count) == 2*count
            Emit.Ldarg(nameof(count));
            Emit.Ldc_I4_2();
            Emit.Mul();
            // Cpblk -> takes dest*, source*, uint byteCount
            Emit.Cpblk();
        }

        /* All the public methods for CopyBlock allow for the most efficient conversion of
         * source + dest to what CpBlk is expecting
         *
         * Source types: `in char`, `char[]`, `ReadOnlySpan<char>`, `string`
         * Destination types: `ref char`, `char[]`, `Span<char>`
         */

        /// <summary>
        /// Copies a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// An <c>in char</c> reference to the start of some text
        /// </param>
        /// <param name="destination">
        /// A <c>ref char</c> reference to the start of a writeable text buffer
        /// </param>
        /// <param name="count">
        /// The total number of <see cref="char">characters</see> to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(in char source, ref char destination, int count)
        {
            CpBlk(in source, ref destination, count);
        }

        /// <summary>
        /// Copies a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// An <c>in char</c> reference to the start of some text
        /// </param>
        /// <param name="destination">
        /// A character array (<c>char[]</c>) to be written to
        /// </param>
        /// <param name="count">
        /// The total number of <see cref="char">characters</see> to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(in char source, char[] destination, int count)
        {
            CpBlk(in source, ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// An <c>in char</c> reference to the start of some text
        /// </param>
        /// <param name="destination">
        /// A character span (<c>Span&lt;char&gt;</c>) to be written to
        /// </param>
        /// <param name="count">
        /// The total number of <see cref="char">characters</see> to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(in char source, Span<char> destination, int count)
        {
            CpBlk(in source, ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// A character array (<c>char[]</c>) to be read from
        /// </param>
        /// <param name="destination">
        /// A <c>ref char</c> reference to the start of a writeable text buffer
        /// </param>
        /// <param name="count">
        /// The total number of <see cref="char">characters</see> to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(char[] source, ref char destination, int count)
        {
            CpBlk(in source.GetPinnableReference(), ref destination, count);
        }

        /// <summary>
        /// Copies a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// A character array (<c>char[]</c>) to be read from
        /// </param>
        /// <param name="destination">
        /// A character array (<c>char[]</c>) to be written to
        /// </param>
        /// <param name="count">
        /// The total number of <see cref="char">characters</see> to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(char[] source, char[] destination, int count)
        {
            CpBlk(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// A character array (<c>char[]</c>) to be read from
        /// </param>
        /// <param name="destination">
        /// A character span (<c>Span&lt;char&gt;</c>) to be written to
        /// </param>
        /// <param name="count">
        /// The total number of <see cref="char">characters</see> to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(char[] source, Span<char> destination, int count)
        {
            CpBlk(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// A <c>ReadOnlySpan&lt;char&gt;</c> to be read from
        /// </param>
        /// <param name="destination">
        /// A <c>ref char</c> reference to the start of a writeable text buffer
        /// </param>
        /// <param name="count">
        /// The total number of <see cref="char">characters</see> to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        public static void CopyBlock(text source, ref char destination, int count)
        {
            CpBlk(in source.GetPinnableReference(), ref destination, count);
        }

        /// <summary>
        /// Copies a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// A <c>ReadOnlySpan&lt;char&gt;</c> to be read from
        /// </param>
        /// <param name="destination">
        /// A character array (<c>char[]</c>) to be written to
        /// </param>
        /// <param name="count">
        /// The total number of <see cref="char">characters</see> to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(text source, char[] destination, int count)
        {
            CpBlk(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// A <c>ReadOnlySpan&lt;char&gt;</c> to be read from
        /// </param>
        /// <param name="destination">
        /// A character span (<c>Span&lt;char&gt;</c>) to be written to
        /// </param>
        /// <param name="count">
        /// The total number of <see cref="char">characters</see> to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(text source, Span<char> destination, int count)
        {
            CpBlk(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// A <see cref="string"/> to be read from
        /// </param>
        /// <param name="destination">
        /// A <c>ref char</c> reference to the start of a writeable text buffer
        /// </param>
        /// <param name="count">
        /// The total number of <see cref="char">characters</see> to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(string source, ref char destination, int count)
        {
            CpBlk(in source.GetPinnableReference(), ref destination, count);
        }

        /// <summary>
        /// Copies a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// A <see cref="string"/> to be read from
        /// </param>
        /// <param name="destination">
        /// A character array (<c>char[]</c>) to be written to
        /// </param>
        /// <param name="count">
        /// The total number of <see cref="char">characters</see> to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(string source, char[] destination, int count)
        {
            CpBlk(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }

        /// <summary>
        /// Copies a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// A <see cref="string"/> to be read from
        /// </param>
        /// <param name="destination">
        /// A character span (<c>Span&lt;char&gt;</c>) to be written to
        /// </param>
        /// <param name="count">
        /// The total number of <see cref="char">characters</see> to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(string source, Span<char> destination, int count)
        {
            CpBlk(in source.GetPinnableReference(), ref destination.GetPinnableReference(), count);
        }
    }
#endregion

#region Copy + TryCopy
    /* All *Copy methods validate inputs before calling Notsafe.CopyBlock
     * Source Types: char, string?, ReadOnlySpan<char>, char[]?
     * Dest.  Types: Span<char>, char[]?
     */

    public static bool TryCopy(char source, Span<char> destination)
    {
        if (destination.Length == 0)
            return false;
        destination[0] = source;
        return true;
    }

    public static bool TryCopy(char source, char[]? destination)
    {
        if (destination is null || destination.Length == 0)
            return false;
        destination[0] = source;
        return true;
    }

    public static bool TryCopy(string? source, Span<char> destination)
    {
        if (source is null)
            return true;
        int count = source.Length;
        if (count <= destination.Length)
        {
            Notsafe.CopyBlock(source, destination, count);
            return true;
        }
        return false;
    }

    public static bool TryCopy(string? source, char[]? destination)
    {
        if (source is null)
            return true;
        if (destination is null)
            return false;
        int count = source.Length;
        if (count <= destination.Length)
        {
            Notsafe.CopyBlock(source, destination, count);
            return true;
        }
        return false;
    }

    public static bool TryCopy(text source, Span<char> destination)
    {
        int count = source.Length;
        if (count <= destination.Length)
        {
            Notsafe.CopyBlock(source, destination, count);
            return true;
        }
        return false;
    }

    public static bool TryCopy(text source, char[]? destination)
    {
        if (destination is null)
            return false;
        int count = source.Length;
        if (count <= destination.Length)
        {
            Notsafe.CopyBlock(source, destination, count);
            return true;
        }
        return false;
    }

    public static bool TryCopy(Span<char> source, Span<char> destination)
    {
        int count = source.Length;
        if (count <= destination.Length)
        {
            Notsafe.CopyBlock(source, destination, count);
            return true;
        }
        return false;
    }

    public static bool TryCopy(Span<char> source, char[]? destination)
    {
        if (destination is null)
            return false;
        int count = source.Length;
        if (count <= destination.Length)
        {
            Notsafe.CopyBlock(source, destination, count);
            return true;
        }
        return false;
    }

    public static bool TryCopy(char[]? source, Span<char> destination)
    {
        if (source is null)
            return true;
        int count = source.Length;
        if (count <= destination.Length)
        {
            Notsafe.CopyBlock(source, destination, count);
            return true;
        }
        return false;
    }

    public static bool TryCopy(char[]? source, char[]? destination)
    {
        if (source is null)
            return true;
        if (destination is null)
            return false;
        int count = source.Length;
        if (count <= destination.Length)
        {
            Notsafe.CopyBlock(source, destination, count);
            return true;
        }
        return false;
    }
#endregion


#region Compare
#region
    public static int Compare(string? left, string? right) => string.CompareOrdinal(left, right);

    public static int Compare(string? left, text right) => MemoryExtensions.SequenceCompareTo<char>(left.AsSpan(), right);

    public static int Compare(string? left, char[]? right) => MemoryExtensions.SequenceCompareTo<char>(left.AsSpan(), right);

    public static int Compare(text left, string? right) => MemoryExtensions.SequenceCompareTo<char>(left, right.AsSpan());

    public static int Compare(text left, text right) => MemoryExtensions.SequenceCompareTo<char>(left, right);

    public static int Compare(text left, char[]? right) => MemoryExtensions.SequenceCompareTo<char>(left, right);

    public static int Compare(char[]? left, string? right) => MemoryExtensions.SequenceCompareTo<char>(left, right.AsSpan());

    public static int Compare(char[]? left, text right) => MemoryExtensions.SequenceCompareTo<char>(left, right);

    public static int Compare(char[]? left, char[]? right) => MemoryExtensions.SequenceCompareTo<char>(left, right);
#endregion

#region w/StringComparison
    public static int Compare(string? left, string? right, StringComparison comparison) => string.Compare(left, right, comparison);

    public static int Compare(string? left, text right, StringComparison comparison) => MemoryExtensions.CompareTo(left.AsSpan(), right, comparison);

    public static int Compare(string? left, char[]? right, StringComparison comparison) => MemoryExtensions.CompareTo(left.AsSpan(), right, comparison);

    public static int Compare(text left, string? right, StringComparison comparison) => MemoryExtensions.CompareTo(left, right.AsSpan(), comparison);

    public static int Compare(text left, text right, StringComparison comparison) => MemoryExtensions.CompareTo(left, right, comparison);

    public static int Compare(text left, char[]? right, StringComparison comparison) => MemoryExtensions.CompareTo(left, right, comparison);

    public static int Compare(char[]? left, string? right, StringComparison comparison) => MemoryExtensions.CompareTo(left, right.AsSpan(), comparison);

    public static int Compare(char[]? left, text right, StringComparison comparison) => MemoryExtensions.CompareTo(left, right, comparison);

    public static int Compare(char[]? left, char[]? right, StringComparison comparison) => MemoryExtensions.CompareTo(left, right, comparison);
#endregion
#endregion

#region Equals
#region
    public static bool Equal(string? left, string? right)
        => string.Equals(left, right, StringComparison.Ordinal);

    public static bool Equal(string? left, text right)
        => MemoryExtensions.SequenceEqual<char>(left.AsSpan(), right);

    public static bool Equal(string? left, char[]? right)
        => MemoryExtensions.SequenceEqual<char>(left.AsSpan(), right);

    public static bool Equal(text left, string? right)
        => MemoryExtensions.SequenceEqual<char>(left, right.AsSpan());

    public static bool Equal(text left, text right)
        => MemoryExtensions.SequenceEqual<char>(left, right);

    public static bool Equal(text left, char[]? right)
        => MemoryExtensions.SequenceEqual<char>(left, right);

    public static bool Equal(char[]? left, string? right)
        => MemoryExtensions.SequenceEqual<char>(left, right.AsSpan());

    public static bool Equal(char[]? left, text right)
        => MemoryExtensions.SequenceEqual<char>(left, right);

    public static bool Equal(char[]? left, char[]? right)
        => MemoryExtensions.SequenceEqual<char>(left, right);
#endregion
#region w/StringComparison
    public static bool Equal(string? left, string? right, StringComparison comparison) => string.Equals(left, right, comparison);

    public static bool Equal(string? left, text right, StringComparison comparison) => MemoryExtensions.Equals(left.AsSpan(), right, comparison);

    public static bool Equal(string? left, char[]? right, StringComparison comparison) => MemoryExtensions.Equals(left.AsSpan(), right, comparison);

    public static bool Equal(text left, string? right, StringComparison comparison) => MemoryExtensions.Equals(left, right.AsSpan(), comparison);

    public static bool Equal(text left, text right, StringComparison comparison) => MemoryExtensions.Equals(left, right, comparison);

    public static bool Equal(text left, char[]? right, StringComparison comparison) => MemoryExtensions.Equals(left, right, comparison);

    public static bool Equal(char[]? left, string? right, StringComparison comparison) => MemoryExtensions.Equals(left, right.AsSpan(), comparison);

    public static bool Equal(char[]? left, text right, StringComparison comparison) => MemoryExtensions.Equals(left, right, comparison);

    public static bool Equal(char[]? left, char[]? right, StringComparison comparison) => MemoryExtensions.Equals(left, right, comparison);
#endregion
#endregion

    #region Contains

    public static bool Contains(string? str, char match, StringComparison comparison = StringComparison.Ordinal)
    {
        if (str is null)
            return false;
        return str.Contains(match, comparison);
    }

    public static bool Contains(string? str, string? match, StringComparison comparison = StringComparison.Ordinal)
    {
        if (str is null || match is null)
            return false;
        return MemoryExtensions.Contains(str.AsSpan(), match.AsSpan(), comparison);
    }

    public static bool Contains(string? str, text match, StringComparison comparison = StringComparison.Ordinal)
    {
        if (str is null)
            return false;
        return MemoryExtensions.Contains(str.AsSpan(), match, comparison);
    }

    public static bool Contains(text text, char match, StringComparison comparison = StringComparison.Ordinal)
    {
        return MemoryExtensions.Contains(text, match.AsSpan(), comparison);
    }

    public static bool Contains(text text, string? match, StringComparison comparison = StringComparison.Ordinal)
    {
        if (match is null)
            return false;
        return MemoryExtensions.Contains(text, match.AsSpan(), comparison);
    }

    public static bool Contains(text text, text match, StringComparison comparison = StringComparison.Ordinal)
    {
        return MemoryExtensions.Contains(text, match, comparison);
    }
    #endregion

    #region StartsWith
    public static bool StartsWith(string? str, string? match, StringComparison comparison = StringComparison.Ordinal)
    {
        if (str is null || match is null)
            return false;
        return MemoryExtensions.StartsWith(str.AsSpan(), match.AsSpan(), comparison);
    }

    public static bool StartsWith(string? str, text match, StringComparison comparison = StringComparison.Ordinal)
    {
        if (str is null)
            return false;
        return MemoryExtensions.StartsWith(str.AsSpan(), match, comparison);
    }

    public static bool StartsWith(text text, string? match, StringComparison comparison = StringComparison.Ordinal)
    {
        if (match is null)
            return false;
        return MemoryExtensions.StartsWith(text, match.AsSpan(), comparison);
    }

    public static bool StartsWith(text text, text match, StringComparison comparison = StringComparison.Ordinal)
    {
        return MemoryExtensions.StartsWith(text, match, comparison);
    }

    #endregion

#region EndsWith
    public static bool EndsWith(string? str, string? match, StringComparison comparison = StringComparison.Ordinal)
    {
        if (str is null || match is null)
            return false;
        return MemoryExtensions.EndsWith(str.AsSpan(), match.AsSpan(), comparison);
    }

    public static bool EndsWith(string? str, text match, StringComparison comparison = StringComparison.Ordinal)
    {
        if (str is null)
            return false;
        return MemoryExtensions.EndsWith(str.AsSpan(), match, comparison);
    }

    public static bool EndsWith(text text, string? match, StringComparison comparison = StringComparison.Ordinal)
    {
        if (match is null)
            return false;
        return MemoryExtensions.EndsWith(text, match.AsSpan(), comparison);
    }

    public static bool EndsWith(text text, text match, StringComparison comparison = StringComparison.Ordinal)
    {
        return MemoryExtensions.EndsWith(text, match, comparison);
    }

#endregion



#region Misc.
    public static string Repeat(int count, char ch)
    {
        if (count < 0)
            return string.Empty;
        return new string(ch, count);
    }

    public static string Repeat(int count, scoped text text)
    {
        int textLength = text.Length;
        int totalLength = count * textLength;
        if (totalLength <= 0)
            return string.Empty;
        Span<char> buffer = stackalloc char[totalLength];
        int i = 0;
        do
        {
            Notsafe.CopyBlock(text, buffer[i..], textLength);
            i += textLength;
        } while (i < totalLength);
        return buffer.AsString();
    }
#endregion
}
