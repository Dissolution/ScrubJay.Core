using ScrubJay.Memory;
using ScrubJay.Text;

namespace ScrubJay.Maths;

[PublicAPI]
public static class DecimalHelper
{
#if NET7_0_OR_GREATER
    public readonly record struct FloatingPointInfo(Int128 Mantissa, bool IsNegative, byte Exponent)
#else
    public readonly record struct FloatingPointInfo(BigInteger Mantissa, bool IsNegative, byte Exponent)
#endif
    {
        public override string ToString()
        {
            var text = new Buffer<char>();
            text.Write(Mantissa);
            if (Exponent == 0)
            {
                // fin
            }
            else if (Exponent > text.Count)
            {
                int zeros = Exponent - text.Count;
                int offset = 0;
                if (Mantissa < 0)
                {
                    zeros += 1;
                    offset = 1;
                }

                Span<char> buf = stackalloc char[2 + zeros];
                buf[0] = '0';
                buf[1] = '.';
                buf[2..].Fill('0');

                text.TryInsertMany(offset, buf).ThrowIfError();
            }
            else
            {
                int offset = text.Count - Exponent;
                text.TryInsert(offset, '.').ThrowIfError();
            }

            return text.ToStringAndDispose();
        }
    }

    public static FloatingPointInfo GetInfo(this decimal dec)
    {
        Span<int> ints = stackalloc int[4];
#if NETFRAMEWORK || NETSTANDARD
        decimal.GetBits(dec).CopyTo(ints);
#else
        decimal.GetBits(dec, ints);
#endif
        // Convert lower 96-bits to Mantissa
#if NET7_0_OR_GREATER
        Int128 mantissa;
        ulong lower = (((ulong)ints[1]) << 32) | ((uint)ints[0]);
        ulong upper = (ulong)ints[2];
        mantissa = new Int128(upper, lower);
#elif NET6_0 || NETSTANDARD2_1
        Span<byte> mantissaBytes = MemoryMarshal.Cast<int, byte>(ints[..3]);
        BigInteger mantissa = new BigInteger(mantissaBytes, true, false);
#else
        BigInteger mantissa;
        BigInteger low = new BigInteger((uint)ints[0]);
        BigInteger mid = new BigInteger((uint)ints[1]) << 32;
        BigInteger high = new BigInteger((uint)ints[2]) << 64;
        mantissa = low | mid | high;
#endif

        // Highest 16 bits contain sign + exponent
        ushort info = (ushort)(((uint)ints[3]) >> 16);

        // sign is the highest bit
        bool isNegative = (info & 0b10000000_00000000) != 0;

        // exponent is the lowest 8 bits
        byte exponent = (byte)(info & 0b00000000_11111111);

        if (isNegative)
        {
            mantissa = -mantissa;
        }

        return new FloatingPointInfo(mantissa, isNegative, exponent);
    }

    public static FloatingPointInfo GetInfo(this float f32)
    {
        // Translate the float into sign, exponent and mantissa.
        int bits = Notsafe.DirectCast<float, int>(f32);

        // Note that the shift is sign-extended, hence the test against -1 not 1
        bool isNegative = (bits & (1 << 31)) != 0;
        byte exponent = (byte)((bits >> 23) & 0b11111111);
        int mantissa = bits & 0b01111111_11111111_11111111;

        // Subnormal numbers; exponent is effectively one higher,
        // but there's no extra normalisation bit in the mantissa
        if (exponent == 0)
        {
            exponent++;
        }
        // Normal numbers; leave exponent as it is but add extra
        // bit to the front of the mantissa
        else
        {
            mantissa |= (1 << 23);
        }

        // Bias the exponent.
        exponent -= 127;

        if (mantissa == 0)
        {
            return new FloatingPointInfo(0, isNegative, 0);
        }

        /* Normalize */
        while ((mantissa & 1) == 0) /*  i.e., Mantissa is even */
        {
            mantissa >>= 1;
            exponent++;
        }

        return new FloatingPointInfo(mantissa, isNegative, (byte)exponent);
    }

    public static FloatingPointInfo GetInfo(this double f64)
    {
        // https://stackoverflow.com/a/390072/2871210

        // Translate the double into sign, exponent and mantissa.
        long bits = BitConverter.DoubleToInt64Bits(f64);
        // Note that the shift is sign-extended, hence the test against -1 not 1
        bool negative = (bits & (1L << 63)) != 0;
        int exponent = (int)((bits >> 52) & 0x7FFL);
        long mantissa = bits & 0xFFFFFFFFFFFFFL;

        // Subnormal numbers; exponent is effectively one higher,
        // but there's no extra normalisation bit in the mantissa
        if (exponent == 0)
        {
            exponent++;
        }
        // Normal numbers; leave exponent as it is but add extra
        // bit to the front of the mantissa
        else
        {
            mantissa = mantissa | (1L << 52);
        }

        // Bias the exponent. It's actually biased by 1023, but we're
        // treating the mantissa as m.0 rather than 0.m, so we need
        // to subtract another 52 from it.
        exponent -= 1075;

        if (mantissa == 0)
        {
            return new FloatingPointInfo(mantissa, negative, 0);
        }

        /* Normalize */
        while ((mantissa & 1) == 0)
        {    /*  i.e., Mantissa is even */
            mantissa >>= 1;
            exponent++;
        }

        return new FloatingPointInfo(mantissa, negative, (byte)exponent);
    }
}