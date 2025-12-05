namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="Range"/>
/// </summary>
[PublicAPI]
public static class RangeExtensions
{
    extension(Range range)
    {
        public void Deconstruct(out Index start, out Index end)
        {
            start = range.Start;
            end = range.End;
        }

        public string Render()
        {
            return TextBuilder.New
                .If(range.Start,
                    static start => !start.IsUnbounded,
                    static (tb, start) => tb.If(start.IsFromEnd, '-').Format(start.Value))
                .Append("..")
                .If(range.End,
                    static end => !end.IsUnbounded,
                    static (tb, end) => tb.If(end.IsFromEnd, '-').Format(end.Value))
                .ToStringAndDispose();
        }

        public (int Offset, int Length) UnsafeGetOffsetAndLength(int length)
        {
            int start = range.Start.GetOffset(length);
            int end = range.End.GetOffset(length);
            return (start, end-start);
        }
    }

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