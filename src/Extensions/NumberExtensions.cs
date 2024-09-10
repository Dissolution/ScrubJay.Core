using ScrubJay.Collections;

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
}