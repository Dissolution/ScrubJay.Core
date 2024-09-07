namespace ScrubJay.Extensions;

public static class RangeExtensions
{
    /// <summary>
    /// Gets the total length this <see cref="Range"/> spans (if it can be calculated)
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public static Option<int> GetLength(this Range range)
    {
        if (range.Start.IsFromEnd)
        {
            if (range.End.IsFromEnd)
            {
                int length = range.Start.Value - range.End.Value;
                Debug.Assert(length >= 0);
                return Some(length);
            }
        }
        else 
        {
            if (!range.End.IsFromEnd)
            {
                int length = range.End.Value - range.Start.Value;
                Debug.Assert(length >= 0);
                return Some(length);
            }
        }

        return default;
    }
}