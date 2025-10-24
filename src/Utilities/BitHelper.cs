using System.Text;
using static InlineIL.IL;
using bytes = System.ReadOnlySpan<byte>;

namespace ScrubJay.Utilities;

/// <summary>
/// Helper utility for working with <see cref="byte">bytes</see> and <c>unmanaged</c> types
/// </summary>
[PublicAPI]
public static class BitHelper
{
    static BitHelper()
    {
    }

    extension(BitConverter)
    {
        public static byte[] GetBytes<U>(in U value)
            where U : unmanaged
        {
            return AsBytes<U>(in value).ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bytes AsBytes<U>(in U value)
            where U : unmanaged
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            unsafe
            {
                return new bytes(Notsafe.InAsVoidPtr<U>(in value), sizeof(U));
            }
        }

        public static bytes AsEnumBytes<E>(in E e)
            where E : struct, Enum
        {
            unsafe
            {
                return new bytes(Notsafe.InAsVoidPtr<E>(in e), Notsafe.SizeOf<E>());
            }
        }

        public static int TryWriteBytes<U>(Span<byte> destination, in U value)
            where U : unmanaged
        {
            unsafe
            {
                int size = sizeof(U);
                if (size > destination.Length)
                    return 0;
                Notsafe.Bytes.WriteTo(destination, in value);
                return size;
            }
        }

        public static U To<U>(bytes bytes)
            where U : unmanaged
        {
            unsafe
            {
                if (bytes.Length < sizeof(U))
                    throw Ex.Arg(bytes, $"Only {bytes.Length}/{sizeof(U)} bytes were supplied");
                return Notsafe.Bytes.Read<U>(bytes);
            }
        }

        public static string ToString(bytes bytes, Encoding? encoding = null)
        {
            return (encoding ?? Encoding.Default).GetString(bytes);
        }

        public static O Cast<I, O>(I input)
#if NET9_0_OR_GREATER
            where I : unmanaged, allows ref struct
            where O : unmanaged, allows ref struct
#endif
        {
            unsafe
            {
                if (Notsafe.SizeOf<I>() != Notsafe.SizeOf<O>())
                    throw Ex.Arg<I>(input.Stringify(), $"{typeof(I):@} Input `{input}` was not the same size as a {typeof(O):@}");
                return Notsafe.As<I, O>(input);
            }
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteTo<U>(Span<byte> destination, in U value)
        where U : unmanaged
    {
        unsafe
        {
            Throw.IfLessThan(destination.Length, sizeof(U));
            Unsafe.WriteUnaligned<U>(ref MemoryMarshal.GetReference(destination), value);
        }
    }

    public static U AsValue<U>(scoped bytes bytes)
        where U : unmanaged
    {
        unsafe
        {
            Throw.IfLessThan(bytes.Length, sizeof(U));
            return Unsafe.ReadUnaligned<U>(ref MemoryMarshal.GetReference(bytes));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static U AsValueUnsafe<U>(scoped bytes bytes)
    {
        Emit.Ldarg(nameof(bytes));
        Emit.Unaligned(0x1);
        Emit.Ldobj<U>();
        return Return<U>();
    }
}