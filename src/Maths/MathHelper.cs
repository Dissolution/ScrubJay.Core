namespace ScrubJay.Maths;

public static class MathHelper
{
    public static BigInteger BigInteger_Ten { get; } = new BigInteger(10);
    
    public static BigInteger BigInteger_FloatMinValue { get; } = new BigInteger(float.MinValue);
    public static BigInteger BigInteger_FloatMaxValue { get; } = new BigInteger(float.MaxValue);
    
    public static BigInteger BigInteger_DoubleMinValue { get; } = new BigInteger(double.MinValue);
    public static BigInteger BigInteger_DoubleMaxValue { get; } = new BigInteger(double.MaxValue);
    
    public static BigInteger BigInteger_DecimalMinValue { get; } = new BigInteger(decimal.MinValue);
    public static BigInteger BigInteger_DecimalMaxValue { get; } = new BigInteger(decimal.MaxValue);
    
    public static BigInteger BigInteger_LongMinValue { get; } = new BigInteger(long.MinValue);
    public static BigInteger BigInteger_LongMaxValue { get; } = new BigInteger(long.MaxValue);


    public static int NumberOfDigits(this BigInteger bigInt)
    {
        return (int)Math.Ceiling(BigInteger.Log10(bigInt * bigInt.Sign));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BigInteger TenPow(int exponent)
    {
        return BigInteger.Pow(BigInteger_Ten, exponent);
    }


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
}