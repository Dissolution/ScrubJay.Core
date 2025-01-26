namespace ScrubJay.Memory;

[PublicAPI]
public enum StopReason
{
    /// <summary>
    /// There were no more items remaining to be read
    /// </summary>
    EndOfSpan = 0,
    /// <summary>
    /// The requested number of items are included
    /// </summary>
    Satisified = 1,
    /// <summary>
    /// A read predicate indicated that reading should stop
    /// </summary>
    Predicate = 2,
}
/*

public static class SpanReaderExtensions
{

    public static bool TryTakeUntil<T>(
        this scoped ref SpanReader<T> spanReader,
        T match,
        out ReadOnlySpan<T> taken,
        bool inclusive = false,
        bool skipFirst = false)
    {
        // Operate on the remaining span
        var span = spanReader.RemainingSpan;
        int len = span.Length;

        int i = skipFirst ? 1 : 0;
        while (i < len)
        {
            if (EqualityComparer<T>.Default.Equals(match, span[i]))
            {
                if (inclusive)
                {
                    i += 1;
                }
                taken = span[..i];
                spanReader.Skip(count: i);
                return true;
            }
            i++;
        }
        // Did not find the ending match
        taken = default;
        return false;
    }
}
*/
