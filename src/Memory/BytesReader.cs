using System.Buffers.Binary;
using System.Text;

namespace ScrubJay.Memory;

/// <summary>
/// Represents a Cursor in a <see cref="ReadOnlySpan{T}">ReadOnlySpan&lt;byte&gt;</see>
/// that can read and convert bytes
/// </summary>
public ref struct BytesReader
{
    private readonly ReadOnlySpan<byte> _span;
    private readonly Encoding _encoding;
    private readonly bool _reverseEndianness;

    private int _position;

    public Encoding Encoding => _encoding;

    public Endianness Endianness
    {
        get
        {
            var e = EndianHelper.SystemEndianness;
            return _reverseEndianness ? e.Flipped() : e;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the current position reached the end of the span.
    /// </summary>
    public bool IsCompleted => _span.Length - _position <= 0;

    public ReadOnlySpan<byte> Previous => _span[.._position];

    public ReadOnlySpan<byte> Next => _span[_position..];


    public BytesReader(
        ReadOnlySpan<byte> span,
        Encoding? encoding = null,
        Endianness endianness = Endianness.System)
    {
        _span = span;
        _encoding = encoding ?? Encoding.UTF8;
        _reverseEndianness = endianness switch
        {
            Endianness.Little => (EndianHelper.SystemEndianness != Endianness.Little),
            Endianness.Big => (EndianHelper.SystemEndianness != Endianness.Big),
            Endianness.System => false,
            Endianness.NonSystem => true,
            _ => throw InvalidEnumException.Create(endianness),
        };
    }

/*
    /// <summary>
    /// Aligns the position to the given byte multiple.
    /// </summary>
    /// <param name="alignment">The byte multiple to align to. If negative, the position is decreased to the
    /// previous multiple rather than the next one.</param>
    /// <returns>The new position.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Align(int alignment) => _position += MathTools.GetAlignmentDelta(_position, alignment);
*/

#region Read

#region Read Primitives

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte ReadU8() => _span[_position++];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sbyte ReadI8() => (sbyte)_span[_position++];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ReadI16()
    {
        short value = MemoryMarshal.Read<short>(_span[_position..]);
        _position += sizeof(short);
        return _reverseEndianness ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort ReadU16()
    {
        ushort value = MemoryMarshal.Read<ushort>(_span[_position..]);
        _position += sizeof(ushort);
        return _reverseEndianness ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadI32()
    {
        int value = MemoryMarshal.Read<int>(_span[_position..]);
        _position += sizeof(int);
        return _reverseEndianness ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadU32()
    {
        uint value = MemoryMarshal.Read<uint>(_span[_position..]);
        _position += sizeof(uint);
        return _reverseEndianness ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadI64()
    {
        long value = MemoryMarshal.Read<long>(_span[_position..]);
        _position += sizeof(long);
        return _reverseEndianness ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadU64()
    {
        ulong value = MemoryMarshal.Read<ulong>(_span[_position..]);
        _position += sizeof(ulong);
        return _reverseEndianness ? BinaryPrimitives.ReverseEndianness(value) : value;
    }

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Half ReadF16()
    {
        return Notsafe.As<ushort, Half>(ReadU16());
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ReadF32()
    {
        return Notsafe.As<uint, float>(ReadU32());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double ReadF64()
    {
        return Notsafe.As<ulong, double>(ReadU64());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public decimal ReadDecimal()
    {
        decimal value = MemoryMarshal.Read<decimal>(_span[_position..]);
        _position += sizeof(decimal);
        return value;
    }

#endregion

#region Special Encodings

    public int Read7BitEncodedI32()
    {
        uint result = 0;
        byte u8;

        const int MAX_BYTES_WITHOUT_OVERFLOW = 4;
        for (int shift = 0; shift < MAX_BYTES_WITHOUT_OVERFLOW * 7; shift += 7)
        {
            // ReadU8 handles end of stream
            u8 = ReadU8();
            result |= (u8 & 0x7Fu) << shift;

            if (u8 <= 0x7Fu)
            {
                return (int)result; // early exit
            }
        }

        u8 = ReadU8();
        if (u8 > 0b_1111u)
        {
            throw new InvalidOperationException();
        }

        result |= (uint)u8 << (MAX_BYTES_WITHOUT_OVERFLOW * 7);
        return (int)result;
    }

    public long Read7BitEncodedI64()
    {
        ulong result = 0;
        byte u8;

        const int MAX_BYTES_WITHOUT_OVERFLOW = 9;
        for (int shift = 0; shift < MAX_BYTES_WITHOUT_OVERFLOW * 7; shift += 7)
        {
            // ReadU8 handles end of stream cases for us.
            u8 = ReadU8();
            result |= (u8 & 0x7Ful) << shift;

            if (u8 <= 0x7Fu)
            {
                return (long)result; // early exit
            }
        }

        u8 = ReadU8();
        if (u8 > 0b_1u)
        {
            throw new InvalidOperationException();
        }

        result |= (ulong)u8 << (MAX_BYTES_WITHOUT_OVERFLOW * 7);
        return (long)result;
    }

#endregion

#region Boolean

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadBool() => _span[_position++] != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadBool(int byteCount)
    {
        var bytes = ReadBytes(byteCount);
        foreach (var u8 in bytes)
        {
            if (u8 != 0)
                return true;
        }

        return false;
    }

#endregion


    public E ReadEnum<E>()
        where E : struct, Enum
    {
        int len = Notsafe.SizeOf<E>();

        if (_reverseEndianness)
        {
            Span<byte> buffer = stackalloc byte[len];
            Notsafe.Bytes.CopyTo(_span.Slice(_position, len), buffer, len);
            _position += len;
            buffer.Reverse();
            return MemoryMarshal.Read<E>(buffer);
        }
        else
        {
            var buffer = _span.Slice(_position, len);
            _position += len;
            return MemoryMarshal.Read<E>(buffer);
        }
    }

    public U ReadUnmanaged<U>()
        where U : unmanaged
    {
        int len = Notsafe.SizeOf<U>();
        if (_reverseEndianness)
        {
            Span<byte> buffer = stackalloc byte[len];
            Notsafe.Bytes.CopyTo(_span.Slice(_position, len), buffer, len);
            buffer.Reverse();
            _position += len;
            return MemoryMarshal.Read<U>(buffer);
        }
        else
        {
            var buffer = _span.Slice(_position, len);
            _position += len;
            return MemoryMarshal.Read<U>(buffer);
        }
    }

#region Text

    public char ReadChar()
    {
        return Notsafe.As<ushort, char>(ReadU16());
    }

    public enum StringFix
    {
        SevenBitEncodedLenPrefix,
        U8Prefix,
        U16Prefix,
        U32Prefix,
        U64Prefix,
        I8Prefix,
        I16Prefix,
        I32Prefix,
        I64Prefix,
        NullTerminated,
    }


    public string ReadString(StringFix fix)
    {
        switch (fix)
        {
            case StringFix.SevenBitEncodedLenPrefix:
            {
                int len = Read7BitEncodedI32();
                return ReadString(len);
            }
            case StringFix.U8Prefix:
            {
                byte len = ReadU8();
                return ReadString(len);
            }
            case StringFix.U16Prefix:
            {
                ushort len = ReadU16();
                return ReadString(len);
            }
            case StringFix.U32Prefix:
            {
                uint len = ReadU32();
                return ReadString(len);
            }
            case StringFix.U64Prefix:
            {
                ulong len = ReadU64();
                if (len > (ulong)int.MaxValue)
                    throw new InvalidOperationException();
                return ReadString((int)len);
            }
            case StringFix.I8Prefix:
            {
                sbyte len = ReadI8();
                return ReadString(len);
            }
            case StringFix.I16Prefix:
            {
                short len = ReadI16();
                return ReadString(len);
            }
            case StringFix.I32Prefix:
            {
                int len = ReadI32();
                return ReadString(len);
            }
            case StringFix.I64Prefix:
            {
                long len = ReadI64();
                if (len > (long)int.MaxValue)
                    throw new InvalidOperationException();
                return ReadString((int)len);
            }
            case StringFix.NullTerminated:
            {
                // calculate the size of a 'char' in our encoding
                int glyphSize = _encoding.GetByteCount("J");
                int length = 0;
                for (byte u8 = 1; u8 != 0; length += glyphSize)
                {
                    for (int i = 0; i < glyphSize; i++)
                    {
                        u8 = _span[_position + length + i];
                        if (u8 != 0)
                            break;
                    }
                }

                string str = _encoding.GetString(_span.Slice(_position, length - glyphSize));
                _position += (length - glyphSize);
                return str;
            }
            default:
                throw InvalidEnumException.Create(fix);
        }
    }

    public string ReadString(int length)
    {
        string str = _encoding.GetString(_span.Slice(_position, length));
        _position += length;
        return str;
    }

    public string ReadString(uint length)
    {
        int len = checked((int)length);
        return ReadString(len);
    }

#endregion

#region Time

    public enum TimeFix
    {
        /// <summary>
        /// Specifies the date as the number of 100-nanosecond intervals
        /// that have elapsed since <c>0001-01-01 00:00</c>
        /// </summary>
        /// <remarks>
        /// Size = 8 bytes
        /// </remarks>
        Ticks,

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// Size = 4 bytes
        /// </remarks>
        /// <seealso href="https://en.cppreference.com/w/c/chrono/time_t"/>
        TimeT4,
        TimeT8,
    }

    private static readonly DateTime _dateOrigin = new DateTime(1970, 1, 1);


    public TimeSpan ReadTimeSpan(TimeFix fix)
    {
        switch (fix)
        {
            case TimeFix.Ticks:
            {
                long ticks = ReadI64();
                return new TimeSpan(ticks);
            }
            case TimeFix.TimeT4:
            {
                uint seconds = ReadU32();
                return TimeSpan.FromSeconds(seconds);
            }
            case TimeFix.TimeT8:
            {
                ulong seconds = ReadU64();
                return TimeSpan.FromSeconds(seconds);
            }
            default:
                throw InvalidEnumException.Create(fix);
        }
    }

    public DateTime ReadDateTime(TimeFix fix)
    {
        switch (fix)
        {
            case TimeFix.Ticks:
            {
                long ticks = ReadI64();
                return new DateTime(ticks);
            }
            case TimeFix.TimeT4:
            {
                uint seconds = ReadU32();
                return _dateOrigin.AddSeconds(seconds);
            }
            case TimeFix.TimeT8:
            {
                ulong seconds = ReadU64();
                return _dateOrigin.AddSeconds(seconds);
            }
            default:
                throw InvalidEnumException.Create(fix);
        }
    }

#endregion

#region Bytes

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> ReadBytes(int count)
    {
        var bytes = _span.Slice(_position, count);
        _position += count;
        return bytes;
    }

#endregion

#endregion

#region Peek

#endregion
}