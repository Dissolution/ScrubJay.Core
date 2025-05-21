// BigInt is nested on purpose
#pragma warning disable CA1034, IDE0060

using static InlineIL.IL;

namespace ScrubJay.Maths;

[PublicAPI]
public static class MathHelper
{
    /// <summary>
    /// <see cref="BigInt"/> constants
    /// </summary>
    public static class BigInt
    {
        public static BigInteger Ten { get; } = new BigInteger(10);

        public static BigInteger FloatMinValue { get; } = new BigInteger(float.MinValue);
        public static BigInteger FloatMaxValue { get; } = new BigInteger(float.MaxValue);

        public static BigInteger DoubleMinValue { get; } = new BigInteger(double.MinValue);
        public static BigInteger DoubleMaxValue { get; } = new BigInteger(double.MaxValue);

        public static BigInteger DecimalMinValue { get; } = new BigInteger(decimal.MinValue);
        public static BigInteger DecimalMaxValue { get; } = new BigInteger(decimal.MaxValue);

        public static BigInteger LongMinValue { get; } = new BigInteger(long.MinValue);
        public static BigInteger LongMaxValue { get; } = new BigInteger(long.MaxValue);
    }

    public static int NumberOfDigits(this BigInteger bigInt) => (int)Math.Ceiling(BigInteger.Log10(bigInt * bigInt.Sign));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BigInteger TenPow(int exponent) => BigInteger.Pow(BigInt.Ten, exponent);


    public static ulong GreatestCommonDivisor(ulong left, ulong right)
    {
        while ((left != 0UL) && (right != 0UL))
        {
            if (left > right)
                left %= right;
            else
                right %= left;
        }

        return left | right;
    }

    /*
     *
Oh, this does work just fine with negative input(s): just flip the sign of negative values before entering the while-loop... if(a<0)a=-a; if(b<0)b=-b; –
Scre
Commented Aug 27, 2017 at 10:08

     */


    public static long GreatestCommonDivisor(long left, long right)
    {
        long temp;
        while (right != 0L)
        {
            temp = right;
            right = left % right;
            left = temp;
        }

        return left;
    }
    //
    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // public static T LeastCommonMultiple<T>(T left, T right)
    //     where T : INumber<T>
    //     => (left / GreatestCommonDivisor(left, right)) * right;



    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int HalfRoundDown(int value)
    {
        Emit.Ldarg(nameof(value));
        Emit.Ldc_I4_1();
        Emit.Shr();
        return Return<int>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Twice(int value)
    {
        Emit.Ldarg(nameof(value));
        Emit.Ldc_I4_1();
        Emit.Shl();
        return Return<int>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Min(int a, int b, int c)
    {
        if (a <= b)
        {
            if (a <= c)
            {
                // no assumptions about b ?? c
                return a;
            }
        }
        else
        {
            Debug.Assert(b < a);
            if (b <= c)
            {
                // no assumptions about a ?? c
                return b;
            }
        }

        Debug.Assert(c < b);
        Debug.Assert(c < a);
        return c;
    }

        /// <summary>
    /// Rotates the specified value left by the specified number of bits.
    /// Similar in behavior to the x86 instruction ROL.
    /// </summary>
    /// <param name="value">The value to rotate.</param>
    /// <param name="offset">The number of bits to rotate by.
    /// Any value outside the range [0..31] is treated as congruent mod 32.</param>
    /// <returns>The rotated value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint RotateLeft(uint value, int offset)
        => (value << offset) | (value >> (32 - offset));

    /// <summary>
    /// Rotates the specified value left by the specified number of bits.
    /// Similar in behavior to the x86 instruction ROL.
    /// </summary>
    /// <param name="value">The value to rotate.</param>
    /// <param name="offset">The number of bits to rotate by.
    /// Any value outside the range [0..63] is treated as congruent mod 64.</param>
    /// <returns>The rotated value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong RotateLeft(ulong value, int offset)
        => (value << offset) | (value >> (64 - offset));

    /// <summary>
    /// Rotates the specified value right by the specified number of bits.
    /// Similar in behavior to the x86 instruction ROR.
    /// </summary>
    /// <param name="value">The value to rotate.</param>
    /// <param name="offset">The number of bits to rotate by.
    /// Any value outside the range [0..31] is treated as congruent mod 32.</param>
    /// <returns>The rotated value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint RotateRight(uint value, int offset)
        => (value >> offset) | (value << (32 - offset));

    /// <summary>
    /// Rotates the specified value right by the specified number of bits.
    /// Similar in behavior to the x86 instruction ROR.
    /// </summary>
    /// <param name="value">The value to rotate.</param>
    /// <param name="offset">The number of bits to rotate by.
    /// Any value outside the range [0..63] is treated as congruent mod 64.</param>
    /// <returns>The rotated value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong RotateRight(ulong value, int offset)
        => (value >> offset) | (value << (64 - offset));




    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong BigMul(ulong a, ulong b, out ulong low)
    {
#if NET5_0_OR_GREATER
        return Math.BigMul(a, b, out low);
#else
        // Adaptation of algorithm for multiplication
        // of 32-bit unsigned integers described
        // in Hacker's Delight by Henry S. Warren, Jr. (ISBN 0-201-91465-4), Chapter 8
        // Basically, it's an optimized version of FOIL method applied to
        // low and high dwords of each operand

        // Use 32-bit uints to optimize the fallback for 32-bit platforms.
        uint al = (uint)a;
        uint ah = (uint)(a >> 32);
        uint bl = (uint)b;
        uint bh = (uint)(b >> 32);

        ulong mull = ((ulong)al) * bl;
        ulong t = ((ulong)ah) * bl + (mull >> 32);
        ulong tl = ((ulong)al) * bh + (uint)t;

        low = tl << 32 | (uint)mull;

        return ((ulong)ah) * bh + (t >> 32) + (tl >> 32);
#endif
    }
}
