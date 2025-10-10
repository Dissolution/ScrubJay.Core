﻿using System.Buffers.Binary;
using System.Text;

namespace ScrubJay.Memory;

[PublicAPI]
public static class ByteSpanWriterExtensions
{
    extension(ref SpanWriter<byte> writer)
    {
        public void WriteMany(scoped ReadOnlySpan<byte> bytes, Endianness endianness = Endianness.System)
        {
            writer.TryWriteMany(bytes, endianness).ThrowIfError();
        }

        public Result<int> TryWriteMany(scoped ReadOnlySpan<byte> bytes, Endianness endianness = Endianness.System)
        {
            int count = bytes.Length;
            if (count == 0)
                return Ok(0);
            if (count > writer.RemainingCount)
                return new InvalidOperationException($"Could not write span count {count}");

            if (endianness.IsSwap)
            {
                for (var i = count - 1; i >= 0; i--)
                {
                    writer.WriteUnsafe(bytes[i]);
                }
            }
            else
            {
                writer.WriteUnsafe(bytes);
            }

            return Ok(count);
        }

#region Write Primitive

        public void Write(byte u8, Endianness endianness = Endianness.System)
        {
            writer.TryWrite(u8).ThrowIfError();
        }

        public void Write(sbyte i8, Endianness endianness = Endianness.System)
        {
            writer.TryWrite((byte)i8).ThrowIfError();
        }

        public void Write(short i16, Endianness endianness = Endianness.System)
        {
            writer.WriteMany(BitConverter.AsBytes(i16), endianness);
        }

        public void Write(ushort u16, Endianness endianness = Endianness.System)
        {
            writer.WriteMany(BitConverter.AsBytes(u16), endianness);
        }

        public void Write(int i32, Endianness endianness = Endianness.System)
        {
            writer.WriteMany(BitConverter.AsBytes(i32), endianness);
        }

        public void Write(uint u32, Endianness endianness = Endianness.System)
        {
            writer.WriteMany(BitConverter.AsBytes(u32), endianness);
        }

        public void Write(long i64, Endianness endianness = Endianness.System)
        {
            writer.WriteMany(BitConverter.AsBytes(i64), endianness);
        }

        public void Write(ulong u64, Endianness endianness = Endianness.System)
        {
            writer.WriteMany(BitConverter.AsBytes(u64), endianness);
        }

#if NET6_0_OR_GREATER
        public void Write(Half f16, Endianness endianness = Endianness.System)
        {
            writer.WriteMany(BitConverter.AsBytes(f16), endianness);
        }
#endif

        public void Write(float f32, Endianness endianness = Endianness.System)
        {
            writer.WriteMany(BitConverter.AsBytes(f32), endianness);
        }

        public void Write(double f64, Endianness endianness = Endianness.System)
        {
            writer.WriteMany(BitConverter.AsBytes(f64), endianness);
        }

        public void Write(decimal dec, Endianness endianness = Endianness.System)
        {
            writer.WriteMany(BitConverter.AsBytes(dec), endianness);
        }

#endregion

#region Less Primitive

        public void Write(bool boolean)
            => writer.Write(boolean ? 1 : 0);

        public void Write(bool boolean, int byteCount, Endianness endianness = Endianness.System)
        {
            if (byteCount == 0)
                return;
            if (byteCount > writer.RemainingCount)
                throw Ex.Invalid();

            if (endianness.IsNoSwap)
            {
                int end = byteCount - 1;
                for (var i = 0; i < end; i++)
                {
                    writer.WriteUnsafe((byte)0);
                }

                writer.WriteUnsafe(boolean ? (byte)1 : (byte)0);
            }
            else
            {
                writer.WriteUnsafe(boolean ? (byte)1 : (byte)0);
                int end = byteCount - 1;
                for (var i = 0; i < end; i++)
                {
                    writer.WriteUnsafe((byte)0);
                }
            }
        }

        public void Write<U>(in U value, Endianness endianness = Endianness.System)
            where U : unmanaged
#if NET9_0_OR_GREATER
            , allows ref struct
#endif
        {
            var bytes = BitConverter.AsBytes<U>(in value);
            writer.WriteMany(bytes, endianness);
        }

