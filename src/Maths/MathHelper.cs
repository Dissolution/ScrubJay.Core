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
        while (left != 0UL && right != 0UL)
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
}
