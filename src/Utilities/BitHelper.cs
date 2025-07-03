using static InlineIL.IL;
using bytes = System.ReadOnlySpan<byte>;

namespace ScrubJay.Utilities;

public static class BitHelper
{
    public static bytes AsBytes<U>(in U value)
        where U : unmanaged
    {
        unsafe
        {
            return new bytes(Notsafe.InAsVoidPtr<U>(in value), sizeof(U));
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

    public static U Read<U>(bytes bytes)
        where U : unmanaged
    {
        unsafe
        {
            Throw.IfLessThan(bytes.Length, sizeof(U));
            return Unsafe.ReadUnaligned<U>(ref MemoryMarshal.GetReference(bytes));
        }
    }

    public static U ReadUnsafe<U>(bytes bytes)
        where U : unmanaged
    {
        Emit.Ldarg(nameof(bytes));
        Emit.Unaligned(0x1);
        Emit.Ldobj<U>();
        return Return<U>();
    }
}