        public void Write<E>(E e, Endianness endianness = Endianness.System)
            where E : struct, Enum
        {
            var bytes = BitConverter.AsEnumBytes<E>(e);
            writer.WriteMany(bytes, endianness);
        }

#endregion

        public void Write7BitEncodedInt(int value)
        {
            uint uValue = (uint)value;

            // Write out an int 7 bits at a time. The high bit of the byte,
            // when on, tells reader to continue reading more bytes.
            //
            // Using the constants 0x7F and ~0x7F below offers smaller
            // codegen than using the constant 0x80.

            while (uValue > 0x7Fu)
            {
                writer.Write((byte)(uValue | ~0x7Fu));
                uValue >>= 7;
            }

            writer.Write((byte)uValue);
        }

#region Text

        public void Write(char ch, Endianness endianness = Endianness.System)
        {
            var bytes = BitConverter.AsBytes(in ch);
            writer.WriteMany(bytes, endianness);
        }

        public void Write(scoped text text,
            Encoding? encoding = null,
            Endianness endianness = Endianness.System,
            StringFix fix = StringFix.None)
        {
            int len = text.Length;
            if (len == 0)
                return;

            encoding ??= Encoding.Default;
            int byteCount = encoding.GetByteCount(text);
            Span<byte> bytes = stackalloc byte[byteCount];
            int encoded = encoding.GetBytes(text, bytes);
            Debug.Assert(encoded == byteCount);

            switch (fix)
            {
                case StringFix.None:
                    writer.WriteMany(bytes, endianness);
                    return;
                case StringFix.SevenBitEncodedLenPrefix:
                {
                    writer.Write7BitEncodedInt(len);
                    writer.WriteMany(bytes, endianness);
                    return;
                }
                case StringFix.U8Prefix:
                {
                    if (len > byte.MaxValue)
                        throw Ex.Arg(fix, $"String length of {len} cannot fit in U8Prefix");
                    writer.Write((byte)len);
                    writer.WriteMany(bytes, endianness);
                    return;
                }
                case StringFix.U16Prefix:
                {
                    if (len > ushort.MaxValue)
                        throw Ex.Arg(fix, $"String length of {len} cannot fit in U16Prefix");
                    writer.Write((ushort)len, endianness);
                    writer.WriteMany(bytes, endianness);
                    return;
                }
                case StringFix.U32Prefix:
                {
                    writer.Write((uint)len, endianness);
                    writer.WriteMany(bytes, endianness);
                    return;
                }
                case StringFix.U64Prefix:
                {
                    writer.Write((ulong)len, endianness);
                    writer.WriteMany(bytes, endianness);
                    return;
                }
                case StringFix.I8Prefix:
                {
                    if (len > sbyte.MaxValue)
                        throw Ex.Arg(fix, $"String length of {len} cannot fit in I8Prefix");
                    writer.Write((sbyte)len);
                    writer.WriteMany(bytes, endianness);
                    return;
                }
                case StringFix.I16Prefix:
                {
                    if (len > short.MaxValue)
                        throw Ex.Arg(fix, $"String length of {len} cannot fit in I16Prefix");
                    writer.Write((short)len, endianness);
                    writer.WriteMany(bytes, endianness);
                    return;
                }
                case StringFix.I32Prefix:
                {
                    writer.Write(len, endianness);
                    writer.WriteMany(bytes, endianness);
                    return;
                }
                case StringFix.I64Prefix:
                {
                    writer.Write((long)len, endianness);
                    writer.WriteMany(bytes, endianness);
                    return;
                }
                case StringFix.NullTerminated:
                {
                    writer.WriteMany(bytes, endianness);
                    writer.Write((byte)0);
                    return;
                }
                default:
                    throw Ex.Enum(fix);
            }
        }

