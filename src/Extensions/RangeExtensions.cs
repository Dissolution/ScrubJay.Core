namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="Range"/>
/// </summary>
[PublicAPI]
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

        return None;
    }

    public static IEnumerator<int> GetEnumerator(this Range range)
    {
        int start = range.Start.GetOffset(0);
        int end = range.End.GetOffset(0);
        int step = end.CompareTo(start);
        for (int current = start; current != end; current += step)
        {
            yield return current;
        }
    }
}