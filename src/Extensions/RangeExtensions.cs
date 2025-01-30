namespace ScrubJay.Extensions;

public static class RangeExtensions
{
    public static int? GetLength(this Range range)
    {
        if (range.Start.IsFromEnd || range.End.IsFromEnd)
            return null;
        return range.End.Value - range.Start.Value;
    }
    
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