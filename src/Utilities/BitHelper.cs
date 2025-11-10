#pragma warning disable CS8500

using System.Text;
using static InlineIL.IL;
using bytes = System.Span<byte>;
using robytes = System.ReadOnlySpan<byte>;


namespace ScrubJay.Utilities;

[PublicAPI]
public static class BitHelper
{
    public static unsafe class Notsafe
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static robytes InToBytes<T>(scoped in T value)
#if NET9_0_OR_GREATER
            where T : allows ref struct
#endif
        {
            return new robytes(Utilities.Notsafe.InAsVoidPtr<T>(in value), sizeof(T));
        }

        public static void InitBlock(scoped ref byte u8, int count, byte value = 0)
        {
            // address, value, byte count
            Emit.Ldarg(nameof(u8));
            Emit.Ldarg(nameof(value));
            Emit.Ldarg(nameof(count));
            Emit.Initblk();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyBlock(scoped in byte source, scoped ref byte destination, int count)
        {
            Emit.Ldarg(nameof(destination));
            Emit.Ldarg(nameof(source));
            Emit.Ldarg(nameof(count));
            Emit.Cpblk();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ReadFrom<T>(scoped ref readonly byte source)
#if NET9_0_OR_GREATER
            where T : allows ref struct
#endif
        {
            Emit.Ldarg(nameof(source));
            Emit.Unaligned(0x1);
            Emit.Ldobj<T>();
            return Return<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteTo<T>(scoped ref byte dest, scoped in T source)
#if NET9_0_OR_GREATER
            where T : allows ref struct
#endif
        {
            Emit.Ldarg(nameof(dest));
            Emit.Ldarg(nameof(source));
            Emit.Unaligned(0x1);
            Emit.Stobj<T>();
            Emit.Ret();
        }
    }

    extension(bytes bytes)
    {
        public string AsString(Encoding? encoding = null)
        {
            return (encoding ?? Encoding.Default).GetString(bytes);
        }
    }

    extension(robytes bytes)
    {
        public string AsString(Encoding? encoding = null)
        {
            return (encoding ?? Encoding.Default).GetString(bytes);
        }
    }

    static BitHelper()
    {
    }

    public static robytes AsBytes<U>(in U value, GenericTypeConstraint.IsUnmanagedRefStruct<U> _ = default)
        where U : unmanaged
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        return Notsafe.InToBytes<U>(in value);
    }

    public static robytes AsBytes<E>(in E @enum, GenericTypeConstraint.IsEnum<E> _ = default)
        where E : struct, Enum
    {
        return Notsafe.InToBytes<E>(in @enum);
    }

    public static RefResult<U> TryFromBytes<U>(robytes bytes, GenericTypeConstraint.IsUnmanagedRefStruct<U> _ = default)
        where U : unmanaged
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        int size = Utilities.Notsafe.SizeOf<U>();
        if (bytes.Length < size)
            return Ex.Argument(bytes, $"Needed to read {size} bytes and only found {bytes.Length}");
        return Ok(Notsafe.ReadFrom<U>(in bytes.GetPinnableReference()));
    }

#region Copy

    /* Source types: `byte[]`, `Span<byte>`, `ReadOnlySpan<byte>`
     * Destination types: `byte[]`, `Span<byte>`
     */


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo(byte[] source, byte[] destination, int count)
        => Notsafe.CopyBlock(
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
        => Notsafe.CopyBlock(
#if NET5_0_OR_GREATER
            in MemoryMarshal.GetArrayDataReference<byte>(source),
#else
                in MemoryMarshal.GetReference<byte>(source),
#endif
            ref MemoryMarshal.GetReference(destination),
            count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo(Span<byte> source, byte[] destination, int count)
        => Notsafe.CopyBlock(
            in MemoryMarshal.GetReference<byte>(source),
#if NET5_0_OR_GREATER
            ref MemoryMarshal.GetArrayDataReference<byte>(destination),
#else
                ref MemoryMarshal.GetReference<byte>(destination),
#endif
            count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo(Span<byte> source, Span<byte> destination, int count)
        => Notsafe.CopyBlock(
            in MemoryMarshal.GetReference(source),
            ref MemoryMarshal.GetReference(destination),
            count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo(ReadOnlySpan<byte> source, byte[] destination, int count)
        => Notsafe.CopyBlock(
            in MemoryMarshal.GetReference(source),
#if NET5_0_OR_GREATER
            ref MemoryMarshal.GetArrayDataReference(destination),
#else
                ref MemoryMarshal.GetReference(source),
#endif
            count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CopyTo(ReadOnlySpan<byte> source, Span<byte> destination, int count)
        => Notsafe.CopyBlock(
            in MemoryMarshal.GetReference(source),
            ref MemoryMarshal.GetReference(destination),
            count);

#endregion

#region Init

    public static void Init(scoped bytes bytes, byte value = 0)
    {
        Notsafe.InitBlock(ref bytes.GetPinnableReference(), bytes.Length, value);
    }

#endregion

#region Read

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static U Read<U>(scoped ReadOnlySpan<byte> bytes, GenericTypeConstraint.IsUnmanagedRefStruct<U> _ = default)
        where U : unmanaged
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        return Notsafe.ReadFrom<U>(in bytes.GetPinnableReference());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static U Read<U>(scoped Span<byte> bytes, GenericTypeConstraint.IsUnmanagedRefStruct<U> _ = default)
        where U : unmanaged
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        return Notsafe.ReadFrom<U>(in bytes.GetPinnableReference());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static E Read<E>(scoped ReadOnlySpan<byte> bytes, GenericTypeConstraint.IsEnum<E> _ = default)
        where E : struct, Enum
    {
        return Notsafe.ReadFrom<E>(in bytes.GetPinnableReference());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static E Read<E>(scoped Span<byte> bytes, GenericTypeConstraint.IsEnum<E> _ = default)
        where E : struct, Enum
    {
        return Notsafe.ReadFrom<E>(in bytes.GetPinnableReference());
    }

#endregion


#region Write

    public static Result<int> TryWriteTo<U>(scoped Span<byte> destination, in U source)
        where U : unmanaged
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        int size = Utilities.Notsafe.SizeOf<U>();
        if (size > destination.Length)
            return Ex.Argument(source, $"Source has size {size} and destination can only contain {destination.Length} bytes");
        Notsafe.WriteTo<U>(ref destination.GetPinnableReference(), in source);
        return Ok(size);
    }

#endregion
}