        public void Write(string? str,
            Encoding? encoding = null,
            Endianness endianness = Endianness.System,
            StringFix fix = StringFix.None)
        {
            if (str is null)
                return;

            int len = str.Length;
            if (len == 0)
                return;

            var bytes = (encoding ?? Encoding.Default).GetBytes(str);
            switch (fix)
            {
                case StringFix.None:
                    writer.WriteMany(bytes, endianness);
                    return;
                case StringFix.SevenBitEncodedLenPrefix:
                {
                    writer.Write7BitEncodedInt(len);
                    writer.WriteMany(bytes, endianness);
                    return;
                }
                case StringFix.U8Prefix:
                {
                    if (len > byte.MaxValue)
                        throw Ex.Arg(fix, $"String length of {len} cannot fit in U8Prefix");
                    writer.Write((byte)len);
                    writer.WriteMany(bytes, endianness);
                    return;
                }
                case StringFix.U16Prefix:
                {
                    if (len > ushort.MaxValue)
                        throw Ex.Arg(fix, $"String length of {len} cannot fit in U16Prefix");
                    writer.Write((ushort)len, endianness);
                    writer.WriteMany(bytes, endianness);
                    return;
                }
                case StringFix.U32Prefix:
                {
                    writer.Write((uint)len, endianness);
                    writer.WriteMany(bytes, endianness);
                    return;
                }
                case StringFix.U64Prefix:
                {
                    writer.Write((ulong)len, endianness);
                    writer.WriteMany(bytes, endianness);
                    return;
                }
                case StringFix.I8Prefix:
                {
                    if (len > sbyte.MaxValue)
                        throw Ex.Arg(fix, $"String length of {len} cannot fit in I8Prefix");
                    writer.Write((sbyte)len);
                    writer.WriteMany(bytes, endianness);
                    return;
                }
                case StringFix.I16Prefix:
                {
                    if (len > short.MaxValue)
                        throw Ex.Arg(fix, $"String length of {len} cannot fit in I16Prefix");
                    writer.Write((short)len, endianness);
                    writer.WriteMany(bytes, endianness);
                    return;
                }
                case StringFix.I32Prefix:
                {
                    writer.Write(len, endianness);
                    writer.WriteMany(bytes, endianness);
                    return;
                }
                case StringFix.I64Prefix:
                {
                    writer.Write((long)len, endianness);
                    writer.WriteMany(bytes, endianness);
                    return;
                }
                case StringFix.NullTerminated:
                {
                    writer.WriteMany(bytes, endianness);
                    writer.Write((byte)0);
                    return;
                }
                default:
                    throw Ex.Enum(fix);
            }
        }

#endregion

#region Time

        public void Write(TimeSpan timeSpan,
            Endianness endianness = Endianness.System,
            TimeFix fix = TimeFix.Ticks)
        {
            switch (fix)
            {
                case TimeFix.Ticks:
                {
                    long ticks = timeSpan.Ticks;
                    writer.Write(ticks, endianness);
                    return;
                }
                case TimeFix.TimeU32:
                {
                    Throw.IfGreaterThan(timeSpan.TotalSeconds, uint.MaxValue);
                    writer.Write((uint)timeSpan.TotalSeconds, endianness);
                    return;
                }
                case TimeFix.TimeU64:
                {
                    Throw.IfGreaterThan(timeSpan.TotalSeconds, ulong.MaxValue);
                    writer.Write((ulong)timeSpan.TotalSeconds, endianness);
                    return;
                }
                default:
                    throw Ex.Enum(fix);
            }
        }


        public void Write(DateTime dateTime,
            Endianness endianness = Endianness.System,
            TimeFix fix = TimeFix.Ticks)
        {
            switch (fix)
            {
                case TimeFix.Ticks:
                {
                    long ticks = dateTime.Ticks;
                    writer.Write(ticks, endianness);
                    return;
                }
                case TimeFix.TimeU32:
                {
                    var seconds = (dateTime - TimeFix.OriginDateTime).TotalSeconds;
                    Throw.IfGreaterThan(seconds, uint.MaxValue);
                    writer.Write((uint)seconds, endianness);
                    return;
                }
                case TimeFix.TimeU64:
                {
                    var seconds = (dateTime - TimeFix.OriginDateTime).TotalSeconds;
                    Throw.IfGreaterThan(seconds, ulong.MaxValue);
                    writer.Write((ulong)seconds, endianness);
                    return;
                }
                default:
                    throw Ex.Enum(fix);
            }
        }

#endregion
    }
}