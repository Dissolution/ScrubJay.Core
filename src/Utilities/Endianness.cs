namespace ScrubJay.Utilities;

[PublicAPI]
public enum Endianness
{
    Little = 0,
    Big = 1,
}

[PublicAPI]
public static class EndianHelper
{
    public static readonly Endianness EnvironmentEndianness = BitConverter.IsLittleEndian ? Endianness.Little : Endianness.Big;

    public static ReadOnlySpan<byte> AsSpan<U>(in U value, Endianness endianness = Endianness.Little)
        where U : unmanaged
    {
        unsafe
        {
            if (endianness == EnvironmentEndianness)
            {
                return new ReadOnlySpan<byte>(Notsafe.InAsVoidPtr<U>(in value), sizeof(U));
            }

            Span<byte> span = new Span<byte>(Notsafe.InAsVoidPtr<U>(in value), sizeof(U));
            span.Reverse();
            return span;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Swap<T>(Span<T> span) => span.Reverse();

    public static ushort Swap(ushort u16)
    {
        return (ushort)((u16 >> 8) | (u16 << 8));
    }

    public static short Swap(short i16)
    {
        return (short)((i16 >> 8) | (i16 << 8));
    }

    public static uint Swap(uint u32)
    {
        // Swap adjacent 16-bit blocks
        u32 = (u32 >> 16) | (u32 << 16);
        // Swap adjacent 8-bit blocks
        u32 = ((u32 & 0xFF00FF00U) >> 8) | ((u32 & 0x00FF00FFU) << 8);
        return u32;
    }

    public static int Swap(int i32)
    {
        unchecked
        {
            return (int)Swap((uint)i32);
        }
    }

    public static ulong Swap(ulong u64)
    {
        // Swap adjacent 32-bit blocks
        u64 = (u64 >> 32) | (u64 << 32);
        // Swap adjacent 16-bit blocks
        u64 = ((u64 & 0xFFFF0000FFFF0000U) >> 16) | ((u64 & 0x0000FFFF0000FFFFU) << 16);
        // Swap adjacent 8-bit blocks
        u64 = ((u64 & 0xFF00FF00FF00FF00U) >> 8) | ((u64 & 0x00FF00FF00FF00FFU) << 8);
        return u64;
    }

    public static long Swap(long i64)
    {
        unchecked
        {
            return (long)Swap((ulong)i64);
        }
    }

    public static float Swap(float f32)
    {
        return Notsafe.As<uint, float>(Swap(Notsafe.As<float, uint>(f32)));
    }

    public static double Swap(double f64)
    {
        return Notsafe.As<ulong, double>(Swap(Notsafe.As<double, ulong>(f64)));
    }
}