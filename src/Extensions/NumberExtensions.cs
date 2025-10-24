namespace ScrubJay.Extensions;

[PublicAPI]
public static class NumberExtensions
{
#if !NET7_0_OR_GREATER
    extension(sbyte)
    {
        public static sbyte Min(sbyte left, sbyte right) => Math.Min(left, right);
        public static sbyte Max(sbyte left, sbyte right) => Math.Max(left, right);

    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Clamp(this int number, int inclusiveMin)
    {
        if (number <= inclusiveMin)
            return inclusiveMin;
        return number;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Clamp(this int number, int inclusiveMin, int inclusiveMax)
    {
        if (number <= inclusiveMin)
            return inclusiveMin;
        if (number >= inclusiveMax)
            return inclusiveMax;
        return number;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEven(this int number) => (number % 2) == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOdd(this int number) => (number % 2) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEven(this long number) => (number % 2L) == 0L;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOdd(this long number) => (number % 2L) != 0L;
}