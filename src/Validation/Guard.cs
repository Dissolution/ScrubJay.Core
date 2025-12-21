namespace ScrubJay.Validation;

[PublicAPI]
public static partial class Guard
{
    private static string GetOutOfBoundsMessage<T>(LowerBound<T> lowerBound, UpperBound<T> upperBound)
    {
        var (lowValue, lowInc) = lowerBound;
        var (hiValue, hiInc) = upperBound;

        return TextBuilder.New
            .Append("must be in ")
            .If(lowValue,
                (tb, lower) => tb
                    .If(lowInc, '[', '(')
                    .Append<T>(lower))
            .Append("..")
            .If(hiValue,
                (tb, upper) => tb
                    .Append<T>(upper)
                    .If(hiInc, ']', ')'))
            .ToStringAndDispose();
    }

    public static T Between<T>(T value,
        LowerBound<T> lowerBound = default,
        UpperBound<T> upperBound = default,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        if (!lowerBound.Contains(value) || !upperBound.Contains(value))
            throw Ex.ArgRange<T>(value, GetOutOfBoundsMessage<T>(lowerBound, upperBound), valueName);
        return value;
    }

    public static T Between<T>(T value,
        T inclusiveLowerBound,
        T exclusiveUpperBound,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        int c = Comparer<T>.Default.Compare(value, inclusiveLowerBound);
        if (c >= 0)
        {
            c = Comparer<T>.Default.Compare(value, exclusiveUpperBound);
            if (c < 0)
            {
                return value;
            }
        }

        throw Ex.ArgRange<T>(value, $"must be in [{inclusiveLowerBound}..{exclusiveUpperBound})", argumentName: valueName);
    }

    public static int Index(int index, int available,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        if (index >= 0 && index < available)
            return index;
        throw Ex.Index(index, available, indexName: indexName);
    }

    public static int Index(Index index, int available,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        int offset = index.GetOffset(available);
        if (offset >= 0 && offset < available)
            return offset;
        throw Ex.Index(index, available, indexName: indexName);
    }

    public static int InsertIndex(int index, int available,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        if (index >= 0 && index <= available)
            return index;
        throw Ex.Index(index, available, indexName: indexName);
    }

    public static int InsertIndex(Index index, int available,
        [CallerArgumentExpression(nameof(index))]
        string? indexName = null)
    {
        int offset = index.GetOffset(available);
        if (offset >= 0 && offset <= available)
            return offset;
        throw Ex.Index(index, available, indexName: indexName);
    }
}