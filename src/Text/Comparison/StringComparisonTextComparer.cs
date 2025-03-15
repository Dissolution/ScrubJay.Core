namespace ScrubJay.Text.Comparison;

public abstract class StringComparisonTextComparer : TextComparer
{
    public StringComparison StringComparison { get; }

    protected StringComparisonTextComparer(StringComparison stringComparison)
    {
        StringComparison = stringComparison;
    }
}
