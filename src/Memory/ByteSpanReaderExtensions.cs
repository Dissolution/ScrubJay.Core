using System.Buffers.Binary;
using System.Text;
using static ScrubJay.Utilities.Notsafe;

namespace ScrubJay.Memory;

/// <summary>
/// Extensions on <see cref="SpanReader{T}">SpanReader&lt;byte&gt;</see>
/// </summary>
[PublicAPI]
public static class ByteSpanReaderExtensions
{
#region Read

#region Primitives

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ReadU8(this ref SpanReader<byte> reader,
        Endianness endianness = Endianness.System)
    {
        return reader.Take();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte ReadI8(this ref SpanReader<byte> reader,
        Endianness endianness = Endianness.System)
    {
        return (sbyte)reader.Take();
    }

    public static short ReadI16(
        this ref SpanReader<byte> reader,
        Endianness endianness = Endianness.System)
    {
        var bytes = reader.Take(sizeof(short));
        short i16 = BitHelper.Read<short>(bytes);
        if (endianness.IsNoSwap)
            return i16;
        return BinaryPrimitives.ReverseEndianness(i16);
    }

    public static ushort ReadU16(
        this ref SpanReader<byte> reader,
        Endianness endianness = Endianness.System)
    {
        var bytes = reader.Take(sizeof(ushort));
        ushort u16 = BitHelper.Read<ushort>(bytes);
        if (endianness.IsNoSwap)
            return u16;
        return BinaryPrimitives.ReverseEndianness(u16);
    }

    public static int ReadI32(
        this ref SpanReader<byte> reader,
        Endianness endianness = Endianness.System)
    {
        var bytes = reader.Take(sizeof(int));
        int i32 = BitHelper.Read<int>(bytes);
        if (endianness.IsNoSwap)
            return i32;
        return BinaryPrimitives.ReverseEndianness(i32);
    }

    public static uint ReadU32(
        this ref SpanReader<byte> reader,
        Endianness endianness = Endianness.System)
    {
        var bytes = reader.Take(sizeof(uint));
        uint u32 = BitHelper.Read<uint>(bytes);
        if (endianness.IsNoSwap)
            return u32;
        return BinaryPrimitives.ReverseEndianness(u32);
    }

    public static long ReadI64(
        this ref SpanReader<byte> reader,
        Endianness endianness = Endianness.System)
    {
        var bytes = reader.Take(sizeof(long));
        long i64 = BitHelper.Read<long>(bytes);
        if (endianness.IsNoSwap)
            return i64;
        return BinaryPrimitives.ReverseEndianness(i64);
    }

    public static ulong ReadU64(
        this ref SpanReader<byte> reader,
        Endianness endianness = Endianness.System)
    {
        var bytes = reader.Take(sizeof(ulong));
        ulong u64 = BitHelper.Read<ulong>(bytes);
        if (endianness.IsNoSwap)
            return u64;
        return BinaryPrimitives.ReverseEndianness(u64);
    }

#if NET6_0_OR_GREATER
    public static Half ReadF16(
        this ref SpanReader<byte> reader,
        Endianness endianness = Endianness.System)
    {
        if (endianness.IsNoSwap)
        {
            return BitHelper.Read<Half>(reader.Take(2)); // sizeof(Half)
        }

        Span<byte> buffer = stackalloc byte[2]; // sizeof(Half)
        reader.TakeInto(buffer);
        buffer.Reverse();
        return BitHelper.Read<Half>(buffer);
    }
#endif

    public static float ReadF32(
        this ref SpanReader<byte> reader,
        Endianness endianness = Endianness.System)
    {
        if (endianness.IsNoSwap)
        {
            return BitHelper.Read<float>(reader.Take(sizeof(float)));
        }

        Span<byte> buffer = stackalloc byte[sizeof(float)];
        reader.TakeInto(buffer);
        buffer.Reverse();
        return BitHelper.Read<float>(buffer);
    }

    public static double ReadF64(
        this ref SpanReader<byte> reader,
        Endianness endianness = Endianness.System)
    {
        if (endianness.IsNoSwap)
        {
            return BitHelper.Read<double>(reader.Take(sizeof(double)));
        }

        Span<byte> buffer = stackalloc byte[sizeof(double)];
        reader.TakeInto(buffer);
        buffer.Reverse();
        return BitHelper.Read<double>(buffer);
    }

    public static decimal ReadDec(
        this ref SpanReader<byte> reader,
        Endianness endianness = Endianness.System)
    {
        if (endianness.IsNoSwap)
        {
            return BitHelper.Read<decimal>(reader.Take(sizeof(decimal)));
        }

        Span<byte> buffer = stackalloc byte[sizeof(decimal)];
        reader.TakeInto(buffer);
        buffer.Reverse();
        return BitHelper.Read<decimal>(buffer);
    }

#endregion

#region Less Primitives

    public static bool ReadBool(this ref SpanReader<byte> reader)
    {
        return reader.Take() != 0;
    }

    public static bool ReadBool(this ref SpanReader<byte> reader, int byteCount)
    {
        foreach (byte u8 in reader.Take(byteCount))
        {
            if (u8 != 0) return true;
        }

        return false;
    }

    public static U ReadUnmanaged<U>(this ref SpanReader<byte> reader,
        Endianness endianness = Endianness.System)
        where U : unmanaged
    {
        if (endianness.IsNoSwap)
        {
            return BitHelper.Read<U>(reader.Take(SizeOf<U>()));
        }

        Span<byte> buffer = stackalloc byte[SizeOf<U>()];
        reader.TakeInto(buffer);
        buffer.Reverse();
        return BitHelper.Read<U>(buffer);
    }

    public static E ReadEnum<E>(this ref SpanReader<byte> reader,
        Endianness endianness = Endianness.System)
        where E : struct, Enum
    {
        E @enum;
        int size = SizeOf<E>();

        if (endianness.IsNoSwap)
        {
            var bytes = reader.Take(size);
            @enum = BitHelper.Read<E>(bytes);
        }
        else
        {
            Span<byte> buffer = stackalloc byte[size];
            reader.TakeInto(buffer);
            buffer.Reverse();
            @enum = BitHelper.Read<E>(buffer);
        }

#if NET5_0_OR_GREATER
        if (!Enum.IsDefined<E>(@enum))
#else
        if (!Enum.IsDefined(typeof(E), @enum))
#endif

        {
            throw Ex.Enum(@enum);
        }

        return @enum;
    }

#endregion

#region Text

    public static char ReadChar(this ref SpanReader<byte> reader,
        Endianness endianness = Endianness.System)
    {
        if (endianness.IsNoSwap)
        {
            return BitHelper.Read<char>(reader.Take(sizeof(char)));
        }

        Span<byte> buffer = stackalloc byte[sizeof(char)];
        reader.TakeInto(buffer);
        buffer.Reverse();
        return BitHelper.Read<char>(buffer);
    }

    public static string ReadString(
        this ref SpanReader<byte> reader,
        int length,
        Endianness endianness = Endianness.System,
        Encoding? encoding = null)
    {
        Throw.IfLessThan(length, 0);
        if (endianness.IsNoSwap)
        {
            return (encoding ?? Encoding.UTF8).GetString(reader.Take(length));
        }

        Span<byte> buffer = stackalloc byte[length];
        reader.TakeInto(buffer);
        buffer.Reverse();
        return (encoding ?? Encoding.UTF8).GetString(buffer);
    }

    public static string ReadString(
        this ref SpanReader<byte> reader,
        uint length,
        Endianness endianness = Endianness.System,
        Encoding? encoding = null)
        => ReadString(ref reader, checked((int)length), endianness, encoding);

    public static string ReadString(
        this ref SpanReader<byte> reader,
        StringFix fix,
        Endianness endianness = Endianness.System,
        Encoding? encoding = null)
    {
        switch (fix)
        {
            case StringFix.SevenBitEncodedLenPrefix:
            {
                int len = Read7BitEncodedI32(ref reader);
                return ReadString(ref reader, len, endianness, encoding);
            }
            case StringFix.U8Prefix:
            {
                byte len = ReadU8(ref reader);
                return ReadString(ref reader, len, endianness, encoding);
            }
            case StringFix.U16Prefix:
            {
                ushort len = ReadU16(ref reader);
                return ReadString(ref reader, len, endianness, encoding);
            }
            case StringFix.U32Prefix:
            {
                uint len = ReadU32(ref reader);
                return ReadString(ref reader, len, endianness, encoding);
            }
            case StringFix.U64Prefix:
            {
                ulong len = ReadU64(ref reader);
                if (len > (ulong)int.MaxValue)
                    throw Ex.Invalid();
                return ReadString(ref reader, (int)len, endianness, encoding);
            }
            case StringFix.I8Prefix:
            {
                sbyte len = ReadI8(ref reader);
                return ReadString(ref reader, len, endianness, encoding);
            }
            case StringFix.I16Prefix:
            {
                short len = ReadI16(ref reader);
                return ReadString(ref reader, len, endianness, encoding);
            }
            case StringFix.I32Prefix:
            {
                int len = ReadI32(ref reader);
                return ReadString(ref reader, len, endianness, encoding);
            }
            case StringFix.I64Prefix:
            {
                long len = ReadI64(ref reader);
                if (len > (long)int.MaxValue)
                    throw Ex.Invalid();
                return ReadString(ref reader, (int)len, endianness, encoding);
            }
            case StringFix.NullTerminated:
            {
                encoding ??= Encoding.UTF8;
                int charSize = encoding.GetByteCount("J");
                Span<byte> nullChar = stackalloc byte[charSize];

                var bytes = reader.TakeUntilMatching(nullChar, chunk: true);
                string str = encoding.GetString(bytes);
                return str;
            }
            default:
                throw Ex.Enum(fix);
        }
    }

#endregion

#region Special Encodings

    public static int Read7BitEncodedI32(this ref SpanReader<byte> reader)
    {
        uint result = 0;
        byte u8;

        const int MAX_BYTES_WITHOUT_OVERFLOW = 4;
        for (int shift = 0; shift < MAX_BYTES_WITHOUT_OVERFLOW * 7; shift += 7)
        {
            // ReadU8 handles end of stream
            u8 = reader.ReadU8();
            result |= (u8 & 0x7Fu) << shift;

            if (u8 <= 0x7Fu)
            {
                return (int)result; // early exit
            }
        }

        u8 = reader.ReadU8();
        if (u8 > 0b_1111u)
        {
            throw Ex.Invalid();
        }

        result |= (uint)u8 << (MAX_BYTES_WITHOUT_OVERFLOW * 7);
        return (int)result;
    }

    public static long Read7BitEncodedI64(this ref SpanReader<byte> reader)
    {
        ulong result = 0;
        byte u8;

        const int MAX_BYTES_WITHOUT_OVERFLOW = 9;
        for (int shift = 0; shift < MAX_BYTES_WITHOUT_OVERFLOW * 7; shift += 7)
        {
            // ReadU8 handles end of stream cases for us.
            u8 = reader.ReadU8();
            result |= (u8 & 0x7Ful) << shift;

            if (u8 <= 0x7Fu)
            {
                return (long)result; // early exit
            }
        }

        u8 = reader.ReadU8();
        if (u8 > 0b_1u)
        {
            throw Ex.Invalid();
        }

        result |= (ulong)u8 << (MAX_BYTES_WITHOUT_OVERFLOW * 7);
        return (long)result;
    }

#endregion

#region Time

    public static TimeSpan ReadTimeSpan(
        this ref SpanReader<byte> reader,
        TimeFix fix)
    {
        switch (fix)
        {
            case TimeFix.Ticks:
            {
                long ticks = ReadI64(ref reader);
                return new TimeSpan(ticks);
            }
            case TimeFix.TimeU32:
            {
                uint seconds = ReadU32(ref reader);
                return TimeSpan.FromSeconds(seconds);
            }
            case TimeFix.TimeU64:
            {
                ulong seconds = ReadU64(ref reader);
                return TimeSpan.FromSeconds(seconds);
            }
            default:
                throw Ex.Enum(fix);
        }
    }

    public static DateTime ReadDateTime(
        this ref SpanReader<byte> reader,
        TimeFix fix)
    {
        switch (fix)
        {
            case TimeFix.Ticks:
            {
                long ticks = ReadI64(ref reader);
                return new DateTime(ticks);
            }
            case TimeFix.TimeU32:
            {
                uint seconds = ReadU32(ref reader);
                return TimeFix.OriginDateTime.AddSeconds(seconds);
            }
            case TimeFix.TimeU64:
            {
                ulong seconds = ReadU64(ref reader);
                return TimeFix.OriginDateTime.AddSeconds(seconds);
            }
            default:
                throw Ex.Enum(fix);
        }
    }

#endregion

#endregion
}