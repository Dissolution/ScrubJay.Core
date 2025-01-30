namespace ScrubJay.Extensions;

/// <summary>
/// Extensions for <see cref="decimal"/>
/// </summary>
public static class DecimalExtensions
{

    /// <summary>
    /// Rounds a <see cref="decimal"/> value to the specified number of places.
    /// </summary>
    /// <param name="number"></param>
    /// <param name="places"></param>
    /// <returns></returns>
    public static decimal Round(this decimal number, int places)
    {
        return Math.Round(number, places);
    }

    /// <summary>
    /// Returns the absolute value of a Decimal.
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static decimal Abs(this decimal number)
    {
        return Math.Abs(number);
    }

    /// <summary>
    /// Returns the number of places of a Decimal.
    /// </summary>
    public static int Places(this decimal number)
    {
        number = Math.Abs(number); //make sure it is positive.
        number -= (int)number;     //remove the integer part of the number.
        var decimalPlaces = 0;
        while (number > 0)
        {
            decimalPlaces++;
            number *= 10;
            number -= (int)number;
        }
        return decimalPlaces;
    }
}