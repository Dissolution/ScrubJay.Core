namespace ScrubJay.Extensions;

public static class RangeExtensions
{
    public static int? GetLength(this Range range)
    {
        if (range.Start.IsFromEnd || range.End.IsFromEnd)
            return null;
        return range.End.Value - range.Start.Value;
    }
}