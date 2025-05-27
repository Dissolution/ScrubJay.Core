#pragma warning disable IDE0060, CS8500, CA1045, CA1034, CA1715, CA1724
// ReSharper disable InconsistentNaming

using InlineIL;
using static InlineIL.IL;

namespace ScrubJay.Utilities;

/// <summary>
/// Very <b>unsafe</b> methods, more dangerous than <see cref="Unsafe"/><br/>
/// Many of the methods in here have no validations of any kind
/// </summary>
/// <seealso href="https://github.com/ltrzesniewski/InlineIL.Fody/blob/master/src/InlineIL.Examples/UnsafeNet9.cs"/>
[PublicAPI]
public static unsafe class Notsafe
{
    /// <summary>
    /// <b>Very unsafe</b> methods that operate on <c>unmanaged</c> values
    /// </summary>
    public static class Unmanaged
    {
#region CopyTo
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CopyUnmanagedBlock<U>(in U source, ref U destination, int count)
            where U : unmanaged
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            Emit.Ldarg(nameof(destination));
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(count));
            Emit.Sizeof<U>();
            Emit.Mul();
            Emit.Cpblk();
        }

        /* Source types: `T[]`, `Span<T>`, `ReadOnlySpan<T>`
         * Destination types: `T[]`, `Span<T>`
         */


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo<U>(U[] source, U[] destination, int count)
            where U : unmanaged
            => CopyUnmanagedBlock<U>(
#if NET5_0_OR_GREATER
                in MemoryMarshal.GetArrayDataReference<U>(source),
                ref MemoryMarshal.GetArrayDataReference<U>(destination),
#else
                in MemoryMarshal.GetReference<U>(source),
                ref MemoryMarshal.GetReference<U>(destination),
#endif
                count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo<U>(U[] source, Span<U> destination, int count)
            where U : unmanaged
            => CopyUnmanagedBlock<U>(
#if NET5_0_OR_GREATER
                in MemoryMarshal.GetArrayDataReference<U>(source),
#else
                in MemoryMarshal.GetReference<U>(source),
#endif
                ref MemoryMarshal.GetReference<U>(destination),
                count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo<U>(Span<U> source, U[] destination, int count)
            where U : unmanaged
            => CopyUnmanagedBlock<U>(
                in MemoryMarshal.GetReference<U>(source),
#if NET5_0_OR_GREATER
                ref MemoryMarshal.GetArrayDataReference<U>(destination),
#else
                ref MemoryMarshal.GetReference<U>(destination),
#endif
                count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo<U>(Span<U> source, Span<U> destination, int count)
            where U : unmanaged
            => CopyUnmanagedBlock<U>(
                in MemoryMarshal.GetReference<U>(source),
                ref MemoryMarshal.GetReference<U>(destination),
                count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo<U>(ReadOnlySpan<U> source, U[] destination, int count)
            where U : unmanaged
            => CopyUnmanagedBlock<U>(
                in MemoryMarshal.GetReference<U>(source),
#if NET5_0_OR_GREATER
                ref MemoryMarshal.GetArrayDataReference<U>(destination),
#else
                ref MemoryMarshal.GetReference<U>(destination),
#endif
                count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo<U>(ReadOnlySpan<U> source, Span<U> destination, int count)
            where U : unmanaged
            => CopyUnmanagedBlock<U>(
                in MemoryMarshal.GetReference<U>(source),
                ref MemoryMarshal.GetReference<U>(destination),
                count);

        /* And the below is just crazy */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CopyBlock<USource, UDest>(in USource source, ref UDest destination, int count)
            where USource : unmanaged
            where UDest : unmanaged
        {
            Debug.Assert(SizeOf<USource>() == SizeOf<UDest>());
            Emit.Ldarg(nameof(destination));
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(count));
            Emit.Sizeof<USource>();
            Emit.Mul();
            Emit.Cpblk();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo<USource, UDest>(ReadOnlySpan<USource> source, Span<UDest> destination, int count)
            where USource : unmanaged
            where UDest : unmanaged
        {
            Debug.Assert(SizeOf<USource>() == SizeOf<UDest>());
            CopyBlock<USource, UDest>(
                in MemoryMarshal.GetReference<USource>(source),
                ref MemoryMarshal.GetReference<UDest>(destination),
                count);
        }
#endregion

#region Arrays
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T* ArrayAlloc<T>(int size)
            where T : unmanaged
            => (T*)Marshal.AllocHGlobal(size * sizeof(T));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArrayFree<T>(T* array)
            where T : unmanaged
            => Marshal.FreeHGlobal((IntPtr)array);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArrayClear<T>(T* array, int size)
            where T : unmanaged
        {
            Unsafe.InitBlock(array, 0, (uint)(size * sizeof(T)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ArrayCopy<T>(T* src, T* dst, int size)
            where T : unmanaged
        {
            CopyBlock(in PtrAsIn(src), ref PtrAsRef(dst), size * sizeof(T));
        }
#endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero<U>(U value)
            where U : unmanaged
        {
            Emit.Ldarg(nameof(value));
            Emit.Ldc_I4_0();
            Emit.Ceq();
            return Return<bool>();
        }
    }

    /// <summary>
    /// <b>Very unsafe</b> methods that operate on <see cref="byte"/> values
    /// </summary>
    public static class Bytes
    {
#region Copy
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CopyByteBlock(in byte source, ref byte destination, int count)
        {
            Emit.Ldarg(nameof(destination));
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(count));
            Emit.Cpblk();
        }

        /* Source types: `T[]`, `Span<T>`, `ReadOnlySpan<T>`
         * Destination types: `T[]`, `Span<T>`
         */


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo(byte[] source, byte[] destination, int count)
            => CopyByteBlock(
#if NET5_0_OR_GREATER
                in MemoryMarshal.GetArrayDataReference(source),
                ref MemoryMarshal.GetArrayDataReference(destination),
                count);
#else
                in MemoryMarshal.GetReference<byte>(source),
                ref MemoryMarshal.GetReference<byte>(destination),
                count);
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo(byte[] source, Span<byte> destination, int count)
            => CopyByteBlock(
#if NET5_0_OR_GREATER
                in MemoryMarshal.GetArrayDataReference<byte>(source),
#else
                in MemoryMarshal.GetReference<byte>(source),
#endif
                ref MemoryMarshal.GetReference(destination),
                count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo(Span<byte> source, byte[] destination, int count)
            => CopyByteBlock(
                in MemoryMarshal.GetReference<byte>(source),
#if NET5_0_OR_GREATER
                ref MemoryMarshal.GetArrayDataReference<byte>(destination),
#else
                ref MemoryMarshal.GetReference<byte>(destination),
#endif
                count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo(Span<byte> source, Span<byte> destination, int count)
            => CopyByteBlock(
                in MemoryMarshal.GetReference(source),
                ref MemoryMarshal.GetReference(destination),
                count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo(ReadOnlySpan<byte> source, byte[] destination, int count)
            => CopyByteBlock(
                in MemoryMarshal.GetReference(source),
#if NET5_0_OR_GREATER
                ref MemoryMarshal.GetArrayDataReference(destination),
#else
                ref MemoryMarshal.GetReference(source),
#endif
                count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo(ReadOnlySpan<byte> source, Span<byte> destination, int count)
            => CopyByteBlock(
                in MemoryMarshal.GetReference(source),
                ref MemoryMarshal.GetReference(destination),
                count);
#endregion





    }



    /// <summary>
    /// <c>unsafe</c> methods on textual types
    /// </summary>
    /// <remarks>
    /// <b>WARNING</b>: All methods in <see cref="Notsafe"/> lack bounds checking and can cause undefined behavior
    /// </remarks>
    public static class Text
    {
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
        private static void CopyCharBlock(in char source, ref char destination, int count)
        {
            /* Instruction                         Stack
             * -----------------------------       -----------*/
            Emit.Ldarg(nameof(destination)); // char*                        // destination pointer
            Emit.Ldarg(nameof(source)); // char* | char*                // source pointer
            Emit.Ldarg(nameof(count)); // char* | char* | i32          // count
            /* We need to convert the count of chars to the count of bytes
             * sizeof(char) == 2 // bytes
             * bytes = count * 2 // count * sizeof(char)
             *
             * There are two easy ways to do this
             *  Ldc_I4_2()                      // char* | char* | i32 | i32    // 2
             *  Mul()                           // char* | char* | i32          // count * 2
             * and the below, which is usually faster                           */
            Emit.Ldc_I4_1(); // char* | char* | i32 | i32    // 1
            Emit.Shl(); // char* | char* | i32          // count << 1
            Emit.Cpblk(); //                              // stack is empty

            /* OpCodes:
             * Ldc_I4_1     - https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.ldc_i4_1
             * Shl          - https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.shl
             * Cpblk        - https://learn.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.cpblk
             */
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
            => CopyCharBlock(in source, ref destination, count);

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
            => CopyCharBlock(in source, ref MemoryMarshal.GetArrayDataReference<char>(destination), count);
#else
        {
            CopyCharBlock(
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
            => CopyCharBlock(in source, ref MemoryMarshal.GetReference<char>(destination), count);

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
            => CopyCharBlock(in MemoryMarshal.GetArrayDataReference<char>(source), ref destination, count);
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
#endif
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullRef<T>([NotNullWhen(false)] ref readonly T source)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg_0();
        Emit.Ldc_I4_0();
        Emit.Conv_U();
        Emit.Ceq();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNonNullRef<T>([NotNullWhen(true)] ref readonly T source)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldc_I4_0();
        Emit.Ldarg_0();
        Emit.Ldc_I4_0();
        Emit.Conv_U();
        Emit.Ceq();
        Emit.Ceq();
        return Return<bool>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T NullRef<T>()
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldc_I4_0();
        Emit.Conv_U();
        return ref ReturnRef<T>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SizeOf<T>()
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Sizeof<T>();
        return Return<int>();
    }


#region Referencing
    /************************************
     * Roughly:
     * void*, T*, in T, ref T
     ************************************
     */

    // void* -> ?

    /// <summary>
    /// <c>void* -> T*</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* VoidPtrAsPtr<T>(void* voidPointer)
        where T : unmanaged
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        Emit.Ldarg(nameof(voidPointer));
        return ReturnPointer<T>();
    }

    /// <summary>
    /// <c>void* -> ref T</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T VoidPtrAsRef<T>(void* voidPointer)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(voidPointer));
        return ref ReturnRef<T>();
    }

    /// <summary>
    /// <c>void* -> in T</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly T VoidPtrAsIn<T>(void* voidPointer)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(voidPointer));
        return ref ReturnRef<T>();
    }

    // T* -> ?

    /// <summary>
    /// <c>T* -> void*</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* PtrAsVoidPtr<T>(T* sourcePointer)
        where T : unmanaged
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        Emit.Ldarg(nameof(sourcePointer));
        return ReturnPointer();
    }

    /// <summary>
    /// <c>T* -> ref T</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T PtrAsRef<T>(T* sourcePointer)
        where T : unmanaged
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        Emit.Ldarg(nameof(sourcePointer));
        return ref ReturnRef<T>();
    }

    /// <summary>
    /// <c>T* -> in T</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly T PtrAsIn<T>(T* sourcePointer)
        where T : unmanaged
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        Emit.Ldarg(nameof(sourcePointer));
        return ref ReturnRef<T>();
    }

    // in T -> ?

    /// <summary>
    /// <c>in T -> void*</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* InAsVoidPtr<T>(in T inValue)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(inValue));
        return ReturnPointer();
    }

    /// <summary>
    /// <c>in T -> T*</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* InAsPtr<T>(in T inValue)
        where T : unmanaged
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        Emit.Ldarg(nameof(inValue));
        return ReturnPointer<T>();
    }

    /// <summary>
    /// <c>in T -> ref T</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T InAsRef<T>(in T inValue)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(inValue));
        return ref ReturnRef<T>();
    }

    // ref T -> ?

    /// <summary>
    /// <c>ref T -> void*</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* RefAsVoidPtr<T>(ref T inValue)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(inValue));
        Emit.Conv_U();
        return ReturnPointer();
    }

    /// <summary>
    /// <c>ref T -> T*</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* RefAsPtr<T>(ref T inValue)
        where T : unmanaged
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        Emit.Ldarg(nameof(inValue));
        return ReturnPointer<T>();
    }

    /// <summary>
    /// <c>ref T -> in T</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly T RefAsIn<T>(ref T inValue)
#if NET9_0_OR_GREATER
        where T: allows ref struct
#endif
    {
        Emit.Ldarg(nameof(inValue));
        return ref ReturnRef<T>();
    }

    // out T -> ?
    // everything in here is so very, very dangerous

    public static ref T OutAsRef<T>(out T outValue)
#if NET9_0_OR_GREATER
        where T: allows ref struct
#endif
    {
        Emit.Ldarg(nameof(outValue));
        Emit.Ret();
        throw Unreachable();
    }

#endregion

#region Casting
    // object -> ?

    /// <summary>
    /// Unbox an <see cref="object"/> to a <typeparamref name="T"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Unbox<T>(object obj)
    {
        Emit.Ldarg_0();
        Emit.Unbox_Any<T>();
        return Return<T>();
    }

    /// <summary>
    /// Gets a <c>ref </c><typeparamref name="TStruct"/> to the contents of an <see cref="object"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref TStruct UnboxRef<TStruct>(object obj)
        //where TStruct : struct
    {
        Emit.Ldarg_0();
        Emit.Unbox<TStruct>();
        return ref ReturnRef<TStruct>();
    }

    public static ref T TryUnboxRef<T>(object obj)
    {
        Debug.Assert(obj is not null);
        Emit.Ldarg(nameof(obj));
        Emit.Isinst<T>();
        Emit.Brtrue("is_inst");
        // return a null ref
        Emit.Ldc_I4_0();
        Emit.Conv_U();
        Emit.Ret();
        MarkLabel("is_inst");
        Emit.Ldarg(nameof(obj));
        Emit.Unbox<T>();
        return ref ReturnRef<T>();
    }

    /// <summary>
    /// Casts an <see cref="object"/> to a <c>class</c>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TClass CastClass<TClass>(object obj)
        //where TClass : class
    {
        Emit.Ldarg_0();
        Emit.Castclass<TClass>();
        return Return<TClass>();
    }

    /// <summary>
    /// Returns the <typeparamref name="TIn"/> <paramref name="input"/> as a <typeparamref name="TOut"/> with no type checking
    /// </summary>
    /// <remarks>
    /// If <typeparamref name="TIn"/> and <typeparamref name="TOut"/> do not have the same size and layout,
    /// memory corruption, undefined behavior, and Exceptions may occur
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TOut As<TIn, TOut>(TIn input)
#if NET9_0_OR_GREATER
        where TIn : allows ref struct
        where TOut : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(input));
        return Return<TOut>();
    }

    /// <summary>
    /// Returns the <typeparamref name="TIn"/> <c>in</c> <paramref name="input"/> as a <c>ref readonly</c> <typeparamref name="TOut"/> with no type checking
    /// </summary>
    /// <remarks>
    /// If <typeparamref name="TIn"/> and <typeparamref name="TOut"/> do not have the same size and layout,
    /// memory corruption, undefined behavior, and Exceptions may occur
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly TOut InAs<TIn, TOut>(in TIn input)
#if NET9_0_OR_GREATER
        where TIn : allows ref struct
        where TOut : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(input));
        return ref ReturnRef<TOut>();
    }

    /// <summary>
    /// Returns the <typeparamref name="TIn"/> <c>ref</c> <paramref name="input"/> as a <c>ref</c> <typeparamref name="TOut"/> with no type checking
    /// </summary>
    /// <remarks>
    /// If <typeparamref name="TIn"/> and <typeparamref name="TOut"/> do not have the same size and layout,
    /// memory corruption, undefined behavior, and Exceptions may occur
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref TOut RefAs<TIn, TOut>(ref TIn input)
#if NET9_0_OR_GREATER
        where TIn : allows ref struct
        where TOut : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(input));
        return ref ReturnRef<TOut>();
    }

    /// <summary>
    /// Interpret the <see cref="Span{TIn}">Span&lt;TIn&gt;</see> <paramref name="input"/> as a <see cref="Span{TOut}">Span&lt;TOut&gt;</see>
    /// </summary>
    /// <remarks>
    /// Unless <typeparamref name="TIn"/> and <typeparamref name="TOut"/> have the same size and layout,
    /// exceptions can be thrown and possible memory corruption
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<TOut> SpanAs<TIn, TOut>(Span<TIn> input)
    {
        return new Span<TOut>(
            pointer: RefAsVoidPtr(ref input.GetPinnableReference()),
            length: input.Length);
    }

    /// <summary>
    /// Interpret the <see cref="ReadOnlySpan{TIn}">ReadOnlySpan&lt;TIn&gt;</see> <paramref name="input"/> as a <see cref="ReadOnlySpan{TOut}">ReadOnlySpan&lt;TOut&gt;</see>
    /// </summary>
    /// <remarks>
    /// Unless <typeparamref name="TIn"/> and <typeparamref name="TOut"/> have the same size and layout,
    /// exceptions can be thrown and possible memory corruption
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<TOut> SpanAs<TIn, TOut>(ReadOnlySpan<TIn> input)
    {
        return new ReadOnlySpan<TOut>(
            pointer: InAsVoidPtr(in input.GetPinnableReference()),
            length: input.Length);
    }
