using static InlineIL.IL;

namespace ScrubJay.Text;

partial class TextHelper
{
    public static unsafe class Notsafe
    {
#region Copy

        /// <summary>
        /// Copies a specified <paramref name="count"/> of <see cref="char">chars</see>
        /// from a <paramref name="source"/> to a <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// The readonly reference (<c>in</c>) to the first character in the block to copy from
        /// </param>
        /// <param name="destination">
        /// The reference (<c>ref</c>) to the first character in the block to copy to
        /// </param>
        /// <param name="count">
        /// The total number of characters to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyCharBlock(in char source, ref char destination, int count)
        {
            // dest: void*, source: void*, byte_count: nuint
            Emit.Ldarg(nameof(destination));
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(count));

            // The count of characters must be converted to a count of bytes
            Emit.Sizeof<char>();
            Emit.Mul();

            // CopyBlock
            Emit.Cpblk();
        }


        /* All the public methods for CopyBlock allow for the most efficient conversion of
         * source + dest to what CpBlk is expecting
         *
         * Source types: `in char`, `char[]`, `Span<char>`, `ReadOnlySpan<char>`, `string`
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
            => Notsafe.CopyCharBlock(in source, ref destination, count);

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
#if NET5_0_OR_GREATER
            => Notsafe.CopyCharBlock(in source, ref MemoryMarshal.GetArrayDataReference<char>(destination), count);
#else
        {
            Notsafe.CopyCharBlock(
                in source,
                ref MemoryMarshal.GetReference<char>(destination),
                count);
        }
#endif

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
            => Notsafe.CopyCharBlock(in source, ref MemoryMarshal.GetReference<char>(destination), count);

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
#if NET5_0_OR_GREATER
            => Notsafe.CopyCharBlock(in MemoryMarshal.GetArrayDataReference<char>(source), ref destination, count);
#else
        {
            CopyCharBlock(
                in MemoryMarshal.GetReference<char>(source),
                ref destination,
                count);
        }
#endif

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
#if NET5_0_OR_GREATER
            => CopyCharBlock(in MemoryMarshal.GetArrayDataReference<char>(source),
                ref MemoryMarshal.GetArrayDataReference<char>(destination), count);
#else
        {
            CopyCharBlock(
                in MemoryMarshal.GetReference<char>(source),
                ref MemoryMarshal.GetReference<char>(destination),
                count);
        }
#endif

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
#if NET5_0_OR_GREATER
            => CopyCharBlock(in MemoryMarshal.GetArrayDataReference<char>(source),
                ref MemoryMarshal.GetReference<char>(destination), count);
#else
        {
            CopyCharBlock(
                in MemoryMarshal.GetReference<char>(source),
                ref MemoryMarshal.GetReference<char>(destination),
                count);
        }
#endif

        /// <summary>
        /// Copies a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// A <c>Span&lt;char&gt;</c> to be read from
        /// </param>
        /// <param name="destination">
        /// A <c>ref char</c> reference to the start of a writeable text buffer
        /// </param>
        /// <param name="count">
        /// The total number of <see cref="char">characters</see> to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        public static void CopyBlock(Span<char> source, ref char destination, int count)
            => CopyCharBlock(in MemoryMarshal.GetReference<char>(source), ref destination, count);

        /// <summary>
        /// Copies a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// A <c>Span&lt;char&gt;</c> to be read from
        /// </param>
        /// <param name="destination">
        /// A character array (<c>char[]</c>) to be written to
        /// </param>
        /// <param name="count">
        /// The total number of <see cref="char">characters</see> to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(Span<char> source, char[] destination, int count)
#if NET5_0_OR_GREATER
            => CopyCharBlock(in MemoryMarshal.GetReference<char>(source),
                ref MemoryMarshal.GetArrayDataReference<char>(destination), count);
#else
        {
            CopyCharBlock(
                in MemoryMarshal.GetReference<char>(source),
                ref MemoryMarshal.GetReference<char>(destination),
                count);
        }
#endif

        /// <summary>
        /// Copies a block of <see cref="char">characters</see> from <paramref name="source"/> to <paramref name="destination"/>
        /// </summary>
        /// <param name="source">
        /// A <c>Span&lt;char&gt;</c> to be read from
        /// </param>
        /// <param name="destination">
        /// A character span (<c>Span&lt;char&gt;</c>) to be written to
        /// </param>
        /// <param name="count">
        /// The total number of <see cref="char">characters</see> to copy from <paramref name="source"/> to <paramref name="destination"/>
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(Span<char> source, Span<char> destination, int count)
            => CopyCharBlock(in MemoryMarshal.GetReference<char>(source), ref MemoryMarshal.GetReference<char>(destination),
                count);

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
            => CopyCharBlock(in MemoryMarshal.GetReference<char>(source), ref destination, count);

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
#if NET5_0_OR_GREATER
            => CopyCharBlock(in MemoryMarshal.GetReference<char>(source),
                ref MemoryMarshal.GetArrayDataReference<char>(destination), count);
#else
        {
            CopyCharBlock(
                in MemoryMarshal.GetReference<char>(source),
                ref MemoryMarshal.GetReference<char>(destination),
                count);
        }
#endif

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
            => CopyCharBlock(in MemoryMarshal.GetReference<char>(source), ref MemoryMarshal.GetReference<char>(destination),
                count);

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
#if NET5_0_OR_GREATER
            => CopyCharBlock(in MemoryMarshal.GetReference<char>(source), ref destination, count);
#else
        {
            CopyCharBlock(
                in MemoryMarshal.GetReference<char>(source.AsSpan()),
                ref destination,
                count);
        }
#endif

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
#if NET5_0_OR_GREATER
            => CopyCharBlock(in MemoryMarshal.GetReference<char>(source),
                ref MemoryMarshal.GetArrayDataReference<char>(destination), count);
#else
        {
            CopyCharBlock(
                in MemoryMarshal.GetReference<char>(source.AsSpan()),
                ref MemoryMarshal.GetReference<char>(destination),
                count);
        }
#endif

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
#if NET5_0_OR_GREATER
            => CopyCharBlock(in MemoryMarshal.GetReference<char>(source), ref MemoryMarshal.GetReference<char>(destination),
                count);
#else
        {
            CopyCharBlock(
                in MemoryMarshal.GetReference<char>(source.AsSpan()),
                ref MemoryMarshal.GetReference<char>(destination),
                count);
        }


        public static void SelfCopy(Span<char> chars, Range source, Range destination)
        {
            int length = chars.Length;
            (int sourceOffset, int sourceLen) = source.UnsafeGetOffsetAndLength(length);
            Debug.Assert(sourceOffset >= 0);
            Debug.Assert(sourceLen >= 0);
            Debug.Assert(sourceLen <= length);
            (int destinationOffset, int destinationLength) = destination.UnsafeGetOffsetAndLength(length);
            Debug.Assert(destinationOffset >= 0);
            Debug.Assert(destinationLength >= 0);
            Debug.Assert(destinationLength <= length);
            Debug.Assert(destinationLength >= sourceLen);
            ref char src = ref chars[sourceOffset];
            ref char dst = ref chars[destinationOffset];
            CopyCharBlock(in src, ref dst, sourceLen);
        }

#endif

#endregion

#region Init

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InitCharBlock(ref char source, int count)
        {
            /*         Instruction      ||  Stack */
            Emit.Ldarg(nameof(source)); //  char*
            Emit.Ldc_I4_0(); //             char* | 0_i32
            Emit.Ldarg(nameof(count)); //   char* | 0_i32 | count_i32
            Emit.Ldc_I4_1(); //             char* | 0_i32 | count_i32 | 1_i32
            Emit.Shl(); //                  char* | 0_i32 | (count*2)_i32
            Emit.Initblk(); //              _
        }

#endregion

        // REALLY BAD
        public static Span<char> AsWritableSpan(text text)
        {
            return new Span<char>(Utilities.Notsafe.InAsVoidPtr(in text.GetPinnableReference()), text.Length);
        }

        public static Span<char> AsWritableSpan(string? str)
        {
            if (str!.IsEmpty)
                return default;

            return new Span<char>(Utilities.Notsafe.InAsVoidPtr(in str.GetPinnableReference()), str.Length);
        }
    }

#region Init

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear(scoped Span<char> chars)
        => Notsafe.InitCharBlock(ref MemoryMarshal.GetReference(chars), chars.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear(char[]? chars)
    {
        if (chars is not null)
        {
#if NET5_0_OR_GREATER
            Notsafe.InitCharBlock(ref MemoryMarshal.GetArrayDataReference(chars), chars.Length);
#else
                Notsafe.InitCharBlock(ref chars[0], chars.Length);
#endif
        }
    }

#endregion
}