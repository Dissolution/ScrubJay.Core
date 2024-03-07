namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="Range"/>
/// </summary>
public static class RangeExtensions
{
    public static IEnumerator<int> GetEnumerator(this Range range)
    {
        if (range.Start.IsFromEnd)
        {
            for (var i = range.Start.Value; i >= range.End.Value; i--)
            {
                yield return i;
            }
        }
        else
        {
            for (var i = range.Start.Value; i <= range.End.Value; i++)
            {
                yield return i;
            }
        }
    }
}