#endregion

    // crazy stuff below

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsReadOnlySpan<T>(object obj)
    {
        if (typeof(T).IsValueType)
        {
            return FastUnboxToSpan<T>(obj);
        }
        return new ReadOnlySpan<T>([(T)obj]);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ReadOnlySpan<T> FastUnboxToSpan<T>(object obj)
    {
        Emit.Ldarg(nameof(obj)); // load the object
        Emit.Unbox<T>(); // unbox it to a T*
#if NET7_0_OR_GREATER
        // call `new ReadOnlySpan<T>(ref T)`
        Emit.Newobj(MethodRef.Constructor(typeof(ReadOnlySpan<T>), typeof(T).MakeByRefType()));
#else
        // have to call `new ReadOnlySpan<T>(void*, int)`
        Emit.Ldc_I4_1(); // count == 1
        Emit.Newobj(MethodRef.Constructor(typeof(ReadOnlySpan<T>), typeof(void*), typeof(int)));
#endif
        Emit.Ret();
        throw Unreachable();
    }


#region Reference Offsetting
    /// <summary>
    /// Adds an element offset to the given reference.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T Add<T>(ref T source, int elementOffset)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(source));
        Emit.Ldarg(nameof(elementOffset));
        Emit.Sizeof(typeof(T));
        Emit.Conv_I();
        Emit.Mul();
        Emit.Add();
        return ref ReturnRef<T>();
    }

    /// <summary>
    /// Adds an element offset to the given reference.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T Add<T>(ref T source, nuint elementOffset)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(source));
        Emit.Ldarg(nameof(elementOffset));
        Emit.Sizeof<T>();
        Emit.Mul();
        Emit.Add();
        return ref ReturnRef<T>();
    }

    /// <summary>
    /// Adds an element offset to the given reference.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T Add<T>(ref T source, nint elementOffset)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(source));
        Emit.Ldarg(nameof(elementOffset));
        Emit.Sizeof(typeof(T));
        Emit.Mul();
        Emit.Add();
        return ref ReturnRef<T>();
    }

    /// <summary>
    /// Adds a byte offset to the given reference.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T AddByteOffset<T>(ref T source, nuint byteOffset)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(source));
        Emit.Ldarg(nameof(byteOffset));
        Emit.Add();
        return ref ReturnRef<T>();
    }
#endregion

#region Comparison
    /// <summary>
    /// Determines whether the specified references point to the same location.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AreSame<T>([AllowNull] ref readonly T left, [AllowNull] ref readonly T right)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(left));
        Emit.Ldarg(nameof(right));
        Emit.Ceq();
        return Return<bool>();
    }
#endregion
}