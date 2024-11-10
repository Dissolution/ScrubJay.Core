using ScrubJay.Constraints;

namespace ScrubJay.Extensions;

[PublicAPI]
public static class NumberExtensions
{
    public static int Clamp(this int number, Bounds<int> bounds)
    {
        return bounds.Clamped(number);
    }

    public static int Clamp(this int number, int inclusiveMin, int inclusiveMax)
    {
        if (number <= inclusiveMin)
            return inclusiveMin;
        if (number >= inclusiveMax)
            return inclusiveMax;
        return number;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEven(this int number) => number % 2 == 0;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOdd(this int number) => number % 2 != 0;
}